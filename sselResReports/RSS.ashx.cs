using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace sselResReports
{
    public class _RSS : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.Redirect("~/rss/inlab" + context.Request.Url.Query);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}