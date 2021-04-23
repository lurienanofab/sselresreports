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
        public static ExecuteReaderResult GetAllToolsFromScheduler()
        {
            var reader = DataCommand.Create()
                .Param("Action", "AllResources")
                .ExecuteReader("dbo.sselScheduler_Select");

            return reader;
        }

        public static IEnumerable<ListItem> GetToolSelectItems(bool includeSelectItem = false)
        {
            List<ListItem> result = new List<ListItem>();
            using (var reader = GetAllToolsFromScheduler())
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
            var dt = DataCommand.Create()
                .Param("Action", "SelectFutureToolStatus")
                .FillDataTable("sselScheduler.dbo.procReservationSelect");

            return dt;
        }

        public static DataTable GetReservationsByClientID(int year, int month, int clientId)
        {
            DateTime sDate = new DateTime(year, month, 1);
            DateTime eDate = sDate.AddMonths(1);

            var dt = DataCommand.Create()
                .Param("@Action", "ReservationByClientID")
                .Param("@sDate", sDate)
                .Param("@eDate", eDate)
                .Param("@ClientID", clientId)
                .FillDataTable("dbo.sselScheduler_Select");

            return dt;
        }
    }
}
