﻿using LNF.Models.Data;
using sselResReports.AppCode;
using System;

namespace sselResReports
{
    public partial class ResReservationDetails : ReportPage
    {
        public override ClientPrivilege AuthTypes
        {
            get{ return ClientPrivilege.Administrator; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnReport_Click(object sender, EventArgs e)
        {
            gvRes.DataSourceID = "odsRes";
            gvRes.DataBind();
        }
    }
}