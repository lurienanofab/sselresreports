using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using LNF.Web;
using LNF.Web.Content;
using LNF.Repository;
using LNF.Repository.Data;

namespace sselResReports.AppCode
{
    public abstract class ReportPage : LNFPage
    {
        protected void BackButton_Click(object sender, EventArgs e)
        {
            Session["Updated"] = null;
            Cache.Remove(Session["Cache"].ToString()); // remove anything left in cache
            Response.Redirect("~");
        }

        protected void FillYearSelect(DropDownList ddl, int startYear = 2003)
        {
            WebUtility.BindYearData(ddl, startYear);
        }

        public static void CreateAccountCheckList(CheckBoxList cblAccountType)
        {
            AccountType[] allAccountTypes = DA.Current.Query<AccountType>().ToArray();
            cblAccountType.DataSource = allAccountTypes.Select(x => new ListItem { Text = x.AccountTypeName, Value = x.AccountTypeID.ToString() });
            cblAccountType.DataBind();
        }

        /// <summary>
        /// Gets a comma separated list of selected AccountTypeIDs
        /// </summary>
        public static string GetSelectedAccountTypes(CheckBoxList cblAccountType)
        {
            var selectedAccountTypes = new List<string>();

            foreach (ListItem li in cblAccountType.Items)
            {
                if (li.Selected)
                    selectedAccountTypes.Add(li.Value);
            }

            return string.Join(",", selectedAccountTypes);
        }

        public T ReadReportOptionsFromCookie<T>(string cookieName) where T : new()
        {
            if (Request.Cookies[cookieName] != null)
                return JsonConvert.DeserializeObject<T>(Request.Cookies[cookieName].Value);
            else
                return new T();

            //bool haveAccountCookie = false;

            //// read from cookies
            //if (request.Cookies[cookieName] != null)
            //{
            //    if (request.Cookies[cookieName][ACCOUNT_TYPES] != null)
            //    {
            //        string[] cookieAccountTypes = request.Cookies[cookieName][ACCOUNT_TYPES].Split(',');
            //        foreach (ListItem item in cblAccountType.Items)
            //        {
            //            if (cookieAccountTypes.Contains(item.Value))
            //            {
            //                item.Selected = true;
            //                haveAccountCookie = true;
            //            }
            //        }
            //    }
            //}

            //if (haveAccountCookie == false)  // if no account type previously selected then select all
            //{
            //    foreach (ListItem item in cblAccountType.Items)
            //    {
            //        item.Selected = true;
            //    }
            //}
        }

        public void WriteReportOptionsToCookie(object options, string cookieName)
        {
            var value = JsonConvert.SerializeObject(options);
            Response.Cookies[cookieName].Value = value;
        }
    }
}
