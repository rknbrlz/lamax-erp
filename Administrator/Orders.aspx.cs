using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using System.Security.Cryptography;
using System.Web;
using System.Web.UI;
using System.Web.UI.DataVisualization.Charting;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class Orders : System.Web.UI.Page
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
                //FillGrid();

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
                        FillGrid();
                        BindData();
                        DashboardValue();
                        OpenOrderQuantityValue();
                        //BindOrdersGrid();
                        //DataTable dt = this.FillGrid();
                        //gvOrder.DataSource = dt;
                        //gvOrder.DataBind();

                        //Response.AppendHeader("Refresh", "120");

                        {
                            //{

                            //    string constrx = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                            //    SqlConnection conx = new SqlConnection(constrx);
                            //    SqlCommand com = new SqlCommand(("Select * From V_ActualandRemaining"), conx);
                            //    DataTable dtx = new DataTable();
                            //    SqlDataAdapter dax = new SqlDataAdapter(com);
                            //    dax.Fill(dtx);
                            //    if (dtx.Rows.Count > 0)
                            //    {
                            //        //String Rate = dtx.Rows[0][0].ToString();
                            //        {
                            //            //string[] x = new string[dtx.Rows.Count];
                            //            string[] x = new string[dtx.Rows.Count];
                            //            double[] y = new double[dtx.Rows.Count];
                            //            //double[] y2 = new double[dtx.Rows.Count];
                            //            //int[] y = new int[dt.Rows.Count];
                            //            for (int i = 0; i < dtx.Rows.Count; i++)
                            //            {
                            //                x[i] = dtx.Rows[i][0].ToString();
                            //                y[i] = Convert.ToDouble(dtx.Rows[i][3]);
                            //                //y2[i] = Convert.ToDouble(dtx.Rows[i][7]);
                            //                //y[i] = Convert.ToInt32(dt.Rows[i][7]);
                            //            }
                            //            Chart1.Series[0].Points.DataBindXY(x, y);

                            //            Chart1.Series["Data"].ChartType = SeriesChartType.Doughnut;
                            //            //Chart1.Series["Data"]["PieLabelStyle"] = "outside";
                            //            //Chart1.Series["Data"]["PieLineColor"] = "Blue";
                            //            Chart1.Series["Data"]["DoughnutRadius"] = "10";
                            //            Chart1.Series["Data"]["PieStartAngle"] = "270";
                            //            Chart1.ChartAreas[0].InnerPlotPosition.Height = 90;
                            //            Chart1.ChartAreas[0].InnerPlotPosition.Width = 90;
                            //            Chart1.ChartAreas[0].Area3DStyle.Enable3D = true;
                            //            //Chart1.ChartAreas[0].Area3DStyle.Inclination = 0;
                            //            Chart1.Series["Data"].Font = new Font("Arial", 16.0f, FontStyle.Bold);
                            //            //Chart1.Series[0].IsValueShownAsLabel = false;
                            //            foreach (DataPoint p in Chart1.Series["Data"].Points)
                            //            {
                            //                //p.Label = "#PERCENT";
                            //                //p.LabelToolTip = "#VALX";
                            //                //p.LabelForeColor = Color.Brown;
                            //                //p.LegendText = "#VALX";
                            //                //p.LegendToolTip = "#PERCENT";
                            //                //p.LabelToolTip = "#PERCENT\n#VALX";
                            //            }            
                            //        }
                            //    }
                            //    else
                            //    {
                            //        //lblmsg.Text = "We are sorry. No E-mail Campaign found!";
                            //        Chart1.Visible = false;
                            //    }
                            //}
                        }
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

        protected void Chart8_Load(object sender, EventArgs e)
        {
            //Chart8.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            //Chart8.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

        }
        protected void ChartExample_DataBound(object sender, EventArgs e)
        {
            // If there is no data in the series, show a text annotation

            //Chart8.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
            //Chart8.ChartAreas[0].AxisY.MajorGrid.Enabled = false;

            //if (Chart4.Series[0].Points.Count == 0)
            //{
            //    System.Web.UI.DataVisualization.Charting.TextAnnotation annotation =
            //        new System.Web.UI.DataVisualization.Charting.TextAnnotation();
            //    annotation.Text = "No data for this period";
            //    annotation.X = 40;
            //    annotation.Y = 50;
            //    annotation.Font = new System.Drawing.Font("Arial", 12);
            //    annotation.ForeColor = System.Drawing.Color.Red;
            //    Chart4.Annotations.Add(annotation);
            //}
        }
        private void BindData()
        {
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT Marketplace FROM T_Marketplace Order by Marketplace asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddMarketplaceFilter.DataSource = cmd.ExecuteReader();
                        ddMarketplaceFilter.DataTextField = "Marketplace";
                        ddMarketplaceFilter.DataValueField = "Marketplace";
                        ddMarketplaceFilter.DataBind();
                        con.Close();
                    }
                }
                ddMarketplaceFilter.Items.Insert(0, new ListItem("--All--", ""));
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                using (SqlConnection con = new SqlConnection(strConnString))
                {
                    using (SqlCommand cmd = new SqlCommand("SELECT ShippingStatus FROM T_ShippingStatus Order by ShippingStatus asc"))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Connection = con;
                        con.Open();
                        ddStatusFilter.DataSource = cmd.ExecuteReader();
                        ddStatusFilter.DataTextField = "ShippingStatus";
                        ddStatusFilter.DataValueField = "ShippingStatus";
                        ddStatusFilter.DataBind();
                        con.Close();
                    }
                }
                ddStatusFilter.Items.Insert(0, new ListItem("--All--", ""));
            }
        }
        private DataTable FillGrid()
        {
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
            using (SqlConnection con = new SqlConnection(strConnString))
            {
                //using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM V_OrdersforMainPage ORDER BY OrderDate DESC, ShippingStatusID", con))
                using (SqlDataAdapter sda = new SqlDataAdapter("SELECT * FROM T_OrdersPage ORDER BY OrderDate DESC, ShippingStatusID", con))              
                {
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    gvOrder.DataSource = dt;
                    gvOrder.DataBind();

                    return dt;
                }
            }
        }
        protected void RefreshGridView(object sender, EventArgs e)
        {
            FillGrid();
        }
        private void DashboardValue()
        {
            //{
            //    string s = System.Configuration.ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            //    SqlConnection con = new SqlConnection(s);

            //    SqlCommand cmd1 = new SqlCommand("select Top (1) OrderQty from V_OrdersPageDashboard4 where Month='" + lblActualMonth + "'", con);
            //    //SqlCommand cmd1 = new SqlCommand(("SELECT Top (1) SatID FROM V_LastSatID where ClaimID='" + lblLoginName.Text + "'"' order by SatID desc")', con))

            //    cmd1.Connection = con;

            //    con.Open();

            //    SqlDataReader dr1 = cmd1.ExecuteReader();

            //    while (dr1.Read())

            //    {
            //        lblTotalQuantity.Text = dr1["OrderQty"].ToString();

            //    }

            //    dr1.Close();

            //    con.Close();
            //}
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) ActualMonth From V_NowMonth"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblActualMonth.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblActualMonth.Text = "1";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) OrderQty From V_OrdersPageDashboard4"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblTotalQuantity.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblTotalQuantity.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) OrdeTotal,TotalProfit From V_OrdersPageDashboard1"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblTotalOrder.Text = dtx.Rows[0][0].ToString();
                    lblTotalProfit.Text = dtx.Rows[0][1].ToString();
                }
                else
                {
                    lblTotalOrder.Text = "0";
                    lblTotalProfit.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) OrderQty From V_OrdersPageDashboard2 where Marketplace='Etsy'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblEtsyTotalQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblEtsyTotalQty.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) TotalPrice From V_OrdersPageDashboard3 where Marketplace='Etsy'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblEtsyTotalOrder.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblEtsyTotalOrder.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) Profit From V_OrdersPageDashboard3 where Marketplace='Etsy'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblEtsyTotalProfit.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblEtsyTotalProfit.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) OrderQty From V_OrdersPageDashboard2 where Marketplace='Amazon'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblAmazonTotalQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblAmazonTotalQty.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) TotalPrice From V_OrdersPageDashboard3 where Marketplace='Amazon'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblAmazonTotalOrder.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblAmazonTotalOrder.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) Profit From V_OrdersPageDashboard3 where Marketplace='Amazon'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblAmazonTotalProfit.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblAmazonTotalProfit.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) OrderQty From V_OrdersPageDashboard2 where Marketplace='eBay'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lbleBayTotalQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lbleBayTotalQty.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) TotalPrice From V_OrdersPageDashboard3 where Marketplace='eBay'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lbleBayTotalOrder.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lbleBayTotalOrder.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) Profit From V_OrdersPageDashboard3 where Marketplace='eBay'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lbleBayTotalProfit.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lbleBayTotalProfit.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) OrderQty From V_OrdersPageDashboard2 where Marketplace='hgerman.shop'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblhgermanTotalQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblhgermanTotalQty.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) TotalPrice From V_OrdersPageDashboard3 where Marketplace='hgerman.shop'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblhgermanTotalOrder.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblhgermanTotalOrder.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) Profit From V_OrdersPageDashboard3 where Marketplace='hgerman.shop'"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblhgermanTotalProfit.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblhgermanTotalProfit.Text = "0";
                }
            }
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) MonthlyProfit,TargetRemaining From V_Target"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblActual.Text = dtx.Rows[0][0].ToString();
                    lblRemaining.Text = dtx.Rows[0][1].ToString();
                }
                else
                {
                    lblActual.Text = "0";
                    lblRemaining.Text = "0";
                }
            }
        }
        private void OpenOrderQuantityValue()
        {
            {
                string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;
                SqlConnection con = new SqlConnection(strConnString);
                SqlCommand com = new SqlCommand(("Select Top (1) OpenOrderQty From V_OpenOrderQuantity"), con);
                DataTable dtx = new DataTable();
                SqlDataAdapter dax = new SqlDataAdapter(com);
                dax.Fill(dtx);
                if (dtx.Rows.Count > 0)
                {
                    lblOpenQty.Text = dtx.Rows[0][0].ToString();
                }
                else
                {
                    lblOpenQty.Text = "0";
                }
            }
        }
        protected void ddMarketplaceFilter_Changed(object sender, EventArgs e)
        {
            string ProductTypex = ddMarketplaceFilter.SelectedItem.Value;
            DataTable dt = this.FillGrid();
            DataView dataView = dt.DefaultView;
            if (!string.IsNullOrEmpty(ProductTypex))
            {
                dataView.RowFilter = "Marketplace = '" + ProductTypex + "'";
            }
            gvOrder.DataSource = dataView;
            gvOrder.DataBind();
        }
        protected void ddStatusFilter_Changed(object sender, EventArgs e)
        {
            string ProductTypex = ddStatusFilter.SelectedItem.Value;
            DataTable dt = this.FillGrid();
            DataView dataView = dt.DefaultView;
            if (!string.IsNullOrEmpty(ProductTypex))
            {
                dataView.RowFilter = "ShippingStatus = '" + ProductTypex + "'";
            }
            gvOrder.DataSource = dataView;
            gvOrder.DataBind();
        }
        protected void gvOrder_RowDataBound(object sender, GridViewRowEventArgs e)
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

                Label lblShippingStatus = (Label)e.Row.FindControl("lblShippingStatusID");

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

                if (lblShippingStatus.Text == "1")
                {
                    //btnShippingStatus1.Visible = true;
                    Status1.Visible = true;
                }
                else if (lblShippingStatus.Text == "2")
                {
                    //btnShippingStatus2.Visible = true;
                    Status2.Visible = true;
                }
                else if (lblShippingStatus.Text == "3")
                {
                    Status3.Visible = true;
                }
                else if (lblShippingStatus.Text == "4")
                {
                    Status4.Visible = true;
                }
                else if (lblShippingStatus.Text == "5")
                {
                    Status5.Visible = true;
                }
                else if (lblShippingStatus.Text == "6")
                {
                    Status6.Visible = true;
                }
                else if (lblShippingStatus.Text == "7")
                {
                    Status7.Visible = true;
                }
                else if (lblShippingStatus.Text == "8")
                {
                    Status8.Visible = true;
                }
                else if (lblShippingStatus.Text == "9")
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
        protected void gvOrder_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            FillGrid();
            gvOrder.PageIndex = e.NewPageIndex;
            gvOrder.DataBind();
        }
        protected void gvOrder_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "btnOrderDetails")
            {
                lblOrderNumberforOrderDetails.Text = e.CommandArgument.ToString();

                Response.Redirect("~/Administrator/OrderDetails.aspx?OrderID=" + lblOrderNumberforOrderDetails.Text);
            }
        }
        protected void btnBack_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }
        protected void btnShipping_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/Shipping.aspx");
        }


        [System.Web.Services.WebMethod]
        public static string SaveShippingInfo(string OrderNumber, string ShippingCompanyID, string KKID, string TrackingNumber, string ShipDate, string ShippingPrice)
        {
            string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connStr))
            {
                con.Open();

                // Eğer shipping kaydı varsa update et yoksa insert et
                string sqlCheck = "SELECT COUNT(*) FROM T_Shipping WHERE OrderNumber=@OrderNumber";
                SqlCommand cmdCheck = new SqlCommand(sqlCheck, con);
                cmdCheck.Parameters.AddWithValue("@OrderNumber", OrderNumber);

                int exists = Convert.ToInt32(cmdCheck.ExecuteScalar());

                string sql;

                if (exists > 0)
                {
                    sql = @"UPDATE T_Shipping
                    SET ShippingCompanyID=@ShippingCompanyID,
                        KKID=@KKID,
                        TrackingNumber=@TrackingNumber,
                        ShipDate=@ShipDate,
                        ShippingPrice=@ShippingPrice
                    WHERE OrderNumber=@OrderNumber";
                }
                else
                {
                    sql = @"INSERT INTO T_Shipping (OrderNumber, ShippingCompanyID, KKID, TrackingNumber, ShipDate, ShippingPrice)
                    VALUES (@OrderNumber, @ShippingCompanyID, @KKID, @TrackingNumber, @ShipDate, @ShippingPrice)";
                }

                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.Parameters.AddWithValue("@OrderNumber", OrderNumber);
                cmd.Parameters.AddWithValue("@ShippingCompanyID", ShippingCompanyID);
                cmd.Parameters.AddWithValue("@KKID", KKID);
                cmd.Parameters.AddWithValue("@TrackingNumber", TrackingNumber);

                DateTime parsedDate;
                if (DateTime.TryParse(ShipDate, out parsedDate))
                    cmd.Parameters.AddWithValue("@ShipDate", parsedDate);
                else
                    cmd.Parameters.AddWithValue("@ShipDate", DBNull.Value);

                decimal price;
                if (Decimal.TryParse(ShippingPrice, out price))
                    cmd.Parameters.AddWithValue("@ShippingPrice", price);
                else
                    cmd.Parameters.AddWithValue("@ShippingPrice", 0);

                cmd.ExecuteNonQuery();
            }

            return "OK";
        }


        protected void btnDeleteFromModal_Click(object sender, EventArgs e)
        {
            string orderNumber = hfOrderToDelete.Value;
            if (!string.IsNullOrEmpty(orderNumber))
            {
                DeleteOrderRecords(orderNumber);
                

                // AJAX sonrası modal’ı kapat ve grid yenile
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "closeModalAndRefresh",
                    "$('#orderDetailsModal').modal('hide');", true);

                FillGrid();
            }
        }

        private void DeleteOrderRecords(string order)
        {
            string strConnString = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

            using (SqlConnection con = new SqlConnection(strConnString))
            {
                con.Open();

                string sql = @"
            DELETE FROM T_Shipping      WHERE OrderNumber = @Order;
            DELETE FROM T_PackExpenses      WHERE OrderNumber = @Order;
            DELETE FROM T_Order      WHERE OrderNumber = @Order;
            DELETE FROM T_OrdersPage WHERE OrderNumber = @Order;
        ";

                using (SqlCommand cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Order", order);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        private void RebindOrdersGrid()
        {
            DataTable dt = FillGrid(); // FillGrid hem dt döndürüyor hem gvOrder bind ediyor (şu an öyle)

            // Eğer filtre seçiliyse, aynı filtreyi refresh sonrası tekrar uygula:
            string marketplace = ddMarketplaceFilter.SelectedValue;
            string status = ddStatusFilter.SelectedValue;

            DataView dv = dt.DefaultView;

            List<string> filters = new List<string>();

            if (!string.IsNullOrEmpty(marketplace))
                filters.Add("Marketplace = '" + marketplace.Replace("'", "''") + "'");

            if (!string.IsNullOrEmpty(status))
                filters.Add("ShippingStatus = '" + status.Replace("'", "''") + "'");

            dv.RowFilter = string.Join(" AND ", filters);

            gvOrder.DataSource = dv;
            gvOrder.DataBind();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            RebindOrdersGrid();
            DashboardValue();
            OpenOrderQuantityValue();
        }

        protected void btnNewOrder_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Administrator/OrderCreate.aspx");
        }

        protected void btnDeleteOrder_Click(object sender, EventArgs e)
        {

        }

        protected void btnAddExpenses_Click(object sender, EventArgs e)
        {
            //Response.Redirect("~/Administrator/MenuforAdmin.aspx");
        }
    }
}