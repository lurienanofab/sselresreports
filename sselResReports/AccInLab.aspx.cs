using LNF.Data;
using sselResReports.AppCode;
using sselResReports.AppCode.DAL;
using System;
using System.Collections.Generic;

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
                using (var reader = DataCommand().Param("Action", "PassbackRooms").ExecuteReader("dbo.Room_Select"))
                {
                    ddlRoom.DataSource = reader;
                    ddlRoom.DataBind();
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

        protected void BtnReport_Click(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void DdlRoom_SelectedIndexChanged(object sender, EventArgs e)
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