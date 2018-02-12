using LNF.CommonTools;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI.WebControls;

namespace sselResReports.AppCode.DAL
{
    public static class SchedulerTool
    {
        public static IDataReader GetAllToolsFromScheduler()
        {
            SQLDBAccess dba = new SQLDBAccess("cnSselData");
            return dba.ApplyParameters(new { Action = "AllResources" }).ExecuteReader("sselScheduler_Select");
        }

        public static IEnumerable<ListItem> GetToolSelectItems(bool includeSelectItem = false)
        {
            List<ListItem> result = new List<ListItem>();
            using (IDataReader reader = SchedulerTool.GetAllToolsFromScheduler())
            {
                while (reader.Read())
                    result.Add(new ListItem(reader["ResourceName"].ToString(), reader["ResourceID"].ToString()));
                reader.Close();
            }
            IList<ListItem> ordered = result.OrderBy(x => x.Text).ToList();
            if (includeSelectItem)
                ordered.Insert(0, new ListItem("-- Select --", "0"));
            return ordered;
        }

        public static DataTable GetFutureToolStatus()
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselScheduler"))
                return dba.ApplyParameters(new { Action = "SelectFutureToolStatus" }).FillDataTable("procReservationSelect");
        }

        public static DataTable GetReservationsByClientID(int year, int month, int clientId)
        {
            DateTime sDate = new DateTime(year, month, 1);
            DateTime eDate = sDate.AddMonths(1);

            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.SelectCommand
                    .AddParameter("@Action", "ReservationByClientID")
                    .AddParameter("@sDate", sDate)
                    .AddParameter("@eDate", eDate)
                    .AddParameter("@ClientID", clientId);
                return dba.FillDataTable("sselScheduler_Select");
            }
        }
    }
}
