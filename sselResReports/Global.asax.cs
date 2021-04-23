using LNF.Impl;
using LNF.Web;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Compilation;
using System.Web.Security;

namespace sselResReports
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup

            Assembly[] assemblies = BuildManager.GetReferencedAssemblies().Cast<Assembly>().ToArray();

            // setup up dependency injection container
            var wcc = new WebContainerConfiguration(WebApp.Current.Container);
            wcc.EnablePropertyInjection();
            wcc.RegisterAllTypes();

            // setup web dependency injection
            WebApp.Current.Bootstrap(assemblies);

            Application["AppServer"] = "/";
        }

        void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // Fires upon attempting to authenticate the use
            if (Request.IsAuthenticated)
            {
                FormsIdentity identity = (FormsIdentity)User.Identity;
                string[] roles = identity.Ticket.UserData.Split('|');
                Context.User = new System.Security.Principal.GenericPrincipal(identity, roles);
            }
        }

        void Session_Start(object sender, EventArgs e)
        {
            if (Session["ClientID"] != null)
            {
                Session.Abandon();
                Response.Redirect("~");
            }

            Session["Logout"] = Application["AppServer"].ToString() + "sselOnLine/Login.aspx";
            Session["NoAccess"] = Application["AppServer"].ToString() + "sselOnLine/Information.aspx";

            // remember - to get here, the user is already authenticated
            SqlConnection cnSselData = new SqlConnection(ConfigurationManager.ConnectionStrings["cnSselData"].ConnectionString);

            SqlCommand cmdCheck_Login = new SqlCommand("Client_CheckAuth", cnSselData);
            cmdCheck_Login.CommandType = CommandType.StoredProcedure;
            cmdCheck_Login.Parameters.AddWithValue("@Action", "GetSessionInfo");
            cmdCheck_Login.Parameters.AddWithValue("@UserName", Context.User.Identity.Name);

            // check if record exists in DB
            cnSselData.Open();
            SqlDataReader myRdr = cmdCheck_Login.ExecuteReader(CommandBehavior.CloseConnection);

            // if this doesn't return true... 
            if (myRdr.Read())
            {
                Session["ClientID"] = myRdr["ClientID"];
                Session["DisplayName"] = myRdr["DisplayName"];
                Session["Privs"] = myRdr["Privs"];
                Session["OrgID"] = myRdr["OrgID"];
                Session["Cache"] = Guid.NewGuid().ToString();
            }
            else
                Response.Redirect(Session["Logout"].ToString());
            myRdr.Close();
        }
    }
}
