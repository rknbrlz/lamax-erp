<%@ Page Title="Profile" Language="C#" MasterPageFile="~/Site.master"
    AutoEventWireup="true" CodeBehind="Profile.aspx.cs"
    Inherits="Feniks.Administrator.Profile" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="container" style="max-width:720px;margin-top:22px;">
        <div class="panel panel-default" style="border-radius:14px; box-shadow:0 10px 26px rgba(0,0,0,.06);">
            <div class="panel-heading" style="border-top-left-radius:14px;border-top-right-radius:14px;">
                <h3 class="panel-title" style="font-weight:800;">Profile</h3>
            </div>

            <div class="panel-body">
                <div class="row" style="display:flex; align-items:center; gap:18px; flex-wrap:wrap;">

                    <div style="width:130px; height:130px;">
                        <asp:Image ID="imgPreview" runat="server"
                            Style="width:130px;height:130px;border-radius:50%;object-fit:cover;border:1px solid #ddd;background:#f7f7f7;"
                            ImageUrl="~/Images/avatar-default.png" />
                    </div>

                    <div style="flex:1; min-width:260px;">
                        <div style="font-size:13px;color:#666;margin-bottom:6px;">
                            Signed in as:
                            <strong><asp:Literal ID="litUser" runat="server" /></strong>
                        </div>

                        <label style="font-weight:700;">Profile photo (JPG/PNG, max 2MB)</label>
                        <asp:FileUpload ID="fuAvatar" runat="server" CssClass="form-control" />

                        <div style="margin-top:12px; display:flex; gap:10px; flex-wrap:wrap;">
                            <asp:Button ID="btnUpload" runat="server"
                                Text="Upload"
                                CssClass="btn btn-primary"
                                OnClick="btnUpload_Click" />

                            <asp:Button ID="btnRemove" runat="server"
                                Text="Remove photo"
                                CssClass="btn btn-default"
                                OnClick="btnRemove_Click" />
                        </div>

                        <div style="margin-top:10px;">
                            <asp:Label ID="lblMsg" runat="server"></asp:Label>
                        </div>
                    </div>
                </div>

                <hr />

                <div style="font-size:12px;color:#666;">
                    Tip: Photo is saved under <code>/Uploads/avatars/</code> and shown on the topbar automatically.
                </div>
            </div>
        </div>
    </div>

</asp:Content>