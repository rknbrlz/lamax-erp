using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class WebForm2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //Page.ClientScript.RegisterOnSubmitStatement(typeof(Page), "closePage", "window.onunload = CloseWindow();");

            if (!IsPostBack)
            {
                Response.AppendHeader("Refresh", "2;url=/Administrator/Orders.aspx");
                //Response.AppendHeader("Refresh", "2;url=/Administrator/MenuforAdmin.aspx");
                //Response.Redirect("~/Administrator/MenuforAdmin.aspx");
            }         
        }
    }
}