using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class NewProduct : System.Web.UI.Page
    {
        //private object bytes;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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

                    using (SqlCommand cmd = new SqlCommand("SELECT ProductTypeID, ProductType FROM T_ProductType Order by ProductType asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddProductType.DataSource = cmd.ExecuteReader();
                        ddProductType.DataTextField = "ProductType";
                        ddProductType.DataValueField = "ProductTypeID";
                        ddProductType.DataBind();
                        con.Close();
                    }
                }
                ddProductType.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT MaterialID, Material FROM T_Material Order by Material asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddMaterial.DataSource = cmd.ExecuteReader();
                        ddMaterial.DataTextField = "Material";
                        ddMaterial.DataValueField = "MaterialID";
                        ddMaterial.DataBind();
                        con.Close();
                    }
                }
                ddMaterial.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT BandTypeID, BandType FROM T_BandType Order by BandType asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddBandType.DataSource = cmd.ExecuteReader();
                        ddBandType.DataTextField = "BandType";
                        ddBandType.DataValueField = "BandTypeID";
                        ddBandType.DataBind();
                        con.Close();
                    }
                }
                ddBandType.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT ColorID, Color FROM T_Color Order by Color asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddColor.DataSource = cmd.ExecuteReader();
                        ddColor.DataTextField = "Color";
                        ddColor.DataValueField = "ColorID";
                        ddColor.DataBind();
                        con.Close();
                    }
                }
                ddColor.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT StockAddressID, StockAddress FROM T_StockAddress Order by StockAddress asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddStockAddress.DataSource = cmd.ExecuteReader();
                        ddStockAddress.DataTextField = "StockAddress";
                        ddStockAddress.DataValueField = "StockAddressID";
                        ddStockAddress.DataBind();
                        con.Close();
                    }
                }
                ddStockAddress.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT StoneID, Stone FROM T_Stone Order by Stone asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddStone1.DataSource = cmd.ExecuteReader();
                        ddStone1.DataTextField = "Stone";
                        ddStone1.DataValueField = "StoneID";
                        ddStone1.DataBind();
                        con.Close();
                    }
                }
                ddStone1.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
            //{
            //    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            //    using (SqlConnection con = new SqlConnection(constr))
            //    {

            //        using (SqlCommand cmd = new SqlCommand("SELECT StoneID, Stone FROM T_Stone Order by Stone asc"))
            //        {
            //            cmd.CommandType = CommandType.Text;
            //            cmd.Connection = con;
            //            con.Open();
            //            ddStone2.DataSource = cmd.ExecuteReader();
            //            ddStone2.DataTextField = "Stone";
            //            ddStone2.DataValueField = "StoneID";
            //            ddStone2.DataBind();
            //            con.Close();
            //        }
            //    }
            //    ddStone2.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            //}
            //{
            //    string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            //    using (SqlConnection con = new SqlConnection(constr))
            //    {

            //        using (SqlCommand cmd = new SqlCommand("SELECT StoneID, Stone FROM T_Stone Order by Stone asc"))
            //        {
            //            cmd.CommandType = CommandType.Text;
            //            cmd.Connection = con;
            //            con.Open();
            //            ddStone3.DataSource = cmd.ExecuteReader();
            //            ddStone3.DataTextField = "Stone";
            //            ddStone3.DataValueField = "StoneID";
            //            ddStone3.DataBind();
            //            con.Close();
            //        }
            //    }
            //    ddStone3.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            //}
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {

                    using (SqlCommand cmd = new SqlCommand("SELECT SupplierID, Supplier, SupplierCode FROM T_Supplier Order by Supplier asc"))
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

                    using (SqlCommand cmd = new SqlCommand("SELECT StoneStatusID, StoneStatus FROM T_StoneStatus Order by StoneStatusID asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddStoneStatus.DataSource = cmd.ExecuteReader();
                        ddStoneStatus.DataTextField = "StoneStatus";
                        ddStoneStatus.DataValueField = "StoneStatusID";
                        ddStoneStatus.DataBind();
                        con.Close();
                    }
                }
                ddStoneStatus.Items.Insert(0, new ListItem("--- please choose ---", "0"));
            }
        }
        protected void btnMainMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }
        protected void toOneBack_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
        }
        protected void btnNewProductSave_Click(object sender, EventArgs e)
        {
            {
                byte[] bytes;
                using (BinaryReader br = new BinaryReader(FileUploadImage.PostedFile.InputStream))
                {
                    bytes = br.ReadBytes(FileUploadImage.PostedFile.ContentLength);
                }
                string constrxx = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection conxx = new SqlConnection(constrxx))
                {
                    string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                    //decimal JWeight = decimal.Parse(txtWeight.Text);

                    using (SqlCommand cmd = new SqlCommand("INSERT INTO T_Product (ProductTypeID,Personalized,SKU,StockAddress,Watermark,Video,HandPhoto,Etsy,Amazon,Ebay,Wix,Catawiki,MaterialID,ColorID,BandTypeID,StoneStatusID,Stone1,Weight,Diameter,Length,Width,FirstPurchaseDate,SupplierID,ImageName,ContentType,Photo) VALUES(@ProductType,@Personalized,@SKU,@StockAddress,@Watermark,@Video,@HandPhoto,@Etsy,@Amazon,@Ebay,@Wix,@Catawiki,@Material,@Color,@BandType,@StoneStatus,@Stone1,@Weight,@Diameter,@Length,@Width,@FirstPurchaseDate,@Supplier,@ImageName,@ContentType,@Photo)", conxx))
                    {
                        cmd.Parameters.AddWithValue("@ProductType", ddProductType.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@Personalized", ddPersonalized.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@SKU", txtSKU.Text.ToString());
                        cmd.Parameters.AddWithValue("@StockAddress", ddStockAddress.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@Watermark", rdbtnWatermark.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@Video", rdbtnVideo.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@HandPhoto", rdbtnHandPhoto.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@Etsy", cbxEtsy.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@Amazon", cbxAmazon.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@Ebay", cbxEbay.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@Wix", cbxWix.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@Catawiki", cbxCatawiki.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@Material", ddMaterial.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@Color", ddColor.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@BandType", ddBandType.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@StoneStatus", ddStoneStatus.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@Stone1", ddStone1.SelectedValue.ToString());
                        //cmd.Parameters.AddWithValue("@Weight", Convert.ToDecimal(txtWeight.Text).ToString());
                        cmd.Parameters.AddWithValue("@Weight", txtWeight.Text.ToString());
                        cmd.Parameters.AddWithValue("@Diameter", txtDiameter.Text.ToString());
                        cmd.Parameters.AddWithValue("@Length", txtLenght.Text.ToString());
                        cmd.Parameters.AddWithValue("@Width", txtWidth.Text.ToString());
                        cmd.Parameters.AddWithValue("@FirstPurchaseDate", txtFirstPurchaseDate.Text.ToString());
                        cmd.Parameters.AddWithValue("@Supplier", ddSupplier.SelectedValue.ToString());
                        cmd.Parameters.AddWithValue("@ImageName", Path.GetFileName(FileUploadImage.PostedFile.FileName));
                        cmd.Parameters.AddWithValue("@ContentType", FileUploadImage.PostedFile.ContentType);
                        cmd.Parameters.AddWithValue("@Photo", bytes);
                        conxx.Open();
                        cmd.ExecuteNonQuery();
                        conxx.Close();
                    }
                }
            }
            {
                Response.Redirect("~/Administrator/Products.aspx");
            }
        }

        protected void btnNewProductCancel_Click(object sender, EventArgs e)
        {

        }

        protected void txtWeight_TextChanged(object sender, EventArgs e)
        {

        }
    }
}