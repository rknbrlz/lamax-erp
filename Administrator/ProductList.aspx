<%@ Page Title="Product Management" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="ProductList.aspx.cs" Inherits="Feniks.Administrator.ProductList" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

<style>
    body { background:#f5f6f8; }

    .page-wrap { padding:14px; }

    .card-box{
        background:#fff;
        border:1px solid #e8e8e8;
        border-radius:18px;
        padding:20px;
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
        margin-bottom:0;
    }

    .top-row{
        display:flex;
        align-items:center;
        justify-content:space-between;
        gap:12px;
        flex-wrap:wrap;
        margin-bottom:18px;
    }

    .btn-pill{
        border-radius:999px;
        padding:10px 18px;
        font-weight:600;
    }

    .mode-badge{
        display:inline-block;
        min-width:28px;
        text-align:center;
        padding:5px 10px;
        border-radius:999px;
        font-weight:700;
        font-size:12px;
    }

    .mode-s{ background:#dff3e2; color:#1b5e20; }
    .mode-a{ background:#dbeafe; color:#1e40af; }
    .mode-n{ background:#fef3c7; color:#92400e; }

    .table > thead > tr > th { vertical-align:middle; font-weight:700; }
    .table > tbody > tr > td { vertical-align:middle; }

    .search-row .form-control{
        border-radius:12px;
        min-height:42px;
    }
</style>

<div class="page-wrap">

    <div class="top-row">
        <div>
            <h2 class="page-title">Product Management</h2>
            <p class="page-subtitle">Create, search and manage your product master data.</p>
        </div>
        <div>
            <a href="ProductEdit.aspx" class="btn btn-primary btn-pill">+ New Product</a>
        </div>
    </div>

    <asp:Label ID="lblMessage" runat="server" Visible="false" CssClass="alert alert-info" Style="display:block;"></asp:Label>

    <div class="card-box">
        <div class="row search-row">
            <div class="col-md-4">
                <label>SKU contains</label>
                <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" />
            </div>

            <div class="col-md-3">
                <label>Product Type</label>
                <asp:DropDownList ID="ddlProductType" runat="server" CssClass="form-control" />
            </div>

            <div class="col-md-3">
                <label>Stock Mode</label>
                <asp:DropDownList ID="ddlStockMode" runat="server" CssClass="form-control">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="Sized Ring (S)" Value="S" />
                    <asp:ListItem Text="Adjustable Ring (A)" Value="A" />
                    <asp:ListItem Text="Normal (N)" Value="N" />
                </asp:DropDownList>
            </div>

            <div class="col-md-2">
                <label>&nbsp;</label>
                <div>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-primary btn-pill" OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>
    </div>

    <div class="card-box">
        <asp:GridView ID="gvProducts" runat="server"
            AutoGenerateColumns="false"
            CssClass="table table-bordered table-hover"
            GridLines="None"
            EmptyDataText="No product found."
            OnRowCommand="gvProducts_RowCommand"
            OnRowDataBound="gvProducts_RowDataBound">
            <Columns>
                <asp:BoundField DataField="ProductID" HeaderText="ID" />
                <asp:BoundField DataField="SKU" HeaderText="SKU" />
                <asp:BoundField DataField="ProductType" HeaderText="Product Type" />

                <asp:TemplateField HeaderText="Mode">
                    <ItemTemplate>
                        <asp:Label ID="lblMode" runat="server"
                            Text='<%# Eval("StockMode") %>'
                            CssClass="mode-badge">
                        </asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:BoundField DataField="SupplierID" HeaderText="Supplier" />
                <asp:BoundField DataField="StockAddress" HeaderText="Stock Address" />
                <asp:BoundField DataField="RecordBy" HeaderText="Record By" />
                <asp:BoundField DataField="RecordDate" HeaderText="Record Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />

                <asp:TemplateField HeaderText="Actions">
                    <ItemTemplate>
                        <asp:LinkButton ID="btnEdit" runat="server"
                            CommandName="EditProduct"
                            CommandArgument='<%# Eval("ProductID") %>'
                            CssClass="btn btn-xs btn-primary">Edit</asp:LinkButton>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>
        </asp:GridView>
    </div>

</div>

</asp:Content>