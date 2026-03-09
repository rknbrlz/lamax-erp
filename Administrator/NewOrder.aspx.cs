using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class WebForm1 : System.Web.UI.Page
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
            if (!IsPostBack)
            {
                //if (this.Page.User.Identity.IsAuthenticated)
                //{
                //    string username = this.Page.User.Identity.Name;
                //    lblLoginName.Text = username;
                //}
                if (gvNewOrder2.Rows.Count == 0)
                {
                    tblSaveButton.Visible = false;
                    space1.Visible = false;
                }

                BindValue();
            }
        }
        private void BindValue()
        {
            {
                //string HomeDelivery = "Yes";

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT MarketplaceID, Marketplace FROM T_Marketplace Order by Marketplace asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddMarketplaces.DataSource = cmd.ExecuteReader();
                        ddMarketplaces.DataTextField = "Marketplace";
                        ddMarketplaces.DataValueField = "MarketplaceID";
                        ddMarketplaces.DataBind();
                        con.Close();
                    }
                }
                ddMarketplaces.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
            {
                //string HomeDelivery = "Yes";

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT CountryID, Country FROM T_Country Order by Country asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddMarketplaces.DataSource = cmd.ExecuteReader();
                        ddMarketplaces.DataTextField = "Country";
                        ddMarketplaces.DataValueField = "CountryID";
                        ddMarketplaces.DataBind();
                        con.Close();
                    }
                }
                ddCountry.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
            {
                //string HomeDelivery = "Yes";

                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT StateID, State FROM T_State where CountryID='\" + ddCountry.Text + \"'"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddMarketplaces.DataSource = cmd.ExecuteReader();
                        ddMarketplaces.DataTextField = "State";
                        ddMarketplaces.DataValueField = "StateID";
                        ddMarketplaces.DataBind();
                        con.Close();
                    }
                }
                ddState.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
        }
        protected void btnFilter_Click(object sender, ImageClickEventArgs e)
        {

        }
        protected void gvNewOrder_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //gvSatCreation1GridViewData();

            gvNewOrder.PageIndex = e.NewPageIndex;
            //gvSatCreation1.DataBind();
            //gvSatCreation1GridViewData();
            //this.DataFilterMaterialName();
            //this.DataFilterMaterialDesc();
            //this.DataFilterUBBCode();
            //this.DataFilterCategory();
        }
        protected void gvNewOrder_RowDataBound(object sender, GridViewRowEventArgs e)
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
                    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                    SqlConnection con = new SqlConnection(constr);
                    con.Open();
                    DropDownList DropDownList1 = (e.Row.FindControl("ddRingSize") as DropDownList);

                    SqlCommand cmd = new SqlCommand("select RingSizeID,RingSize from T_RingSize", con);
                    SqlDataAdapter sda = new SqlDataAdapter(cmd);
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    con.Close();
                    DropDownList1.DataSource = dt;

                    DropDownList1.DataTextField = "RingSize";
                    DropDownList1.DataValueField = "RingSizeID";
                    DropDownList1.DataBind();
                    DropDownList1.Items.Insert(0, new ListItem("--Please Select--", "0"));
                }
            }
        }
        protected void btnNewOrderSave_Click(object sender, EventArgs e)
        {

        }
        protected void btnAddList_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvNewOrder.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {

                    Label strlblSKU = row.FindControl("lblSKU") as Label;
                    DropDownList strddRingSize = row.FindControl("ddRingSize") as DropDownList;
                    TextBox strtxtQty = row.FindControl("txtQty") as TextBox;
                    TextBox strtxtItemPrice = row.FindControl("txtItemPrice") as TextBox;
                    TextBox strtxtCouponPrice = row.FindControl("txtCouponPrice") as TextBox;
                    TextBox strtxtTax = row.FindControl("txtTax") as TextBox;
                    TextBox strtxtShippingPrice = row.FindControl("txtShippingPrice") as TextBox;
                    //Label strlblPolyclinicID = row.FindControl("lblPolyclinicID") as Label;

                    //DropDownList strddPolyclinic = row.FindControl("ddPolyclinic") as DropDownList;

                    CheckBox chkRow = (row.Cells[0].FindControl("chkRow") as CheckBox);
                    if (chkRow.Checked)
                    {
                        decimal ItemQuantitydes = decimal.Parse(strtxtQty.Text);

                        if (ItemQuantitydes == 0)
                        {
                            tblSaveButton.Visible = false;
                            space1.Visible = false;
                        }
                        else if (ItemQuantitydes > 0)
                        {
                            {
                                tblSaveButton.Visible = true;
                                space1.Visible = true;

                                SqlConnection con = new SqlConnection(strConnString);
                                SqlCommand cmd = new SqlCommand("Insert into T_OrderCreationDynamic (SKU,Quantity,ItemPrice,CoupunPrice,ShippingPrice,RingSizeID,Tax) VALUES(@SKU,@Quantity,@ItemPrice,@CoupunPrice,@ShippingPrice,@RingSizeID,@Tax)", con);
                                cmd.Parameters.AddWithValue("@SKU", strlblSKU.Text);
                                cmd.Parameters.AddWithValue("@Quantity", strtxtQty.Text);
                                cmd.Parameters.AddWithValue("@ItemPrice", strtxtItemPrice.Text);
                                cmd.Parameters.AddWithValue("@CoupunPrice", strtxtCouponPrice.Text);
                                cmd.Parameters.AddWithValue("@ShippingPrice", strtxtShippingPrice.Text);
                                cmd.Parameters.AddWithValue("@RingSizeID", strddRingSize.SelectedValue);
                                cmd.Parameters.AddWithValue("@Tax", strtxtTax.Text);
                                //cmd.Parameters.AddWithValue("@Quantity", strtxtSatQuantity.Text);
                                //cmd.Parameters.AddWithValue("@RequestQuantity", strtxtSatQuantity.Text);
                                //cmd.Parameters.AddWithValue("@UserName", lblLoginName.Text);
                                con.Open();
                                cmd.ExecuteNonQuery();

                                gvNewOrder2GridViewData();

                                chkRow.Checked = false;
                                strtxtQty.Text = "0";
                            }
                            //if (strddPolyclinic.SelectedValue != "0")
                            //{
                            //    tblSaveButton.Visible = true;
                            //    space1.Visible = true;

                            //    SqlConnection con = new SqlConnection(strConnString);
                            //    SqlCommand cmd = new SqlCommand("Insert into T_Order (MaterialID,PolyclinicID,Quantity,RequestQuantity,UserName) VALUES(@MaterialID,@PolyclinicID,@Quantity,@RequestQuantity,@UserName)", con);
                            //    cmd.Parameters.AddWithValue("@MaterialID", strlblMaterialID.Text);
                            //    //cmd.Parameters.AddWithValue("@PolyclinicID", strddPolyclinic.SelectedValue);
                            //    //cmd.Parameters.AddWithValue("@Quantity", strtxtSatQuantity.Text);
                            //    //cmd.Parameters.AddWithValue("@RequestQuantity", strtxtSatQuantity.Text);
                            //    //cmd.Parameters.AddWithValue("@UserName", lblLoginName.Text);
                            //    con.Open();
                            //    cmd.ExecuteNonQuery();

                            //    gvNewOrder1();

                            //    chkRow.Checked = false;
                            //    strtxtSatQuantity.Text = "0";
                            //}
                        }
                    }
                    else if (chkRow.Checked == false)
                    {
                        if (gvNewOrder2.Rows.Count == 0)
                        {
                            tblSaveButton.Visible = false;
                            space1.Visible = false;
                        }
                    }
                }
            }
        }
        private void gvNewOrder2GridViewData()
        {
            {
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("SELECT * FROM V_SatCreationDynamic where Username='" + lblLoginName.Text + "' order by MaterialName asc", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                gvNewOrder2.DataSource = dt;
                gvNewOrder2.DataBind();

                if (gvNewOrder2.Rows.Count == 0)
                {
                    tblSaveButton.Visible = false;
                    space1.Visible = false;
                }
            }
        }
        protected void gvNewOrder2_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int SatCreationDynamicID = Convert.ToInt32(e.CommandArgument);
                SqlConnection con = new SqlConnection(strConnString);
                string cmdText = "DELETE FROM T_SatCreationDynamic WHERE SatCreationDynamicID=@SatCreationDynamicID";
                SqlCommand cmd = new SqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@SatCreationDynamicID", SatCreationDynamicID);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                gvNewOrder2GridViewData();

                if (gvNewOrder2.Rows.Count == 0)
                {
                    tblSaveButton.Visible = false;
                    space1.Visible = false;
                }
            }

            if (gvNewOrder2.Rows.Count == 0)
            {
                tblSaveButton.Visible = false;
                space1.Visible = false;
            }
        }
        protected void btnMainMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }
        protected void toOneBack_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/Orders.aspx");
        }
        protected void btnNewOrderCancel_Click(object sender, EventArgs e)
        {

        }
    }
}