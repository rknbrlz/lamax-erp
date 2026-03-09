using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class Listing : System.Web.UI.Page
    {
        String strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        public string query, constr;
        public SqlCommand com;
        public SqlConnection con;

        public void connection()
        {

            constr = ConfigurationManager.ConnectionStrings["constr"].ToString();
            con = new SqlConnection(constr);
            con.Open();

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            //Response.AppendHeader("Refresh", "60");

            if (!IsPostBack)
            {
                if (this.Page.User.Identity.IsAuthenticated)
                {
                    string username = this.Page.User.Identity.Name;
                    lblLoginName.Text = username;

                    if (lblLoginName.Text == "Label")
                    {
                        Response.Redirect("~/Login.aspx");
                    }
                    else
                    {
                        //FillGrid();
                        //BindValue();
                        KeywordFillGrid();
                        KeywordFillGridAmazon();
                        //TitleData();
                    }
                }
                if (lblLoginName.Text == "Label")
                {
                    Response.Redirect("~/Login.aspx");
                }
                else
                {

                }
            }
            if (lblLoginName.Text == "Label")
            {
                Response.Redirect("~/Login.aspx");
            }
            else
            {

            }
        }
        //private DataTable FillGrid()
        //{
        //    string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(strConnString))
        //    {
        //        using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM V_Listing ORDER BY Title_Etsy desc", con))
        //        {
        //            DataTable dt = new DataTable();
        //            sda.Fill(dt);
        //            gvProduct.DataSource = dt;
        //            gvProduct.DataBind();

        //            return dt;
        //        }
        //    }
        //}
        private void LoadPhoto()
        {
            {
                string s = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                SqlConnection con = new SqlConnection(s);

                SqlCommand cmd1 = new SqlCommand("select Top (1) * from V_Product where SKU='" + lblSKU.Text + "'", con);
                //SqlCommand cmd1 = new SqlCommand(("SELECT Top (1) SatID FROM V_LastSatID where ClaimID='" + lblLoginName.Text + "'"' order by SatID desc")', con))

                cmd1.Connection = con;

                con.Open();



                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())

                {

                    if (!Convert.IsDBNull(dr1["Photo"]))
                    {
                        //string imageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dr1["Photo"]);
                        ProductPhoto.ImageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dr1["Photo"]);
                    }
                }

                dr1.Close();

                con.Close();
            }
        }
        protected void gvProduct_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "&nbsp;")
                    {
                        e.Row.Cells[i].Text = "---";
                    }
                }

                Label lblstrEtsy = (Label)e.Row.FindControl("lblEtsy");
                Label lblstrEtsyOk = (Label)e.Row.FindControl("lblEtsyOk");
                Label lblstrEtsyNok = (Label)e.Row.FindControl("lblEtsyNok");

                if (lblstrEtsy.Text == "Yes")
                {
                    lblstrEtsyOk.Visible = true;
                    lblstrEtsyNok.Visible = false;
                }
                else if (lblstrEtsy.Text == "No")
                {
                    lblstrEtsyOk.Visible = false;
                    lblstrEtsyNok.Visible = true;
                }

                Label lblstrAmazon = (Label)e.Row.FindControl("lblAmazon");
                Label lblstrAmazonOk = (Label)e.Row.FindControl("lblAmazonOk");
                Label lblstrAmazonNok = (Label)e.Row.FindControl("lblAmazonNok");

                if (lblstrAmazon.Text == "Yes")
                {
                    lblstrAmazonOk.Visible = true;
                    lblstrAmazonNok.Visible = false;
                }
                else if (lblstrAmazon.Text == "No")
                {
                    lblstrAmazonOk.Visible = false;
                    lblstrAmazonNok.Visible = true;
                }

                Label lblstrEbay = (Label)e.Row.FindControl("lblEbay");
                Label lblstrEbayOk = (Label)e.Row.FindControl("lblEbayOk");
                Label lblstrEbayNok = (Label)e.Row.FindControl("lblEbayNok");

                if (lblstrEbay.Text == "Yes")
                {
                    lblstrEbayOk.Visible = true;
                    lblstrEbayNok.Visible = false;
                }
                else if (lblstrEbay.Text == "No")
                {
                    lblstrEbayOk.Visible = false;
                    lblstrEbayNok.Visible = true;
                }

                Label lblstrWix = (Label)e.Row.FindControl("lblWix");
                Label lblstrWixOk = (Label)e.Row.FindControl("lblWixOk");
                Label lblstrWixNok = (Label)e.Row.FindControl("lblWixNok");

                if (lblstrWix.Text == "Yes")
                {
                    lblstrWixOk.Visible = true;
                    lblstrWixNok.Visible = false;
                }
                else if (lblstrWix.Text == "No")
                {
                    lblstrWixOk.Visible = false;
                    lblstrWixNok.Visible = true;
                }

            }
        }
        protected void gvProduct_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //FillGrid();
            gvProduct.PageIndex = e.NewPageIndex;
            gvProduct.DataBind();
        }
        protected void gvProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblSKUforWhere.Text = gvProduct.SelectedRow.Cells[1].Text;

            //LoadPhoto();
            KeywordFillGrid();
            KeywordFillGridAmazon();
            //TitleData();

            {
                string s = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                SqlConnection con = new SqlConnection(s);

                SqlCommand cmd1 = new SqlCommand("select Top (1) * from V_Product where SKU='" + lblSKUforWhere.Text + "'", con);
                //SqlCommand cmd1 = new SqlCommand(("SELECT Top (1) SatID FROM V_LastSatID where ClaimID='" + lblLoginName.Text + "'"' order by SatID desc")', con))

                cmd1.Connection = con;

                con.Open();

                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())

                {
                    lblPhotoCheck.Text = dr1["PhotoCheck"].ToString();

                    if (lblPhotoCheck.Text == "PhotoOK")
                    {
                        KeywordFillGrid();
                        KeywordFillGridAmazon();
                        LoadPhoto();
                        ProductPhoto.Visible = true;
                        Noimage.Visible = false;
                        //TitleData();
                    }
                    else if (lblPhotoCheck.Text == "PhotoNOK")
                    {
                        KeywordFillGrid();
                        KeywordFillGridAmazon();
                        ProductPhoto.Visible = false;
                        Noimage.Visible = true;
                        //TitleData();
                    }

                    //containerSKU.Visible = true;
                    //containerDesc.Visible = true;
                    //UpdatePanel2.Visible = true;
                    //UpdatePanel4.Visible = true;

                }

                //dr1.Close();

                con.Close();
            }

            {
                string s = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                SqlConnection con = new SqlConnection(s);

                SqlCommand cmd1 = new SqlCommand("select Top (1) * from V_Listing where SKU='" + lblSKUforWhere.Text + "'", con);
                //SqlCommand cmd1 = new SqlCommand(("SELECT Top (1) SatID FROM V_LastSatID where ClaimID='" + lblLoginName.Text + "'"' order by SatID desc")', con))

                cmd1.Connection = con;

                con.Open();



                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())

                {
                    lblTitle.Text = dr1["Title"].ToString();
                    lblSKU.Text = dr1["SKU"].ToString();
                    txtEtsyDescription.Text = dr1["Description_Etsy"].ToString();
                    lblAmazonDescription.Text = dr1["Description_Amazon"].ToString();
                    txtEbayDescription.Text = dr1["Description_Ebay"].ToString();
                    txtWixDescription.Text = dr1["Description_Wix"].ToString();
                    lblSalePriceEtsy.Text = dr1["SalePrice_Etsy"].ToString();
                    lblSalePriceAmazon.Text = dr1["SalePrice_Amazon"].ToString();
                    lblSalePriceEbay.Text = dr1["SalePrice_Ebay"].ToString();
                    lblSalePriceWix.Text = dr1["SalePrice_Wix"].ToString();
                    lblKeywordsAmazonFormat.Text = dr1["AmazonKeywords"].ToString();
                    lblBulletPoints1.Text = dr1["BulletPoint1"].ToString();
                    lblBulletPoints2.Text = dr1["BulletPoint2"].ToString();
                    lblBulletPoints3.Text = dr1["BulletPoint3"].ToString();
                    lblBulletPoints4.Text = dr1["BulletPoint4"].ToString();
                    lblBulletPoints5.Text = dr1["BulletPoint5"].ToString();

                    txtTitle.Text = dr1["Title"].ToString();

                    int chars = txtTitle.Text.Length;
                    int ProfitPercentage = chars;
                    lblTitleCount.Text = ProfitPercentage.ToString();

                    txtEtsyDescriptionEdit.Text = dr1["Description_Etsy"].ToString();
                    txtEtsySalePriceEdit.Text = dr1["SalePrice_Etsy"].ToString();

                    txtAmazonDescriptionEdit.Text = dr1["Description_Amazon"].ToString();
                    txtAmazonSalePriceEdit.Text = dr1["SalePrice_Amazon"].ToString();
                    txtAmazonBulletPoint1.Text = dr1["BulletPoint1"].ToString();
                    txtAmazonBulletPoint2.Text = dr1["BulletPoint2"].ToString();
                    txtAmazonBulletPoint3.Text = dr1["BulletPoint3"].ToString();
                    txtAmazonBulletPoint4.Text = dr1["BulletPoint4"].ToString();
                    txtAmazonBulletPoint5.Text = dr1["BulletPoint5"].ToString();

                    txtEtsyListingEdit.Text = dr1["Etsy"].ToString();
                    txtAmazonListingEdit.Text = dr1["Amazon"].ToString();
                    txtEbayListingEdit.Text = dr1["Ebay"].ToString();
                    txtWixListingEdit.Text = dr1["Wix"].ToString();

                    txtEbayDescriptionEdit.Text = dr1["Description_Ebay"].ToString();
                    txtWixDescriptionEdit.Text = dr1["Description_Wix"].ToString();

                    txtEbaySalePriceEdit.Text = dr1["SalePrice_Ebay"].ToString();
                    txtWixSalePriceEdit.Text = dr1["SalePrice_Wix"].ToString();

                    //htmlLiteral.Text = lblAmazonDescription.Text;
                    //txtShippingPrice.Text = (Convert.ToDouble(dr1["ShippingPricePaidSeller"])).ToString();

                    if (lblTitle.Text == "")
                    {
                        lblTitle.Text = "There is no title!";
                    }
                    else if (lblTitle.Text == "&nbsp;")
                    {
                        lblTitle.Text = "There is no title!";
                    }

                    if (txtEtsyDescription.Text == "")
                    {
                        txtEtsyDescription.Text = "There is no description!";
                    }
                    else if (txtEtsyDescription.Text == "&nbsp;")
                    {
                        txtEtsyDescription.Text = "There is no description!";
                    }

                    if (lblAmazonDescription.Text == "")
                    {
                        lblAmazonDescription.Text = "There is no description!";
                    }
                    else if (lblAmazonDescription.Text == "&nbsp;")
                    {
                        lblAmazonDescription.Text = "There is no description!";
                    }

                    if (txtEbayDescription.Text == "")
                    {
                        txtEbayDescription.Text = "There is no description!";
                    }
                    else if (txtEbayDescription.Text == "&nbsp;")
                    {
                        txtEbayDescription.Text = "There is no description!";
                    }

                    if (txtWixDescription.Text == "")
                    {
                        txtWixDescription.Text = "There is no description!";
                    }
                    else if (txtWixDescription.Text == "&nbsp;")
                    {
                        txtWixDescription.Text = "There is no description!";
                    }

                    if (lblSalePriceEtsy.Text == "")
                    {
                        lblSalePriceEtsy.Text = "0,00";
                    }
                    else if (lblSalePriceEtsy.Text == "&nbsp;")
                    {
                        lblSalePriceEtsy.Text = "0,00";
                    }

                    if (lblSalePriceAmazon.Text == "")
                    {
                        lblSalePriceAmazon.Text = "0,00";
                    }
                    else if (lblSalePriceAmazon.Text == "&nbsp;")
                    {
                        lblSalePriceAmazon.Text = "0,00";
                    }

                    if (lblSalePriceEbay.Text == "")
                    {
                        lblSalePriceEbay.Text = "0,00";
                    }
                    else if (lblSalePriceEbay.Text == "&nbsp;")
                    {
                        lblSalePriceEbay.Text = "0,00";
                    }

                    if (lblSalePriceWix.Text == "")
                    {
                        lblSalePriceWix.Text = "0,00";
                    }
                    else if (lblSalePriceWix.Text == "&nbsp;")
                    {
                        lblSalePriceWix.Text = "0,00";
                    }
                }

                dr1.Close();

                con.Close();
            }



            foreach (GridViewRow row in gvProduct.Rows)
            {
                if (row.RowIndex == gvProduct.SelectedIndex)
                {
                    row.BackColor = ColorTranslator.FromHtml("#F3F3F3");

                    LoadPhoto();
                    KeywordFillGrid();
                    KeywordFillGridAmazon();
                    //TitleData();
                    //containerSKU.Visible = true;
                    //containerDesc.Visible = true;
                    //UpdatePanel2.Visible = true;
                    //UpdatePanel4.Visible = true;
                }
                else
                {
                    row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                }
            }
        }
        private DataTable KeywordFillGrid()
        {
            SqlConnection con = new SqlConnection(strConnString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM V_ListingwithKeywords where SKU='" + lblSKUforWhere.Text + "'", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            gvEtsyKeyword.DataSource = dt;
            gvEtsyKeyword.DataBind();

            return dt;
        }
        protected void gvEtsyKeyword_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Left;

                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "&nbsp;")
                    {
                        e.Row.Cells[i].Text = "---";
                    }
                }

                Label lblstrSuperStarKeyWord = (Label)e.Row.FindControl("lblSuperStarKeyWord");
                Label lblstrSuperStarKeyWordOk = (Label)e.Row.FindControl("lblSuperStarKeyWordOk");
                Label lblstrSuperStarKeyWordNok = (Label)e.Row.FindControl("lblSuperStarKeyWordNok");

                if (lblstrSuperStarKeyWord.Text == "Yes")
                {
                    lblstrSuperStarKeyWordOk.Visible = true;
                    lblstrSuperStarKeyWordNok.Visible = false;
                }
                else if (lblstrSuperStarKeyWord.Text == "No")
                {
                    lblstrSuperStarKeyWordOk.Visible = false;
                    lblstrSuperStarKeyWordNok.Visible = true;
                }

                Label lblstrStyleKeywords = (Label)e.Row.FindControl("lblStyleKeywords");
                Label lblstrStyleKeywordsOk = (Label)e.Row.FindControl("lblStyleKeywordsOk");
                Label lblstrStyleKeywordsNok = (Label)e.Row.FindControl("lblStyleKeywordsNok");

                if (lblstrStyleKeywords.Text == "Yes")
                {
                    lblstrStyleKeywordsOk.Visible = true;
                    lblstrStyleKeywordsNok.Visible = false;
                }
                else if (lblstrStyleKeywords.Text == "No")
                {
                    lblstrStyleKeywordsOk.Visible = false;
                    lblstrStyleKeywordsNok.Visible = true;
                }
            }
        }
        protected void gvEtsyKeyword_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            KeywordFillGrid();
            KeywordFillGridAmazon();
            //TitleData();
            gvEtsyKeyword.PageIndex = e.NewPageIndex;
            gvEtsyKeyword.DataBind();
        }
        private DataTable KeywordFillGridAmazon()
        {
            SqlConnection con = new SqlConnection(strConnString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM V_ListingwithKeywords where SKU='" + lblSKUforWhere.Text + "'", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            gvAmazonKeyword.DataSource = dt;
            gvAmazonKeyword.DataBind();

            return dt;
        }
        protected void gvAmazonKeyword_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[1].HorizontalAlign = HorizontalAlign.Left;
                e.Row.Cells[2].HorizontalAlign = HorizontalAlign.Left;

                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "&nbsp;")
                    {
                        e.Row.Cells[i].Text = "---";
                    }
                }

                Label lblstrSuperStarKeyWord = (Label)e.Row.FindControl("lblSuperStarKeyWord");
                Label lblstrSuperStarKeyWordOk = (Label)e.Row.FindControl("lblSuperStarKeyWordOk");
                Label lblstrSuperStarKeyWordNok = (Label)e.Row.FindControl("lblSuperStarKeyWordNok");

                if (lblstrSuperStarKeyWord.Text == "Yes")
                {
                    lblstrSuperStarKeyWordOk.Visible = true;
                    lblstrSuperStarKeyWordNok.Visible = false;
                }
                else if (lblstrSuperStarKeyWord.Text == "No")
                {
                    lblstrSuperStarKeyWordOk.Visible = false;
                    lblstrSuperStarKeyWordNok.Visible = true;
                }

                Label lblstrStyleKeywords = (Label)e.Row.FindControl("lblStyleKeywords");
                Label lblstrStyleKeywordsOk = (Label)e.Row.FindControl("lblStyleKeywordsOk");
                Label lblstrStyleKeywordsNok = (Label)e.Row.FindControl("lblStyleKeywordsNok");

                if (lblstrStyleKeywords.Text == "Yes")
                {
                    lblstrStyleKeywordsOk.Visible = true;
                    lblstrStyleKeywordsNok.Visible = false;
                }
                else if (lblstrStyleKeywords.Text == "No")
                {
                    lblstrStyleKeywordsOk.Visible = false;
                    lblstrStyleKeywordsNok.Visible = true;
                }
            }
        }
        protected void gvAmazonKeyword_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            KeywordFillGrid();
            KeywordFillGridAmazon();
            gvAmazonKeyword.PageIndex = e.NewPageIndex;
            gvAmazonKeyword.DataBind();
        }
        protected void btnFilterClear_Click(object sender, ImageClickEventArgs e)
        {
            txtFilterSKU.Text = "";
        }

        protected void btnFilter_Click(object sender, ImageClickEventArgs e)
        {

        }
        protected void toTitleEdit_click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalTitleEdit", "$('#ModalTitleEdit').modal();", true);
                upModal.Update();

                //TitleData();
            }
        }
        //private void TitleData()
        //{
        //    {
        //        string s = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        //        SqlConnection con = new SqlConnection(s);

        //        SqlCommand cmd1 = new SqlCommand("select Top (1) * from T_Listing where SKU='" + lblSKUforWhere + "'", con);

        //        cmd1.Connection = con;

        //        con.Open();

        //        SqlDataReader dr1 = cmd1.ExecuteReader();

        //        while (dr1.Read())

        //        {
        //            txtTitle.Text = dr1["Title"].ToString();
        //        }

        //        dr1.Close();

        //        con.Close();
        //    }
        //}
        protected void btnTitleSave_Click(object sender, EventArgs e)
        {
            {
                //string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_Listing set Title=@Title where SKU =@SKU", con);
                cmd.Parameters.AddWithValue("@Title", txtTitle.Text);
                cmd.Parameters.AddWithValue("@SKU", lblSKUforWhere.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }
        protected void btnEtsyDescriptionEdit_click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalEtsyDescriptionEdit", "$('#ModalEtsyDescriptionEdit').modal();", true);
                upModal.Update();

                //TitleData();
            }
        }
        protected void btnEtsyDescriptionSave_Click(object sender, EventArgs e)
        {
            {
                //string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_Listing set Description_Etsy=@Description_Etsy where SKU =@SKU", con);
                cmd.Parameters.AddWithValue("@Description_Etsy", txtEtsyDescriptionEdit.Text);
                cmd.Parameters.AddWithValue("@SKU", lblSKUforWhere.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }
        protected void toEtsySalePriceEdit_click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalEtsySalePriceEdit", "$('#ModalEtsySalePriceEdit').modal();", true);
                upModal.Update();

                //TitleData();
            }
        }
        protected void btnEtsySalePriceSave_Click(object sender, EventArgs e)
        {
            {
                //string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_Listing set SalePrice_Etsy=@SalePrice_Etsy where SKU =@SKU", con);
                cmd.Parameters.AddWithValue("@SalePrice_Etsy", txtEtsySalePriceEdit.Text);
                cmd.Parameters.AddWithValue("@SKU", lblSKUforWhere.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }
        protected void btnAmazonDescriptionEdit_click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalAmazonDescriptionEdit", "$('#ModalAmazonDescriptionEdit').modal();", true);
                upModal.Update();

                //TitleData();
            }
        }
        protected void btnAmazonDescriptionSave_Click(object sender, EventArgs e)
        {
            {
                //string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_Listing set Description_Amazon=@Description_Amazon where SKU =@SKU", con);
                cmd.Parameters.AddWithValue("@Description_Amazon", txtAmazonDescriptionEdit.Text);
                cmd.Parameters.AddWithValue("@SKU", lblSKUforWhere.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }
        protected void btnAmazonSalePriceEdit_click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalAmazonSalePriceEdit", "$('#ModalAmazonSalePriceEdit').modal();", true);
                upModal.Update();

                //TitleData();
            }
        }
        protected void btnAmazonSalePriceSave_Click(object sender, EventArgs e)
        {
            {
                //string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_Listing set SalePrice_Amazon=@SalePrice_Amazon where SKU =@SKU", con);
                cmd.Parameters.AddWithValue("@SalePrice_Amazon", txtAmazonSalePriceEdit.Text);
                cmd.Parameters.AddWithValue("@SKU", lblSKUforWhere.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }
        protected void btnAmazonBulletPointEdit_click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalAmazonBulletPointEdit", "$('#ModalAmazonBulletPointEdit').modal();", true);
                upModal.Update();

                //TitleData();
            }
        }
        protected void btnAmazonBulletPointsSave_Click(object sender, EventArgs e)
        {
            {
                //string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_BulletPointsforAmazon set BulletPoint1=@BulletPoint1,BulletPoint2=@BulletPoint2,BulletPoint3=@BulletPoint3,BulletPoint4=@BulletPoint4,BulletPoint5=@BulletPoint5 where SKU =@SKU", con);
                cmd.Parameters.AddWithValue("@BulletPoint1", txtAmazonBulletPoint1.Text);
                cmd.Parameters.AddWithValue("@BulletPoint2", txtAmazonBulletPoint2.Text);
                cmd.Parameters.AddWithValue("@BulletPoint3", txtAmazonBulletPoint3.Text);
                cmd.Parameters.AddWithValue("@BulletPoint4", txtAmazonBulletPoint4.Text);
                cmd.Parameters.AddWithValue("@BulletPoint5", txtAmazonBulletPoint5.Text);
                cmd.Parameters.AddWithValue("@SKU", lblSKUforWhere.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }

        protected void btnListingStatus_click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalListingStatusEdit", "$('#ModalListingStatusEdit').modal();", true);
                upModal.Update();

                //TitleData();
            }
        }
        protected void btnListingStatusSave_Click(object sender, EventArgs e)
        {
            {
                //string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_Listing set EtsyListing=@EtsyListing,AmazonListing=@AmazonListing,EbayListing=@EbayListing,WixListing=@WixListing where SKU =@SKU", con);
                cmd.Parameters.AddWithValue("@EtsyListing", txtEtsyListingEdit.Text);
                cmd.Parameters.AddWithValue("@AmazonListing", txtAmazonListingEdit.Text);
                cmd.Parameters.AddWithValue("@EbayListing", txtEbayListingEdit.Text);
                cmd.Parameters.AddWithValue("@WixListing", txtWixListingEdit.Text);
                cmd.Parameters.AddWithValue("@SKU", lblSKUforWhere.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }
        protected void btnTotalListingStatus_Click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalTotalListingStatusList", "$('#ModalTotalListingStatusList').modal();", true);
                upModal.Update();
            }
        }
        protected void gvTotalListingStatusList_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "&nbsp;")
                    {
                        e.Row.Cells[i].Text = "---";
                    }
                }
            }
        }
        protected void gvTotalListingStatusList_SelectedIndexChanged(object sender, GridViewPageEventArgs e)
        {
            //FillGrid();
            gvTotalListingStatusList.PageIndex = e.NewPageIndex;
            gvTotalListingStatusList.DataBind();
        }
        protected void btnEbayDescriptionEdit_click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalEbayDescriptionEdit", "$('#ModalEbayDescriptionEdit').modal();", true);
                upModal.Update();

                //TitleData();
            }
        }
        protected void btnEbayDescriptionSave_Click(object sender, EventArgs e)
        {
            {
                //string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_Listing set Description_Ebay=@Description_Ebay where SKU =@SKU", con);
                cmd.Parameters.AddWithValue("@Description_Ebay", txtEbayDescriptionEdit.Text);
                cmd.Parameters.AddWithValue("@SKU", lblSKUforWhere.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }
        protected void btnWixDescriptionEdit_click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalWixDescriptionEdit", "$('#ModalWixDescriptionEdit').modal();", true);
                upModal.Update();

                //TitleData();
            }
        }
        protected void btnWixDescriptionSave_Click(object sender, EventArgs e)
        {
            {
                //string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_Listing set Description_Wix=@Description_Wix where SKU =@SKU", con);
                cmd.Parameters.AddWithValue("@Description_Wix", txtWixDescriptionEdit.Text);
                cmd.Parameters.AddWithValue("@SKU", lblSKUforWhere.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }
        protected void toEbaySalePriceEdit_click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalEbaySalePriceEdit", "$('#ModalEbaySalePriceEdit').modal();", true);
                upModal.Update();

                //TitleData();
            }
        }
        protected void btnEbaySalePriceSave_Click(object sender, EventArgs e)
        {
            {
                //string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_Listing set SalePrice_Ebay=@SalePrice_Ebay where SKU =@SKU", con);
                cmd.Parameters.AddWithValue("@SalePrice_Ebay", txtEbaySalePriceEdit.Text);
                cmd.Parameters.AddWithValue("@SKU", lblSKUforWhere.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }
        protected void toWixSalePriceEdit_click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalWixSalePriceEdit", "$('#ModalWixSalePriceEdit').modal();", true);
                upModal.Update();

                //TitleData();
            }
        }
        protected void btnWixSalePriceSave_Click(object sender, EventArgs e)
        {
            {
                //string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_Listing set SalePrice_Wix=@SalePrice_Wix where SKU =@SKU", con);
                cmd.Parameters.AddWithValue("@SalePrice_Wix", txtWixSalePriceEdit.Text);
                cmd.Parameters.AddWithValue("@SKU", lblSKUforWhere.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                //Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }
        protected void toOneBack_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
        }
        protected void btnMainMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }
    }
}