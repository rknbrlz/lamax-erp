using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Feniks.Administrator
{
    public partial class OrderKanban : Page
    {
        private string ConnStr
        {
            get { return ConfigurationManager.ConnectionStrings["constr"].ConnectionString; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadBoard();
            }
        }

        private void LoadBoard()
        {
            DataTable dt = GetBoardData();

            EnsureComputedColumns(dt);
            FillComputedColumns(dt);

            BindColumn(dt, "OPEN", rptOpen, litBadgeOpen, litSummaryOpen, phOpenEmpty);
            BindColumn(dt, "INPROGRESS", rptProgress, litBadgeProgress, litSummaryProgress, phProgressEmpty);
            BindColumn(dt, "CLOSED", rptClosed, litBadgeClosed, litSummaryClosed, phClosedEmpty);
        }

        private DataTable GetBoardData()
        {
            DataTable dt = new DataTable();

            using (SqlConnection con = new SqlConnection(ConnStr))
            using (SqlCommand cmd = new SqlCommand("dbo.usp_OrderKanban_GetBoard", con))
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                da.Fill(dt);
            }

            return dt;
        }

        private void BindColumn(
            DataTable dt,
            string status,
            Repeater repeater,
            Literal litBadge,
            Literal litSummary,
            PlaceHolder phEmpty)
        {
            DataView dv = new DataView(dt);
            dv.RowFilter = "BoardStatus = '" + status.Replace("'", "''") + "'";
            dv.Sort = "SortDate DESC";

            repeater.DataSource = dv;
            repeater.DataBind();

            string count = dv.Count.ToString();
            litBadge.Text = count;
            litSummary.Text = count;
            phEmpty.Visible = dv.Count == 0;
        }

        private void EnsureComputedColumns(DataTable dt)
        {
            if (!dt.Columns.Contains("OrderDateText"))
                dt.Columns.Add("OrderDateText", typeof(string));

            if (!dt.Columns.Contains("OrderTotalText"))
                dt.Columns.Add("OrderTotalText", typeof(string));

            if (!dt.Columns.Contains("BreakdownText"))
                dt.Columns.Add("BreakdownText", typeof(string));
        }

        private void FillComputedColumns(DataTable dt)
        {
            foreach (DataRow row in dt.Rows)
            {
                DateTime parsedDate = GetOrderDate(row);

                row["OrderDateText"] = parsedDate == DateTime.MinValue
                    ? "-"
                    : parsedDate.ToString("dd.MM.yyyy");

                row["OrderTotalText"] = BuildMoneyText(GetValue(row, "OrderTotalCalc"), GetValue(row, "Currency"));
                row["BreakdownText"] = BuildBreakdownText(row);
            }
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

            int waitingDays = 0;
            object waitingObj = DataBinder.Eval(e.Item.DataItem, "WaitingDays");

            if (waitingObj != null && waitingObj != DBNull.Value)
                int.TryParse(waitingObj.ToString(), out waitingDays);

            string boardStatus = Convert.ToString(DataBinder.Eval(e.Item.DataItem, "BoardStatus"))
                .Trim()
                .ToUpperInvariant();

            Literal litWaiting = e.Item.FindControl("litWaiting") as Literal;
            if (litWaiting == null) return;

            string css;
            string text;

            if (boardStatus == "CLOSED")
            {
                text = "Completed in " + waitingDays + " day" + (waitingDays == 1 ? "" : "s");

                if (waitingDays <= 3)
                    css = "waiting-closed-fast";
                else if (waitingDays <= 7)
                    css = "waiting-closed-mid";
                else
                    css = "waiting-closed-slow";
            }
            else
            {
                text = waitingDays + " day" + (waitingDays == 1 ? "" : "s");

                if (waitingDays <= 2)
                    css = "waiting-open-good";
                else if (waitingDays <= 5)
                    css = "waiting-open-warn";
                else
                    css = "waiting-open-hot";
            }

            litWaiting.Text = string.Format(
                "<span class=\"waiting-pill {0}\">{1}</span>",
                css,
                text
            );
        }

        private DateTime GetOrderDate(DataRow row)
        {
            DateTime dt;

            if (row.Table.Columns.Contains("OrderDateParsed") &&
                row["OrderDateParsed"] != DBNull.Value &&
                DateTime.TryParse(row["OrderDateParsed"].ToString(), out dt))
            {
                return dt;
            }

            if (row.Table.Columns.Contains("OrderDate") &&
                row["OrderDate"] != DBNull.Value &&
                DateTime.TryParse(row["OrderDate"].ToString(), out dt))
            {
                return dt;
            }

            return DateTime.MinValue;
        }

        private object GetValue(DataRow row, string columnName)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(columnName))
                return DBNull.Value;

            return row[columnName];
        }

        private string BuildMoneyText(object amountObj, object currencyObj)
        {
            decimal amount = 0m;

            if (amountObj != null && amountObj != DBNull.Value)
            {
                if (amountObj is decimal)
                    amount = (decimal)amountObj;
                else if (amountObj is double)
                    amount = Convert.ToDecimal((double)amountObj);
                else if (amountObj is float)
                    amount = Convert.ToDecimal((float)amountObj);
                else
                {
                    string s = amountObj.ToString().Trim();

                    if (!decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out amount))
                        decimal.TryParse(s, NumberStyles.Any, CultureInfo.GetCultureInfo("tr-TR"), out amount);
                }
            }

            string cur = "";
            if (currencyObj != null && currencyObj != DBNull.Value)
                cur = currencyObj.ToString().Trim().ToUpperInvariant();

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
            string items = BuildMoneyText(GetValue(row, "ItemsTotalCalc"), GetValue(row, "Currency"));
            string shipping = BuildMoneyText(GetValue(row, "ShippingCalc"), GetValue(row, "Currency"));
            string tax = BuildMoneyText(GetValue(row, "TaxCalc"), GetValue(row, "Currency"));

            decimal couponVal = 0m;
            object couponObj = GetValue(row, "CouponCalc");

            if (couponObj != null && couponObj != DBNull.Value)
            {
                if (couponObj is decimal)
                    couponVal = (decimal)couponObj;
                else
                    decimal.TryParse(couponObj.ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out couponVal);
            }

            if (couponVal > 0)
            {
                string coupon = BuildMoneyText(couponObj, GetValue(row, "Currency"));
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

        private static int GetShippingStatusIdFromBoardStatus(string boardStatus)
        {
            switch ((boardStatus ?? "").Trim().ToUpperInvariant())
            {
                case "OPEN":
                    return 1;   // Preparing
                case "INPROGRESS":
                    return 2;   // Waiting for Decision
                case "CLOSED":
                    return 8;   // Final Shipping
                default:
                    return 1;
            }
        }

        private static int GetOrderStatusFromShippingStatusId(int shippingStatusId)
        {
            if (shippingStatusId == 1)
                return 1;

            if (shippingStatusId >= 2 && shippingStatusId <= 7)
                return 2;

            if (shippingStatusId == 8)
                return 3;

            if (shippingStatusId == 9)
                return 4;

            return 1;
        }

        private static string GetKanbanStatusTextFromShippingStatusId(int shippingStatusId)
        {
            if (shippingStatusId == 1)
                return "OPEN";

            if (shippingStatusId >= 2 && shippingStatusId <= 7)
                return "INPROGRESS";

            if (shippingStatusId == 8 || shippingStatusId == 9)
                return "CLOSED";

            return "OPEN";
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static object UpdateKanbanStatus(string orderNumber, string newStatus)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(orderNumber))
                {
                    return new
                    {
                        success = false,
                        message = "Order number cannot be empty."
                    };
                }

                string boardStatus = (newStatus ?? "").Trim().ToUpperInvariant();

                if (boardStatus != "OPEN" && boardStatus != "INPROGRESS" && boardStatus != "CLOSED")
                {
                    return new
                    {
                        success = false,
                        message = "Invalid status."
                    };
                }

                int shippingStatusId = GetShippingStatusIdFromBoardStatus(boardStatus);
                int orderStatus = GetOrderStatusFromShippingStatusId(shippingStatusId);
                string kanbanStatus = GetKanbanStatusTextFromShippingStatusId(shippingStatusId);

                string connStr = ConfigurationManager.ConnectionStrings["constr"].ConnectionString;

                using (SqlConnection con = new SqlConnection(connStr))
                {
                    con.Open();

                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            using (SqlCommand cmdOrder = new SqlCommand(@"
UPDATE O
SET
    O.OrderStatus = @OrderStatus,
    O.KanbanStatus = @KanbanStatus,
    O.KanbanUpdatedDate = GETDATE()
FROM dbo.T_Order O
INNER JOIN dbo.T_OrdersPage OP ON OP.OrderID = O.OrderID
WHERE LTRIM(RTRIM(CAST(OP.OrderNumber AS NVARCHAR(100)))) = LTRIM(RTRIM(@OrderNumber))
", con, tran))
                            {
                                cmdOrder.Parameters.AddWithValue("@OrderStatus", orderStatus);
                                cmdOrder.Parameters.AddWithValue("@KanbanStatus", kanbanStatus);
                                cmdOrder.Parameters.AddWithValue("@OrderNumber", orderNumber.Trim());

                                int affectedOrder = cmdOrder.ExecuteNonQuery();

                                if (affectedOrder <= 0)
                                {
                                    tran.Rollback();
                                    return new
                                    {
                                        success = false,
                                        message = "Order not found in T_Order."
                                    };
                                }
                            }

                            using (SqlCommand cmdShipping = new SqlCommand(@"
;WITH X AS
(
    SELECT TOP 1 S.ShippingID
    FROM dbo.T_Shipping S
    WHERE LTRIM(RTRIM(CAST(S.OrderNumber AS NVARCHAR(100)))) = LTRIM(RTRIM(@OrderNumber))
    ORDER BY 
        ISNULL(S.ShipDate, '19000101') DESC,
        ISNULL(S.RecordDate, '19000101') DESC,
        S.ShippingID DESC
)
UPDATE S
SET S.ShippingStatusID = @ShippingStatusID
FROM dbo.T_Shipping S
INNER JOIN X ON X.ShippingID = S.ShippingID
", con, tran))
                            {
                                cmdShipping.Parameters.AddWithValue("@ShippingStatusID", shippingStatusId);
                                cmdShipping.Parameters.AddWithValue("@OrderNumber", orderNumber.Trim());

                                cmdShipping.ExecuteNonQuery();
                            }

                            tran.Commit();
                        }
                        catch (Exception exTran)
                        {
                            tran.Rollback();
                            return new
                            {
                                success = false,
                                message = exTran.Message
                            };
                        }
                    }
                }

                return new
                {
                    success = true,
                    message = "Kanban updated."
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    success = false,
                    message = ex.Message
                };
            }
        }
    }
}