using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class StockHistory : Page
    {
        private string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindLocations();
                SetDefaultDates();
                LoadFromQueryString();
                BindGrid();
            }
        }

        private void LoadFromQueryString()
        {
            string sku = Request.QueryString["sku"];
            if (!string.IsNullOrWhiteSpace(sku))
            {
                txtSKU.Text = sku.Trim();
            }

            string txnType = Request.QueryString["txntype"];
            if (!string.IsNullOrWhiteSpace(txnType))
            {
                ListItem item = ddlTxnType.Items.FindByValue(txnType.Trim().ToUpperInvariant());
                if (item != null)
                    ddlTxnType.SelectedValue = item.Value;
            }
        }

        private void SetDefaultDates()
        {
            txtDateFrom.Text = DateTime.Today.AddDays(-30).ToString("yyyy-MM-dd");
            txtDateTo.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }

        private void BindLocations()
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand(
                "SELECT LocationID, LocationName FROM dbo.T_StockLocation WHERE IsActive = 1 ORDER BY LocationName", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                DataTable dt = new DataTable();
                da.Fill(dt);

                ddlLocation.DataSource = dt;
                ddlLocation.DataTextField = "LocationName";
                ddlLocation.DataValueField = "LocationID";
                ddlLocation.DataBind();
                ddlLocation.Items.Insert(0, new ListItem("All", ""));
            }
        }

        private void BindGrid()
        {
            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_StockHistory_Search", con))
            {
                cmd.CommandType = CommandType.StoredProcedure;

                cmd.Parameters.AddWithValue("@SKUContains",
                    string.IsNullOrWhiteSpace(txtSKU.Text)
                        ? (object)DBNull.Value
                        : txtSKU.Text.Trim());

                cmd.Parameters.AddWithValue("@VariantID", DBNull.Value);

                cmd.Parameters.AddWithValue("@LocationID",
                    string.IsNullOrWhiteSpace(ddlLocation.SelectedValue)
                        ? (object)DBNull.Value
                        : Convert.ToInt32(ddlLocation.SelectedValue));

                cmd.Parameters.AddWithValue("@TxnType",
                    string.IsNullOrWhiteSpace(ddlTxnType.SelectedValue)
                        ? (object)DBNull.Value
                        : ddlTxnType.SelectedValue.Trim().ToUpperInvariant());

                cmd.Parameters.AddWithValue("@DateFrom",
                    TryParseDate(txtDateFrom.Text) ?? (object)DBNull.Value);

                cmd.Parameters.AddWithValue("@DateTo",
                    TryParseDate(txtDateTo.Text) ?? (object)DBNull.Value);

                using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    gvHistory.DataSource = dt;
                    gvHistory.DataBind();

                    litCount.Text = "<div class='muted-count'>Rows: " + dt.Rows.Count + "</div>";
                }
            }
        }

        private object TryParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            DateTime parsed;
            if (DateTime.TryParseExact(value.Trim(), "yyyy-MM-dd", CultureInfo.InvariantCulture,
                DateTimeStyles.None, out parsed))
            {
                return parsed;
            }

            return null;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        public string GetTxnTypeBadge(object txnTypeObj)
        {
            string txnType = Convert.ToString(txnTypeObj);
            if (string.IsNullOrWhiteSpace(txnType))
                txnType = "";

            switch (txnType.Trim().ToUpperInvariant())
            {
                case "RECEIVE":
                case "RECEIPT":
                    return "<span class='tag tag-receipt'>RECEIVE</span>";

                case "ADJUST":
                    return "<span class='tag tag-adjust'>ADJUST</span>";

                case "TRANSFER":
                    return "<span class='tag tag-transfer'>TRANSFER</span>";

                case "SALE":
                case "SHIP":
                    return "<span class='tag tag-sale'>SALE</span>";

                case "RETURN":
                    return "<span class='tag tag-return'>RETURN</span>";

                default:
                    return "<span class='tag tag-other'>" + Server.HtmlEncode(txnType) + "</span>";
            }
        }

        public string GetQtyHtml(object qtyObj)
        {
            decimal qty = 0;

            decimal.TryParse(
                Convert.ToString(qtyObj),
                NumberStyles.Any,
                CultureInfo.InvariantCulture,
                out qty);

            if (qty > 0)
                return "<span class='qty-in'>+" + qty.ToString("0.####", CultureInfo.InvariantCulture) + "</span>";

            if (qty < 0)
                return "<span class='qty-out'>" + qty.ToString("0.####", CultureInfo.InvariantCulture) + "</span>";

            return "<span class='qty-zero'>0</span>";
        }
    }
}