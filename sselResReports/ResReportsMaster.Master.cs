using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace sselResReports
{
    public partial class ResReportsMaster : LNF.Web.Content.LNFMasterPage
    {
        public override bool ShowMenu
        {
            get
            {
                return bool.Parse(ConfigurationManager.AppSettings["ShowMenu"]) || Request.QueryString["menu"] == "1";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}