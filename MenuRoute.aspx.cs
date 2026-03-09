using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks
{
    public partial class MenuRoute : System.Web.UI.Page
    {
        String strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        public string query, constr;
        public SqlCommand com;
        public SqlConnection con;

        public void connection()
        {

            constr = ConfigurationManager.ConnectionStrings["dbconn"].ToString();
            con = new SqlConnection(constr);
            con.Open();

        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Page.User.Identity.IsAuthenticated)
            {
                string username = this.Page.User.Identity.Name;
                lblLoginName.Text = username;
            }
            {
                string s = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                SqlConnection conx = new SqlConnection(s);

                SqlCommand cmd1 = new SqlCommand("select FullName,RoleName from V_Users where Username='" + lblLoginName.Text + "'", conx);
                //SqlCommand cmd1 = new SqlCommand(("SELECT Top (1) SatID FROM V_LastSatID where ClaimID='" + lblLoginName.Text + "'"' order by SatID desc")', con))

                cmd1.Connection = conx;

                conx.Open();



                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())

                {
                    lblRole.Text = dr1["RoleName"].ToString();
                    //lblHospitalID.Text = dr1["HospitalID"].ToString();

                    if (lblRole.Text == "Administrator")
                    {
                        Response.Redirect("~/Administrator/MenuforAdmin.aspx");
                        //Response.Redirect("~/Home.aspx");
                    }
                    else if (lblRole.Text == "User")
                    {
                        //Response.Redirect("~/HomeKeyUser.aspx");
                        Response.Redirect("~/UserMainMenu.aspx");
                    }
                }

                dr1.Close();

                conx.Close();
            }
        }
    }
}