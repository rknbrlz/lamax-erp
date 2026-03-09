<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AmazonAccountingReport.aspx.cs" Inherits="Feniks.Administrator.AmazonAccountingReport" %>

<!DOCTYPE html>
<html>
<head runat="server">
    <title>Amazon Accounting Report | lamaX</title>
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/3.4.1/css/bootstrap.min.css" />
    <style>
        body{ background:#f5f6f8; font-family:Arial; padding:14px; }
        .card{
            background:#fff; border-radius:12px; padding:16px; margin-bottom:16px;
            box-shadow:0 1px 3px rgba(0,0,0,.08);
            border:1px solid rgba(0,0,0,.04);
        }
        .title{ font-weight:900; font-size:18px; margin:0 0 10px 0; }
        .muted{ color:#777; font-size:12px; }
        .help{ margin-top:8px; }
        .hr{ height:1px; background:#eee; margin:14px 0; }
        .grid{ margin-top:12px; }
        .grid th{ font-size:12px; text-transform:uppercase; color:#555; }
        .btn{ font-weight:800; }
        .msg-ok{ color:#1a7f37; font-weight:800; }
        .msg-err{ color:#b42318; font-weight:800; }
        code{ background:#f6f6f6; padding:2px 6px; border-radius:6px; }
    </style>
</head>
<body>
<form id="form1" runat="server">

    <div class="card">
        <div class="title">Amazon Accounting Reports</div>
        <div class="muted">
            Upload the Amazon TXT report. Output is an Excel-ready <b>TSV</b> with fixed columns:
            <code>order-id</code>, <code>Delivery Number</code>, <code>CC599C</code>, totals + PLN conversion.
        </div>

        <div class="hr"></div>

        <div class="row">
            <div class="col-md-8">
                <asp:FileUpload ID="fuTxt" runat="server" CssClass="form-control" />
                <div class="muted help">
                    Notes:
                    <ul style="margin:6px 0 0 18px;">
                        <li><b>Delivery Number</b> comes from <code>dbo.T_ShippingLeg.TrackingNumber</code> by <code>OrderNumber</code>.</li>
                        <li><b>FX</b> comes from <code>dbo.T_FxUsdPln</code> using <b>purchase-date - 1 day</b>.</li>
                        <li>TSV is best for Excel because it avoids comma/dot issues.</li>
                    </ul>
                </div>
            </div>
            <div class="col-md-4">
                <asp:Button ID="btnGenerate" runat="server" Text="Generate & Download (TSV)"
                    CssClass="btn btn-primary btn-block" OnClick="btnGenerate_Click" />
                <div style="margin-top:10px;">
                    <asp:Label ID="lblMsg" runat="server" />
                </div>
            </div>
        </div>

        <div class="hr"></div>

        <div class="muted">Preview (first 25 rows):</div>
        <div class="grid">
            <asp:GridView ID="gvPreview" runat="server" CssClass="table table-striped table-bordered"
                AutoGenerateColumns="true" />
        </div>
    </div>

</form>
</body>
</html>