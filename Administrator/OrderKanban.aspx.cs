using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class OrderKanban : Page
    {
        private readonly string _connStr =
            ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBoard();
            }
        }

        private void LoadBoard()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(_connStr))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_OrderKanban_GetBoard", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                da.Fill(dt);
            }

            if (!dt.Columns.Contains("OrderDateText"))
                dt.Columns.Add("OrderDateText", typeof(string));

            if (!dt.Columns.Contains("OrderTotalText"))
                dt.Columns.Add("OrderTotalText", typeof(string));

            if (!dt.Columns.Contains("BreakdownText"))
                dt.Columns.Add("BreakdownText", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                row["OrderDateText"] = BuildOrderDateText(row);
                row["OrderTotalText"] = BuildMoneyText(row["OrderTotalCalc"], row["Currency"]);
                row["BreakdownText"] = BuildBreakdownText(row);
            }

            DataView dvOpen = new DataView(dt);
            dvOpen.RowFilter = "KanbanStatus = 'OPEN'";
            rptOpen.DataSource = dvOpen;
            rptOpen.DataBind();
            litBadgeOpen.Text = dvOpen.Count.ToString();
            phOpenEmpty.Visible = dvOpen.Count == 0;

            DataView dvProgress = new DataView(dt);
            dvProgress.RowFilter = "KanbanStatus = 'INPROGRESS'";
            rptProgress.DataSource = dvProgress;
            rptProgress.DataBind();
            litBadgeProgress.Text = dvProgress.Count.ToString();
            phProgressEmpty.Visible = dvProgress.Count == 0;

            DataView dvClosed = new DataView(dt);
            dvClosed.RowFilter = "KanbanStatus = 'CLOSED'";
            rptClosed.DataSource = dvClosed;
            rptClosed.DataBind();
            litBadgeClosed.Text = dvClosed.Count.ToString();
            phClosedEmpty.Visible = dvClosed.Count == 0;
        }

        protected void rptOpen_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            SetWaitingBadge(e);
        }

        protected void rptProgress_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            SetWaitingBadge(e);
        }

        protected void rptClosed_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            SetWaitingBadge(e);
        }

        private void SetWaitingBadge(RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item &&
                e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            object waitingObj = DataBinder.Eval(e.Item.DataItem, "WaitingDays");
            int waitingDays = 0;

            if (waitingObj != null && waitingObj != DBNull.Value)
                int.TryParse(waitingObj.ToString(), out waitingDays);

            Literal litWaiting = e.Item.FindControl("litWaiting") as Literal;
            if (litWaiting == null) return;

            string css = "waiting-ok";

            if (waitingDays >= 5)
                css = "waiting-hot";
            else if (waitingDays >= 3)
                css = "waiting-mid";

            litWaiting.Text = string.Format(
                "<span class='waiting-tag {0}'>{1} day{2}</span>",
                css,
                waitingDays,
                waitingDays == 1 ? "" : "s"
            );
        }

        private string BuildOrderDateText(DataRow row)
        {
            DateTime dt;

            if (row.Table.Columns.Contains("OrderDateParsed") &&
                row["OrderDateParsed"] != DBNull.Value &&
                DateTime.TryParse(row["OrderDateParsed"].ToString(), out dt))
            {
                return dt.ToString("dd.MM.yyyy");
            }

            if (row.Table.Columns.Contains("RecordDate") &&
                row["RecordDate"] != DBNull.Value &&
                DateTime.TryParse(row["RecordDate"].ToString(), out dt))
            {
                return dt.ToString("dd.MM.yyyy");
            }

            return "-";
        }

        private string BuildMoneyText(object amountObj, object currencyObj)
        {
            decimal amount = 0m;

            if (amountObj != null && amountObj != DBNull.Value)
            {
                if (amountObj is decimal)
                {
                    amount = (decimal)amountObj;
                }
                else if (amountObj is double)
                {
                    amount = Convert.ToDecimal((double)amountObj);
                }
                else if (amountObj is float)
                {
                    amount = Convert.ToDecimal((float)amountObj);
                }
                else
                {
                    string s = amountObj.ToString().Trim();

                    if (!decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                    {
                        decimal.TryParse(s, NumberStyles.Any, CultureInfo.GetCultureInfo("tr-TR"), out amount);
                    }
                }
            }

            string cur = "";
            if (currencyObj != null && currencyObj != DBNull.Value)
                cur = currencyObj.ToString().Trim();

            switch (cur)
            {
                case "1":
                case "USD":
                    return "$" + amount.ToString("N2", CultureInfo.InvariantCulture);

                case "2":
                case "EUR":
                    return "€" + amount.ToString("N2", CultureInfo.InvariantCulture);

                case "3":
                case "PLN":
                    return amount.ToString("N2", CultureInfo.InvariantCulture) + " PLN";

                case "4":
                case "TRY":
                    return "₺" + amount.ToString("N2", CultureInfo.InvariantCulture);

                default:
                    return amount.ToString("N2", CultureInfo.InvariantCulture);
            }
        }

        private string BuildBreakdownText(DataRow row)
        {
            string items = BuildMoneyText(row["ItemsTotalCalc"], row["Currency"]);
            string shipping = BuildMoneyText(row["ShippingCalc"], row["Currency"]);
            string tax = BuildMoneyText(row["TaxCalc"], row["Currency"]);

            decimal couponVal = 0m;
            if (row.Table.Columns.Contains("CouponCalc") && row["CouponCalc"] != DBNull.Value)
            {
                if (row["CouponCalc"] is decimal)
                {
                    couponVal = (decimal)row["CouponCalc"];
                }
                else
                {
                    decimal.TryParse(
                        row["CouponCalc"].ToString(),
                        NumberStyles.Any,
                        CultureInfo.InvariantCulture,
                        out couponVal
                    );
                }
            }

            if (couponVal > 0)
            {
                string coupon = BuildMoneyText(row["CouponCalc"], row["Currency"]);
                return string.Format(
                    "Items: {0} | Shipping: {1} | Tax: {2} | Coupon: -{3}",
                    items, shipping, tax, coupon
                );
            }

            return string.Format(
                "Items: {0} | Shipping: {1} | Tax: {2}",
                items, shipping, tax
            );
        }
    }
}