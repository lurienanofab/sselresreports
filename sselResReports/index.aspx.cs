using LNF.Models.Data;
using sselResReports.AppCode;
using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;

namespace sselResReports
{
    public partial class index : sselResReports.AppCode.ReportPage
    {
        Dictionary<Button, ReportPage> appPages = new Dictionary<Button, ReportPage>();

        public override ClientPrivilege AuthTypes
        {
            get { return 0; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            // store the relationship between the buttons and pages
            // there should be one line per button, equals number of pages

            appPages.Add(btnAccChrono, new AccChrono());
            appPages.Add(btnAccInLab, new AccInLab());
            appPages.Add(btnResDetailed, new ResDetailed());
            appPages.Add(btnResStatus, new ResStatus());
            appPages.Add(btnResUtil, new ResUtil());
            appPages.Add(btnResToolUsageSummary, new ResToolUsageSummary());
            appPages.Add(btnResReservationDetails, new ResReservationDetails());
            appPages.Add(btnResToolReport, new ResToolReport());

            if (!Page.IsPostBack)
            {
                // check to see if session is valid
                if (Request.QueryString.Count > 0) // probably coming from sselOnLine
                {
                    int clientId;
                    string strClientID = Request.QueryString["ClientID"] ?? string.Empty;
                    if (int.TryParse(strClientID.Trim(), out clientId) && Session["ClientID"] != null)
                    {
                        if (Convert.ToInt32(Session["ClientID"]) != clientId)
                        {
                            Session.Abandon();
                            Response.Redirect("~");
                        }
                    }
                }

                btnResStatus.ToolTip = "Displays all user activity - enter/exit lab and tool reservations";
                btnResDetailed.ToolTip = "Displays summary of a users usage - total time in labs per day, total tool time, and store charges";
                btnResUtil.ToolTip = "Displays the list of tools the selected user is authorized to use";
                btnAccInLab.ToolTip = "Displays a listing of the number of entries and total time spent by active users in each of the passback controlled rooms";
                btnAccChrono.ToolTip = "Displays a report showing the number of hours spent in each passback controlled room by demographic category";
                btnResReservationDetails.ToolTip = "Displays a detail report on reservations per user";
                btnResToolReport.ToolTip = "Displays a detail report on reservations per user";

                foreach (KeyValuePair<Button, ReportPage> item in appPages)
                {
                    string reportType = item.Key.ID.Substring(3, 3);
                    Label lbl = (Label)FindControlRecursive("lbl" + reportType);
                    lbl.Visible = lbl.Visible | DisplayButton(item.Key, item.Value.AuthTypes);
                }

                lblName.Text = Session["DisplayName"].ToString();
            }
        }

        private bool DisplayButton(Button btn, ClientPrivilege auths)
        {
            btn.Visible = CurrentUser.HasPriv(auths);
            return btn.Visible;
        }

        // handles all button clicks
        protected void Button_Command(object sender, CommandEventArgs e)
        {
            string RedirectPage = string.Format("~/{0}.aspx{1}", e.CommandName, e.CommandArgument);
            Response.Redirect(RedirectPage);
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Cache.Remove(Session["Cache"].ToString());
            Session.Abandon();
            Response.Redirect(Session["Logout"].ToString() + "?Action=Blank");
        }
    }
}