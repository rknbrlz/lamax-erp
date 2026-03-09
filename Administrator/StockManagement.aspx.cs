using Antlr.Runtime;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class StockManagement : System.Web.UI.Page
    {
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
                        //FillNoRingGrid();
                        DashboardValue();

                        //DataTable dt = this.FillGrid();
                        //gvStock.DataSource = dt;
                        //gvStock.DataBind();

                        //DataTable dtx = this.FillNoRingGrid();
                        //gvStockNoRing.DataSource = dtx;
                        //gvStockNoRing.DataBind();
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
        //        using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM V_StockRing order by SKU asc", con))
        //        {
        //            DataTable dt = new DataTable();
        //            sda.Fill(dt);
        //            gvStock.DataSource = dt;
        //            gvStock.DataBind();

        //            return dt;
        //        }
        //    }
        //}
        //private DataTable FillNoRingGrid()
        //{
        //    string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
        //    using (SqlConnection con = new SqlConnection(strConnString))
        //    {
        //        using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM V_StockNoRing order by SKU desc", con))
        //        {
        //            DataTable dt = new DataTable();
        //            sda.Fill(dt);
        //            gvStockNoRing.DataSource = dt;
        //            gvStockNoRing.DataBind();

        //            return dt;
        //        }
        //    }
        //}
        private void DashboardValue()
        {
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) RingStockQty From V_RingStockQty"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblRingStockQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblRingStockQty.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) StockQty From V_NoRingStockQty where ProductType='Necklace'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblNecklaceQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblNecklaceQty.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) StockQty From V_NoRingStockQty where ProductType='Earrings'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblEarringsceQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblEarringsceQty.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) StockQty From V_NoRingStockQty where ProductType='Bracelet'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblBraceletQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblBraceletQty.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) StockQty From V_NoRingStockQty where ProductType='Pendant'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblPendantQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblPendantQty.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) StockQty From V_NoRingStockQty where ProductType='Set'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblSetQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblSetQty.Text = "0";
                }
            }
        }
        protected void gvStock_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            HtmlGenericControl divadj1;
            HtmlGenericControl divadj2;
            HtmlGenericControl divadj3;

            HtmlGenericControl div5USCA1;
            HtmlGenericControl div5USCA2;
            HtmlGenericControl div5USCA3;

            HtmlGenericControl div512USCA1;
            HtmlGenericControl div512USCA2;
            HtmlGenericControl div512USCA3;

            HtmlGenericControl div6USCA1;
            HtmlGenericControl div6USCA2;
            HtmlGenericControl div6USCA3;

            HtmlGenericControl div612USCA1;
            HtmlGenericControl div612USCA2;
            HtmlGenericControl div612USCA3;

            HtmlGenericControl div7USCA1;
            HtmlGenericControl div7USCA2;
            HtmlGenericControl div7USCA3;

            HtmlGenericControl div712USCA1;
            HtmlGenericControl div712USCA2;
            HtmlGenericControl div712USCA3;

            HtmlGenericControl div8USCA1;
            HtmlGenericControl div8USCA2;
            HtmlGenericControl div8USCA3;

            HtmlGenericControl div812USCA1;
            HtmlGenericControl div812USCA2;
            HtmlGenericControl div812USCA3;

            HtmlGenericControl div9USCA1;
            HtmlGenericControl div9USCA2;
            HtmlGenericControl div9USCA3;

            HtmlGenericControl div912USCA1;
            HtmlGenericControl div912USCA2;
            HtmlGenericControl div912USCA3;

            HtmlGenericControl div10USCA1;
            HtmlGenericControl div10USCA2;
            HtmlGenericControl div10USCA3;

            HtmlGenericControl div1012USCA1;
            HtmlGenericControl div1012USCA2;
            HtmlGenericControl div1012USCA3;

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                for (int i = 0; i < e.Row.Cells.Count; i++)
                {
                    if (e.Row.Cells[i].Text == "&nbsp;")
                    {
                        e.Row.Cells[i].Text = "---";
                    }
                }
                int zerostock;
                int criticstock;
                int AdjStockQty;
                zerostock = 0;
                criticstock = 5;

                Label lblAdjustable1 = (Label)e.Row.FindControl("lblAdjustable1");
                AdjStockQty = int.Parse(lblAdjustable1.Text);

                divadj1 = (HtmlGenericControl)e.Row.FindControl("divadj1") as HtmlGenericControl;
                divadj2 = (HtmlGenericControl)e.Row.FindControl("divadj2") as HtmlGenericControl;
                divadj3 = (HtmlGenericControl)e.Row.FindControl("divadj3") as HtmlGenericControl;

                if (AdjStockQty >= 5)
                {
                    divadj1.Visible = true;
                    divadj2.Visible = false;
                    divadj3.Visible = false;
                }
                else if (AdjStockQty <= 0)
                {
                    divadj3.Visible = true;
                    divadj1.Visible = false;
                    divadj2.Visible = false;
                }
                else if (AdjStockQty == 1 && AdjStockQty == 2 && AdjStockQty == 3 && AdjStockQty == 4 && AdjStockQty == 5)
                {
                    divadj2.Visible = true;
                    divadj3.Visible = false;
                    divadj1.Visible = false;
                }

                //int zerostock;
                //int criticstock;

                //zerostock = 0;
                //criticstock = 5;

                int x5USCAQty;
                Label lbl5USCA1 = (Label)e.Row.FindControl("lbl5USCA1");
                x5USCAQty = int.Parse(lbl5USCA1.Text);

                div5USCA1 = (HtmlGenericControl)e.Row.FindControl("div5USCA1") as HtmlGenericControl;
                div5USCA2 = (HtmlGenericControl)e.Row.FindControl("div5USCA2") as HtmlGenericControl;
                div5USCA3 = (HtmlGenericControl)e.Row.FindControl("div5USCA3") as HtmlGenericControl;

                if (x5USCAQty >= 5)
                {
                    div5USCA1.Visible = true;
                    div5USCA2.Visible = false;
                    div5USCA3.Visible = false;
                }
                else if (x5USCAQty <= 0)
                {
                    div5USCA3.Visible = true;
                    div5USCA1.Visible = false;
                    div5USCA2.Visible = false;
                }
                else if (x5USCAQty == 1)
                {
                    div5USCA2.Visible = true;
                    div5USCA3.Visible = false;
                    div5USCA1.Visible = false;
                }
                else if (x5USCAQty == 2)
                {
                    div5USCA2.Visible = true;
                    div5USCA3.Visible = false;
                    div5USCA1.Visible = false;
                }
                else if (x5USCAQty == 3)
                {
                    div5USCA2.Visible = true;
                    div5USCA3.Visible = false;
                    div5USCA1.Visible = false;
                }
                else if (x5USCAQty == 4)
                {
                    div5USCA2.Visible = true;
                    div5USCA3.Visible = false;
                    div5USCA1.Visible = false;
                }
                else if (x5USCAQty == 5)
                {
                    div5USCA2.Visible = true;
                    div5USCA3.Visible = false;
                    div5USCA1.Visible = false;
                }

                int x512USCAQty;
                Label lbl512USCA1 = (Label)e.Row.FindControl("lbl512USCA1");
                x512USCAQty = int.Parse(lbl512USCA1.Text);

                div512USCA1 = (HtmlGenericControl)e.Row.FindControl("div512USCA1") as HtmlGenericControl;
                div512USCA2 = (HtmlGenericControl)e.Row.FindControl("div512USCA2") as HtmlGenericControl;
                div512USCA3 = (HtmlGenericControl)e.Row.FindControl("div512USCA3") as HtmlGenericControl;

                if (x512USCAQty >= 5)
                {
                    div512USCA1.Visible = true;
                    div512USCA2.Visible = false;
                    div512USCA3.Visible = false;
                }
                else if (x512USCAQty <= 0)
                {
                    div512USCA3.Visible = true;
                    div512USCA1.Visible = false;
                    div512USCA2.Visible = false;
                }
                else if (x512USCAQty == 1)
                {
                    div512USCA2.Visible = true;
                    div512USCA3.Visible = false;
                    div512USCA1.Visible = false;
                }
                else if (x512USCAQty == 2)
                {
                    div512USCA2.Visible = true;
                    div512USCA3.Visible = false;
                    div512USCA1.Visible = false;
                }
                else if (x512USCAQty == 3)
                {
                    div512USCA2.Visible = true;
                    div512USCA3.Visible = false;
                    div512USCA1.Visible = false;
                }
                else if (x512USCAQty == 4)
                {
                    div512USCA2.Visible = true;
                    div512USCA3.Visible = false;
                    div512USCA1.Visible = false;
                }
                else if (x512USCAQty == 5)
                {
                    div512USCA2.Visible = true;
                    div512USCA3.Visible = false;
                    div512USCA1.Visible = false;
                }

                int x6USCAQty;
                Label lbl6USCA1 = (Label)e.Row.FindControl("lbl6USCA1");
                x6USCAQty = int.Parse(lbl6USCA1.Text);

                div6USCA1 = (HtmlGenericControl)e.Row.FindControl("div6USCA1") as HtmlGenericControl;
                div6USCA2 = (HtmlGenericControl)e.Row.FindControl("div6USCA2") as HtmlGenericControl;
                div6USCA3 = (HtmlGenericControl)e.Row.FindControl("div6USCA3") as HtmlGenericControl;

                if (x6USCAQty >= 5)
                {
                    div6USCA1.Visible = true;
                    div6USCA2.Visible = false;
                    div6USCA3.Visible = false;
                }
                else if (x6USCAQty <= 0 )
                {
                    div6USCA3.Visible = true;
                    div6USCA1.Visible = false;
                    div6USCA2.Visible = false;
                }
                else if (x6USCAQty == 1)
                {
                    div6USCA2.Visible = true;
                    div6USCA3.Visible = false;
                    div6USCA1.Visible = false;
                }
                else if (x6USCAQty == 2)
                {
                    div6USCA2.Visible = true;
                    div6USCA3.Visible = false;
                    div6USCA1.Visible = false;
                }
                else if (x6USCAQty == 3)
                {
                    div6USCA2.Visible = true;
                    div6USCA3.Visible = false;
                    div6USCA1.Visible = false;
                }
                else if (x6USCAQty == 4)
                {
                    div6USCA2.Visible = true;
                    div6USCA3.Visible = false;
                    div6USCA1.Visible = false;
                }
                else if (x6USCAQty == 5)
                {
                    div6USCA2.Visible = true;
                    div6USCA3.Visible = false;
                    div6USCA1.Visible = false;
                }

                int x612USCAQty;
                Label lbl612USCA1 = (Label)e.Row.FindControl("lbl612USCA1");
                x612USCAQty = int.Parse(lbl612USCA1.Text);

                div612USCA1 = (HtmlGenericControl)e.Row.FindControl("div612USCA1") as HtmlGenericControl;
                div612USCA2 = (HtmlGenericControl)e.Row.FindControl("div612USCA2") as HtmlGenericControl;
                div612USCA3 = (HtmlGenericControl)e.Row.FindControl("div612USCA3") as HtmlGenericControl;

                if (x612USCAQty >= 5)
                {
                    div612USCA1.Visible = true;
                    div612USCA2.Visible = false;
                    div612USCA3.Visible = false;
                }
                else if (x612USCAQty <= 0 )
                {
                    div612USCA3.Visible = true;
                    div612USCA1.Visible = false;
                    div612USCA2.Visible = false;
                }
                else if (x612USCAQty == 1)
                {
                    div612USCA2.Visible = true;
                    div612USCA3.Visible = false;
                    div612USCA1.Visible = false;
                }
                else if (x612USCAQty == 2)
                {
                    div612USCA2.Visible = true;
                    div612USCA3.Visible = false;
                    div612USCA1.Visible = false;
                }
                else if (x612USCAQty == 3)
                {
                    div612USCA2.Visible = true;
                    div612USCA3.Visible = false;
                    div612USCA1.Visible = false;
                }
                else if (x612USCAQty == 4)
                {
                    div612USCA2.Visible = true;
                    div612USCA3.Visible = false;
                    div612USCA1.Visible = false;
                }
                else if (x612USCAQty == 5)
                {
                    div612USCA2.Visible = true;
                    div612USCA3.Visible = false;
                    div612USCA1.Visible = false;
                }

                int x7USCAQty;
                Label lbl7USCA1 = (Label)e.Row.FindControl("lbl7USCA1");
                x7USCAQty = int.Parse(lbl7USCA1.Text);

                div7USCA1 = (HtmlGenericControl)e.Row.FindControl("div7USCA1") as HtmlGenericControl;
                div7USCA2 = (HtmlGenericControl)e.Row.FindControl("div7USCA2") as HtmlGenericControl;
                div7USCA3 = (HtmlGenericControl)e.Row.FindControl("div7USCA3") as HtmlGenericControl;

                if (x7USCAQty >= 5)
                {
                    div7USCA1.Visible = true;
                    div7USCA2.Visible = false;
                    div7USCA3.Visible = false;
                }
                else if (x7USCAQty <= 0 )
                {
                    div7USCA3.Visible = true;
                    div7USCA1.Visible = false;
                    div7USCA2.Visible = false;
                }
                else if (x7USCAQty == 1)
                {
                    div7USCA2.Visible = true;
                    div7USCA3.Visible = false;
                    div7USCA1.Visible = false;
                }
                else if (x7USCAQty == 2)
                {
                    div7USCA2.Visible = true;
                    div7USCA3.Visible = false;
                    div7USCA1.Visible = false;
                }
                else if (x7USCAQty == 3)
                {
                    div7USCA2.Visible = true;
                    div7USCA3.Visible = false;
                    div7USCA1.Visible = false;
                }
                else if (x7USCAQty == 4)
                {
                    div7USCA2.Visible = true;
                    div7USCA3.Visible = false;
                    div7USCA1.Visible = false;
                }
                else if (x7USCAQty == 5)
                {
                    div7USCA2.Visible = true;
                    div7USCA3.Visible = false;
                    div7USCA1.Visible = false;
                }

                int x712USCAQty;
                Label lbl712USCA1 = (Label)e.Row.FindControl("lbl712USCA1");
                x712USCAQty = int.Parse(lbl712USCA1.Text);

                div712USCA1 = (HtmlGenericControl)e.Row.FindControl("div712USCA1") as HtmlGenericControl;
                div712USCA2 = (HtmlGenericControl)e.Row.FindControl("div712USCA2") as HtmlGenericControl;
                div712USCA3 = (HtmlGenericControl)e.Row.FindControl("div712USCA3") as HtmlGenericControl;

                if (x712USCAQty >= 5)
                {
                    div712USCA1.Visible = true;
                    div712USCA2.Visible = false;
                    div712USCA3.Visible = false;
                }
                else if (x712USCAQty <= 0 )
                {
                    div712USCA3.Visible = true;
                    div712USCA1.Visible = false;
                    div712USCA2.Visible = false;
                }
                else if (x712USCAQty == 1)
                {
                    div712USCA2.Visible = true;
                    div712USCA3.Visible = false;
                    div712USCA1.Visible = false;
                }
                else if (x712USCAQty == 2)
                {
                    div712USCA2.Visible = true;
                    div712USCA3.Visible = false;
                    div712USCA1.Visible = false;
                }
                else if (x712USCAQty == 3)
                {
                    div712USCA2.Visible = true;
                    div712USCA3.Visible = false;
                    div712USCA1.Visible = false;
                }
                else if (x712USCAQty == 4)
                {
                    div712USCA2.Visible = true;
                    div712USCA3.Visible = false;
                    div712USCA1.Visible = false;
                }
                else if (x712USCAQty == 5)
                {
                    div712USCA2.Visible = true;
                    div712USCA3.Visible = false;
                    div712USCA1.Visible = false;
                }

                int x8USCAQty;
                Label lbl8USCA1 = (Label)e.Row.FindControl("lbl8USCA1");
                x8USCAQty = int.Parse(lbl8USCA1.Text);

                div8USCA1 = (HtmlGenericControl)e.Row.FindControl("div8USCA1") as HtmlGenericControl;
                div8USCA2 = (HtmlGenericControl)e.Row.FindControl("div8USCA2") as HtmlGenericControl;
                div8USCA3 = (HtmlGenericControl)e.Row.FindControl("div8USCA3") as HtmlGenericControl;

                if (x8USCAQty >= 5)
                {
                    div8USCA1.Visible = true;
                    div8USCA2.Visible = false;
                    div8USCA3.Visible = false;
                }
                else if (x8USCAQty <= 0 )
                {
                    div8USCA3.Visible = true;
                    div8USCA1.Visible = false;
                    div8USCA2.Visible = false;
                }
                else if (x8USCAQty == 1)
                {
                    div8USCA2.Visible = true;
                    div8USCA3.Visible = false;
                    div8USCA1.Visible = false;
                }
                else if (x8USCAQty == 2)
                {
                    div8USCA2.Visible = true;
                    div8USCA3.Visible = false;
                    div8USCA1.Visible = false;
                }
                else if (x8USCAQty == 3)
                {
                    div8USCA2.Visible = true;
                    div8USCA3.Visible = false;
                    div8USCA1.Visible = false;
                }
                else if (x8USCAQty == 4)
                {
                    div8USCA2.Visible = true;
                    div8USCA3.Visible = false;
                    div8USCA1.Visible = false;
                }
                else if (x8USCAQty == 5)
                {
                    div8USCA2.Visible = true;
                    div8USCA3.Visible = false;
                    div8USCA1.Visible = false;
                }

                int x812USCAQty;
                Label lbl812USCA1 = (Label)e.Row.FindControl("lbl812USCA1");
                x812USCAQty = int.Parse(lbl812USCA1.Text);

                div812USCA1 = (HtmlGenericControl)e.Row.FindControl("div812USCA1") as HtmlGenericControl;
                div812USCA2 = (HtmlGenericControl)e.Row.FindControl("div812USCA2") as HtmlGenericControl;
                div812USCA3 = (HtmlGenericControl)e.Row.FindControl("div812USCA3") as HtmlGenericControl;

                if (x812USCAQty >= 5)
                {
                    div812USCA1.Visible = true;
                    div812USCA2.Visible = false;
                    div812USCA3.Visible = false;
                }
                else if (x812USCAQty <= 0 )
                {
                    div812USCA3.Visible = true;
                    div812USCA1.Visible = false;
                    div812USCA2.Visible = false;
                }
                else if (x812USCAQty == 1)
                {
                    div812USCA2.Visible = true;
                    div812USCA3.Visible = false;
                    div812USCA1.Visible = false;
                }
                else if (x812USCAQty == 2)
                {
                    div812USCA2.Visible = true;
                    div812USCA3.Visible = false;
                    div812USCA1.Visible = false;
                }
                else if (x812USCAQty == 3)
                {
                    div812USCA2.Visible = true;
                    div812USCA3.Visible = false;
                    div812USCA1.Visible = false;
                }
                else if (x812USCAQty == 4)
                {
                    div812USCA2.Visible = true;
                    div812USCA3.Visible = false;
                    div812USCA1.Visible = false;
                }
                else if (x812USCAQty == 5)
                {
                    div812USCA2.Visible = true;
                    div812USCA3.Visible = false;
                    div812USCA1.Visible = false;
                }

                int x9USCAQty;
                Label lbl9USCA1 = (Label)e.Row.FindControl("lbl9USCA1");
                x9USCAQty = int.Parse(lbl9USCA1.Text);

                div9USCA1 = (HtmlGenericControl)e.Row.FindControl("div9USCA1") as HtmlGenericControl;
                div9USCA2 = (HtmlGenericControl)e.Row.FindControl("div9USCA2") as HtmlGenericControl;
                div9USCA3 = (HtmlGenericControl)e.Row.FindControl("div9USCA3") as HtmlGenericControl;

                if (x9USCAQty >= 5)
                {
                    div9USCA1.Visible = true;
                    div9USCA2.Visible = false;
                    div9USCA3.Visible = false;
                }
                else if (x9USCAQty <= 0)
                {
                    div9USCA3.Visible = true;
                    div9USCA1.Visible = false;
                    div9USCA2.Visible = false;
                }
                else if (x9USCAQty == 1 && x9USCAQty == 2 && x9USCAQty == 3 && x9USCAQty == 4 && x9USCAQty == 5)
                {
                    div9USCA2.Visible = true;
                    div9USCA3.Visible = false;
                    div9USCA1.Visible = false;
                }

                int x912USCAQty;
                Label lbl912USCA1 = (Label)e.Row.FindControl("lbl912USCA1");
                x912USCAQty = int.Parse(lbl912USCA1.Text);

                div912USCA1 = (HtmlGenericControl)e.Row.FindControl("div912USCA1") as HtmlGenericControl;
                div912USCA2 = (HtmlGenericControl)e.Row.FindControl("div912USCA2") as HtmlGenericControl;
                div912USCA3 = (HtmlGenericControl)e.Row.FindControl("div912USCA3") as HtmlGenericControl;

                if (x912USCAQty >= 5)
                {
                    div912USCA1.Visible = true;
                    div912USCA2.Visible = false;
                    div912USCA3.Visible = false;
                }
                else if (x912USCAQty <= 0)
                {
                    div912USCA3.Visible = true;
                    div912USCA1.Visible = false;
                    div912USCA2.Visible = false;
                }
                else if (x912USCAQty == 1)
                {
                    div912USCA2.Visible = true;
                    div912USCA3.Visible = false;
                    div912USCA1.Visible = false;
                }
                else if (x912USCAQty == 2)
                {
                    div912USCA2.Visible = true;
                    div912USCA3.Visible = false;
                    div912USCA1.Visible = false;
                }
                else if (x912USCAQty == 3)
                {
                    div912USCA2.Visible = true;
                    div912USCA3.Visible = false;
                    div912USCA1.Visible = false;
                }
                else if (x912USCAQty == 4)
                {
                    div912USCA2.Visible = true;
                    div912USCA3.Visible = false;
                    div912USCA1.Visible = false;
                }
                else if (x912USCAQty == 5)
                {
                    div912USCA2.Visible = true;
                    div912USCA3.Visible = false;
                    div912USCA1.Visible = false;
                }

                int x10USCAQty;
                Label lbl10USCA1 = (Label)e.Row.FindControl("lbl10USCA1");
                x10USCAQty = int.Parse(lbl10USCA1.Text);

                div10USCA1 = (HtmlGenericControl)e.Row.FindControl("div10USCA1") as HtmlGenericControl;
                div10USCA2 = (HtmlGenericControl)e.Row.FindControl("div10USCA2") as HtmlGenericControl;
                div10USCA3 = (HtmlGenericControl)e.Row.FindControl("div10USCA3") as HtmlGenericControl;

                if (x10USCAQty >= 5)
                {
                    div10USCA1.Visible = true;
                    div10USCA2.Visible = false;
                    div10USCA3.Visible = false;
                }
                else if (x10USCAQty <= 0)
                {
                    div10USCA3.Visible = true;
                    div10USCA1.Visible = false;
                    div10USCA2.Visible = false;
                }
                else if (x10USCAQty == 1)
                {
                    div10USCA2.Visible = true;
                    div10USCA3.Visible = false;
                    div10USCA1.Visible = false;
                }
                else if (x10USCAQty == 2)
                {
                    div10USCA2.Visible = true;
                    div10USCA3.Visible = false;
                    div10USCA1.Visible = false;
                }
                else if (x10USCAQty == 3)
                {
                    div10USCA2.Visible = true;
                    div10USCA3.Visible = false;
                    div10USCA1.Visible = false;
                }
                else if (x10USCAQty == 4)
                {
                    div10USCA2.Visible = true;
                    div10USCA3.Visible = false;
                    div10USCA1.Visible = false;
                }
                else if (x10USCAQty == 5)
                {
                    div10USCA2.Visible = true;
                    div10USCA3.Visible = false;
                    div10USCA1.Visible = false;
                }

                int x1012USCAQty;
                Label lbl1012USCA1 = (Label)e.Row.FindControl("lbl1012USCA1");
                x1012USCAQty = int.Parse(lbl1012USCA1.Text);

                div1012USCA1 = (HtmlGenericControl)e.Row.FindControl("div1012USCA1") as HtmlGenericControl;
                div1012USCA2 = (HtmlGenericControl)e.Row.FindControl("div1012USCA2") as HtmlGenericControl;
                div1012USCA3 = (HtmlGenericControl)e.Row.FindControl("div1012USCA3") as HtmlGenericControl;

                if (x1012USCAQty >= 5)
                {
                    div1012USCA1.Visible = true;
                    div1012USCA2.Visible = false;
                    div1012USCA3.Visible = false;
                }
                else if (x1012USCAQty <= 0 )
                {
                    div1012USCA3.Visible = true;
                    div1012USCA1.Visible = false;
                    div1012USCA2.Visible = false;
                }
                else if (x1012USCAQty == 1)
                {
                    div1012USCA2.Visible = true;
                    div1012USCA3.Visible = false;
                    div1012USCA1.Visible = false;
                }
                else if (x1012USCAQty == 2)
                {
                    div1012USCA2.Visible = true;
                    div1012USCA3.Visible = false;
                    div1012USCA1.Visible = false;
                }
                else if (x1012USCAQty == 3)
                {
                    div1012USCA2.Visible = true;
                    div1012USCA3.Visible = false;
                    div1012USCA1.Visible = false;
                }
                else if (x1012USCAQty == 4)
                {
                    div1012USCA2.Visible = true;
                    div1012USCA3.Visible = false;
                    div1012USCA1.Visible = false;
                }
                else if (x1012USCAQty == 5)
                {
                    div1012USCA2.Visible = true;
                    div1012USCA3.Visible = false;
                    div1012USCA1.Visible = false;
                }
            }
        }
        protected void gvStockNoRing_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void gvStock_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //FillGrid();
            //gvStock.PageIndex = e.NewPageIndex;
            //gvStock.DataBind();
            gvStock.PageIndex = e.NewPageIndex;
        }
        protected void gvStockNoRing_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            //FillNoRingGrid();
            gvStockNoRing.PageIndex = e.NewPageIndex;
            //gvStockNoRing.DataBind();
        }
        protected void ImageButton8_Click(object sender, ImageClickEventArgs e)
        {

        }
        protected void ImageButton1_Click(object sender, ImageClickEventArgs e)
        {

        }
        protected void btnFilterClear_Click(object sender, ImageClickEventArgs e)
        {
            txtFilterSKU.Text = "";
        }
        protected void btnFilter2Clear_Click(object sender, ImageClickEventArgs e)
        {
            TextBox1.Text = "";
        }
        protected void btnMainMenu_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }

        protected void toOneBack_click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/Orders.aspx");
        }

        protected void btnStockEntry_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/StockEntry.aspx");
        }
    }
}