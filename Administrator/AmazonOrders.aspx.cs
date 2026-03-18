using Feniks.Services;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class AmazonOrders : Page
    {
        private string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                BindGrid();
        }

        protected void btnSync_Click(object sender, EventArgs e)
        {
            try
            {
                AmazonOrderSyncService svc = new AmazonOrderSyncService();
                SyncRunResult result = svc.SyncInboxOnly(true);

                if (result.WasSkippedBecauseLocked)
                {
                    lblResult.Text = "<span style='color:#b45309;font-weight:700;'>Another sync is already running. Current request skipped.</span>";
                }
                else
                {
                    lblResult.Text = "<span style='color:green;font-weight:700;'>Amazon inbox sync completed. Orders="
                                     + result.OrderCount + ", NewOrders=" + result.NewOrderCount + ", Items=" + result.ItemCount + "</span>";
                }
            }
            catch (Exception ex)
            {
                try
                {
                    new AmazonOrderSyncService().SaveSyncError(ex.ToString());
                }
                catch { }

                lblResult.Text = "<span style='color:#b91c1c;font-weight:700;'>Error: " + Server.HtmlEncode(ex.Message) + "</span>";
            }

            BindGrid();
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void gvOrders_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "promote")
            {
                try
                {
                    string amazonOrderId = Convert.ToString(e.CommandArgument);
                    AmazonOrderSyncService svc = new AmazonOrderSyncService();
                    svc.PromoteToLamax(amazonOrderId);

                    lblResult.Text = "<span style='color:green;font-weight:700;'>Order promoted to LamaX: " + Server.HtmlEncode(amazonOrderId) + "</span>";
                }
                catch (Exception ex)
                {
                    lblResult.Text = "<span style='color:#b91c1c;font-weight:700;'>Promote error: " + Server.HtmlEncode(ex.Message) + "</span>";
                }

                BindGrid();
            }
        }

        private void BindGrid()
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(@"
SELECT TOP 200
    AmazonOrderId,
    PurchaseDateUtc,
    OrderStatus,
    OrderTotalAmount,
    OrderTotalCurrency,
    ShippingName,
    ItemCount,
    ImportedToLamax,
    LamaxOrderNumber
FROM dbo.V_AmazonOrderInbox_ManualPromote
ORDER BY PurchaseDateUtc DESC, AmazonOrderInboxID DESC;", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvOrders.DataSource = dt;
                gvOrders.DataBind();
            }
        }
    }
}