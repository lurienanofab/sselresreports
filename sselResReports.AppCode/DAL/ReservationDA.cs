using LNF.Repository;
using System;
using System.Data;

namespace sselResReports.AppCode.DAL
{
    public static class ReservationDA
    {
        public static DataTable GetReservationsByResourceIDAndDate(int resourceId, DateTime period)
        {
            var dt = DataCommand.Create()
                .Param("Action", "SelectByResourceIDAndDate")
                .Param("ResourceID", resourceId)
                .Param("BeginDateTime", period)
                .FillDataTable("sselScheduler.dbo.procReservationSelect");

            return dt;
	    }
    }
}
