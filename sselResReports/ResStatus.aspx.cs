using LNF.Data;
using LNF.Impl.Repository.Control;
using LNF.Impl.Repository.Scheduler;
using sselResReports.AppCode;
using sselResReports.AppCode.DAL;
using System;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace sselResReports
{
    public partial class ResStatus : ReportPage
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

            if (!Page.IsPostBack)
            {
                if (ddlReportType.SelectedValue == "Current")
                {
                    gvFuture.Visible = false;
                    dgToolStatus.Visible = true;
                    FillCurrentStatusTable();
                }
                else
                {
                    gvFuture.Visible = true;
                    dgToolStatus.Visible = false;
                    BindFutureStatusTable();
                }
            }
        }

        private string GetResourceState(ResourceTree item)
        {
            string[] resourceState = { "<i>Offline</i>", "<b>Online</b>", "Limited" };
            return resourceState[(int)item.State];
        }

        private string GetCurrentActivity(ResourceTree item)
        {
            if (item.CurrentClientID == 0)
                return "No activity at the present time";
            else
                return string.Format("Being used by {0} {1} for {2}", item.CurrentFirstName, item.CurrentLastName, item.CurrentActivityName);
        }

        private ActionInstance[] _instances;

        private ActionInstance[] GetInstances()
        {
            if (_instances == null)
                _instances = DataSession.Query<ActionInstance>().ToArray();
            return _instances;
        }

        private int GetPoint(int resourceId)
        {
            var inst = GetInstances().FirstOrDefault(x => x.ActionID == resourceId && x.ActionName == "Interlock");

            if (inst != null)
                return inst.Point;
            else
                return 0;
        }

        private void FillCurrentStatusTable()
        {
            //var mgr = new InterlockManager();
            //var instances = mgr.GetAllActionInstances();

            var query = DataSession.Query<ResourceTree>().Where(x => x.ClientID == CurrentUser.ClientID && x.ResourceIsActive).OrderBy(x => x.BuildingName).ThenBy(x => x.LabName).ThenBy(x => x.ProcessTechName).ThenBy(x => x.ResourceName).ToArray();

            tblToolStatus.Rows.Clear();

            int previousLab = 0;
            int previousProcTech = 0;

            TableRow row;

            row = new TableRow() { CssClass = "GridHeader" };
            row.Cells.Add(new TableCell() { Text = "Resource Name" });
            row.Cells.Add(new TableCell() { Text = "Status" });
            row.Cells.Add(new TableCell() { Text = "Current Activity" });
            row.Cells.Add(new TableCell() { Text = "Interlock State" });
            tblToolStatus.Rows.Add(row);

            int r = 0;
            foreach (var item in query)
            {
                if (item.LabID != previousLab)
                {
                    row = new TableRow
                    {
                        CssClass = "OutlineLevel1"
                    };
                    row.Cells.Add(new TableCell() { Text = item.LabName, ColumnSpan = 4, CssClass = "lab" });
                    tblToolStatus.Rows.Add(row);
                    previousLab = item.LabID;
                }

                if (item.ProcessTechID != previousProcTech)
                {
                    row = new TableRow
                    {
                        CssClass = "OutlineLevel2"
                    };
                    row.Cells.Add(new TableCell() { Text = item.ProcessTechName, ColumnSpan = 4, CssClass = "proctech" });
                    tblToolStatus.Rows.Add(row);
                    previousProcTech = item.ProcessTechID;
                }

                row = new TableRow();

                if (r % 2 == 0)
                    row.CssClass = "Item";
                else
                    row.CssClass = "AlternatingItem";

                row.Cells.Add(new TableCell() { Text = item.ResourceName, CssClass = "resource" });
                row.Cells.Add(new TableCell() { Text = GetResourceState(item) });
                row.Cells.Add(new TableCell() { Text = GetCurrentActivity(item) });

                TableCell cell = new TableCell() { Text = "<img src=\"images/loader.gif\" alt=\"loading...\" />", CssClass = "status" };
                cell.Attributes.Add("data-point", GetPoint(item.ResourceID).ToString());
                row.Cells.Add(cell);

                tblToolStatus.Rows.Add(row);

                r++;
            }
        }

        //private void BindCurrentStatusTable()
        //{
        //    //SqlConnection cnSselData = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);
        //    DataTable dtToolStatus = null;
        //    using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
        //    {
        //        dtToolStatus = dba.ApplyParameters(new { Action = "ToolStatus" }).FillDataTable("sselScheduler_Select");
        //        //dtToolStatus.Columns.Add("InterlockStatus", typeof(string));
        //    }

        //    int i = 0;
        //    DataRow dr = null;
        //    string lastLab = string.Empty;
        //    string lastProcTech = string.Empty;
        //    while (i <= dtToolStatus.Rows.Count - 1)
        //    {
        //        if (dtToolStatus.Rows[i]["LabName"].ToString() != lastLab)
        //        {
        //            lastLab = dtToolStatus.Rows[i]["LabName"].ToString();
        //            dr = dtToolStatus.NewRow();
        //            dr["LabName"] = lastLab;
        //            dr["ProcessTechName"] = null;
        //            dr["ResourceName"] = null;
        //            dtToolStatus.Rows.InsertAt(dr, i);
        //            i += 1;
        //        }

        //        if (dtToolStatus.Rows[i]["ProcessTechName"].ToString() != lastProcTech)
        //        {
        //            lastProcTech = dtToolStatus.Rows[i]["ProcessTechName"].ToString();
        //            dr = dtToolStatus.NewRow();
        //            dr["LabName"] = lastLab;
        //            dr["ProcessTechName"] = lastProcTech;
        //            dr["ResourceName"] = null;
        //            dtToolStatus.Rows.InsertAt(dr, i);
        //            i += 1;
        //        }

        //        i += 1;
        //    }

        //    WagoInterlock.AllToolStatus(dtToolStatus);

        //    dgToolStatus.DataSource = dtToolStatus;
        //    dgToolStatus.DataBind();
        //}

        protected void DgToolStatus_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                TableRow tr = (TableRow)((Label)e.Item.FindControl("lblResourceName")).Parent.Parent;
                if (drv["ProcessTechName"] == DBNull.Value) // new lab row
                {
                    tr.CssClass = "OutlineLevel1";
                    ((Label)e.Item.FindControl("lblResourceName")).Text = drv["LabName"].ToString();
                }
                else if (drv["ResourceName"] == DBNull.Value) // new processtech row
                {
                    tr.CssClass = "OutlineLevel2";
                    ((Label)e.Item.FindControl("lblResourceName")).Text = "&nbsp; &nbsp; &nbsp; " + drv["ProcessTechName"].ToString();
                }
                else
                {
                    if (e.Item.ItemType == ListItemType.Item)
                        tr.CssClass = "Item";
                    else
                        tr.CssClass = "AlternatingItem";
                    ((Label)e.Item.FindControl("lblResourceName")).Text = "&nbsp; &nbsp; &nbsp; &nbsp; &nbsp; &nbsp; " + drv["ResourceName"].ToString();
                    ((Label)e.Item.FindControl("lblStatus")).Text = "";// ResourceState[Utility.ConvertTo(drv["State"], 0)];
                    ((Label)e.Item.FindControl("lblActivity")).Text = drv["CurrentActivity"].ToString();

                    try
                    {
                        if (drv["InterlockStatus"].ToString() == "Tool Disabled")
                            ((Label)e.Item.FindControl("lblInterlock")).Text = "Tool Off";
                        else if (drv["InterlockStatus"].ToString() == "Tool Enabled")
                            ((Label)e.Item.FindControl("lblInterlock")).Text = "Tool On";
                        else
                            ((Label)e.Item.FindControl("lblInterlock")).Text = drv["InterlockStatus"].ToString();
                    }
                    catch
                    {
                        ((Label)e.Item.FindControl("lblInterlock")).Text = "NULL";
                    }
                }
            }
        }

        protected void DdlReportType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ddlReportType.SelectedValue == "Current")
            {
                gvFuture.Visible = false;
                dgToolStatus.Visible = true;
                FillCurrentStatusTable();
            }
            else
            {
                gvFuture.Visible = true;
                dgToolStatus.Visible = false;
                BindFutureStatusTable();
            }
        }

        //Create the table that shows repair/maintenace status for every tool in next 7 days
        private void BindFutureStatusTable()
        {
            gvFuture.DataSource = SchedulerTool.GetFutureToolStatus();
            gvFuture.DataBind();
        }
    }
}