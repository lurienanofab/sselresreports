using LNF.CommonTools;
using LNF.Repository;
using System;
using System.Data;

namespace sselResReports.AppCode.DAL
{
    public static class ReservationDA
    {
        public static DataTable GetReservationsByResourceIDAndDate(int resourceId, DateTime period)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselScheduler"))
            {
                dba.SelectCommand
                    .AddParameter("@Action", "SelectByResourceIDAndDate")
                    .AddParameter("@ResourceID", resourceId)
                    .AddParameter("@BeginDateTime", period);
		        return dba.FillDataTable("procReservationSelect");
            }
	    }
    }
}
