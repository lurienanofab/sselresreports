using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using LNF.CommonTools;
using LNF.Repository;
using LNF.Repository.Data;

namespace sselResReports.AppCode.DAL
{
    public static class ToolUsageSummary
    {
        public static DataTable GetActivatedReservations(int month, int year, int numMonths, int resourceId, string selectedAccountTypes)
        {
            DateTime sdate = new DateTime(year, month, 1);
            DateTime edate = sdate.AddMonths(numMonths);
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.SelectCommand
                    .AddParameter("@Action", "ToolUsageSummary")
                    .AddParameter("@ResourceID", resourceId)
                    .AddParameter("@StartPeriod", sdate)
                    .AddParameter("@EndPeriod", edate)
                    .AddParameter("@ToolUsageSummaryOption", "Activated")
                    .AddParameter("@AccountTypeIDs", selectedAccountTypes);
                return dba.FillDataTable("ToolBilling_Select");
            }
        }

        public static DataTable GetUnactivatedReservations(int month, int year, int numMonths, int resourceId, string selectedAccountTypes)
        {
            DateTime sdate = new DateTime(year, month, 1);
            DateTime edate = sdate.AddMonths(numMonths);
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.SelectCommand
                    .AddParameter("@Action", "ToolUsageSummary")
                    .AddParameter("@ResourceID", resourceId)
                    .AddParameter("@StartPeriod", sdate)
                    .AddParameter("@EndPeriod", edate)
                    .AddParameter("@ToolUsageSummaryOption", "Unactivated")
                    .AddParameter("@AccountTypeIDs", selectedAccountTypes);
                return dba.FillDataTable("ToolBilling_Select");
            }

        }

        public static DataTable GetForgivenReservations(int month, int year, int numMonths, int resourceId, string selectedAccountTypes)
        {
            DateTime sdate = new DateTime(year, month, 1);
            DateTime edate = sdate.AddMonths(numMonths);
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.SelectCommand
                    .AddParameter("@Action", "ToolUsageSummary")
                    .AddParameter("@ResourceID", resourceId)
                    .AddParameter("@StartPeriod", sdate)
                    .AddParameter("@EndPeriod", edate)
                    .AddParameter("@ToolUsageSummaryOption", "Forgiven")
                    .AddParameter("@AccountTypeIDs", selectedAccountTypes);
                return dba.FillDataTable("ToolBilling_Select");
            }
        }

        public static DataTable GetCombinedReservations(int month, int year, int numMonths, int resourceId, string selectedAccountTypes)
        {
            DateTime sdate = new DateTime(year, month, 1);
            DateTime edate = sdate.AddMonths(numMonths);
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.SelectCommand
                    .AddParameter("@Action", "ToolUsageSummary")
                    .AddParameter("@ResourceID", resourceId)
                    .AddParameter("@StartPeriod", sdate)
                    .AddParameter("@EndPeriod", edate)
                    .AddParameter("@ToolUsageSummaryOption", "Combined")
                    .AddParameter("@AccountTypeIDs", selectedAccountTypes);
                return dba.FillDataTable("ToolBilling_Select");
            }
        }

        public static DataTable GetToolHourlyRateByPeriod(int Month, int Year, int NumMonths, int ResourceID)
        {
            DateTime sdate = new DateTime(Year, Month, 1);
            DateTime edate = sdate.AddMonths(NumMonths);
            string sql = "SELECT * FROM dbo.udf_GetToolHourlyRateByPeriod(@sdate, @edate, @ResourceID)";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString))
            using (SqlCommand cmd = new SqlCommand(sql, conn))
            using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
            {
                adap.SelectCommand.Parameters.AddWithValue("@sdate", sdate);
                adap.SelectCommand.Parameters.AddWithValue("@edate", edate);
                adap.SelectCommand.Parameters.AddWithValue("@ResourceID", ResourceID);

                DataTable dt = new DataTable();
                adap.Fill(dt);

                DataTable result = new DataTable();
                result.Columns.Add("ChargeTypeID", typeof(int));
                result.Columns.Add("ChargeTypeName", typeof(string));
                DateTime d = sdate;
                int index = 0;
                while (d < edate)
                {
                    result.Columns.Add(string.Format("PERIOD_{0}", index), typeof(double));
                    index += 1;
                    d = sdate.AddMonths(index);
                }

                var chargeTypes = DA.Current.Query<ChargeType>().ToArray();

                foreach (ChargeType ct in chargeTypes)
                {
                    DataRow ndr = result.NewRow();
                    ndr["ChargeTypeID"] = ct.ChargeTypeID;
                    ndr["ChargeTypeName"] = ct.ChargeTypeName;
                    d = sdate;
                    index = 0;
                    while (d < edate)
                    {
                        DataRow[] rows = dt.Select(string.Format("ChargeTypeID = {0} AND Period = '{1}'", ct.ChargeTypeID, d));
                        if (rows.Length == 1)
                        {
                            DataRow dr = rows[0];
                            ndr[string.Format("PERIOD_{0}", index)] = dr["HourlyRate"];
                        }
                        else
                            throw new Exception(string.Format("There is more than one row for ChargeTypeID = {0} and Period = '{1}'", ct.ChargeTypeID, d));
                        index += 1;
                        d = sdate.AddMonths(index);
                    }
                    result.Rows.Add(ndr);
                }

                return result;
            }
        }
    }
}
