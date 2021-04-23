using LNF;
using System.Web;

namespace sselResReports.Ajax
{
    /// <summary>
    /// Summary description for index
    /// </summary>
    public class Index : IHttpHandler
    {
        [Inject] public IProvider Provider { get; set; }

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";

            string command = context.Request.QueryString["command"];

            object result = null;

            switch (command)
            {
                case "blocks":
                    var points = Provider.Control.GetToolStatus();
                    result = new { Error = false, Message = string.Empty, Points = points };
                    break;
                case "blockstate":
                    int blockId;
                    if (int.TryParse(context.Request.QueryString["blockId"], out blockId))
                    {
                        var blockState = Provider.Control.GetBlockState(blockId);
                        result = new { Error = false, Message = string.Empty, BlockState = blockState };
                    }
                    else
                        result = new { Error = true, Message = "Missing parameter blockId" };
                    break;
                default:
                    result = new { Error = true, Message = "Invalid command" };
                    break;
            }

            context.Response.Write(Provider.Utility.Serialization.Json.SerializeObject(result));
        }

        public bool IsReusable
        {
            get { return false; }
        }
    }
}