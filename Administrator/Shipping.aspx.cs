using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;

namespace Feniks.Administrator
{
    public partial class Shipping : System.Web.UI.Page
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
        //private DataTable FillGrid()
        //{
        //    string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(strConnString))
        //    {
        //        using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM V_Orders order by OrderDate desc", con))
        //        {
        //            DataTable dt = new DataTable();
        //            sda.Fill(dt);
        //            gvOpenOrder.DataSource = dt;
        //            gvOpenOrder.DataBind();

        //            return dt;
        //        }
        //    }
        //}
        protected void gvOpenOrder_RowDataBound(object sender, GridViewRowEventArgs e)
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
                //check if the row is a datarow
                //if (e.Row.RowType == DataControlRowType.DataRow)
                //{
                    //find the select button in the row (in this case the first control in the first cell)
                    //txtOrderNumber.Text  = e.Row.Cells[0].Controls[0] as LinkButton;
                    //txtOrderNumber.Text = gvOpenOrder.SelectedRow.Cells[3].Text;

                    //hide the button, but it still needs to be on the page
                    //lb.Attributes.Add("style", "display:none");

                    //add the click event to the gridview row
                    //e.Row.Attributes.Add("onclick", Page.ClientScript.GetPostBackClientHyperlink((GridView)sender, "Select$" + e.Row.RowIndex));
                //}

                Label lblShippingStatus = (Label)e.Row.FindControl("lblShippingStatus");

                //Button btnShippingStatus1 = (Button)e.Row.FindControl("btnShippingStatus1");
                //Button btnShippingStatus2 = (Button)e.Row.FindControl("btnShippingStatus2");
                Label Status1 = (Label)e.Row.FindControl("Status1");
                Label Status2 = (Label)e.Row.FindControl("Status2");
                Label Status3 = (Label)e.Row.FindControl("Status3");
                Label Status4 = (Label)e.Row.FindControl("Status4");
                Label Status5 = (Label)e.Row.FindControl("Status5");
                Label Status6 = (Label)e.Row.FindControl("Status6");
                Label Status7 = (Label)e.Row.FindControl("Status7");
                Label Status8 = (Label)e.Row.FindControl("Status8");
                Label Status9 = (Label)e.Row.FindControl("Status9");

                if (lblShippingStatus.Text == "Preparing")
                {
                    //btnShippingStatus1.Visible = true;
                    Status1.Visible = true;
                }
                else if (lblShippingStatus.Text == "In repair")
                {
                    //btnShippingStatus2.Visible = true;
                    Status2.Visible = true;
                }
                else if (lblShippingStatus.Text == "Size Revision")
                {
                    Status3.Visible = true;
                }
                else if (lblShippingStatus.Text == "Awaiting from Supplier")
                {
                    Status4.Visible = true;
                }
                else if (lblShippingStatus.Text == "Packaged")
                {
                    Status5.Visible = true;
                }
                else if (lblShippingStatus.Text == "Ready")
                {
                    Status6.Visible = true;
                }
                else if (lblShippingStatus.Text == "Pre-Shipping")
                {
                    Status7.Visible = true;
                }
                else if (lblShippingStatus.Text == "Final Shipping")
                {
                    Status8.Visible = true;
                }
                else if (lblShippingStatus.Text == "Cancel")
                {
                    Status9.Visible = true;
                }
                else
                {

                }
                Label lblMarketplace = (Label)e.Row.FindControl("lblMarketplace");

                System.Web.UI.WebControls.Image AmazonIcon = (System.Web.UI.WebControls.Image)e.Row.FindControl("AmazonIcon");
                System.Web.UI.WebControls.Image EtsyIcon = (System.Web.UI.WebControls.Image)e.Row.FindControl("EtsyIcon");
                System.Web.UI.WebControls.Image EbayIcon = (System.Web.UI.WebControls.Image)e.Row.FindControl("EbayIcon");
                System.Web.UI.WebControls.Image HgermanIcon = (System.Web.UI.WebControls.Image)e.Row.FindControl("HgermanIcon");

