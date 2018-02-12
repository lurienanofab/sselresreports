using LNF.CommonTools;
using LNF.Models.Data;
using LNF.Repository;
using LNF.Repository.Data;
using Newtonsoft.Json;
using sselResReports.AppCode;
using sselResReports.AppCode.DAL;
using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

namespace sselResReports
{
    public partial class ResToolUsageSummary : ReportPage
    {
        private static string COOKIE_NAME = "RES_TOOL_USAGE_SUMMARY";

        private struct ToolUsageSummaryUnit
        {
            public ToolUsageSummaryRate Hours;
            public ToolUsageSummaryRate Amount;
        }

        private struct ToolUsageSummaryRate
        {
            public double Gross;
            public double Forgiven;
            public double Net;
        }

        private ToolUsageSummaryUnit totalUsage;
        private ToolUsageSummaryRate totalBookingFee;
        private ToolUsageSummaryRate totalBilled;

        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.Staff | ClientPrivilege.Developer; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentUser.HasPriv(AuthTypes))
            {
                Session.Abandon();
                Response.Redirect(Session["NoAccess"].ToString() + "?Action=Exit");
            }

            hidVarAccountTypes.Value = ReportPage.GetSelectedAccountTypes(cblAccountType);

            if (!Page.IsPostBack)
            {
                ReportPage.CreateAccountCheckList(cblAccountType);
                var options = ReadReportOptionsFromCookie<ToolUsageSummaryOptions>(COOKIE_NAME); //ReportPage.ReadReportOptionsFromCookie(Request, cblAccountType, COOKIENAME);
                ApplyOptions(options);
            }
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            if (ddlTool.SelectedValue == "0")
            {
                throw new Exception("A tool must be selected");
            }
            else
            {
                GetSummary();
                GetExportLinks();
            }

