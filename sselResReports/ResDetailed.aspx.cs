using LNF.CommonTools;
using LNF.Models.Data;
using sselResReports.AppCode;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI.WebControls;

namespace sselResReports
{
    public partial class ResDetailed : ReportPage
    {
        private DataSet dsUsage;
        private SqlConnection cnSselData = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
        private DataTable dtActivity; // inner table - contians date/time and activity

        public override ClientPrivilege AuthTypes
        {
            get{return ClientPrivilege.Staff | ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentUser.HasPriv(AuthTypes))
            {
                Session.Abandon();
                Response.Redirect(Session["NoAccess"].ToString() + "?Action=Exit");
            }

            lblMsg.Visible = false;

            if (Page.IsPostBack)
            {
                dsUsage = (DataSet)Cache.Get(Session["Cache"].ToString());
                if (dsUsage == null)
                    Response.Redirect("~");
                else if (dsUsage.DataSetName != "ResDetailed")
                    Response.Redirect("~");
            }
            else
            {
                Cache.Remove(Session["Cache"].ToString()); // remove anything left in cache
                
                dsUsage = new DataSet("ResDetailed");

                Session["Updated"] = false;

                // get account info
                SqlDataAdapter daAccount = new SqlDataAdapter("Account_Select", cnSselData);
                daAccount.SelectCommand.CommandType = CommandType.StoredProcedure;
                daAccount.SelectCommand.Parameters.AddWithValue("@Action", "All");
                daAccount.Fill(dsUsage, "Account");

                DataColumn[] pka = new DataColumn[1];
                pka[0] = dsUsage.Tables["Account"].Columns["AccountID"];
                dsUsage.Tables["Account"].PrimaryKey = pka;

                SqlDataAdapter daResource = new SqlDataAdapter("sselScheduler_Select", cnSselData);
                daResource.SelectCommand.CommandType = CommandType.StoredProcedure;
                daResource.SelectCommand.Parameters.AddWithValue("@Action", "AllResources");
                daResource.Fill(dsUsage, "Resource");

                ddlTool.DataSource = new DataView(dsUsage.Tables["Resource"], string.Empty, "ResourceName", DataViewRowState.CurrentRows);
                ddlTool.DataTextField = "ResourceName";
                ddlTool.DataValueField = "ResourceID";
                ddlTool.DataBind();
                ddlTool.Items.Insert(0, new ListItem("-- Select --", "0"));
                ddlTool.ClearSelection();

                Cache.Insert(Session["Cache"].ToString(), dsUsage, null, DateTime.MaxValue, TimeSpan.FromMinutes(20));

                pp1.SelectedPeriod = DateTime.Now.Date.AddMonths(-1);

                UpdateClient();
            }
        }

        protected void ddlTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            DisplayUsage();
        }

        protected void pp1_SelectedPeriodChanged(object sender, EventArgs e)
        {
            UpdateClient();
            DisplayUsage();
        }

        private void UpdateClient()
        {
            if (dsUsage.Tables.Contains("Client"))
                dsUsage.Tables.Remove(dsUsage.Tables["Client"]);

            DateTime sDate = pp1.SelectedPeriod;
            DateTime eDate = sDate.AddMonths(1);

            SqlDataAdapter daClient = new SqlDataAdapter("Client_Select", cnSselData);
            daClient.SelectCommand.CommandType = CommandType.StoredProcedure;
            daClient.SelectCommand.Parameters.AddWithValue("@Action", "All");
            daClient.SelectCommand.Parameters.AddWithValue("@sDate", sDate);
            daClient.SelectCommand.Parameters.AddWithValue("@eDate", eDate);
            daClient.Fill(dsUsage, "Client");

            DataColumn[] pkc = new DataColumn[1];
            pkc[0] = dsUsage.Tables["Client"].Columns["ClientID"];
            dsUsage.Tables["Client"].PrimaryKey = pkc;

            if (sDate <= DateTime.Now.Date && eDate > DateTime.Now.Date && !Convert.ToBoolean(Session["Updated"]))
            {
                WriteData mWriteData = new WriteData();
                string[] sTypes = new string[]{"Tool"};
                //mWriteData.UpdateTable(sTypes, 0, 0, eDate, WriteData.UpdateDataType.CleanData)
                Session["Updated"] = false;
            }
        }

