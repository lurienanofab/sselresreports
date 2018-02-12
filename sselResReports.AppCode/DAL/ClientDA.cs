using LNF.CommonTools;
using LNF.Models.Data;
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

            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
            {
                dba.SelectCommand
                    .AddParameter("@Action", "All")
                    .AddParameter("@sDate", sDate)
                    .AddParameter("@eDate", eDate)
                    .AddParameter("@Privs", Convert.ToInt32(privs));
		        return dba.FillDataTable("Client_Select");
            }
	    }
    }
}
