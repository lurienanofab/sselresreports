using LNF;
using LNF.Data;
using LNF.Impl.Repository.Data;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace sselResReports.AppCode.DAL
{
    public static class ToolUsageSummary
    {
        public static DataTable GetActivatedReservations(int month, int year, int numMonths, int resourceId, string selectedAccountTypes)
        {
            DateTime sdate = new DateTime(year, month, 1);
            DateTime edate = sdate.AddMonths(numMonths);

            var dt = DataCommand.Create()
                .Param("Action", "ToolUsageSummary")
                .Param("ResourceID", resourceId)
                .Param("StartPeriod", sdate)
                .Param("EndPeriod", edate)
                .Param("ToolUsageSummaryOption", "Activated")
                .Param("AccountTypeIDs", selectedAccountTypes)
                .FillDataTable("dbo.ToolBilling_Select");

            return dt;
        }

        public static DataTable GetUnactivatedReservations(int month, int year, int numMonths, int resourceId, string selectedAccountTypes)
        {
            DateTime sdate = new DateTime(year, month, 1);
            DateTime edate = sdate.AddMonths(numMonths);

            var dt = DataCommand.Create()
                .Param("Action", "ToolUsageSummary")
                .Param("ResourceID", resourceId)
                .Param("StartPeriod", sdate)
                .Param("EndPeriod", edate)
                .Param("ToolUsageSummaryOption", "Unactivated")
                .Param("AccountTypeIDs", selectedAccountTypes)
                .FillDataTable("dbo.ToolBilling_Select");

            return dt;
        }

        public static DataTable GetForgivenReservations(int month, int year, int numMonths, int resourceId, string selectedAccountTypes)
        {
            DateTime sdate = new DateTime(year, month, 1);
            DateTime edate = sdate.AddMonths(numMonths);

            var dt = DataCommand.Create()
                .Param("Action", "ToolUsageSummary")
                .Param("ResourceID", resourceId)
                .Param("StartPeriod", sdate)
                .Param("EndPeriod", edate)
                .Param("ToolUsageSummaryOption", "Forgiven")
                .Param("AccountTypeIDs", selectedAccountTypes)
                .FillDataTable("dbo.ToolBilling_Select");

            return dt;
        }

        public static DataTable GetCombinedReservations(int month, int year, int numMonths, int resourceId, string selectedAccountTypes)
        {
            DateTime sdate = new DateTime(year, month, 1);
            DateTime edate = sdate.AddMonths(numMonths);

            var dt = DataCommand.Create()
                .Param("Action", "ToolUsageSummary")
                .Param("ResourceID", resourceId)
                .Param("StartPeriod", sdate)
                .Param("EndPeriod", edate)
                .Param("ToolUsageSummaryOption", "Combined")
                .Param("AccountTypeIDs", selectedAccountTypes)
                .FillDataTable("dbo.ToolBilling_Select");

            return dt;
        }

        public static DataTable GetToolHourlyRateByPeriod(int month, int year, int numMonths, int resourceId, IEnumerable<IChargeType> chargeTypes)
        {
            DateTime sd = new DateTime(year, month, 1);
            DateTime ed = sd.AddMonths(numMonths);
            string sql = "SELECT * FROM dbo.udf_GetToolHourlyRateByPeriod(@sd, @ed, @ResourceID)";

            var dt = DataCommand.Create(CommandType.Text)
                .Param("sd", sd)
                .Param("ed", ed)
                .Param("ResourceID", resourceId)
                .FillDataTable(sql);

            DataTable result = new DataTable();
            result.Columns.Add("ChargeTypeID", typeof(int));
            result.Columns.Add("ChargeTypeName", typeof(string));

            DateTime d = sd;
            int index = 0;

            while (d < ed)
            {
                result.Columns.Add(string.Format("PERIOD_{0}", index), typeof(double));
                index += 1;
                d = sd.AddMonths(index);
            }

            foreach (var ct in chargeTypes)
            {
                DataRow ndr = result.NewRow();
                ndr["ChargeTypeID"] = ct.ChargeTypeID;
                ndr["ChargeTypeName"] = ct.ChargeTypeName;

                d = sd;
                index = 0;

                while (d < ed)
                {
                    DataRow[] rows = dt.Select($"ChargeTypeID = {ct.ChargeTypeID} AND Period = #{d}#");

                    if (rows.Length == 0)
                    {
                        ndr[$"PERIOD_{index}"] = 0;
                    }
                    else if (rows.Length == 1)
                    {
                        DataRow dr = rows[0];
                        ndr[$"PERIOD_{index}"] = dr["HourlyRate"];
                    }
                    else
                        throw new Exception($"There is more than one row for ChargeTypeID = {ct.ChargeTypeID} and Period = #{d}#");

                    index += 1;

                    d = sd.AddMonths(index);
                }

                result.Rows.Add(ndr);
            }

            return result;
        }
    }
}
