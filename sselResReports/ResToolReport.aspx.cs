using LNF.Models.Data;
using sselResReports.AppCode;
using sselResReports.AppCode.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace sselResReports
{
    public partial class ResToolReport : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Administrator | ClientPrivilege.Staff; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ddlTool.DataSource = GetTools();
                ddlTool.DataBind();
            }
        }

        private List<ListItem> GetTools()
        {
            List<ListItem> items = new List<ListItem>();
            using (IDataReader reader = SchedulerTool.GetAllToolsFromScheduler())
            {
                while (reader.Read())
                    items.Add(new ListItem(reader["ResourceName"].ToString(), reader["ResourceID"].ToString()));
                reader.Close();
            }
            List<ListItem> result = items.OrderBy(x => x.Text).ToList();
            result.Insert(0, new ListItem("-- Select --", "0"));
            return result;
        }

        protected void ddlTool_SelectedIndexChanged(object sender, EventArgs e)
        {
            DateTime d = DateTime.Now.AddMonths(-1);
            DataTable dtRes = ReservationDA.GetReservationsByResourceIDAndDate(Convert.ToInt32(ddlTool.SelectedValue), d);
            DataTable dtRoomAccess = RoomDataDA.GetCleanRoomAccessData(d);
            int count = 0;

            foreach (DataRow dr in dtRes.Rows)
            {
                DateTime sDate = Convert.ToDateTime(dr["ActualBeginDateTime"]);
                DateTime eDate;
                try
                {
                    eDate = Convert.ToDateTime(dr["ActualEndDateTime"]);
                }
                catch
                {
                    eDate = sDate;
                }

                DataRow[] rows = dtRoomAccess.Select(string.Format("ClientID = {0} AND ExitDT >= '{1}' AND ExitDT <= '{2}'", dr["ClientID"], sDate.ToString("MM/dd/yyyy HH:mm:ss"), eDate.ToString("MM/dd/yyyy HH:mm:ss")));

                if (rows.Length > 0)
                {
                    count += 1;
                    dr["EntryTime"] = rows[0]["EntryDT"];
                    dr["ExitTime"] = rows[0]["ExitDT"];
                }
                else
                    dr.Delete();
            }

            lblMsg.Text = string.Format("There are {0} illegal reservations out of total {1}", count, dtRes.Rows.Count);

            gv.DataSource = dtRes;
            gv.DataBind();
        }
    }
}