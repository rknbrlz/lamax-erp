using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static System.Net.Mime.MediaTypeNames;

namespace Feniks.Administrator
{
    public partial class OrderDetails : System.Web.UI.Page
    {
        int OrderID = 0;
        //string OrderNumber = "";

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
            OrderID = Convert.ToInt32(Request.QueryString["OrderID"].ToString());

            if (!IsPostBack)
            {
                if (this.Page.User.Identity.IsAuthenticated)
                {
                    string username = this.Page.User.Identity.Name;
                    lblLoginName.Text = username;

                    OrderData();
                    OrderDetailFillGrid();
                    ReviewDetailFillGrid();

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
            if (lblLoginName.Text == "Label")
            {
                Response.Redirect("~/Login.aspx");
            }
            else
            {

            }
        }
        private void OrderData()
        {
            {
                string s = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                SqlConnection con = new SqlConnection(s);

                SqlCommand cmd1 = new SqlCommand("select Top (1) * from V_forOrderDetailsPage where OrderID='" + OrderID + "'", con);
                //SqlCommand cmd1 = new SqlCommand(("SELECT Top (1) SatID FROM V_LastSatID where ClaimID='" + lblLoginName.Text + "'"' order by SatID desc")', con))

                cmd1.Connection = con;

                con.Open();

                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())

                {
                    lblOrderNumber.Text = dr1["OrderNumber"].ToString();
                    lblStatusID.Text = dr1["ShippingStatusID"].ToString();
                    lblBuyerFullName.Text= dr1["BuyerFullName"].ToString();
                    lblemail.Text = dr1["Email"].ToString();
                    lblPhoneNumber.Text = dr1["PhoneNumber"].ToString();
                    lblRepeatBuyerCheck.Text = dr1["RepeaterBuyer"].ToString();
                    lblShipTo.Text = dr1["ShipTo"].ToString();
                    lblCountry.Text = dr1["Country"].ToString();
                    lblState.Text = dr1["State"].ToString();
                    lblStateShort.Text = dr1["StateID"].ToString();
                    lblMarketplace.Text = dr1["Marketplace"].ToString();
                    lblBuyerMsg.Text = dr1["BuyerNotes"].ToString();
                    lblGiftMessage.Text = dr1["GiftMessage"].ToString();
                    lblShippingPrice.Text = dr1["ShippingPricePaidBuyer"].ToString();
                    lblCouponPrice.Text = dr1["CoupunPrice"].ToString();
                    lblGiftWrapPrice.Text = dr1["GiftWrapPrice"].ToString();
                    lblTax.Text = dr1["Tax"].ToString();
                    lblSubTotal.Text = dr1["SubTotal"].ToString();
                    lblItemTotal.Text = dr1["ItemPrice"].ToString();
                    lblOrderTotal.Text = dr1["TotalPrice"].ToString();
                    lblOrderDate.Text = dr1["OrderDate"].ToString();
                    lblSellerMsg.Text = dr1["SellerNotes"].ToString();
                    lblCarrier.Text = dr1["ShippingCompany"].ToString();
                    lblShipDate.Text = dr1["ShipDate"].ToString();
                    lblTrackingNumber.Text = dr1["TrackingNumber"].ToString();
                    lblKKID.Text = dr1["KKID"].ToString();

                    lblProductCost.Text = dr1["TotalProductCost"].ToString();
                    lblShipmentExpenses.Text = dr1["ShippingPricePaidSeller"].ToString();
                    lblMarketplaceTaxesandCommissions.Text = dr1["MarketplaceCommission"].ToString();
                    lblOtherExpenses.Text = dr1["OtherExpenses"].ToString();
                    lblRefund.Text = dr1["Refund"].ToString();

                    lblBoxPrice.Text = dr1["BoxPrice"].ToString();
                    lblBagPrice.Text = dr1["BagPrice"].ToString();
                    lblEnvelopePrice.Text = dr1["EnvelopePrice"].ToString();
                    lblJewelryCardPrice.Text = dr1["JewelryCardPrice"].ToString();
                    lblFixCardPrice.Text = dr1["FixCardPrice"].ToString();
                    lblUnicefCardPrice.Text = dr1["UnicefCardPrice"].ToString();
                    lblCircleStickerPrice.Text = dr1["CircleStickerPrice"].ToString();
                    lblEnvelopeStickerPrice.Text = dr1["EnvelopeStickerPrice"].ToString();
                    lblKargomKolayStickerPrice.Text = dr1["KargomKolayStickerPrice"].ToString();
                    lblPackExpenses.Text = dr1["TotalPackExpences"].ToString();
                    lblProfit.Text = dr1["Profit"].ToString();
                    //lblProfitPercentage.Text = dr1["ProfitPercentage"].ToString();
                    lblProfitPercentage.Text = dr1["ProfitPercentageInt"].ToString();                 

                    {
                        int zero;
                        zero = 0;

                        int ten;
                        ten = 10;

                        int twenty;
                        twenty = 20;

                        int thirty;
                        thirty = 30;

                        int Forty;
                        Forty = 40;

                        int fifty;
                        fifty = 50;

                        int ProfitPercentage;

                        ProfitPercentage = int.Parse(lblProfitPercentage.Text);

                        if (ProfitPercentage <= 9)
                        {
                            speedmeter10.Visible = true;
                            speedmeter20.Visible = false;
                            speedmeter30.Visible = false;
                            speedmeter40.Visible = false;
                            speedmeter50.Visible = false;
                        }
                        else if (ProfitPercentage == 10 || ProfitPercentage <= 19)
                        {
                            speedmeter10.Visible = false;
                            speedmeter20.Visible = true;
                            speedmeter30.Visible = false;
                            speedmeter40.Visible = false;
                            speedmeter50.Visible = false;
                        }
                        else if (ProfitPercentage == 20 || ProfitPercentage <= 29)
                        {
                            speedmeter10.Visible = false;
                            speedmeter20.Visible = false;
                            speedmeter30.Visible = true;
                            speedmeter40.Visible = false;
                            speedmeter50.Visible = false;
                        }
                        else if (ProfitPercentage == 30 || ProfitPercentage <= 39)
                        {
                            speedmeter10.Visible = false;
                            speedmeter20.Visible = false;
                            speedmeter30.Visible = false;
                            speedmeter40.Visible = true;
                            speedmeter50.Visible = false;
                        }
                        else if (ProfitPercentage >= 40)
                        {
                            speedmeter10.Visible = false;
                            speedmeter20.Visible = false;
                            speedmeter30.Visible = false;
                            speedmeter40.Visible = false;
                            speedmeter50.Visible = true;
                        }
                    }

                    if (lblRefund.Text == "")
                    {
                        lblRefund.Text = "0,00";
                    }
                    else if (lblRefund.Text == "&nbsp;")
                    {
                        lblRefund.Text = "0,00";
                    }
                    else
                    {
                        lblRefund.Text = dr1["Refund"].ToString();
                    }

                    if (lblOtherExpenses.Text == "")
                    {
                        lblOtherExpenses.Text = "0,00";
                    }
                    else if (lblOtherExpenses.Text == "&nbsp;")
                    {
                        lblOtherExpenses.Text = "0,00";
                    }
                    else
                    {
                        lblOtherExpenses.Text = dr1["OtherExpenses"].ToString();
                    }

                    if (lblShipmentExpenses.Text == "")
                    {
                        lblShipmentExpenses.Text = "0,00";
                    }
                    else if (lblShipmentExpenses.Text == "&nbsp;")
                    {
                        lblShipmentExpenses.Text = "0,00";
                    }
                    else
                    {
                        lblShipmentExpenses.Text = dr1["ShippingPricePaidSeller"].ToString();
                    }

                    if (lblTrackingNumber.Text == "")
                    {
                        lblTrackingNumber.Text = "---";
                    }
                    else if (lblTrackingNumber.Text == "&nbsp;")
                    {
                        lblTrackingNumber.Text = "---";
                    }
                    else
                    {
                        lblTrackingNumber.Text = dr1["TrackingNumber"].ToString();
                    }

                    if (lblKKID.Text == "")
                    {
                        lblKKID.Text = "---";
                    }
                    else if (lblKKID.Text == "&nbsp;")
                    {
                        lblKKID.Text = "---";
                    }
                    else
                    {
                        lblKKID.Text = dr1["KKID"].ToString();
                    }

                    if (lblShipDate.Text == "")
                    {
                        lblShipDate.Text = "---";
                    }
                    else if (lblShipDate.Text == "&nbsp;")
                    {
                        lblShipDate.Text = "---";
                    }
                    else
                    {
                        lblShipDate.Text = dr1["ShipDate"].ToString();
                    }

                    if (lblPhoneNumber.Text == "")
                    {
                        lblPhoneNumber.Text = "No Phone Number";
                    }
                    else if (lblPhoneNumber.Text == "&nbsp;")
                    {
                        lblPhoneNumber.Text = "No Phone Number";
                    }
                    else
                    {
                        lblPhoneNumber.Text = dr1["PhoneNumber"].ToString();
                    }

                    if (lblemail.Text == "")
                    {
                        lblemail.Text = "No E-mail";
                    }
                    else if (lblemail.Text == "&nbsp;")
                    {
                        lblemail.Text = "No E-mail";
                    }
                    else
                    {
                        lblemail.Text = dr1["Email"].ToString();
                    }

                    if (lblRepeatBuyerCheck.Text == "No")
                    {
                        ImgStarGrey.Visible = true;
                        ImgStar.Visible = false;
                    }
                    else if (lblRepeatBuyerCheck.Text == "Yes")
                    {
                        ImgStarGrey.Visible = false;
                        ImgStar.Visible = true;
                    }

                    if (lblState.Text == "")
                    {
                        lblState.Text = "---";
                    }
                    else if (lblState.Text == "&nbsp;")
                    {
                        lblState.Text = "---";
                    }
                    else
                    {
                        lblState.Text = dr1["State"].ToString();
                    }

                    if (lblStateShort.Text == "")
                    {
                        lblStateShort.Text = "---";
                    }
                    else if (lblStateShort.Text == "&nbsp;")
                    {
                        lblStateShort.Text = "---";
                    }
                    else
                    {
                        lblStateShort.Text = dr1["StateID"].ToString();
                    }

                    if (lblBuyerMsg.Text == "")
                    {
                        lblBuyerMsg.Text = "---";
                    }
                    else if (lblBuyerMsg.Text == "&nbsp;")
                    {
                        lblBuyerMsg.Text = "---";
                    }
                    else
                    {
                        lblBuyerMsg.Text = dr1["BuyerNotes"].ToString();
                    }

                    if (lblGiftMessage.Text == "")
                    {
                        lblGiftMessage.Text = "---";
                    }
                    else if (lblGiftMessage.Text == "&nbsp;")
                    {
                        lblGiftMessage.Text = "---";
                    }
                    else
                    {
                        lblGiftMessage.Text = dr1["GiftMessage"].ToString();
                    }

                    if (lblSellerMsg.Text == "")
                    {
                        lblSellerMsg.Text = "---";
                    }
                    else if (lblSellerMsg.Text == "&nbsp;")
                    {
                        lblSellerMsg.Text = "---";
                    }
                    else
                    {
                        lblSellerMsg.Text = dr1["SellerNotes"].ToString();
                    }

                    if (lblCarrier.Text == "")
                    {
                        lblCarrier.Text = "---";
                    }
                    else if (lblCarrier.Text == "&nbsp;")
                    {
                        lblCarrier.Text = "---";
                    }
                    else
                    {
                        lblCarrier.Text = dr1["ShippingCompany"].ToString();
                    }

                    if (lblStatusID.Text == "1")
                    {
                        Status1.Visible = true;
                    }
                    else if (lblStatusID.Text == "2")
                    {
                        Status234.Visible = true;
                    }
                    else if (lblStatusID.Text == "2")
                    {
                        Status234.Visible = true;
                    }
                    else if (lblStatusID.Text == "3")
                    {
                        Status234.Visible = true;
                    }
                    else if (lblStatusID.Text == "4")
                    {
                        Status234.Visible = true;
                    }
                    else if (lblStatusID.Text == "5")
                    {
                        Status5.Visible = true;
                    }
                    else if (lblStatusID.Text == "6")
                    {
                        Status6.Visible = true;
                    }
                    else if (lblStatusID.Text == "7")
                    {
                        Status7.Visible = true;
                    }
                    else if (lblStatusID.Text == "8")
                    {
                        Status8.Visible = true;
                    }
                }

                dr1.Close();

                con.Close();
            }
        }
        private DataTable OrderDetailFillGrid()
        {
            SqlConnection con = new SqlConnection(strConnString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM V_ShippingOrderDetails where OrderNumber='" + lblOrderNumber.Text + "' order by SKU asc", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            gvOrderDetail.DataSource = dt;
            gvOrderDetail.DataBind();

            return dt;
        }
        private DataTable ReviewDetailFillGrid()
        {
            SqlConnection con = new SqlConnection(strConnString);
            SqlCommand cmd = new SqlCommand("SELECT * FROM V_ShippingOrderDetails where OrderNumber='" + lblOrderNumber.Text + "' order by SKU asc", con);
            SqlDataAdapter sda = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            sda.Fill(dt);
            gvReview.DataSource = dt;
            gvReview.DataBind();

            return dt;
        }
        protected void gvOrderDetail_RowDataBound(object sender, GridViewRowEventArgs e)
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

                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;
                    if (!Convert.IsDBNull(dr["Photo"]))
                    {
                        string imageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dr["Photo"]);
                        (e.Row.FindControl("ProductPhoto") as System.Web.UI.WebControls.Image).ImageUrl = imageUrl;
                        //System.Web.UI.WebControls.Image HgermanIcon = (System.Web.UI.WebControls.Image)e.Row.FindControl("ProductPhoto");
                    }
                }
            }
        }
        protected void gvReview_RowDataBound(object sender, GridViewRowEventArgs e)
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

                Label lblStar = (Label)e.Row.FindControl("lblStar");

                System.Web.UI.WebControls.Image rating0 = (System.Web.UI.WebControls.Image)e.Row.FindControl("rating0");
                System.Web.UI.WebControls.Image rating1 = (System.Web.UI.WebControls.Image)e.Row.FindControl("rating1");
                System.Web.UI.WebControls.Image rating2 = (System.Web.UI.WebControls.Image)e.Row.FindControl("rating2");
                System.Web.UI.WebControls.Image rating3 = (System.Web.UI.WebControls.Image)e.Row.FindControl("rating3");
                System.Web.UI.WebControls.Image rating4 = (System.Web.UI.WebControls.Image)e.Row.FindControl("rating4");
                System.Web.UI.WebControls.Image rating5 = (System.Web.UI.WebControls.Image)e.Row.FindControl("rating5");

                if (lblStar.Text == "1")
                {
                    rating0.Visible = false;
                    rating1.Visible = true;
                    rating2.Visible = false;
                    rating3.Visible = false;
                    rating4.Visible = false;
                    rating5.Visible = false;
                }
                else if (lblStar.Text == "2")
                {
                    rating0.Visible = false;
                    rating1.Visible = false;
                    rating2.Visible = true;
                    rating3.Visible = false;
                    rating4.Visible = false;
                    rating5.Visible = false;
                }
                else if (lblStar.Text == "3")
                {
                    rating0.Visible = false;
                    rating1.Visible = false;
                    rating2.Visible = false;
                    rating3.Visible = true;
                    rating4.Visible = false;
                    rating5.Visible = false;
                }
                else if (lblStar.Text == "4")
                {
                    rating0.Visible = false;
                    rating1.Visible = false;
                    rating2.Visible = false;
                    rating3.Visible = false;
                    rating4.Visible = true;
                    rating5.Visible = false;
                }
                else if (lblStar.Text == "5")
                {
                    rating0.Visible = false;
                    rating1.Visible = false;
                    rating2.Visible = false;
                    rating3.Visible = false;
                    rating4.Visible = false;
                    rating5.Visible = true;
                }

                Label lblReviewDetails = (Label)e.Row.FindControl("lblReviewDetails");
                Label lblNoReview = (Label)e.Row.FindControl("lblNoReview");

                if (lblReviewDetails.Text == "")
                {
                    lblReviewDetails.Visible = false;
                    lblNoReview.Visible = true;
                }
                else if (lblReviewDetails.Text == "&nbsp;")
                {
                    lblReviewDetails.Visible = false;
                    lblNoReview.Visible = true;
                }
                else
                {
                    lblReviewDetails.Visible = true;
                    lblNoReview.Visible = false;
                }

                {
                    DataRowView dr = (DataRowView)e.Row.DataItem;
                    if (!Convert.IsDBNull(dr["Photo"]))
                    {
                        string imageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dr["Photo"]);
                        (e.Row.FindControl("ProductPhoto") as System.Web.UI.WebControls.Image).ImageUrl = imageUrl;
                        //System.Web.UI.WebControls.Image HgermanIcon = (System.Web.UI.WebControls.Image)e.Row.FindControl("ProductPhoto");
                    }
                }
            }
        }
        protected void toOneBack_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/Orders.aspx");
        }

        protected void btnMainMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }
    }
}