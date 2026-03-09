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
using static System.Net.Mime.MediaTypeNames;

namespace Feniks.Administrator
{
    public partial class StockEntry : System.Web.UI.Page
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
                        BindValue();
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
        protected void gvProducts_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void gvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //FillGrid();
            gvProducts.PageIndex = e.NewPageIndex;
            gvProducts.DataBind();
        }
        protected void gvProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtSKU.Text = gvProducts.SelectedRow.Cells[1].Text;
            //txtOrderDate.Text = gvOpenOrder.SelectedRow.Cells[1].Text;
            gvProducts.SelectedRow.BorderColor = System.Drawing.Color.White;

            {
                string s = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                SqlConnection con = new SqlConnection(s);

                SqlCommand cmd1 = new SqlCommand("select Top (1) * from V_Product where SKU='" + txtSKU.Text + "'", con);
                //SqlCommand cmd1 = new SqlCommand(("SELECT Top (1) SatID FROM V_LastSatID where ClaimID='" + lblLoginName.Text + "'"' order by SatID desc")', con))

                cmd1.Connection = con;

                con.Open();



                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())

                {
                    LoadPhoto();
                    //if (!Convert.IsDBNull(dr1["Photo"]))
                    //{
                    //ProductPhoto.ImageUrl = dr1["ImagePath"].ToString();
                    //string imageUrl = "data:image/jpg;base64," + Convert.ToBase64String((byte[])dr1["Photo"]);
                    //(e.Row.FindControl("ProductPhoto") as Image).ImageUrl = imageUrl;
                    //}
                    //txtBuyerName.Text = dr1["BuyerFullName"].ToString();
                    //txtCountry.Text = dr1["Country"].ToString();
                    //txtShipTo.Text = dr1["ShipTo"].ToString();
                    //txtShippingType.Text = dr1["ShippingType"].ToString();
                    //txtPhone.Text = dr1["PhoneNumber"].ToString();
                    //txtEmail.Text = dr1["Email"].ToString();

                    //txtShipDate.Text = dr1["ShipDate"].ToString();
                    //txtKKid.Text = dr1["KKID"].ToString();
                    //txtShippingPrice.Text = (Convert.ToDouble(dr1["ShippingPrice"])).ToString();
                    //txtTrackingNumber.Text = dr1["TrackingNumber"].ToString();
                    //txtStatus.Text = dr1["ShippingStatus"].ToString();
                    //txtForwarder.Text = dr1["ShippingCompany"].ToString();
                    //ddStatus.SelectedValue= dr1["ShippingCompanyID"].ToString();

                    //string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    //using (SqlConnection con = new SqlConnection(constr))


                    //FillGrid();
                    //container2.Visible = true;
                    //container1.Visible = true;
                    ////container3.Visible = true;
                    container4.Visible = true;
                    container5.Visible = true;
                    //container5.Visible = true;

                    //if (txtGiftMessage.Text.Length == 0)
                    //{
                    //    txtGiftMessage.Text = "There is no gift messages.";
                    //}
                    //else
                    //{
                    //    txtGiftMessage.Text = dr1["GiftMessage"].ToString();
                    //}
                }

                //dr1.Close();

                con.Close();
            }

            //foreach (GridViewRow row in gvOpenOrder.Rows)
            //{
            //    if (row.RowIndex == gvOpenOrder.SelectedIndex)
            //    {
            //        row.BackColor = ColorTranslator.FromHtml("#BACFEF");

            //    }
            //    else
            //    {
            //        row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
            //    }
            //}
        }
        private void BindValue()
        {
            {
                //string HomeDelivery = "Yes";

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT SupplierID, Supplier FROM T_Supplier Order by SupplierID asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddSupplier.DataSource = cmd.ExecuteReader();
                        ddSupplier.DataTextField = "Supplier";
                        ddSupplier.DataValueField = "SupplierID";
                        ddSupplier.DataBind();
                        con.Close();
                    }
                }
                ddSupplier.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }

            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT RingSizeID, RingSize FROM T_RingSize Order by RingSizeID asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddRingSize.DataSource = cmd.ExecuteReader();
                        ddRingSize.DataTextField = "RingSize";
                        ddRingSize.DataValueField = "RingSizeID";
                        ddRingSize.DataBind();
                        con.Close();
                    }
                }
                ddRingSize.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
        }
        private void LoadPhoto()
        {
            {
                string s = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                SqlConnection con = new SqlConnection(s);

                SqlCommand cmd1 = new SqlCommand("select Top (1) * from V_Product where SKU='" + txtSKU.Text + "'", con);
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
        protected void btnFilterClear_Click(object sender, ImageClickEventArgs e)
        {
            txtFilterSKU.Text = "";
        }

        protected void ImageButton8_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void btnStockEntrySave_Click(object sender, EventArgs e)
        {
            if (ddRingSize.SelectedValue == "0")
            {
                Alert1.Visible = true;
            }
            else if (txtStockEntryDate.Text.Length == 0)
            {
                Alert1.Visible = true;
            }
            else if (txtQuantity.Text.Length == 0)
            {
                Alert1.Visible = true;
            }
            else if (txtItemPrice.Text.Length == 0)
            {
                Alert1.Visible = true;
            }
            else if (txtWeightPrice.Text.Length == 0)
            {
                Alert1.Visible = true;
            }
            else if (txtDocumentNumber.Text.Length == 0)
            {
                Alert1.Visible = true;
            }
            else if (ddSupplier.SelectedValue == "0")
            {
                Alert1.Visible = true;
            }
            else
            {
                Alert1.Visible = false;

                {
                    string constrxx = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    using (SqlConnection conxx = new SqlConnection(constrxx))
                    {
                        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                        using (SqlCommand cmd = new SqlCommand("INSERT INTO T_StockReceipt (SKU,StockReceiptQty,DocumentNumber,PurchaseDate,RecordDate,RecordBy,RingSizeID,ItemPrice,WeightPrice,SupplierID) VALUES(@SKU, @StockReceiptQty, @DocumentNumber,@PurchaseDate,@RecordDate,@RecordBy,@RingSizeID,@ItemPrice,@WeightPrice,@SupplierID)", conxx))
                        //using (SqlCommand cmd = new SqlCommand("UPDATE T_Order (OrderNumber,OrderDate,MarketplaceID,ShipTo,BuyerFullName,CountryID,StateID,Email,PhoneNumber,RecordDate,RecordBy) VALUES(@OrderNumber, @OrderDate, @MarketplaceID,@ShipTo,@BuyerFullName,@CountryID,@StateID,@Email,@PhoneNumber,@RecordDate,@UserName) WHERE OrderNumber=@OrderNumber", conxx))
                        {
                            cmd.Parameters.AddWithValue("@RecordBy", lblLoginName.Text);
                            cmd.Parameters.AddWithValue("@RecordDate", now);
                            cmd.Parameters.AddWithValue("@SKU", txtSKU.Text);
                            cmd.Parameters.AddWithValue("@StockReceiptQty", txtQuantity.Text);
                            cmd.Parameters.AddWithValue("@DocumentNumber", txtDocumentNumber.Text);
                            cmd.Parameters.AddWithValue("@PurchaseDate", txtStockEntryDate.Text);
                            cmd.Parameters.AddWithValue("@RingSizeID", ddRingSize.SelectedValue);
                            cmd.Parameters.AddWithValue("@ItemPrice", txtItemPrice.Text);
                            cmd.Parameters.AddWithValue("@WeightPrice", txtWeightPrice.Text);
                            cmd.Parameters.AddWithValue("@SupplierID", ddSupplier.Text);
                            conxx.Open();
                            cmd.ExecuteNonQuery();
                            conxx.Close();

                            Response.Redirect("~/Administrator/StockManagement.aspx");
                        }
                    }
                }
            }
        }

        protected void btnStockEntrySaveCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/StockManagement.aspx");
        }

        protected void btnMainMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }

        protected void toOneBack_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/StockManagement.aspx");
        }
    }
}