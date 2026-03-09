using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class KeywordsAssigned : System.Web.UI.Page
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
        protected void gvProductSelect_RowDataBound(object sender, GridViewRowEventArgs e)
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
                        (e.Row.FindControl("ProductPhoto") as Image).ImageUrl = imageUrl;
                    }
                }
            }
        }
        protected void gvProductSelect_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvProductSelect.PageIndex = e.NewPageIndex;
        }
        protected void gvProductSelectedList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int KeywordsAssignedDynamicID = Convert.ToInt32(e.CommandArgument);
                SqlConnection con = new SqlConnection(strConnString);
                string cmdText = "DELETE FROM T_KeywordsAssignedDynamic1 WHERE KeywordsAssignedDynamicID=@KeywordsAssignedDynamicID";
                SqlCommand cmd = new SqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@KeywordsAssignedDynamicID", KeywordsAssignedDynamicID);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                gvProductSelectedListViewData();

                if (gvProductSelectedList.Rows.Count == 0)
                {
                    //tblSaveButton.Visible = false;
                    //space1.Visible = false;
                }
            }

            if (gvProductSelectedList.Rows.Count == 0)
            {
                //tblSaveButton.Visible = false;
                //space1.Visible = false;
            }
        }
        private void gvProductSelectedListViewData()
        {
            {
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("SELECT * FROM T_KeywordsAssignedDynamic1", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                gvProductSelectedList.DataSource = dt;
                gvProductSelectedList.DataBind();

                if (gvProductSelectedList.Rows.Count == 0)
                {
                    //tblSaveButton.Visible = false;
                    //space1.Visible = false;
                }
            }
        }
        protected void btnFilterClear_Click(object sender, ImageClickEventArgs e)
        {
            txtFilterSKU.Text = "";
        }
        protected void btnFilter_Click(object sender, ImageClickEventArgs e)
        {

        }
        protected void toOneBack_click(object sender, EventArgs e)
        {
            {
                string CS = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    Int32 rowsAffected;
                    SqlCommand cmd = new SqlCommand("DeleteKeywordsAssignedDynamic1Table", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            {
                string CS = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    Int32 rowsAffected;
                    SqlCommand cmd = new SqlCommand("DeleteKeywordsAssignedDynamic2Table", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            {
                string CS = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    Int32 rowsAffected;
                    SqlCommand cmd = new SqlCommand("DeleteKeywordsAssignedDynamicTable", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();

                    Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
                }
            }
        }
        protected void btnAddList1_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvProductSelect.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {

                    Label strlblSKU = row.FindControl("lblSKU") as Label;
                    Label strlblProductType = row.FindControl("lblProductType") as Label;
                    Label strlblTitle = row.FindControl("lblTitle") as Label;


                    CheckBox chkRow = (row.Cells[0].FindControl("chkRow") as CheckBox);
                    if (chkRow.Checked)
                    {
                        {
                            //tblSaveButton.Visible = true;
                            //ContainergvProductSelectedList.Visible = true;
                            UpdatePanel2.Visible = true;
                            //space1.Visible = true;

                            SqlConnection con = new SqlConnection(strConnString);
                            SqlCommand cmd = new SqlCommand("insert into T_KeywordsAssignedDynamic1 (SKU,ProductType,Title) VALUES(@SKU,@ProductType,@Title)", con);
                            cmd.Parameters.AddWithValue("@SKU", strlblSKU.Text);
                            cmd.Parameters.AddWithValue("@ProductType", strlblProductType.Text);
                            cmd.Parameters.AddWithValue("@Title", strlblTitle.Text);

                            con.Open();
                            cmd.ExecuteNonQuery();

                            gvProductSelectedListViewData();

                            chkRow.Checked = false;
                            //strtxtQty.Text = "0";

                            //dvNextButton.Visible = true; UpdatePanel2.Visible = true;
                        }
                    }
                    else if (chkRow.Checked == false)
                    {
                        if (gvProductSelectedList.Rows.Count == 0)
                        {
                            //tblSaveButton.Visible = false;
                            //dvNextButton.Visible = false; UpdatePanel2.Visible = false;
                            //space1.Visible = false;
                        }
                    }
                }
            }
        }
        protected void gvKeywordsSelect_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void gvKeywordsSelect_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvKeywordsSelect.PageIndex = e.NewPageIndex;
        }
        protected void gvKeywordsSelectedList_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "DeleteRow")
            {
                int KeywordsAssignedDynamicID = Convert.ToInt32(e.CommandArgument);
                SqlConnection con = new SqlConnection(strConnString);
                string cmdText = "DELETE FROM T_KeywordsAssignedDynamic2 WHERE KeywordsAssignedDynamicID=@KeywordsAssignedDynamicID";
                SqlCommand cmd = new SqlCommand(cmdText, con);
                cmd.Parameters.AddWithValue("@KeywordsAssignedDynamicID", KeywordsAssignedDynamicID);
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                gvKeywordsSelectedListViewData();

                if (gvKeywordsSelectedList.Rows.Count == 0)
                {
                    //tblSaveButton.Visible = false;
                    //space1.Visible = false;
                }
            }

            if (gvKeywordsSelectedList.Rows.Count == 0)
            {
                //tblSaveButton.Visible = false;
                //space1.Visible = false;
            }
        }
        private void gvKeywordsSelectedListViewData()
        {
            {
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand cmd = new SqlCommand("SELECT * FROM T_KeywordsAssignedDynamic2", con);
                SqlDataAdapter sda = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                sda.Fill(dt);
                gvKeywordsSelectedList.DataSource = dt;
                gvKeywordsSelectedList.DataBind();

                if (gvKeywordsSelectedList.Rows.Count == 0)
                {
                    //tblSaveButton.Visible = false;
                    //space1.Visible = false;
                }
            }
        }
        protected void btnAddList2_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in gvKeywordsSelect.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {

                    Label strlblKeyWord = row.FindControl("lblKeyWordRow") as Label;
                    Label strlblSuperStarKeyWord = row.FindControl("lblSuperStarKeyWord") as Label;
                    Label strlblSpecialDay = row.FindControl("lblSpecialDay") as Label;


                    CheckBox chkRow = (row.Cells[0].FindControl("chkRow") as CheckBox);
                    if (chkRow.Checked)
                    {
                        {
                            //tblSaveButton.Visible = true;
                            //ContainergvProductSelectedList.Visible = true;
                            //UpdatePanel2.Visible = true;
                            //space1.Visible = true;

                            SqlConnection con = new SqlConnection(strConnString);
                            SqlCommand cmd = new SqlCommand("insert into T_KeywordsAssignedDynamic2 (KeyWord,SuperStarKeyWord,SpecialDay) VALUES(@KeyWord,@SuperStarKeyWord,@SpecialDay)", con);
                            cmd.Parameters.AddWithValue("@KeyWord", strlblKeyWord.Text);
                            cmd.Parameters.AddWithValue("@SuperStarKeyWord", strlblSuperStarKeyWord.Text);
                            cmd.Parameters.AddWithValue("@SpecialDay", strlblSpecialDay.Text);

                            con.Open();
                            cmd.ExecuteNonQuery();

                            gvKeywordsSelectedListViewData();

                            chkRow.Checked = false;
                            //strtxtQty.Text = "0";

                            //dvNextButton.Visible = true; UpdatePanel2.Visible = true;
                        }
                    }
                    else if (chkRow.Checked == false)
                    {
                        if (gvKeywordsSelectedList.Rows.Count == 0)
                        {
                            //tblSaveButton.Visible = false;
                            //dvNextButton.Visible = false; UpdatePanel2.Visible = false;
                            //space1.Visible = false;
                        }
                    }
                }
            }
        }
        protected void btnFilterKeyWordClear_Click(object sender, ImageClickEventArgs e)
        {
            txtFilterKeyWord.Text = "";
        }
        protected void btnFilterKeyWord_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            {
                string CS = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    Int32 rowsAffected;
                    SqlCommand cmd = new SqlCommand("InsertKeywordsAssignedDynamicTable", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            {
                string CS = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    Int32 rowsAffected;
                    SqlCommand cmd = new SqlCommand("InsertListingwithKeyWordsTable", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            {
                string CS = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    Int32 rowsAffected;
                    SqlCommand cmd = new SqlCommand("DeleteKeywordsAssignedDynamic1Table", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            {
                string CS = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    Int32 rowsAffected;
                    SqlCommand cmd = new SqlCommand("DeleteKeywordsAssignedDynamic2Table", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            {
                string CS = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    Int32 rowsAffected;
                    SqlCommand cmd = new SqlCommand("DeleteKeywordsAssignedDynamicTable", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();

                    Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            {
                string CS = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    Int32 rowsAffected;
                    SqlCommand cmd = new SqlCommand("DeleteKeywordsAssignedDynamic1Table", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            {
                string CS = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    Int32 rowsAffected;
                    SqlCommand cmd = new SqlCommand("DeleteKeywordsAssignedDynamic2Table", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();
                }
            }
            {
                string CS = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(CS))
                {
                    Int32 rowsAffected;
                    SqlCommand cmd = new SqlCommand("DeleteKeywordsAssignedDynamicTable", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    rowsAffected = cmd.ExecuteNonQuery();

                    Response.Redirect("~/Administrator/MenuforProductManagement.aspx");
                }
            }
        }

        protected void btnMainMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }

    }
}