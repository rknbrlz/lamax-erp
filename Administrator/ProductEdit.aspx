<%@ Page Title="Product Edit" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ProductEdit.aspx.cs" Inherits="Feniks.Administrator.ProductEdit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body { background:#f5f6f8; }

    .page-wrap { padding:14px; }

    .card-box{
        background:#fff;
        border:1px solid #e8e8e8;
        border-radius:18px;
        padding:22px;
        margin-bottom:18px;
        box-shadow:0 4px 12px rgba(0,0,0,.03);
    }

    .page-title{
        font-size:24px;
        font-weight:800;
        margin:0 0 6px 0;
        color:#1f2937;
    }

    .page-subtitle{
        color:#6b7280;
        margin-bottom:18px;
    }

    .top-row{
        display:flex;
        justify-content:space-between;
        align-items:center;
        gap:12px;
        flex-wrap:wrap;
    }

    .btn-pill{
        border-radius:999px;
        padding:10px 18px;
        font-weight:600;
    }

    .form-control{
        border-radius:12px;
        min-height:42px;
        box-shadow:none;
    }

    .section-title{
        font-size:18px;
        font-weight:700;
        margin-bottom:16px;
        color:#1f2937;
    }

    .help-box{
        margin-top:8px;
        padding:10px 12px;
        border-radius:12px;
        background:#f8fafc;
        border:1px solid #e5e7eb;
        color:#475569;
        font-size:12px;
    }
</style>

<div class="page-wrap">

    <div class="top-row">
        <div>
            <h2 class="page-title">
                <asp:Literal ID="litPageTitle" runat="server" Text="New Product"></asp:Literal>
            </h2>
            <div class="page-subtitle">Create or edit product master card.</div>
        </div>
        <div>
            <a href="ProductList.aspx" class="btn btn-default btn-pill">Back to List</a>
        </div>
    </div>

    <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="alert alert-info" Style="display:block;"></asp:Label>
    <asp:HiddenField ID="hfProductID" runat="server" />

    <div class="card-box">
        <div class="section-title">Basic Info</div>

        <div class="row">
            <div class="col-md-4">
                <label>SKU</label>
                <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" MaxLength="14" />
            </div>

            <div class="col-md-4">
                <label>Product Type</label>
                <asp:DropDownList ID="ddlProductType" runat="server"
                    CssClass="form-control"
                    AutoPostBack="true"
                    OnSelectedIndexChanged="ddlProductType_SelectedIndexChanged" />
            </div>

            <div class="col-md-4">
                <label>Stock Mode</label>
                <asp:DropDownList ID="ddlStockMode" runat="server" CssClass="form-control" />
                <div class="help-box">
                    Ring için sadece <strong>S</strong> veya <strong>A</strong><br />
                    Ring dışı ürünlerde otomatik <strong>N</strong>
                </div>
            </div>
        </div>

        <br />

        <div class="row">
            <div class="col-md-6">
                <label>Stock Address</label>
                <asp:TextBox ID="txtStockAddress" runat="server" CssClass="form-control" />
            </div>

            <div class="col-md-6">
                <label>Supplier</label>
                <asp:TextBox ID="txtSupplier" runat="server" CssClass="form-control" MaxLength="2" />
            </div>
        </div>
    </div>

    <div class="card-box">
        <asp:Button ID="btnSave" runat="server" Text="Save Product" CssClass="btn btn-primary btn-pill" OnClick="btnSave_Click" />
        <a href="ProductEdit.aspx" class="btn btn-success btn-pill">New Blank Form</a>
        <a href="ProductList.aspx" class="btn btn-default btn-pill">Cancel</a>
    </div>

</div>

</asp:Content>