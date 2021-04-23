using LNF;
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
            IList<Badge> inlab = ServiceProvider.Current.PhysicalAccess.GetCurrentlyInArea("all").ToList();

            List<InLabClient> result = inlab
                .Where(x => x.CurrentAreaName == AreaName)
                .Select(x => new InLabClient(x))
                .OrderBy(x => x.LastName)
                .ToList();

            return result;
        }

        public static DataTable GetCleanRoomAccessData(DateTime period)
        {
            var dt = DataCommand.Create()
                .Param("Action", "ForReservationAbnormality")
                .Param("sDate", period)
                .FillDataTable("dbo.RoomDataClean_Select");

            return dt;
        }
    }
}