            WriteReportOptionsToCookie(CreateOptions(), COOKIE_NAME);
        }

        protected void GetExportLinks()
        {
            int months = int.Parse(txtNumMonths.Text);
            DateTime sd = pp1.SelectedPeriod;
            DateTime ed = sd.AddMonths(months);
            string atys = hidVarAccountTypes.Value;
            litSummaryExport.Text = string.Format(@"<div class=""export-link""><a href=""/data/feed/tool-usage-summary/csv?opt=Combined&sd={0}&ed={1}&rid={2}&at={3}"">Export Summary</a></div>", sd.ToString("yyyy-MM-dd"), ed.ToString("yyyy-MM-dd"), ddlTool.SelectedValue, atys);
            litActivatedExport.Text = string.Format(@"<div class=""export-link""><a href=""/data/feed/tool-usage-summary/csv?opt=Activated&sd={0}&ed={1}&rid={2}&at={3}"">Export Activated</a></div>", sd.ToString("yyyy-MM-dd"), ed.ToString("yyyy-MM-dd"), ddlTool.SelectedValue, atys);
            litUnactivatedExport.Text = string.Format(@"<div class=""export-link""><a href=""/data/feed/tool-usage-summary/csv?opt=Unactivated&sd={0}&ed={1}&rid={2}&at={3}"">Export Unactivated</a></div>", sd.ToString("yyyy-MM-dd"), ed.ToString("yyyy-MM-dd"), ddlTool.SelectedValue, atys);
            litForgivenExport.Text = string.Format(@"<div class=""export-link""><a href=""/data/feed/tool-usage-summary/csv?opt=Forgiven&sd={0}&ed={1}&rid={2}&at={3}"">Export Forgiven</a></div>", sd.ToString("yyyy-MM-dd"), ed.ToString("yyyy-MM-dd"), ddlTool.SelectedValue, atys);
        }

        protected void GetSummary()
        {
            int month = pp1.SelectedMonth;
            int year = pp1.SelectedYear;
            int numMonths = int.Parse(txtNumMonths.Text);
            int resourceId = int.Parse(ddlTool.SelectedValue);
            string resourceName = ddlTool.SelectedItem.Text;

            string selectedAccountTypes = ReportPage.GetSelectedAccountTypes(cblAccountType);

            DataTable dtCombined = ToolUsageSummary.GetCombinedReservations(month, year, numMonths, resourceId, selectedAccountTypes);
            DataTable dtHourlyRates = ToolUsageSummary.GetToolHourlyRateByPeriod(month, year, numMonths, resourceId);

            double totalUses = Utility.ConvertTo(dtCombined.Compute("SUM(TotalUses)", string.Empty), 0D);
            double totalSchedHours = Utility.ConvertTo(dtCombined.Compute("SUM(TotalSchedHours)", string.Empty), 0D);
            double totalActHours = Utility.ConvertTo(dtCombined.Compute("SUM(TotalActHours)", string.Empty), 0D);

            totalUsage.Hours.Gross = Utility.ConvertTo(dtCombined.Compute("SUM(NormalHoursGross)", string.Empty), 0D) + Utility.ConvertTo(dtCombined.Compute("SUM(OverTimeHoursGross)", string.Empty), 0D);
            totalUsage.Hours.Forgiven = Utility.ConvertTo(dtCombined.Compute("SUM(NormalHoursForgiven)", string.Empty), 0D) + Utility.ConvertTo(dtCombined.Compute("SUM(OverTimeHoursForgiven)", string.Empty), 0D);
            totalUsage.Hours.Net = Utility.ConvertTo(dtCombined.Compute("SUM(NormalHoursNet)", string.Empty), 0D) + Utility.ConvertTo(dtCombined.Compute("SUM(OverTimeHoursNet)", string.Empty), 0D);
            totalUsage.Amount.Gross = Utility.ConvertTo(dtCombined.Compute("SUM(NormalAmountGross)", string.Empty), 0D) + Utility.ConvertTo(dtCombined.Compute("SUM(OverTimeAmountGross)", string.Empty), 0D);
            totalUsage.Amount.Forgiven = Utility.ConvertTo(dtCombined.Compute("SUM(NormalAmountForgiven)", string.Empty), 0D) + Utility.ConvertTo(dtCombined.Compute("SUM(OverTimeAmountForgiven)", string.Empty), 0D);
            totalUsage.Amount.Net = Utility.ConvertTo(dtCombined.Compute("SUM(NormalAmountNet)", string.Empty), 0D) + Utility.ConvertTo(dtCombined.Compute("SUM(OverTimeAmountNet)", string.Empty), 0D);

            totalBookingFee.Gross = Utility.ConvertTo(dtCombined.Compute("SUM(BookingFeeGross)", string.Empty), 0D);
            totalBookingFee.Forgiven = Utility.ConvertTo(dtCombined.Compute("SUM(BookingFeeForgiven)", string.Empty), 0D);
            totalBookingFee.Net = Utility.ConvertTo(dtCombined.Compute("SUM(BookingFeeNet)", string.Empty), 0D);

            totalBilled.Gross = Utility.ConvertTo(dtCombined.Compute("SUM(BilledAmountGross)", string.Empty), 0D);
            totalBilled.Forgiven = Utility.ConvertTo(dtCombined.Compute("SUM(BilledAmountForgiven)", string.Empty), 0D);
            totalBilled.Net = Utility.ConvertTo(dtCombined.Compute("SUM(BilledAmountNet)", string.Empty), 0D);

            litResource.Text = string.Format("{0} [{1}]", resourceName, resourceId);
            litTotalReservations.Text = Math.Ceiling(totalUses).ToString("#0");
            litTotalSchedHours.Text = totalSchedHours.ToString("#0.0000");
            litTotalActualHours.Text = totalActHours.ToString("#0.0000");
            litHourlyRates.Text = GetHourlyRatesTable(dtHourlyRates, month, year, numMonths);

            rptCombined.DataSource = dtCombined;
            rptCombined.DataBind();

            panSummary.Visible = true;
        }

        protected double GetHours(object item, string rate, bool forgiven)
        {
            DataRowView drv = (DataRowView)item;
            string column = rate + "Hours" + (forgiven ? "Net" : "Gross");
            double hours = Utility.ConvertTo(drv[column], 0D);
            return hours;
        }

        protected double GetAmount(object item, string rate, bool forgiven)
        {
            DataRowView drv = (DataRowView)item;
            string column = rate + "Amount" + (forgiven ? "Net" : "Gross");
            double amount = Utility.ConvertTo(drv[column], 0D);
            return amount;
        }

        protected double GetBookingFee(object item, bool forgiven)
        {
            DataRowView drv = (DataRowView)item;
            string column = "BookingFee" + (forgiven ? "Net" : "Gross");
            double result = Convert.ToDouble(drv[column]);
            return result;
        }

        protected double GetBookingFeeForgiven(object item)
        {
            double bookingFee = GetBookingFee(item, false);
            double forgivenBookingFee = GetBookingFee(item, true);
            return forgivenBookingFee - bookingFee;
        }

        protected double GetBookingFeeForgivenPercent(object item)
        {
            double gross = GetBookingFee(item, false);
            if (gross == 0)
                return 0;
            else
                return GetBookingFeeForgiven(item) / gross;
        }

        protected double GetForgivenHours(object item, string rate)
        {
            double hours = GetHours(item, rate, false);
            double forgivenHours = GetHours(item, rate, true);
            return forgivenHours - hours;
        }

        protected double GetForgivenAmount(object item, string rate)
        {
            double amount = GetAmount(item, rate, false);
            double forgivenAmount = GetAmount(item, rate, true);
            return forgivenAmount - amount;
        }

        protected double GetForgivenPercent(object item, string rate)
        {
            double gross = GetAmount(item, rate, false);
            if (gross == 0)
                return 0;
            else
                return GetForgivenAmount(item, rate) / gross;
        }

        protected double GetTotalUsageHours(bool forgiven)
        {
            if (forgiven)
                return totalUsage.Hours.Net;
            else
                return totalUsage.Hours.Gross;
        }

        protected double GetTotalUsageHoursForgiven()
        {
            return totalUsage.Hours.Forgiven;
        }

        protected double GetTotalUsageAmount(bool forgiven)
        {
            if (forgiven)
                return totalUsage.Amount.Net;
            else
                return totalUsage.Amount.Gross;
        }

        protected double GetTotalUsageAmountForgiven()
        {
            return totalUsage.Amount.Forgiven;
        }

        protected double GetTotalForgivenPercentage()
        {
            double gross = GetTotalUsageAmount(false);
            if (gross == 0)
                return 0;
            else
                return GetTotalUsageAmountForgiven() / gross;
        }

        protected double GetTotalBookingFee(bool forgiven)
        {
            if (forgiven)
                return totalBookingFee.Net;
            else
                return totalBookingFee.Gross;
        }

        protected double GetTotalBookingFeeForgiven()
        {
            return totalBookingFee.Forgiven;
        }

        protected double GetTotalBookingFeeForgivenPercentage()
        {
            double gross = GetTotalBookingFee(false);
            if (gross == 0)
                return 0;
            else
                return GetTotalBookingFeeForgiven() / gross;
        }

        protected double GetTotalBilledAmount(bool forgiven)
        {
            if (forgiven)
                return totalBilled.Net;
            else
                return totalBilled.Gross;
        }

        protected double GetTotalBilledAmountForgiven()
        {
            return totalBilled.Forgiven;
        }

        protected double GetTotalBilledAmountForgivenPercentage()
        {
            double gross = GetTotalBilledAmount(false);
            if (gross == 0)
                return 0;
            else
                return GetTotalBilledAmountForgiven() / gross;
        }

        protected string SetStyle(double value, string format)
        {
            if (value < 0)
                return string.Format(@"<span style=""color: #FF0000"">{0}</span>", value.ToString(format));
            else
                return value.ToString(format);
        }

        protected double GetValue(object item, string rate, string unit, bool forgiven)
        {
            DataRowView drv = (DataRowView)item;
            string column = string.Format("{0}{1}{2}", rate, unit, (forgiven ? "Net" : "Gross"));
            double value = Utility.ConvertTo(drv[column], 0D);
            return value;
        }

        protected double GetForgiven(object item, string rate, string unit)
        {
            double gross = GetValue(item, rate, unit, false);
            double net = GetValue(item, rate, unit, true);
            return net - gross;
        }

        protected string GetHourlyRatesTable(DataTable dt, int month, int year, int numMonths)
        {
            DateTime sdate = new DateTime(year, month, 1);
            DateTime edate = sdate.AddMonths(numMonths);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(@"<table class=""summary-table"">");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th></th>");
            DateTime d = sdate;
            int index = 0;
            while (d < edate)
            {
                sb.AppendLine(string.Format("<th>{0}</th>", d.ToString("MMM yyyy")));
                index += 1;
                d = sdate.AddMonths(index);
            }
            sb.AppendLine("</tr>");
            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendLine("<tr>");
                sb.AppendLine(string.Format(@"<td class=""section-title"">{0}</td>", dr["ChargeTypeName"]));
                d = sdate;
                index = 0;
                while (d < edate)
                {
                    sb.AppendLine(string.Format("<td>{0}</td>", Utility.ConvertTo(dr[string.Format("PERIOD_{0}", index)], 0D).ToString("C")));
                    index += 1;
                    d = sdate.AddMonths(index);
                }
                sb.AppendLine("</tr>");
            }
            sb.AppendLine("</table>");
            return sb.ToString();
        }

        protected void gvActivated_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = (DataRowView)e.Row.DataItem;
                string unit;
                string rate;
                ToolUsageSummaryUnit normal;
                ToolUsageSummaryUnit overTime;

                unit = "Normal";
                rate = "Hours";
                normal.Hours.Gross = Utility.ConvertTo(drv[string.Format("{0}{1}Gross", unit, rate)], 0D);
                normal.Hours.Forgiven = Utility.ConvertTo(drv[string.Format("{0}{1}Forgiven", unit, rate)], 0D);
                normal.Hours.Net = Utility.ConvertTo(drv[string.Format("{0}{1}Net", unit, rate)], 0D);
                rate = "Amount";
                normal.Amount.Gross = Utility.ConvertTo(drv[string.Format("{0}{1}Gross", unit, rate)], 0D);
                normal.Amount.Forgiven = Utility.ConvertTo(drv[string.Format("{0}{1}Forgiven", unit, rate)], 0D);
                normal.Amount.Net = Utility.ConvertTo(drv[string.Format("{0}{1}Net", unit, rate)], 0D);
                unit = "OverTime";
                rate = "Hours";
                overTime.Hours.Gross = Utility.ConvertTo(drv[string.Format("{0}{1}Gross", unit, rate)], 0D);
                overTime.Hours.Forgiven = Utility.ConvertTo(drv[string.Format("{0}{1}Forgiven", unit, rate)], 0D);
                overTime.Hours.Net = Utility.ConvertTo(drv[string.Format("{0}{1}Net", unit, rate)], 0D);
                rate = "Amount";
                overTime.Amount.Gross = Utility.ConvertTo(drv[string.Format("{0}{1}Gross", unit, rate)], 0D);
                overTime.Amount.Forgiven = Utility.ConvertTo(drv[string.Format("{0}{1}Forgiven", unit, rate)], 0D);
                overTime.Amount.Net = Utility.ConvertTo(drv[string.Format("{0}{1}Net", unit, rate)], 0D);

                ToolUsageSummaryRate billed;
                billed.Gross = Utility.ConvertTo(drv["BilledAmountGross"], 0D);
                billed.Forgiven = Utility.ConvertTo(drv["BilledAmountForgiven"], 0D);
                billed.Net = Utility.ConvertTo(drv["BilledAmountNet"], 0D);

                if (normal.Hours.Gross < 0) e.Row.Cells[5].Style["color"] = "#FF0000";
                if (normal.Hours.Forgiven < 0) e.Row.Cells[6].Style["color"] = "#FF0000";
                if (normal.Hours.Net < 0) e.Row.Cells[7].Style["color"] = "#FF0000";
                if (overTime.Hours.Gross < 0) e.Row.Cells[8].Style["color"] = "#FF0000";
                if (overTime.Hours.Forgiven < 0) e.Row.Cells[9].Style["color"] = "#FF0000";
                if (overTime.Hours.Net < 0) e.Row.Cells[10].Style["color"] = "#FF0000";
                if (normal.Amount.Gross < 0) e.Row.Cells[11].Style["color"] = "#FF0000";
                if (normal.Amount.Forgiven < 0) e.Row.Cells[12].Style["color"] = "#FF0000";
                if (normal.Amount.Net < 0) e.Row.Cells[13].Style["color"] = "#FF0000";
                if (overTime.Amount.Gross < 0) e.Row.Cells[14].Style["color"] = "#FF0000";
                if (overTime.Amount.Forgiven < 0) e.Row.Cells[15].Style["color"] = "#FF0000";
                if (overTime.Amount.Net < 0) e.Row.Cells[16].Style["color"] = "#FF0000";
                if (billed.Gross < 0) e.Row.Cells[17].Style["color"] = "#FF0000";
                if (billed.Forgiven < 0) e.Row.Cells[18].Style["color"] = "#FF0000";
                if (billed.Net < 0) e.Row.Cells[19].Style["color"] = "#FF0000";
            }
        }

        protected void gvUnactivated_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = (DataRowView)e.Row.DataItem;

                ToolUsageSummaryUnit normal;
                normal.Hours.Gross = Utility.ConvertTo(drv["NormalHoursGross"], 0D);
                normal.Hours.Forgiven = Utility.ConvertTo(drv["NormalHoursForgiven"], 0D);
                normal.Hours.Net = Utility.ConvertTo(drv["NormalHoursNet"], 0D);
                normal.Amount.Gross = Utility.ConvertTo(drv["NormalAmountGross"], 0D);
                normal.Amount.Forgiven = Utility.ConvertTo(drv["NormalAmountForgiven"], 0D);
                normal.Amount.Net = Utility.ConvertTo(drv["NormalAmountNet"], 0D);

                ToolUsageSummaryRate bookingFee;
                bookingFee.Gross = Utility.ConvertTo(drv["BookingFeeGross"], 0D);
                bookingFee.Forgiven = Utility.ConvertTo(drv["BookingFeeForgiven"], 0D);
                bookingFee.Net = Utility.ConvertTo(drv["BookingFeeNet"], 0D);

                ToolUsageSummaryRate billed;
                billed.Gross = Utility.ConvertTo(drv["BilledAmountGross"], 0D);
                billed.Forgiven = Utility.ConvertTo(drv["BilledAmountForgiven"], 0D);
                billed.Net = Utility.ConvertTo(drv["BilledAmountNet"], 0D);

                if (normal.Hours.Gross < 0) e.Row.Cells[4].Style["color"] = "#FF0000";
                if (normal.Hours.Forgiven < 0) e.Row.Cells[5].Style["color"] = "#FF0000";
                if (normal.Hours.Net < 0) e.Row.Cells[6].Style["color"] = "#FF0000";
                if (normal.Amount.Gross < 0) e.Row.Cells[7].Style["color"] = "#FF0000";
                if (normal.Amount.Forgiven < 0) e.Row.Cells[8].Style["color"] = "#FF0000";
                if (normal.Amount.Net < 0) e.Row.Cells[9].Style["color"] = "#FF0000";
                if (bookingFee.Gross < 0) e.Row.Cells[10].Style["color"] = "#FF0000";
                if (bookingFee.Forgiven < 0) e.Row.Cells[11].Style["color"] = "#FF0000";
                if (bookingFee.Net < 0) e.Row.Cells[12].Style["color"] = "#FF0000";
                if (billed.Gross < 0) e.Row.Cells[13].Style["color"] = "#FF0000";
                if (billed.Forgiven < 0) e.Row.Cells[14].Style["color"] = "#FF0000";
                if (billed.Net < 0) e.Row.Cells[15].Style["color"] = "#FF0000";
            }
        }

        protected void gvForgiven_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = (DataRowView)e.Row.DataItem;
                ToolUsageSummaryRate billed;
                billed.Gross = Utility.ConvertTo(drv["BilledAmountGross"], 0D);
                billed.Forgiven = Utility.ConvertTo(drv["BilledAmountForgiven"], 0D);
                billed.Net = Utility.ConvertTo(drv["BilledAmountNet"], 0D);
                if (billed.Gross < 0) e.Row.Cells[5].Style["color"] = "#FF0000";
                if (billed.Forgiven < 0) e.Row.Cells[6].Style["color"] = "#FF0000";
                if (billed.Net < 0) e.Row.Cells[7].Style["color"] = "#FF0000";
            }
        }

        protected void gvActivated_DataBound(object sender, EventArgs e)
        {
            if (gvActivated.Rows.Count > 0 && gvActivated.Rows[0].RowType != DataControlRowType.EmptyDataRow)
            {
                GridViewRow row = new GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);
                TableCell cell;

                cell =new TableHeaderCell();
                cell.ColumnSpan = 3;
                cell.CssClass = "edge";
                row.Cells.Add(cell);

                cell = new TableHeaderCell();
                cell.ColumnSpan = 2;
                cell.Text = "Duration Hours";
                cell.CssClass = "edge";
                row.Cells.Add(cell);

                cell = new TableHeaderCell();
                cell.ColumnSpan = 3;
                cell.Text = "Normal Hours";
                cell.CssClass = "edge hours";
                row.Cells.Add(cell);

                cell = new TableHeaderCell();
                cell.ColumnSpan = 3;
                cell.Text = "Over Time Hours";
                cell.CssClass = "edge hours";
                row.Cells.Add(cell);

                cell = new TableHeaderCell();
                cell.ColumnSpan = 3;
                cell.Text = "Normal Amount";
                cell.CssClass = "edge amount";
                row.Cells.Add(cell);

                cell = new TableHeaderCell();
                cell.ColumnSpan = 3;
                cell.Text = "Over Time Amount";
                cell.CssClass = "edge amount";
                row.Cells.Add(cell);

                cell = new TableHeaderCell();
                cell.ColumnSpan = 3;
                cell.Text = "Billed Amount";
                cell.CssClass = "edge";
                row.Cells.Add(cell);

                Table table = (Table)gvActivated.Controls[0];
                if (table != null) table.Rows.AddAt(0, row);
            }
        }

        protected void gvUnactivated_DataBound(object sender, EventArgs e)
        {
            if (gvUnactivated.Rows.Count > 0 && gvUnactivated.Rows[0].RowType != DataControlRowType.EmptyDataRow)
            {
                GridViewRow row = new GridViewRow(0, -1, DataControlRowType.Header, DataControlRowState.Normal);
                TableCell cell;

                cell = new TableHeaderCell();
                cell.ColumnSpan = 3;
                cell.CssClass = "edge";
                row.Cells.Add(cell);

                cell = new TableHeaderCell();
                cell.ColumnSpan = 1;
                cell.Text = "Duration Hours";
                cell.CssClass = "edge";
                row.Cells.Add(cell);

                cell = new TableHeaderCell();
                cell.ColumnSpan = 3;
                cell.Text = "Normal Hours";
                cell.CssClass = "edge hours";
                row.Cells.Add(cell);

                cell = new TableHeaderCell();
                cell.ColumnSpan = 3;
                cell.Text = "Normal Amount";
                cell.CssClass = "edge amount";
                row.Cells.Add(cell);

                cell = new TableHeaderCell();
                cell.ColumnSpan = 3;
                cell.Text = "Booking Fee";
                cell.CssClass = "edge";
                row.Cells.Add(cell);

                cell = new TableHeaderCell();
                cell.ColumnSpan = 3;
                cell.Text = "Billed Amount";
                cell.CssClass = "edge";
                row.Cells.Add(cell);

                Table table = (Table)gvUnactivated.Controls[0];
                if (table != null) table.Rows.AddAt(0, row);
            }
        }

        private void ApplyOptions(ToolUsageSummaryOptions options)
        {
            string[] splitter = options.AccountTypes.Split(',');
            foreach (ListItem item in cblAccountType.Items)
            {
                if (splitter.Contains(item.Value))
                    item.Selected = true;
            }
        }

        private ToolUsageSummaryOptions CreateOptions()
        {
            var selectedAccountTypes = ReportPage.GetSelectedAccountTypes(cblAccountType);

            return new ToolUsageSummaryOptions()
            {
                AccountTypes = selectedAccountTypes
            };
        }
    }

    public class ToolUsageSummaryOptions
    {
        [JsonProperty("accountTypes")]
        public string AccountTypes { get; set; }

        public ToolUsageSummaryOptions()
        {
            //defaults (note: this constructor must have no parameters)
            AccountTypes = string.Join(",", DA.Current.Query<AccountType>().ToArray().Select(x => x.AccountTypeID.ToString()));
        }
    }
}