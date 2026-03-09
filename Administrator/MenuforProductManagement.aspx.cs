using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class MenuforProductManagement : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void toNewProduct_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/NewProduct.aspx");
        }
        protected void toProducts_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/Products.aspx");
        }
        protected void toAddPhotos_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/PhotoAddforProduct.aspx");
        }
        protected void toListing_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/Listing.aspx");
        }
        protected void toKeywords_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/Keywords.aspx");
        }
        protected void toKeyWordsAssign_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/KeywordsAssigned.aspx");
        }
        protected void toListingDescription_click(object sender, EventArgs e)
        {
            //Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }
    }
}