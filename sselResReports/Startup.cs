﻿using LNF.Web;
using Microsoft.Owin;
using Owin;

namespace sselResReports
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseDataAccess();
        }
    }
}