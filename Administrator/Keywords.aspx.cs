using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class Keywords : System.Web.UI.Page
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
                        KeyWordsQuantityValue();
                        DropDownListData();
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
        }
        private void KeyWordsQuantityValue()
        {
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) KeyWordsQty From V_KeyWordsTotalQuantity"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblKeyWordsQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblKeyWordsQty.Text = "0";
                }
            }
        }
        protected void gvKeywords_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "&nbsp;")
                    {
                        e.Row.Cells[i].Text = "---";
                    }

                    if (e.Row.Cells[i].Text == "No")
                    {
                        e.Row.Cells[3].ForeColor = System.Drawing.Color.Red;
                    }
                }
            }
        }
        protected void gvKeywords_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //FillGrid();
            gvKeywords.PageIndex = e.NewPageIndex;
            gvKeywords.DataBind();
        }
        protected void gvKeywords_SelectedIndexChanged(object sender, EventArgs e)
        {
            lblKeyWordID.Text = gvKeywords.SelectedRow.Cells[1].Text;
            lblKeyWord.Text = gvKeywords.SelectedRow.Cells[2].Text;
            gvKeywords.SelectedRow.BorderColor = System.Drawing.Color.White;

            {
                string s = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                SqlConnection con = new SqlConnection(s);

                SqlCommand cmd1 = new SqlCommand("select Top (1) * from V_KeyWords where KeyWordID='" + lblKeyWordID.Text + "'", con);
                //SqlCommand cmd1 = new SqlCommand(("SELECT Top (1) SatID FROM V_LastSatID where ClaimID='" + lblLoginName.Text + "'"' order by SatID desc")', con))

                cmd1.Connection = con;

                con.Open();



                SqlDataReader dr1 = cmd1.ExecuteReader();

                while (dr1.Read())

                {
                    tblKeyWord.Visible = true;
                }

                dr1.Close();

                con.Close();
            }

            foreach (GridViewRow row in gvKeywords.Rows)
            {
                if (row.RowIndex == gvKeywords.SelectedIndex)
                {
                    row.BackColor = ColorTranslator.FromHtml("#F3F3F3");
                }
                else
                {
                    row.BackColor = ColorTranslator.FromHtml("#FFFFFF");
                }
            }
        }
        protected void toOneBack_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }

        protected void btnNewKeyWord_Click(object sender, EventArgs e)
        {
            {
                lblModalTitle.Text = "Validation Errors List for HP7 Citation";
                lblModalBody.Text = "This is modal body";
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "ModalNewKeyWord", "$('#ModalNewKeyWord').modal();", true);
                upModal.Update();
            }
        }
        private void DropDownListData()
        {
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM T_SpecialDays Order by StartDate asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddSpecialDay.DataSource = cmd.ExecuteReader();
                        ddSpecialDay.DataTextField = "SpecialDay";
                        ddSpecialDay.DataValueField = "SpecialDayID";
                        ddSpecialDay.DataBind();
                        con.Close();
                    }
                }

                ddSpecialDay.Items.Insert(0, new ListItem("select Special Day", "0"));
            }
            {
                string constr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(constr))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM T_SuperStarSelect Order by SuperStar desc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddSuperStar.DataSource = cmd.ExecuteReader();
                        ddSuperStar.DataTextField = "SuperStar";
                        ddSuperStar.DataValueField = "SuperStar";
                        ddSuperStar.DataBind();
                        con.Close();
                    }
                }
                ddSuperStar.Items.Insert(0, new ListItem("select SuperStar", "0"));
            }
        }
        protected void ddSuperStarChange(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", "$('#ModalNewKeyWord').modal('show')", true);
        }
        protected void ddSpecialDayChange(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", "$('#ModalNewKeyWord').modal('show')", true);
        }
        protected void btnNewKeywordSuccessfull_Click(object sender, EventArgs e)
        {
            ClientScript.RegisterStartupScript(this.GetType(), "Popup", "$('#ModalNewKeyWord').modal('show')", true);

            if (txtNewKeyWord.Text.Length == 0)
            {
                AlertKeyword.Visible = true;
            }
            else
            {
                AlertKeyword.Visible = false;

                if (ddSuperStar.SelectedValue == "0")
                {
                    AlertSuperstar.Visible = true;
                }
                else
                {
                    AlertSuperstar.Visible = false;

                    if (ddSpecialDay.SelectedValue == "0")
                    {
                        AlertSpecialDay.Visible = true;
                    }
                    else
                    {
                        AlertSpecialDay.Visible = false;

                        {
                            string constrx = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                            SqlConnection conx = new SqlConnection(constrx);
                            SqlCommand com = new SqlCommand(("select * from V_KeyWordsCheck where KeyWord='" + txtNewKeyWord.Text + "'"), conx);
                            DataTable dtx = new DataTable();
                            SqlDataAdapter dax = new SqlDataAdapter(com);
                            dax.Fill(dtx);
                            if (dtx.Rows.Count > 0)
                            {
                                lblKeyWordCheck.Text = dtx.Rows[0][1].ToString();

                                if (lblKeyWordCheck.Text == "1")
                                {
                                    AlertKeywordCheck.Visible = true;
                                }
                                else
                                {

                                }
                            }
                            else
                            {
                                AlertKeywordCheck.Visible = false;

                                {
                                    SqlConnection con = new SqlConnection(strConnString);
                                    SqlCommand cmd = new SqlCommand("Insert into T_KeyWords (KeyWord,SuperStarKeyWord,SpecialDay) VALUES(@KeyWord,@SuperStarKeyWord,@SpecialDay)", con);
                                    cmd.Parameters.AddWithValue("@KeyWord", txtNewKeyWord.Text);
                                    cmd.Parameters.AddWithValue("@SuperStarKeyWord", ddSuperStar.SelectedValue);
                                    cmd.Parameters.AddWithValue("@SpecialDay", ddSpecialDay.SelectedValue);
                                    con.Open();
                                    cmd.ExecuteNonQuery();

                                    Response.Redirect("~/Administrator/Keywords.aspx");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}