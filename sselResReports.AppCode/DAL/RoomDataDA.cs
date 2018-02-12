using LNF;
using LNF.CommonTools;
using LNF.PhysicalAccess;
using LNF.Repository;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace sselResReports.AppCode.DAL
{
    public static class RoomDataDA
    {
        public static IList<InLabClient> GetCurrentUsersInRoom(string AreaName)
        {
            IList<Badge> inlab = Providers.PhysicalAccess.CurrentlyInArea().ToList();
            List<InLabClient> result = inlab
                .Where(x => x.CurrentAreaName == AreaName)
                .Select(x => new InLabClient(x))
                .OrderBy(x => x.LastName)
                .ToList();
            return result;
        }

        public static DataTable GetCleanRoomAccessData(DateTime period)
        {
            using (SQLDBAccess dba = new SQLDBAccess("cnSselData"))
                return dba.ApplyParameters(new { Action = "ForReservationAbnormality", sDate = period }).FillDataTable("RoomDataClean_Select");
        }
    }
}
