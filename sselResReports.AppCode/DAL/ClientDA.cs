using LNF.Data;
using LNF.Repository;
using System;
using System.Data;

namespace sselResReports.AppCode.DAL
{
    public static class ClientDA
    {
        public static DataTable GetAllClientsbyPeriod(int year, int month)
        {
            DateTime sDate = new DateTime(year, month, 1);
            DateTime eDate = sDate.AddMonths(1);
            var privs = ClientPrivilege.LabUser | ClientPrivilege.Staff;

            var dt = DataCommand.Create()
                .Param("Action", "All")
                .Param("sDate", sDate)
                .Param("eDate", eDate)
                .Param("Privs", Convert.ToInt32(privs))
                .FillDataTable("dbo.Client_Select");

            return dt;
        }
    }
}
