using LNF.Data;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Billing;
using LNF.Repository.Data;
using sselResReports.AppCode;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.UI.WebControls;

namespace sselResReports
{
    public partial class AccChrono : ReportPage
    {
        private DataSet dsReport;

        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Staff | ClientPrivilege.Executive | ClientPrivilege.Administrator; }
        }

        protected void Page_Load(Object sender, EventArgs e)
        {
            if (!CurrentUser.HasPriv(AuthTypes))
            {
                Session.Abandon();
                Response.Redirect(Session["NoAccess"].ToString() + "?Action=Exit");
            }

            lblMsg.Visible = false;

            if (Page.IsPostBack)
            {
                dsReport = (DataSet)Cache.Get(Session["Cache"].ToString());
                if (dsReport == null)
                    Response.Redirect("~");
                else if (dsReport.DataSetName != "AccChrono")
                    Response.Redirect("~");
            }
            else
            {
                Cache.Remove(Session["Cache"].ToString()); // remove anything left in cache
                SqlConnection cnSselData = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);

                dsReport = new DataSet("AccChrono");

                Session["Updated"] = false;

                // get Client info
                SqlDataAdapter daClient = new SqlDataAdapter("Client_Select", cnSselData);
                daClient.SelectCommand.CommandType = CommandType.StoredProcedure;
                daClient.SelectCommand.Parameters.AddWithValue("@Action", "All");
                daClient.Fill(dsReport, "Client");

                DataColumn[] pkc = new DataColumn[1];
                pkc[0] = dsReport.Tables["Client"].Columns["ClientID"];
                dsReport.Tables["Client"].PrimaryKey = pkc;

                // room info
                SqlDataAdapter daRoom = new SqlDataAdapter("Room_Select", cnSselData);
                daRoom.SelectCommand.CommandType = CommandType.StoredProcedure;
                daRoom.SelectCommand.Parameters.AddWithValue("@Action", "ChargedRooms");
                daRoom.Fill(dsReport, "Room");

                DataColumn[] pkr = new DataColumn[1];
                pkr[0] = dsReport.Tables["Room"].Columns["RoomID"];
                dsReport.Tables["Room"].PrimaryKey = pkr;

                DataView dv = dsReport.Tables["Room"].DefaultView;
                dv.Sort = "Room";
                ddlRoom.DataSource = dv;
                ddlRoom.DataTextField = "Room";
                ddlRoom.DataValueField = "RoomID";
                ddlRoom.DataBind();
                ddlRoom.Items.Insert(0, new ListItem(string.Empty, "0"));
                ddlRoom.ClearSelection();

                txtStartDate.Text = DateTime.Now.AddDays(-6).Date.ToString("M/d/yyyy");
                txtNumDays.Text = "7";

                Cache.Insert(Session["Cache"].ToString(), dsReport, null, DateTime.MaxValue, TimeSpan.FromMinutes(20));
            }
        }

        private void ShowUsersInLab()
        {
            DateTime sDate = DateTime.Parse(txtStartDate.Text);
            DateTime eDate = sDate.AddDays(int.Parse(txtNumDays.Text));

            Room room = DA.Current.Query<Room>().FirstOrDefault(x => x.RoomID == int.Parse(ddlRoom.SelectedValue));

            RoomDataImport[] query = DA.Current.Query<RoomDataImport>().Where(x => x.EventDate >= sDate && x.EventDate < eDate && x.RoomName == room.RoomName).ToArray();

            //this is used to handle cases where they entered before the start date
            Dictionary<int, DateTime> entries = new Dictionary<int, DateTime>();

            //each event (entry or exit) creates a slot, a list of int is used in case two events happen at the same time
            //in most cases there will never be more than one item in the list
            Dictionary<DateTime, List<int>> htUsage = new Dictionary<DateTime, List<int>>();

            foreach (RoomDataImport item in query.OrderBy(x => x.EventDate).ToArray())
            {
                DateTime eventDate = item.EventDate;
                bool isEntry = item.EventDescription == "Local Grant - IN";

                if (isEntry)
                {
                    if (!entries.ContainsKey(item.ClientID))
                        entries.Add(item.ClientID, item.EventDate);
                }
                else
                {
                    if (!entries.ContainsKey(item.ClientID))
                    {
                        //this is an exit event but there is no previous entry record for this client

                        //use the start date as a time slot for entries before the date range
                        if (htUsage.ContainsKey(sDate))
                        {
                            //there is already a time slot for this entry so add this client to the slot
                            List<int> clients = htUsage[sDate];
                            clients.Add(item.ClientID);
                        }
                        else
                        {
                            //add a new time slot and add this client to the slot
                            List<int> clients = new List<int>();
                            clients.Add(item.ClientID);
                            htUsage.Add(sDate, clients);
                        }

                    }
                }

                //each time slot on the report will be a unique EntryDT or ExitDT and will show all the users who were in at that moment

                if (isEntry)
                {
                    if (htUsage.ContainsKey(eventDate))
                    {
                        //there is already a time slot for this entry so add this client to the slot
                        List<int> clients = htUsage[eventDate];
                        clients.Add(item.ClientID);
                    }
                    else
                    {
                        //add a new time slot and add this client to the slot
                        List<int> clients = new List<int>();
                        clients.Add(item.ClientID);
                        htUsage.Add(eventDate, clients);
                    }
                }
                else
                {
                    if (htUsage.ContainsKey(eventDate))
                    {
                        List<int> clients = htUsage[eventDate];
                        clients.Add(-1 * item.ClientID);
                    }
                    else
                    {
                        List<int> clients = new List<int>();
                        clients.Add(-1 * item.ClientID);
                        htUsage.Add(eventDate, clients);
                    }
                }
            }

            // this is the table that will hold the final result
            DataTable dtInLab = new DataTable();
            dtInLab.Columns.Add("EvtTime", typeof(DateTime));
            dtInLab.Columns.Add("Count", typeof(int));
            dtInLab.Columns.Add("InLabUsers", typeof(string));

            // this is a running list of who is in the lab, users are either removed when clientId < 0 or added when clientId > 0
            Dictionary<int, string> htClient = new Dictionary<int, string>();

            foreach (var kvpUsage in htUsage.OrderBy(x => x.Key))
            {
                List<int> clients = kvpUsage.Value;
                foreach (int clientId in clients)
                {
                    if (clientId < 0)
                    {
                        // remove from htClient because negative ClientID means exit
                        int cid = -1 * clientId;
                        if (htClient.ContainsKey(cid))
                            htClient.Remove(cid);
                    }
                    else
                    {
                        // add to htClient
                        if (!htClient.ContainsKey(clientId))
                        {
                            DataRow dr = dsReport.Tables["Client"].Rows.Find(clientId);
                            string displayName = dr == null ? string.Format("[unknown:{0}]", clientId) : dr.Field<string>("DisplayName");
                            htClient.Add(clientId, displayName);
                        }
                    }
                }

                DataRow ndr = dtInLab.NewRow();
                ndr["EvtTime"] = kvpUsage.Key;
                ndr["Count"] = htClient.Count;
                ndr["InLabUsers"] = string.Join("<br />", htClient.OrderBy(x => x.Value).Select(x => x.Value).ToArray());

                dtInLab.Rows.Add(ndr);
            }

            dgInLab.DataSource = dtInLab;
            dgInLab.DataBind();
        }

        protected void dgInLab_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                int count = Convert.ToInt32(drv["Count"]);
                switch (count)
                {
                    case 1:
                        e.Item.BackColor = System.Drawing.Color.Salmon;
                        break;
                    case 2:
                        e.Item.BackColor = System.Drawing.Color.MistyRose;
                        break;
                    case 3:
                        e.Item.BackColor = System.Drawing.Color.LightYellow;
                        break;
                }
            }
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            ShowUsersInLab();
        }
    }
}