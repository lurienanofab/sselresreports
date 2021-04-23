using LNF.Billing;
using LNF.CommonTools;
using LNF.Data;
using LNF.Impl.Repository.Data;
using LNF.Impl.Repository.Scheduler;
using Newtonsoft.Json;
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
    public partial class ResUtil : ReportPage
    {
        private double totalHours;
        private int rowCount;
        private DataRow totalRow;
        private IEnumerable<ResourceInfo> resInfoItems;
        private string br = " ";
        private static string COOKIE_NAME = "RES_UTIL";
        private Activity[] activities;

        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Staff | ClientPrivilege.Executive | ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentUser.HasPriv(AuthTypes))
            {
                Session.Abandon();
                Response.Redirect(Session["NoAccess"].ToString() + "?Action=Exit");
            }

            hidUserData.Value = "Susan Abraham:University of Michigan:532953860"; //wtf?

            if (!Page.IsPostBack)
            {
                Session["Updated"] = false;

                DateTime sdate = GetQueryStringStartDate();
                pp1.SelectedPeriod = sdate;
                txtMonths.Text = GetQueryStringMonths().ToString();

                switch (GetQueryStringSumColumn())
                {
                    case "SchedDuration":
                        rdoScheduled.Checked = true;
                        rdoActual.Checked = false;
                        rdoCharged.Checked = false;
                        break;
                    case "ActDuration":
                        rdoScheduled.Checked = false;
                        rdoActual.Checked = true;
                        rdoCharged.Checked = false;
                        break;
                    default:
                        rdoScheduled.Checked = false;
                        rdoActual.Checked = false;
                        rdoCharged.Checked = true;
                        break;
                }

                chkIncludeForgiven.Checked = GetQueryStringIncludeForgiven();
                chkShowPercentOfTotal.Checked = GetQueryStringShowPercentage();
                hidShowPercentOfTotal.Value = chkShowPercentOfTotal.Checked.ToString().ToLower();

                CreateAccountCheckList(cblAccountType);
                var options = ReadReportOptionsFromCookie<ToolUtilizationOptions>(COOKIE_NAME);
                ApplyOptions(options);

                switch (GetQueryStringCommand())
                {
                    case "export":
                        ExportUsage();
                        break;
                    default:
                        DisplayUsage();
                        break;
                }
            }
        }

        private string GetQueryStringCommand()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["command"]))
                return Request.QueryString["command"];
            else
                return "display";
        }

        private DateTime GetQueryStringStartDate()
        {
            DateTime defval = DateTime.Now.Date.AddMonths(-1);
            DateTime result = defval;
            if (!string.IsNullOrEmpty(Request.QueryString["sdate"]))
            {
                string temp = Request.QueryString["sdate"];
                if (DateTime.TryParse(temp, out result))
                    return result;
                else
                    return defval;
            }
            return result;
        }

        private int GetQueryStringMonths()
        {
            int defval = 1;
            int result = defval;
            if (!string.IsNullOrEmpty(Request.QueryString["months"]))
            {
                string temp = Request.QueryString["months"];
                if (int.TryParse(temp, out result))
                    return result;
                else
                    return defval;
            }
            return result;
        }

        private string GetQueryStringSumColumn()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["sumcol"]))
                return Request.QueryString["sumcol"];
            else
                return "ChargeDuration";
        }

        private bool GetQueryStringIncludeForgiven()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["forgiven"]))
                return Request.QueryString["forgiven"] == "1";
            else
                return false;
        }

        private bool GetQueryStringShowPercentage()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["percentage"]))
                return Request.QueryString["percentage"] == "1";
            else
                return false;
        }

        private DataTable GetData()
        {
            DateTime sDate = pp1.SelectedPeriod;
            DateTime eDate = sDate.AddMonths(Convert.ToInt32(txtMonths.Text));

            //IList<AccountType> allAccountTypes = DA<AccountType>.All();

            if (sDate > DateTime.Now.Date)
                return null;

            if (sDate <= DateTime.Now.Date && eDate > DateTime.Now.Date && !Convert.ToBoolean(Session["Updated"]))
            {
                WriteData writeData = new WriteData(Provider);
                writeData.UpdateTables(BillingCategory.Tool, UpdateDataType.DataClean);
                Session["Updated"] = true;
            }

            if (eDate > DateTime.Now.Date)
                eDate = DateTime.Now.Date.AddDays(1); // otherwise percentages are skewed

            totalHours = eDate.Subtract(sDate).TotalHours;

            // create table to store data
            bool includeForgiven = rdoCharged.Checked ? chkIncludeForgiven.Checked : true;
            bool showPercentages = chkShowPercentOfTotal.Checked;
            hidShowPercentOfTotal.Value = showPercentages.ToString().ToLower();
            string sumcol = GetStatsBasedOn();
            DataTable dt = GetToolUtilizationData(sumcol, includeForgiven, sDate, eDate, ReportPage.GetSelectedAccountTypes(cblAccountType));

            foreach (DataRow dr in dt.Rows)
            {
                dr["IdleTime"] = totalHours;
                for (int i = 1; i <= dt.Columns.Count - 2; i++)
                    dr["IdleTime"] = Convert.ToDouble(dr["IdleTime"]) - Utility.ConvertTo(dr[i], 0D);
                if (Convert.ToDouble(dr["IdleTime"]) < 0.0)
                    dr["IdleTime"] = 0.0;
            }

            Dictionary<string, double> totalColumns = new Dictionary<string, double>();

            activities = DataSession.Query<Activity>().Where(x => x.IsActive).ToArray();

            totalColumns.Add("Lab", 0);
            totalColumns.Add("ProcessTech", 0);
            totalColumns.Add("ResourceID", 0);
            foreach (var act in activities)
                totalColumns.Add(act.ActivityName, 0);
            totalColumns.Add("IdleTime", 0);

            List<string> keys = new List<string>(totalColumns.Keys);

            foreach (DataRow row in dt.Rows)
            {
                foreach (string k in keys)
                {
                    double value = Utility.ConvertTo(row[k], 0D);
                    totalColumns[k] += Utility.Round(value, 1);
                }
            }

            rowCount = dt.Rows.Count;

            totalRow = dt.NewRow();
            totalRow["ResourceName"] = "Total";

            foreach (KeyValuePair<string, double> kvp in totalColumns)
                totalRow[kvp.Key] = kvp.Value;

            return dt;
        }

        private DataTable GetToolUtilizationData(string sumcol, bool includeForgiven, DateTime sdate, DateTime edate, string selectedAccountTypes)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand("Reporting.dbo.Report_ToolUtilization", conn))
            using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@StartPeriod", sdate);
                cmd.Parameters.AddWithValue("@EndPeriod", edate);
                cmd.Parameters.AddWithValue("@AccountTypeIDs", selectedAccountTypes);
                adap.Fill(dt);
            }

            DataTable result = new DataTable();

            result.Columns.Add("ResourceName", typeof(string));
            result.Columns.Add("Lab", typeof(string));
            result.Columns.Add("ProcessTech", typeof(string));
            result.Columns.Add("ResourceID", typeof(int));

            foreach (DataRow dr in dt.Rows)
            {
                int resourceId = Convert.ToInt32(dr["ResourceID"]);
                string actName = dr["ActivityName"].ToString();

                if (!result.Columns.Contains(actName))
                    result.Columns.Add(new DataColumn(actName, typeof(double)));

                DataRow row;
                DataRow[] existing = result.Select(string.Format("ResourceID = {0}", resourceId));
                if (existing.Length == 0)
                {
                    row = result.NewRow();
                    row["ResourceID"] = dr["ResourceID"];
                    row["ResourceName"] = dr["ResourceName"];
                    result.Rows.Add(row);
                }
                else
                    row = existing[0];

                if (sumcol == "ChargeDuration")
                    sumcol = includeForgiven ? sumcol + "Forgiven" : sumcol;

                double amount = Utility.ConvertTo(dr[sumcol], 0D);

                row[actName] = Utility.ConvertTo(row[actName], 0D) + (amount / 60D);
            }

            result.Columns.Add("IdleTime", typeof(double));

            return result;
        }

        private void ExportUsage()
        {
            if (txtMonths.Text.Length == 0)
                return;

            DataTable dtDisp = GetData();
            if (dtDisp != null)
            {
                Response.Clear();
                Response.AddHeader("Content-Disposition", "attachment; filename=tool-utilization-report.csv");
                Response.ContentType = "application/csv";
                string abc = string.Join(",", dtDisp.Columns.Cast<DataColumn>().Select(x => @"""" + x.ColumnName.Replace(@"""", @"\""") + @"""")) + Environment.NewLine;
                Response.Write(abc);
                const string COMMA = ",";
                foreach (DataRow dr in dtDisp.Rows)
                {
                    string line = @"""" + dr["ResourceName"].ToString().Replace(@"""", @"/""") + @"""";

                    int resourceId = Convert.ToInt32(dr["ResourceID"]);
                    line += COMMA + GetLabName(resourceId) + COMMA + GetProcessTechName(resourceId);

                    for (int i = 3; i < dr.ItemArray.Length; i++)
                        line += COMMA + Utility.ConvertTo(dr[i], 0D).ToString("#0.0");
                    Response.Write(line + Environment.NewLine);
                }
                Response.End();
            }
        }

        private void DisplayUsage()
        {
            if (txtMonths.Text.Length == 0) return;
            DataTable dtDisplay = GetData();
            if (dtDisplay != null)
            {
                rptToolUtilizationReport.DataSource = dtDisplay;
                rptToolUtilizationReport.DataBind();
            }
        }

        private string GetStatsBasedOn()
        {
            if (rdoActual.Checked)
                return "ActDuration";
            else if (rdoScheduled.Checked)
                return "SchedDuration";
            else
                return "ChargeDuration";
        }

        private bool AccountTypesAreValid()
        {
            var result = !string.IsNullOrEmpty(ReportPage.GetSelectedAccountTypes(cblAccountType));
            phAccountTypeMessage.Visible = !result;
            return result;
        }

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            if (AccountTypesAreValid())
            {
                WriteReportOptionsToCookie(CreateOptions(), COOKIE_NAME);
                DisplayUsage();
            }
        }

        protected void BtnExport_Click(object sender, EventArgs e)
        {
            if (AccountTypesAreValid())
            {
                WriteReportOptionsToCookie(CreateOptions(), COOKIE_NAME);
                ExportUsage();
            }
        }

        protected string GetCellText(object item, string column)
        {
            DataItemHelper helper = new DataItemHelper(item);
            double val = Utility.ConvertTo(helper[column], 0D);
            if (val == 0D) return string.Empty;
            return val.ToString("#,##0.0") + (chkShowPercentOfTotal.Checked ? br + (val / totalHours).ToString("(0.0%)") : string.Empty);
        }

        private void GetResourceInfoItems()
        {
            if (resInfoItems == null)
            {
                resInfoItems = DataSession.Query<ResourceInfo>().Where(x => x.ResourceIsActive).ToList();
            }
        }

        protected string GetLabName(object resourceId)
        {
            GetResourceInfoItems();
            int resID = int.Parse(resourceId.ToString());
            ResourceInfo resInfo = resInfoItems.FirstOrDefault(x => x.ResourceID == resID);
            if (resInfo != null)
                return resInfo.LabName;
            else
                return string.Empty;
        }

        protected string GetProcessTechName(object resourceId)
        {
            GetResourceInfoItems();
            int resID = int.Parse(resourceId.ToString());
            ResourceInfo resInfo = resInfoItems.FirstOrDefault(x => x.ResourceID == resID);
            if (resInfo != null)
                return resInfo.ProcessTechName;
            else
                return string.Empty;
        }

        protected string GetTotal(string column)
        {
            double val = Utility.ConvertTo(totalRow[column], 0D);
            if (val == 0D) return string.Empty;
            return val.ToString("#,##0.0") + (chkShowPercentOfTotal.Checked ? br + (val / totalHours / rowCount).ToString("(0.0%)") : string.Empty);
        }

        protected void RptToolUtilizationReport_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                var rptHeaderColumns = (Repeater)e.Item.FindControl("rptHeaderColumns");
                rptHeaderColumns.DataSource = activities;
                rptHeaderColumns.DataBind();
            }
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var rptItemColumns = (Repeater)e.Item.FindControl("rptItemColumns");
                rptItemColumns.DataSource = activities;
                rptItemColumns.DataBind();
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                var rptSubtotalColumns = (Repeater)e.Item.FindControl("rptSubtotalColumns");
                rptSubtotalColumns.DataSource = activities;
                rptSubtotalColumns.DataBind();

                var rptTotalColumns = (Repeater)e.Item.FindControl("rptTotalColumns");
                rptTotalColumns.DataSource = activities;
                rptTotalColumns.DataBind();
            }
        }

        private void ApplyOptions(ToolUtilizationOptions options)
        {
            string[] splitter = options.AccountTypes.Split(',');
            foreach (ListItem item in cblAccountType.Items)
            {
                if (splitter.Contains(item.Value))
                    item.Selected = true;
            }

            rdoCharged.Checked = false;
            rdoScheduled.Checked = false;
            rdoActual.Checked = false;

            switch (options.StatsBasedOn)
            {
                case "charged":
                    rdoCharged.Checked = true;
                    break;
                case "scheduled":
                    rdoScheduled.Checked = true;
                    break;
                case "actual":
                    rdoActual.Checked = true;
                    break;
            }

            chkIncludeForgiven.Checked = options.IncludeForgiven;

            chkShowPercentOfTotal.Checked = options.ShowPercentage;
        }

        private ToolUtilizationOptions CreateOptions()
        {
            var selectedAccountTypes = GetSelectedAccountTypes(cblAccountType);

            string statsBasedOn;

            if (rdoScheduled.Checked)
                statsBasedOn = "scheduled";
            else if (rdoActual.Checked)
                statsBasedOn = "actual";
            else
                statsBasedOn = "charged";

            return new ToolUtilizationOptions()
            {
                AccountTypes = selectedAccountTypes,
                StatsBasedOn = statsBasedOn,
                IncludeForgiven = chkIncludeForgiven.Checked,
                ShowPercentage = chkShowPercentOfTotal.Checked
            };
        }

        private ToolUtilizationOptions CreateOptions(IEnumerable<AccountType> items, string statsBasedOn, bool includeForgiven, bool showPercentage)
        {
            return new ToolUtilizationOptions()
            {
                AccountTypes = string.Join(",", DataSession.Query<AccountType>().ToArray().Select(x => x.AccountTypeID.ToString())),
                StatsBasedOn = statsBasedOn,
                IncludeForgiven = includeForgiven,
                ShowPercentage = showPercentage
            };
        }
    }

    public class ToolUtilizationOptions
    {
        [JsonProperty("stats")]
        public string StatsBasedOn { get; set; }

        [JsonProperty("accountTypes")]
        public string AccountTypes { get; set; }

        [JsonProperty("includeForgiven")]
        public bool IncludeForgiven { get; set; }

        [JsonProperty("showPercentage")]
        public bool ShowPercentage { get; set; }
    }
}