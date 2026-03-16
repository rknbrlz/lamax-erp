<%@ Page Title="Etsy Connect" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EtsyOAuthCallback.aspx.cs" Inherits="Feniks.Administrator.EtsyOAuthCallback" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-width:700px;margin:30px auto;background:#fff;border:1px solid #e5e7eb;border-radius:16px;padding:24px;">
        <h2 style="margin-top:0;">Etsy Connection</h2>
        <asp:Literal ID="litResult" runat="server"></asp:Literal>
        <div style="margin-top:18px;">
            <a href="EtsyOrdersInbox.aspx" class="btn btn-primary">Go to Etsy Inbox</a>
        </div>
    </div>
</asp:Content>