        private void DisplayUsage()
        {
            dgActDate.DataSource = null;
            dgActDate.DataBind();

            if (ddlTool.SelectedIndex == 0)
                return;

            DateTime sDate = pp1.SelectedPeriod;
            DateTime eDate = sDate.AddMonths(1);

            if (sDate > DateTime.Now.Date) 
                return;

            DataTable dtToolUsage = ReadData.Tool.ReadToolDataClean(sDate, eDate, 0, int.Parse(ddlTool.SelectedValue));

            if (dtToolUsage.Rows.Count == 0)
            {
                lblMsg.Text = "No lab usage for selected period";
                lblMsg.Visible = true;
                return;
            }

            DataTable dtActDate = new DataTable(); // outer table - contains dates with activities
            dtActDate.Columns.Add("ActDate", typeof(DateTime));

            dtActivity = new DataTable();
            dtActivity.Columns.Add("ActDateTime", typeof(DateTime));
            dtActivity.Columns.Add("ActDate", typeof(DateTime));
            dtActivity.Columns.Add("ActTime", typeof(string));
            dtActivity.Columns.Add("Descrip", typeof(string));

            string strAccount, strClient;
            DateTime StartTime;
            double Duration;
            DataRow ndr;
            foreach (DataRow dr in dtToolUsage.Rows)
            {
                if (Convert.ToBoolean(dr["IsStarted"]))
                {
                    StartTime = Convert.ToDateTime(dr["ActualBeginDateTime"]);
                    Duration = Convert.ToDouble(dr["ActDuration"]);
                }
                else
                {
                    StartTime = Convert.ToDateTime(dr["BeginDateTime"]);
                    Duration = Convert.ToDouble(dr["SchedDuration"]);
                }

                // check if date is in outer table, if not - add it
                DataRow[] fdr = dtActDate.Select(string.Format("ActDate = '{0}'", StartTime.Date));
                if (fdr.Length == 0)
                {
                    ndr = dtActDate.NewRow();
                    ndr["ActDate"] = StartTime.Date;
                    dtActDate.Rows.Add(ndr);
                }

                strAccount = dsUsage.Tables["Account"].Rows.Find(dr["AccountID"])["Name"].ToString();
                strClient = dsUsage.Tables["Client"].Rows.Find(dr["ClientID"])["DisplayName"].ToString();

                ndr = dtActivity.NewRow();
                ndr["ActDateTime"] = StartTime; // used for sorting
                ndr["ActDate"] = StartTime.Date;
                ndr["ActTime"] = StartTime.ToString("HH:mm");
                if (StartTime < DateTime.Now)
                    ndr["Descrip"] = "Used by " + strClient + " for " + (Duration / 60.0).ToString("#.00") + " hours on account: " + strAccount;
                else
                    ndr["Descrip"] = "Reserved by " + strClient + " for " + (Duration / 60.0).ToString("#.00") + " hours on account: " + strAccount;
                dtActivity.Rows.Add(ndr);
            }

            dtActDate.DefaultView.Sort = "ActDate ASC";
            dgActDate.DataSource = dtActDate;
            dgActDate.DataBind();
        }

        protected void dgActDate_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                DataGrid dgActivity = (DataGrid)e.Item.FindControl("dgActivity");
                DataRowView drv = (DataRowView)e.Item.DataItem;

                dtActivity.DefaultView.RowFilter = string.Format("ActDate = '{0}'", drv["ActDate"]);
                dtActivity.DefaultView.Sort = "ActDateTime";
                dgActivity.DataSource = dtActivity;
                dgActivity.DataBind();
            }
        }
    }
}