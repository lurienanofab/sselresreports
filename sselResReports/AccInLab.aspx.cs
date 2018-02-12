using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using LNF.Repository;
using LNF.CommonTools;
using sselResReports.AppCode;
using sselResReports.AppCode.DAL;
using LNF.Models.Data;

namespace sselResReports
{
    public partial class AccInLab : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get { return ClientPrivilege.LabUser | ClientPrivilege.Staff | ClientPrivilege.Executive | ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!CurrentUser.HasPriv(AuthTypes))
            {
                Session.Abandon();
                Response.Redirect(Session["NoAccess"].ToString() + "?Action=Exit");
            }
            lblTime.Text = DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString();

            if (!Page.IsPostBack)
            {
                using (var dba = DA.Current.GetAdapter())
                {
                    dba.AddParameter("@Action", "PassbackRooms");
                    using (var reader = dba.ExecuteReader("Room_Select"))
                    {
                        ddlRoom.DataSource = reader;
                        ddlRoom.DataBind();
                    }
                }

                LoadData();
            }
        }

        private void LoadData()
        {
            IList<InLabClient> query = RoomDataDA.GetCurrentUsersInRoom(ddlRoom.SelectedValue);
            gvUsers.DataSource = query;
            gvUsers.DataBind();

            rptUsers.DataSource = query;
            rptUsers.DataBind();
        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void ddlRoom_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadData();
        }

        protected string GetTimeOfEntrance(object item)
        {
            if (item == null || item == DBNull.Value)
                return "Error";
            else
                return string.Format("{0:h:mm tt on d MMMM yyyy}", item);
        }

        protected string GetAccessMethod(object item)
        {
            if (item == null || item == DBNull.Value)
                return "N/A";
            else
                return item.ToString();
        }

        protected string GetValue(object item, string format, string nullString)
        {
            if (item == null || item == DBNull.Value)
                return nullString;
            else
                return string.Format(format, item);
        }
    }
}