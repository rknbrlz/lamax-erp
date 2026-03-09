using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;

namespace Feniks.Administrator
{
    public partial class Stock : System.Web.UI.Page
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindStockModes();
                BindProductTypes();
                BindLocations();
                BindTransferLocations();
                BindGrid();
            }
        }

        private void BindStockModes()
        {
            var dt = new DataTable();
            dt.Columns.Add("Val");
            dt.Columns.Add("Txt");

            dt.Rows.Add("0", "All");
            dt.Rows.Add("S", "S - Sized (Ring)");
            dt.Rows.Add("A", "A - Adjustable (Ring)");
            dt.Rows.Add("N", "N - Normal");

            ddlStockMode.DataSource = dt;
            ddlStockMode.DataTextField = "Txt";
            ddlStockMode.DataValueField = "Val";
            ddlStockMode.DataBind();
        }

        private void BindProductTypes()
        {
            var dt = ExecDT(@"
SELECT 0 AS ProductTypeID, N'All' AS ProductType
UNION ALL
SELECT ProductTypeID, ProductType FROM dbo.T_ProductType
ORDER BY ProductTypeID;");

            ddlProductType.DataSource = dt;
            ddlProductType.DataTextField = "ProductType";
            ddlProductType.DataValueField = "ProductTypeID";
            ddlProductType.DataBind();
        }

        private void BindLocations()
        {
            var dt = ExecDT(@"
SELECT 0 AS LocationID, N'All' AS LocationName
UNION ALL
SELECT LocationID, LocationName FROM dbo.T_StockLocation WHERE IsActive=1
ORDER BY LocationID;");

            ddlLocationFilter.DataSource = dt;
            ddlLocationFilter.DataTextField = "LocationName";
            ddlLocationFilter.DataValueField = "LocationID";
            ddlLocationFilter.DataBind();
        }

        private void BindTransferLocations()
        {
            var dt = ExecDT(@"SELECT LocationID, LocationName FROM dbo.T_StockLocation WHERE IsActive=1 ORDER BY LocationName;");

            ddlTransferToLocation.DataSource = dt;
            ddlTransferToLocation.DataTextField = "LocationName";
            ddlTransferToLocation.DataValueField = "LocationID";
            ddlTransferToLocation.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void BindGrid()
        {
            string sku = (txtSku.Text ?? "").Trim();
            int productTypeId = SafeInt(ddlProductType.SelectedValue);
            int locId = SafeInt(ddlLocationFilter.SelectedValue);
            string mode = (ddlStockMode.SelectedValue ?? "0").Trim();

            var dt = ExecDT(@"
SELECT
    ss.VariantID,
    ss.ProductID,
    ss.SKU,
    COALESCE(pt.ProductType, N'-') AS ProductType,
    COALESCE(NULLIF(LTRIM(RTRIM(p.StockMode)),''), 'N') AS StockMode,
    COALESCE(NULLIF(LTRIM(RTRIM(ss.VariantName)),''), N'-') AS VariantName,
    ss.LocationID,
    ss.LocationName,
    ss.OnHandQty,
    ss.ReservedQty,
    ss.AvailableQty
FROM dbo.V_StockSummary ss
JOIN dbo.T_Product p ON p.ProductID = ss.ProductID
LEFT JOIN dbo.T_ProductType pt ON pt.ProductTypeID = TRY_CONVERT(INT, p.ProductTypeID)
WHERE
    (@sku='' OR ss.SKU LIKE '%' + @sku + '%')
    AND (@pt=0 OR TRY_CONVERT(INT, p.ProductTypeID)=@pt)
    AND (@loc=0 OR ss.LocationID=@loc)
    AND (@mode='0' OR COALESCE(NULLIF(LTRIM(RTRIM(p.StockMode)),''),'N')=@mode)
ORDER BY ss.SKU, ss.RingSizeID, ss.VariantName, ss.LocationName;",
                new SqlParameter("@sku", sku),
                new SqlParameter("@pt", productTypeId),
                new SqlParameter("@loc", locId),
                new SqlParameter("@mode", mode)
            );

            gv.DataSource = dt;
            gv.DataBind();

            lblMsg.Text = "Rows: " + dt.Rows.Count;
        }

        public string ModeCss(object modeObj)
        {
            var m = (modeObj == null ? "" : modeObj.ToString()).Trim().ToUpperInvariant();
            if (m == "S") return "pill pill-s";
            if (m == "A") return "pill pill-a";
            return "pill pill-n";
        }

        protected void gv_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(e.CommandArgument?.ToString()))
                return;

            var parts = e.CommandArgument.ToString().Split('|');
            int variantId = SafeInt(parts[0]);
            int locationId = parts.Length > 1 ? SafeInt(parts[1]) : 1;

            if (e.CommandName == "RECEIVE")
            {
                hfReceiveVariantID.Value = variantId.ToString();
                hfReceiveLocationID.Value = locationId.ToString();

                txtReceiveQty.Text = "";
                txtReceiveUnitCost.Text = "";
                txtReceiveCurrency.Text = "USD";
                txtReceiveRefNo.Text = "";
                txtReceiveNote.Text = "";

                ClientScript.RegisterStartupScript(GetType(), "m1", "openModal('mdlReceive');", true);
            }
            else if (e.CommandName == "ADJUST")
            {
                hfAdjustVariantID.Value = variantId.ToString();
                hfAdjustLocationID.Value = locationId.ToString();

                txtAdjustDelta.Text = "";
                txtAdjustNote.Text = "";

                ClientScript.RegisterStartupScript(GetType(), "m2", "openModal('mdlAdjust');", true);
            }
            else if (e.CommandName == "TRANSFER")
            {
                hfTransferVariantID.Value = variantId.ToString();
                hfTransferFromLocationID.Value = locationId.ToString();

                txtTransferQty.Text = "";
                txtTransferNote.Text = "";

                ClientScript.RegisterStartupScript(GetType(), "m3", "openModal('mdlTransfer');", true);
            }
        }

        protected void btnReceiveSave_Click(object sender, EventArgs e)
        {
            try
            {
                int variantId = SafeInt(hfReceiveVariantID.Value);
                int locationId = SafeInt(hfReceiveLocationID.Value);

                decimal qty = SafeDec(txtReceiveQty.Text);
                decimal? unitCost = string.IsNullOrWhiteSpace(txtReceiveUnitCost.Text) ? (decimal?)null : SafeDec(txtReceiveUnitCost.Text);
                string currency = (txtReceiveCurrency.Text ?? "").Trim();
                string refNo = (txtReceiveRefNo.Text ?? "").Trim();
                string note = (txtReceiveNote.Text ?? "").Trim();

                ExecNonQuery("dbo.usp_StockReceive",
                    CommandType.StoredProcedure,
                    new SqlParameter("@VariantID", variantId),
                    new SqlParameter("@LocationID", locationId),
                    new SqlParameter("@Qty", qty),
                    new SqlParameter("@UnitCost", (object)unitCost ?? DBNull.Value),
                    new SqlParameter("@Currency", string.IsNullOrEmpty(currency) ? (object)DBNull.Value : currency),
                    new SqlParameter("@RefNo", string.IsNullOrEmpty(refNo) ? (object)DBNull.Value : refNo),
                    new SqlParameter("@Note", string.IsNullOrEmpty(note) ? (object)DBNull.Value : note),
                    new SqlParameter("@CreatedBy", (object)User?.Identity?.Name ?? DBNull.Value)
                );

                lblMsg.Text = "✅ Stock received.";
                BindGrid();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "❌ " + ex.Message;
            }
        }

        protected void btnAdjustSave_Click(object sender, EventArgs e)
        {
            try
            {
                int variantId = SafeInt(hfAdjustVariantID.Value);
                int locationId = SafeInt(hfAdjustLocationID.Value);

                decimal delta = SafeDec(txtAdjustDelta.Text);
                string note = (txtAdjustNote.Text ?? "").Trim();

                ExecNonQuery("dbo.usp_StockAdjust",
                    CommandType.StoredProcedure,
                    new SqlParameter("@VariantID", variantId),
                    new SqlParameter("@LocationID", locationId),
                    new SqlParameter("@QtyDelta", delta),
                    new SqlParameter("@Note", string.IsNullOrEmpty(note) ? (object)DBNull.Value : note),
                    new SqlParameter("@CreatedBy", (object)User?.Identity?.Name ?? DBNull.Value)
                );

                lblMsg.Text = "✅ Stock adjusted.";
                BindGrid();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "❌ " + ex.Message;
            }
        }

        protected void btnTransferSave_Click(object sender, EventArgs e)
        {
            try
            {
                int variantId = SafeInt(hfTransferVariantID.Value);
                int fromLoc = SafeInt(hfTransferFromLocationID.Value);
                int toLoc = SafeInt(ddlTransferToLocation.SelectedValue);

                decimal qty = SafeDec(txtTransferQty.Text);
                string note = (txtTransferNote.Text ?? "").Trim();

                ExecNonQuery("dbo.usp_StockTransfer",
                    CommandType.StoredProcedure,
                    new SqlParameter("@VariantID", variantId),
                    new SqlParameter("@FromLocationID", fromLoc),
                    new SqlParameter("@ToLocationID", toLoc),
                    new SqlParameter("@Qty", qty),
                    new SqlParameter("@Note", string.IsNullOrEmpty(note) ? (object)DBNull.Value : note),
                    new SqlParameter("@CreatedBy", (object)User?.Identity?.Name ?? DBNull.Value)
                );

                lblMsg.Text = "✅ Stock transferred.";
                BindGrid();
            }
            catch (Exception ex)
            {
                lblMsg.Text = "❌ " + ex.Message;
            }
        }

        /* ================== DB Helpers ================== */

        private DataTable ExecDT(string sql, params SqlParameter[] prms)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var da = new SqlDataAdapter(sql, con))
            {
                if (prms != null && prms.Length > 0)
                    da.SelectCommand.Parameters.AddRange(prms);

                var dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        private int ExecNonQuery(string cmdText, CommandType type, params SqlParameter[] prms)
        {
            using (var con = new SqlConnection(ConnStr))
            using (var cmd = new SqlCommand(cmdText, con))
            {
                cmd.CommandType = type;
                if (prms != null && prms.Length > 0)
                    cmd.Parameters.AddRange(prms);

                con.Open();
                return cmd.ExecuteNonQuery();
            }
        }

        private int SafeInt(string s)
        {
            int.TryParse((s ?? "").Trim(), out int x);
            return x;
        }

        private decimal SafeDec(string s)
        {
            s = (s ?? "").Trim();
            if (string.IsNullOrWhiteSpace(s)) return 0m;

            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var a))
                return a;

            if (decimal.TryParse(s, NumberStyles.Any, new CultureInfo("tr-TR"), out var b))
                return b;

            if (decimal.TryParse(s, NumberStyles.Any, new CultureInfo("pl-PL"), out var c))
                return c;

            throw new Exception("Invalid number: " + s);
        }
    }
}