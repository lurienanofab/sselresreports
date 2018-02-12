using LNF;
using OnlineServices.Api.Control;
using System.Web;

namespace sselResReports.ajax
{
    /// <summary>
    /// Summary description for index
    /// </summary>
    public class index : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            string command = context.Request.QueryString["command"];

            object result = null;

            switch (command)
            {
                case "blocks":
                    using (var cc = new ControlClient())
                    {
                        var blocks = cc.GetAllBlockStates().Result;
                        result = new { Error = false, Message = string.Empty, Blocks = blocks };
                    }
                    break;
                case "blockstate":
                    using (var cc = new ControlClient())
                    {
                        int blockId;
                        if (int.TryParse(context.Request.QueryString["blockId"], out blockId))
                        {
                            var blockState = cc.GetBlockState(blockId).Result;
                            result = new { Error = false, Message = string.Empty, BlockState = blockState };
                        }
                        else
                            result = new { Error = true, Message = "Missing parameter blockId" };
                    }
                    break;
                default:
                    result = new { Error = true, Message = "Invalid command" };
                    break;
            }

            context.Response.Write(Providers.Serialization.Json.SerializeObject(result));
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}