using Feniks.Services;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class EtsyOrdersInbox : Page
    {
        private readonly string _cs = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                TryAutoSync();
                BindGrid();
                BindSyncInfo();
            }
        }

        private void TryAutoSync()
        {
            try
            {
                int autoMinutes = 5;
                int.TryParse(ConfigurationManager.AppSettings["EtsyAutoSyncMinutes"], out autoMinutes);
                if (autoMinutes <= 0) autoMinutes = 5;

                DateTime? lastSyncUtc = GetLastSyncUtc();
                if (!lastSyncUtc.HasValue || lastSyncUtc.Value < DateTime.UtcNow.AddMinutes(-autoMinutes))
                {
                    var sync = new EtsyOrderSyncService();
                    sync.Sync(true);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    var sync = new EtsyOrderSyncService();
                    sync.SaveSyncError(ex.ToString());
                }
                catch { }
            }
        }

        private DateTime? GetLastSyncUtc()
        {
            if (!HasTable("dbo.T_EtsySyncState"))
                return null;

            try
            {
                long shopId = new EtsyApiClient().ShopId;

                using (SqlConnection con = new SqlConnection(_cs))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 1 LastSyncUtc
                    FROM dbo.T_EtsySyncState
                    WHERE ShopId = @ShopId", con))
                {
                    cmd.Parameters.AddWithValue("@ShopId", shopId);
                    con.Open();

                    object o = cmd.ExecuteScalar();
                    if (o == null || o == DBNull.Value)
                        return null;

                    return Convert.ToDateTime(o);
                }
            }
            catch
            {
                return null;
            }
        }

        private void BindGrid()
        {
            if (!HasView("dbo.V_EtsyOrderInbox"))
            {
                DataTable empty = BuildEmptyGridSchema();
                gvInbox.DataSource = empty;
                gvInbox.DataBind();

                ShowMessage("Etsy database objects not found yet. Please run the Etsy SQL setup script first.", "alert alert-warning");
                return;
            }

            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM dbo.V_EtsyOrderInbox ORDER BY ImportedToLamax ASC, CreatedAtUtc DESC", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);
                gvInbox.DataSource = dt;
                gvInbox.DataBind();
            }
        }

        private void BindSyncInfo()
        {
            if (!HasTable("dbo.T_EtsySyncState"))
            {
                litLastSync.Text = "-";
                litLastSuccess.Text = "-";
                litLastError.Text = "T_EtsySyncState not found";
                return;
            }

            try
            {
                long shopId = new EtsyApiClient().ShopId;

                using (SqlConnection con = new SqlConnection(_cs))
                using (SqlCommand cmd = new SqlCommand(@"
                    SELECT TOP 1 LastSyncUtc, LastSuccessUtc, LastError
                    FROM dbo.T_EtsySyncState
                    WHERE ShopId = @ShopId", con))
                {
                    cmd.Parameters.AddWithValue("@ShopId", shopId);
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            litLastSync.Text = dr["LastSyncUtc"] == DBNull.Value ? "-" : Convert.ToDateTime(dr["LastSyncUtc"]).ToString("yyyy-MM-dd HH:mm");
                            litLastSuccess.Text = dr["LastSuccessUtc"] == DBNull.Value ? "-" : Convert.ToDateTime(dr["LastSuccessUtc"]).ToString("yyyy-MM-dd HH:mm");
                            litLastError.Text = dr["LastError"] == DBNull.Value ? "-" : Server.HtmlEncode(dr["LastError"].ToString());
                        }
                        else
                        {
                            litLastSync.Text = "-";
                            litLastSuccess.Text = "-";
                            litLastError.Text = "-";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                litLastSync.Text = "-";
                litLastSuccess.Text = "-";
                litLastError.Text = Server.HtmlEncode(ex.Message);
            }
        }

        protected void btnSyncNow_Click(object sender, EventArgs e)
        {
            try
            {
                var sync = new EtsyOrderSyncService();
                sync.Sync(true);
                ShowMessage("Etsy orders synced successfully.", "alert alert-success");
            }
            catch (Exception ex)
            {
                ShowMessage(Server.HtmlEncode(ex.Message), "alert alert-danger");
            }

            BindGrid();
            BindSyncInfo();
        }

        protected void btnImportSelected_Click(object sender, EventArgs e)
        {
            int imported = 0;

            try
            {
                var sync = new EtsyOrderSyncService();

                foreach (GridViewRow row in gvInbox.Rows)
                {
                    CheckBox chk = row.FindControl("chkRow") as CheckBox;
                    if (chk != null && chk.Checked)
                    {
                        long receiptId = Convert.ToInt64(gvInbox.DataKeys[row.RowIndex].Value);
                        sync.PromoteToLamax(receiptId);
                        imported++;
                    }
                }

                ShowMessage(imported + " orders imported into LamaX.", "alert alert-success");
            }
            catch (Exception ex)
            {
                ShowMessage(Server.HtmlEncode(ex.Message), "alert alert-danger");
            }

            BindGrid();
            BindSyncInfo();
        }

        protected void gvInbox_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "ImportRow")
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                long receiptId = Convert.ToInt64(gvInbox.DataKeys[rowIndex].Value);

                try
                {
                    var sync = new EtsyOrderSyncService();
                    sync.PromoteToLamax(receiptId);
                    ShowMessage("Order imported into LamaX.", "alert alert-success");
                }
                catch (Exception ex)
                {
                    ShowMessage(Server.HtmlEncode(ex.Message), "alert alert-danger");
                }

                BindGrid();
                BindSyncInfo();
            }
        }

        private bool HasTable(string fullName)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("SELECT CASE WHEN OBJECT_ID(@Name, 'U') IS NOT NULL THEN 1 ELSE 0 END", con))
            {
                cmd.Parameters.AddWithValue("@Name", fullName);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }

        private bool HasView(string fullName)
        {
            using (SqlConnection con = new SqlConnection(_cs))
            using (SqlCommand cmd = new SqlCommand("SELECT CASE WHEN OBJECT_ID(@Name, 'V') IS NOT NULL THEN 1 ELSE 0 END", con))
            {
                cmd.Parameters.AddWithValue("@Name", fullName);
                con.Open();
                return Convert.ToInt32(cmd.ExecuteScalar()) == 1;
            }
        }

        private DataTable BuildEmptyGridSchema()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ReceiptId", typeof(long));
            dt.Columns.Add("BuyerName", typeof(string));
            dt.Columns.Add("BuyerEmail", typeof(string));
            dt.Columns.Add("CountryIso", typeof(string));
            dt.Columns.Add("CurrencyCode", typeof(string));
            dt.Columns.Add("GrandTotal", typeof(decimal));
            dt.Columns.Add("ShippingCost", typeof(decimal));
            dt.Columns.Add("TaxCost", typeof(decimal));
            dt.Columns.Add("ItemCount", typeof(int));
            dt.Columns.Add("WasPaid", typeof(bool));
            dt.Columns.Add("WasShipped", typeof(bool));
            dt.Columns.Add("ImportedToLamax", typeof(bool));
            dt.Columns.Add("LamaxOrderNumber", typeof(string));
            dt.Columns.Add("CreatedAtUtc", typeof(DateTime));
            return dt;
        }

        private void ShowMessage(string text, string css)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = css;
            pnlMsg.Controls.Clear();
            pnlMsg.Controls.Add(new LiteralControl(text));
        }
    }
}