using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class PhotoAddforProduct : System.Web.UI.Page
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

        }

        protected void btnNewPhotoSaveCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
        }

        protected void toOneBack_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
        }

        protected void btnMainMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }

        protected void NewPhotoSave_Click(object sender, EventArgs e)
        {
            {
                byte[] bytes;
                using (BinaryReader br = new BinaryReader(FileUploadImage.PostedFile.InputStream))
                {
                    bytes = br.ReadBytes(FileUploadImage.PostedFile.ContentLength);
                }
                {
                    SqlConnection con = new SqlConnection(strConnString);
                    SqlCommand cmd = new SqlCommand("update T_Product set ImageName=@ImageName,ContentType=@ContentType,Photo=@Photo where SKU =@SKU", con);
                    cmd.Parameters.AddWithValue("@SKU", txtSKU.Text);
                    cmd.Parameters.AddWithValue("@ImageName", Path.GetFileName(FileUploadImage.PostedFile.FileName));
                    cmd.Parameters.AddWithValue("@ContentType", FileUploadImage.PostedFile.ContentType);
                    cmd.Parameters.AddWithValue("@Photo", bytes);
                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();

                    Response.Redirect("~/Administrator/Products.aspx");
                }
            }
        }
    }
}