                if (lblMarketplace.Text == "Amazon")
                {
                    AmazonIcon.Visible = true;
                }
                else if (lblMarketplace.Text == "Etsy")
                {
                    EtsyIcon.Visible = true;
                }
                else if (lblMarketplace.Text == "eBay")
                {
                    EbayIcon.Visible = true;
                }
                else if (lblMarketplace.Text == "hgerman.shop")
                {
                    HgermanIcon.Visible = true;
                }

                Label lblShippingType = (Label)e.Row.FindControl("lblShippingType");

                Label lblStandard = (Label)e.Row.FindControl("lblStandard");
                Label lblExpedite = (Label)e.Row.FindControl("lblExpedite");

                if (lblShippingType.Text == "Standard")
                {
                    lblShippingType.Visible = true;
                }
                else if (lblShippingType.Text == "Expedited")
                {
                    lblExpedite.Visible = true;
                }
                else
                {

                }

                Label lblMultipleItemCheck = (Label)e.Row.FindControl("lblMultipleItemCheck");

                Label lblSingleItem = (Label)e.Row.FindControl("lblSingleItem");
                Label lblMultipleItem = (Label)e.Row.FindControl("lblMultipleItem");

                if (lblMultipleItemCheck.Text == "1")
                {
                    lblSingleItem.Visible = true;
                }
                else
                {
                    lblMultipleItem.Visible = true;
                }
            }
        }
        protected void gvOpenOrder_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //FillGrid();
            gvOpenOrder.PageIndex = e.NewPageIndex;
            gvOpenOrder.DataBind();
        }
        protected void gvOpenOrder_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtOrderNumber.Text = gvOpenOrder.SelectedRow.Cells[2].Text;
            txtOrderDate.Text = gvOpenOrder.SelectedRow.Cells[1].Text;
            gvOpenOrder.SelectedRow.BorderColor = System.Drawing.Color.White;

            {
                string s = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                SqlConnection con = new SqlConnection(s);

                SqlCommand cmd1 = new SqlCommand("select Top (1) * from V_ShippingTextBoxValue where OrderNumber='" + txtOrderNumber.Text + "'", con);
                //SqlCommand cmd1 = new SqlCommand(("SELECT Top (1) SatID FROM V_LastSatID where ClaimID='" + lblLoginName.Text + "'"' order by SatID desc")', con))

                cmd1.Connection = con;

                con.Open();



                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())

                {
                    txtBuyerName.Text = dr1["BuyerFullName"].ToString();
                    txtCountry.Text = dr1["Country"].ToString();
                    txtShipTo.Text = dr1["ShipTo"].ToString();
                    txtShippingType.Text = dr1["ShippingType"].ToString();
                    txtPhone.Text = dr1["PhoneNumber"].ToString();
                    txtEmail.Text = dr1["Email"].ToString();

                    txtShipDate.Text = dr1["ShipDate"].ToString();
                    txtKKid.Text = dr1["KKID"].ToString();
                    txtShippingPrice.Text = (Convert.ToDouble(dr1["ShippingPricePaidSeller"])).ToString();
                    txtTrackingNumber.Text = dr1["TrackingNumber"].ToString();
                    txtStatus.Text = dr1["ShippingStatus"].ToString();
                    txtForwarder.Text = dr1["ShippingCompany"].ToString();
                    //ddStatus.SelectedValue= dr1["ShippingCompanyID"].ToString();

                    //string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    //using (SqlConnection con = new SqlConnection(constr))


                    FillGrid();
                    container2.Visible = true;
                    container1.Visible = true;
                    //container3.Visible = true;
                    container4.Visible = true;
                    container5.Visible = true;

                    if (txtGiftMessage.Text.Length == 0)
                    {
                        txtGiftMessage.Text = "There is no gift messages.";
                    }
                    else
                    {
                        txtGiftMessage.Text = dr1["GiftMessage"].ToString();
                    }

                    if (txtBuyerNote.Text.Length == 0)
                    {
                        txtBuyerNote.Text = "There is no notes.";
                    }
                    else
                    {
                        txtBuyerNote.Text = dr1["BuyerNotes"].ToString();
                    }

                    if (txtSellerNote.Text.Length == 0)
                    {
                        txtSellerNote.Text = "There is no notes.";
                    }
                    else if (txtSellerNote.Text == "")
                    {
                        txtSellerNote.Text = "There is no notes.";
                    }
                    else
                    {
                        txtSellerNote.Text = dr1["SellerNotes"].ToString();
                    }

                    if (txtPhone.Text.Length == 0)
                    {
                        txtPhone.Text = "There is no phone number.";
                    }
                    else
                    {
                        txtPhone.Text = dr1["PhoneNumber"].ToString();
                    }

                    if (txtEmail.Text.Length == 0)
                    {
                        txtEmail.Text = "There is no e-mail.";
                    }
                    else
                    {
                        txtEmail.Text = dr1["Email"].ToString();
                    }

                    if (txtShipDate.Text.Length == 0)
                    {
                        txtShipDate.Enabled = true;
                    }
                    else
                    {
                        txtShipDate.Enabled = false;
                    }

                    if (txtShippingPrice.Text.Length == 0)
                    {
                        txtShippingPrice.Enabled = true;
                    }
                    else
                    {
                        txtShippingPrice.Enabled = false;
                    }

                    if (txtForwarder.Text.Length == 0)
                    {
                        ddForwarder.Visible = true;
                        txtForwarder.Visible = false;
                    }
                    else
                    {
                        ddForwarder.Visible = false;
                        txtForwarder.Visible = true;
                        txtForwarder.Enabled = false;
                    }

                    if (txtKKid.Text.Length == 0)
                    {
                        txtKKid.Enabled = true;
                    }
                    else
                    {
                        txtKKid.Enabled = false;
                    }

                    if (txtTrackingNumber.Text.Length == 0)
                    {
                        txtTrackingNumber.Enabled = true;
                    }
                    else
                    {
                        txtTrackingNumber.Enabled = false;
                    }

                    if (txtStatus.Text.Length == 0)
                    {
                        txtStatus.Enabled = true;
                    }
                    else
                    {
                        txtStatus.Enabled = false;
                    }

                    if (txtShippingType.Text == "Expedited")
                    {
                        txtShippingType.ForeColor = Color.Red;
                    }
                    else
                    {
                        txtShippingType.ForeColor = Color.Black;
                    }
                }

                dr1.Close();

                con.Close();
            }

            foreach (GridViewRow row in gvOpenOrder.Rows)
            {
                if (row.RowIndex == gvOpenOrder.SelectedIndex)
                {
                    row.BackColor = ColorTranslator.FromHtml("#BACFEF");
                   
                }
                else
                {
                    row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                }
            }
        }
        private DataTable FillGrid()
        {
            //string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            //using (SqlConnection con = new SqlConnection(strConnString))
            //{
            //    using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM V_Shipping ORDER BY SKU", con))
            //    {
            //        DataTable dt = new DataTable();
            //        sda.Fill(dt);
            //        gvOrderDetail.DataSource = dt;
            //        gvOrderDetail.DataBind();

            //        return dt;
            //    }
            //}
                //string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            SqlConnection con = new SqlConnection(strConnString);
            //SqlConnection con = new SqlConnection(constr);
            SqlCommand cmd = new SqlCommand("SELECT * FROM V_ShippingOrderDetails where OrderNumber='" + txtOrderNumber.Text + "' order by SKU asc", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                gvOrderDetail.DataSource = dt;
                gvOrderDetail.DataBind();

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
        //protected void RefreshGridView(object sender, EventArgs e)
        //{
        //    FillGrid();
        //}
        private void BindValue()
        {
            {
                //string HomeDelivery = "Yes";

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT ShippingCompanyID, ShippingCompany FROM T_ShippingCompany Order by ShippingCompanyID asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddForwarder.DataSource = cmd.ExecuteReader();
                        ddForwarder.DataTextField = "ShippingCompany";
                        ddForwarder.DataValueField = "ShippingCompanyID";
                        ddForwarder.DataBind();
                        con.Close();
                    }
                }
                ddForwarder.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }

            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT ShippingStatusID, ShippingStatus FROM T_ShippingStatus Order by ShippingStatusID asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddStatus.DataSource = cmd.ExecuteReader();
                        ddStatus.DataTextField = "ShippingStatus";
                        ddStatus.DataValueField = "ShippingStatusID";
                        ddStatus.DataBind();
                        con.Close();
                    }
                }
                ddStatus.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
        }
        protected void btnNewOrderSave_Click(object sender, EventArgs e)
        {
            if (ddStatus.SelectedValue == "0")
            {
                //AlertStatusSelect.Visible = true;
            }
            else
            {
                //AlertStatusSelect.Visible = false;
                AlertStatus.Visible = true;

                if (ddStatus.SelectedValue == "1")
                {
                    lblStatus.Text = "Order status will be changed to Preparing.";
                }
                else if (ddStatus.SelectedValue == "2")
                {
                    lblStatus.Text = "Order status will be changed to In Repair.";
                }
                else if (ddStatus.SelectedValue == "3")
                {
                    lblStatus.Text = "Order status will be changed to Size Revision.";
                }
                else if (ddStatus.SelectedValue == "4")
                {
                    lblStatus.Text = "Order status will be changed to Awaiting from Supplier.";
                }
                else if (ddStatus.SelectedValue == "5")
                {
                    lblStatus.Text = "Order status will be changed to Packaged.";
                }
                else if (ddStatus.SelectedValue == "6")
                {
                    lblStatus.Text = "Order status will be changed to Ready.";
                }
                else if (ddStatus.SelectedValue == "7")
                {
                    lblStatus.Text = "Order status will be changed to Pre-Shipping.";
                }
                else if (ddStatus.SelectedValue == "8")
                {
                    lblStatus.Text = "Order status will be changed to Final Shipping.";
                }

                string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("update T_Shipping set KKID=@KKID,ShippingPrice=@ShippingPrice,ShippingCompanyID=@ShippingCompanyID,TrackingNumber=@TrackingNumber,ShipDate=@ShipDate,ShippingStatusID=@ShippingStatusID,RecordDate=@RecordDate,RecordBy=@RecordBy where OrderNumber =@OrderNumber", con);
                cmd.Parameters.AddWithValue("@OrderNumber", txtOrderNumber.Text);
                cmd.Parameters.AddWithValue("@KKID", txtKKid.Text);
                cmd.Parameters.AddWithValue("@ShippingPrice", txtShippingPrice.Text);
                cmd.Parameters.AddWithValue("@ShippingCompanyID", ddForwarder.SelectedValue);
                cmd.Parameters.AddWithValue("@ShipDate", txtOrderDate.Text);
                cmd.Parameters.AddWithValue("@TrackingNumber", txtTrackingNumber.Text);
                cmd.Parameters.AddWithValue("@ShippingStatusID", ddStatus.SelectedValue);
                cmd.Parameters.AddWithValue("@RecordDate", now);
                cmd.Parameters.AddWithValue("@RecordBy", lblLoginName.Text);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                Response.Redirect("~/Administrator/Shipping.aspx");
            }
        }

        protected void btnNewOrderCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/Shipping.aspx");
        }

        protected void btntxtShipDateEdit_Click(object sender, EventArgs e)
        {
            //if (txtShipDate.Text.Length == 0)
            //{
                txtShipDate.Enabled = true;
            //}
            //else
            //{
                //txtShipDate.Enabled = false;
            //}
        }

        protected void btntxtShippingPriceEdit_Click(object sender, EventArgs e)
        {
            txtShippingPrice.Enabled = true;
        }

        protected void btnStatusEdit_Click(object sender, EventArgs e)
        {
            ddStatus.Visible = true;
            txtStatus.Visible = false;
        }

        protected void btntxtForwarderEdit_Click(object sender, EventArgs e)
        {

        }

        protected void btntxtKKIDEdit_Click(object sender, EventArgs e)
        {

        }

        protected void btntxtTrackingNumberEdit_Click(object sender, EventArgs e)
        {

        }

        protected void btnMainMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }

        protected void toOneBack_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/Orders.aspx");
        }
    }
}