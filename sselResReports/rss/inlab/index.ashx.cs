using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using sselResReports.AppCode;
using sselResReports.AppCode.DAL;
using sselResReports.AppCode.Rss;

namespace sselResReports.rss.inlab
{
    public class index : IHttpHandler
    {
        private RssFeed rss = new RssFeed();
        private HttpContext currentContext;

        public void ProcessRequest(HttpContext context)
        {
            currentContext = context;

            int a = 0;
            if (!int.TryParse(currentContext.Request.QueryString["rid"], out a))
                a = 0;

            string room = (a == 1) ? "Wet Chemistry" : "Clean Room";

            PopulateRss(room);

            currentContext.Response.ContentType = "application/rss+xml";
            currentContext.Response.Write(rss.ToString());
        }

        private void PopulateRss(string roomName, string channelName = "", string userName = "")
        {
            rss.Channel = new Channel();
            rss.Version = "2.0";
            rss.Channel.Title = string.Format("LNF Users in {0}", roomName);
            rss.Channel.PubDate = DateTime.Now;
            rss.Channel.LastBuildDate = DateTime.Now;
            rss.Channel.WebMaster = "lnf-it@umich.com";
            rss.Channel.Description = "Users currently in one of the LNF lab areas";
            rss.Channel.Link = GetFullUrl("~/AccInLab.aspx");

            if (!string.IsNullOrEmpty(channelName))
            {
                rss.Channel.Title += " '" + channelName + "'";
                if (!string.IsNullOrEmpty(userName))
                    rss.Channel.Title += string.Format(" (generated for {0})", userName);
            }

            rss.Channel.Items = RoomDataDA.GetCurrentUsersInRoom(roomName).Select(CreateRssItem).ToList();
        }

        private Item CreateRssItem(InLabClient x)
        {
            return new Item()
            {
                Title = x.FullName,
                Description = string.Format("{0:#.00 hours}", x.Duration),
                Link = x.CardNumber,
                PubDate = x.EventDateTime,
                Guid = x.ClientID.ToString()
            };
        }

        private string GetFullUrl(string virtualPath)
        {
            string result = string.Empty;
            string absolutePath = VirtualPathUtility.ToAbsolute(virtualPath);
            result += currentContext.Request.Url.GetLeftPart(UriPartial.Authority);
            result += absolutePath;
            return result;
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