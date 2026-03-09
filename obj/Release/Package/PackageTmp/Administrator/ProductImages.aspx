<%@ Page Title="Product Images" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProductImages.aspx.cs" Inherits="Feniks.Administrator.ProductImages" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .img-page-wrap { padding:20px; }
        .img-card {
            background:#fff;
            border:1px solid #e9e9e9;
            border-radius:14px;
            padding:18px;
            box-shadow:0 2px 10px rgba(0,0,0,0.04);
            margin-bottom:18px;
        }
        .img-title {
            font-size:24px;
            font-weight:700;
            margin-bottom:6px;
        }
        .img-subtitle {
            color:#777;
            margin-bottom:20px;
        }
        .img-grid {
            display:flex;
            flex-wrap:wrap;
            gap:16px;
        }
        .img-item {
            width:220px;
            background:#fff;
            border:1px solid #ececec;
            border-radius:14px;
            overflow:hidden;
            box-shadow:0 2px 8px rgba(0,0,0,0.04);
        }
        .img-thumb-wrap {
            width:100%;
            height:220px;
            background:#fafafa;
            display:flex;
            align-items:center;
            justify-content:center;
            border-bottom:1px solid #eee;
        }
        .img-thumb {
            max-width:100%;
            max-height:100%;
        }
        .img-body {
            padding:12px;
        }
        .img-file {
            font-size:12px;
            color:#555;
            word-break:break-word;
            min-height:34px;
        }
        .img-badges {
            margin:8px 0;
        }
        .img-badge {
            display:inline-block;
            padding:3px 8px;
            border-radius:999px;
            font-size:11px;
            margin-right:6px;
            background:#f1f1f1;
        }
        .img-badge-primary {
            background:#dff4e8;
            color:#1f7a45;
            font-weight:700;
        }
        .toolbar-row {
            display:flex;
            flex-wrap:wrap;
            gap:12px;
            align-items:end;
        }
        .toolbar-col {
            min-width:220px;
            flex:1 1 220px;
        }
        .btn-space { margin-right:6px; margin-bottom:6px; }
        .small-muted { font-size:12px; color:#777; }

        .drop-zone {
            margin-top:10px;
            border:2px dashed #cfd6df;
            border-radius:14px;
            background:#fafcff;
            padding:20px;
            text-align:center;
            transition:.2s ease;
            cursor:pointer;
        }
        .drop-zone.dragover {
            border-color:#4a90e2;
            background:#eef6ff;
        }
        .drop-zone-title {
            font-size:16px;
            font-weight:600;
            color:#2b2b2b;
        }
        .drop-zone-sub {
            color:#6d7785;
            margin-top:6px;
            font-size:13px;
        }

        .preview-wrap {
            margin-top:16px;
        }
        .preview-grid {
            display:flex;
            flex-wrap:wrap;
            gap:12px;
        }
        .preview-item {
            width:150px;
            border:1px solid #e8e8e8;
            border-radius:12px;
            overflow:hidden;
            background:#fff;
        }
        .preview-thumb-wrap {
            width:100%;
            height:120px;
            background:#fafafa;
            display:flex;
            align-items:center;
            justify-content:center;
            border-bottom:1px solid #eee;
        }
        .preview-thumb {
            max-width:100%;
            max-height:100%;
        }
        .preview-body {
            padding:8px;
        }
        .preview-name {
            font-size:11px;
            color:#555;
            word-break:break-word;
            min-height:30px;
        }
        .preview-size {
            font-size:11px;
            color:#888;
            margin-top:4px;
        }

        @media (max-width: 768px) {
            .img-item { width:100%; }
            .preview-item { width:48%; }
        }
    </style>

    <div class="img-page-wrap">
        <div class="img-card">
            <div class="img-title">Product Images</div>
            <div class="img-subtitle">SKU bazlı çoklu foto yönetimi</div>

            <div class="toolbar-row">
                <div class="toolbar-col">
                    <label>SKU</label>
                    <asp:TextBox ID="txtSKU" runat="server" CssClass="form-control" placeholder="Örn: SRMI212907260"></asp:TextBox>
                </div>

                <div class="toolbar-col">
                    <label>Marketplace</label>
                    <asp:DropDownList ID="ddlMarketplace" runat="server" CssClass="form-control">
                        <asp:ListItem Text="Default" Value=""></asp:ListItem>
                        <asp:ListItem Text="Amazon" Value="AMAZON"></asp:ListItem>
                        <asp:ListItem Text="Etsy" Value="ETSY"></asp:ListItem>
                        <asp:ListItem Text="eBay" Value="EBAY"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="toolbar-col">
                    <label>Image Role</label>
                    <asp:DropDownList ID="ddlImageRole" runat="server" CssClass="form-control">
                        <asp:ListItem Text="MAIN" Value="MAIN"></asp:ListItem>
                        <asp:ListItem Text="GALLERY" Value="GALLERY"></asp:ListItem>
                        <asp:ListItem Text="HAND" Value="HAND"></asp:ListItem>
                        <asp:ListItem Text="SIZECHART" Value="SIZECHART"></asp:ListItem>
                        <asp:ListItem Text="PACKAGING" Value="PACKAGING"></asp:ListItem>
                        <asp:ListItem Text="OTHER" Value="OTHER"></asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="toolbar-col">
                    <label>Start Sort Order</label>
                    <asp:TextBox ID="txtSortOrder" runat="server" CssClass="form-control" Text=""></asp:TextBox>
                    <div class="small-muted" style="margin-top:4px;">Boş bırakırsan otomatik devam eder.</div>
                </div>

                <div class="toolbar-col" style="min-width:160px; flex:0 0 160px;">
                    <label class="small-muted"> </label>
                    <div>
                        <asp:CheckBox ID="chkIsPrimary" runat="server" Text=" First image primary" />
                    </div>
                </div>
            </div>

            <div style="margin-top:16px;">
                <label>Image Upload</label>
                <asp:FileUpload ID="fuImage" runat="server" CssClass="form-control" />
                <div id="dropZone" class="drop-zone">
                    <div class="drop-zone-title">Dosyaları buraya bırak veya tıkla</div>
                    <div class="drop-zone-sub">JPG, JPEG, PNG, WEBP · çoklu seçim desteklenir · önerilen üst sınır: 10 MB / dosya</div>
                </div>
                <div class="small-muted" style="margin-top:8px;">
                    MAIN görseller otomatik 2000x2000 beyaz canvas içine alınır. Diğer roller uzun kenar 2000 px olacak şekilde optimize edilir.
                </div>
            </div>

            <div class="preview-wrap">
                <div class="small-muted" style="margin-bottom:8px;">Seçilen dosyalar önizleme</div>
                <div id="previewGrid" class="preview-grid"></div>
            </div>

            <div style="margin-top:16px;">
                <asp:Button ID="btnLoad" runat="server" Text="Load Images" CssClass="btn btn-default btn-space" OnClick="btnLoad_Click" />
                <asp:Button ID="btnUpload" runat="server" Text="Upload Image(s)" CssClass="btn btn-primary btn-space" OnClick="btnUpload_Click" />
            </div>

            <div style="margin-top:12px;">
                <asp:Label ID="lblMsg" runat="server"></asp:Label>
            </div>
        </div>

        <div class="img-card">
            <asp:Repeater ID="rptImages" runat="server" OnItemCommand="rptImages_ItemCommand">
                <HeaderTemplate>
                    <div class="img-grid">
                </HeaderTemplate>
                <ItemTemplate>
                    <div class="img-item">
                        <div class="img-thumb-wrap">
                            <img class="img-thumb" src='<%# ResolveUrl("~/ProductImageHandler.ashx?id=" + Eval("ProductImageID")) %>' alt='' />
                        </div>
                        <div class="img-body">
                            <div class="img-file"><%# Eval("FileName") %></div>
                            <div class="img-badges">
                                <%# Convert.ToBoolean(Eval("IsPrimary")) ? "<span class='img-badge img-badge-primary'>PRIMARY</span>" : "" %>
                                <span class="img-badge"><%# Eval("ImageRole") %></span>
                                <span class="img-badge"><%# string.IsNullOrWhiteSpace(Convert.ToString(Eval("Marketplace"))) ? "DEFAULT" : Eval("Marketplace").ToString() %></span>
                            </div>
                            <div class="small-muted">Sort: <%# Eval("SortOrder") %></div>
                            <div class="small-muted">ID: <%# Eval("ProductImageID") %></div>
                            <div style="margin-top:10px;">
                                <asp:LinkButton ID="lnkPrimary" runat="server" CssClass="btn btn-success btn-xs btn-space"
                                    CommandName="makeprimary" CommandArgument='<%# Eval("ProductImageID") %>'>
                                    Make Primary
                                </asp:LinkButton>

                                <asp:LinkButton ID="lnkDelete" runat="server" CssClass="btn btn-danger btn-xs btn-space"
                                    CommandName="deleteimg" CommandArgument='<%# Eval("ProductImageID") %>'
                                    OnClientClick="return confirm('Bu görsel silinsin mi?');">
                                    Delete
                                </asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
                <FooterTemplate>
                    </div>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>

    <script>
        (function () {
            var fileInput = document.getElementById('<%= fuImage.ClientID %>');
            var dropZone = document.getElementById('dropZone');
            var previewGrid = document.getElementById('previewGrid');

            if (!fileInput || !dropZone || !previewGrid) return;

            try {
                fileInput.setAttribute('multiple', 'multiple');
                fileInput.setAttribute('accept', '.jpg,.jpeg,.png,.webp,image/jpeg,image/png,image/webp');
            } catch (e) { }

            function formatSize(bytes) {
                if (bytes < 1024) return bytes + ' B';
                if (bytes < 1024 * 1024) return (bytes / 1024).toFixed(1) + ' KB';
                return (bytes / 1024 / 1024).toFixed(2) + ' MB';
            }

            function clearPreview() {
                previewGrid.innerHTML = '';
            }

            function renderFiles(files) {
                clearPreview();
                if (!files || !files.length) return;

                Array.prototype.forEach.call(files, function (file) {
                    var item = document.createElement('div');
                    item.className = 'preview-item';

                    var thumbWrap = document.createElement('div');
                    thumbWrap.className = 'preview-thumb-wrap';

                    var body = document.createElement('div');
                    body.className = 'preview-body';

                    var name = document.createElement('div');
                    name.className = 'preview-name';
                    name.textContent = file.name;

                    var size = document.createElement('div');
                    size.className = 'preview-size';
                    size.textContent = formatSize(file.size);

                    if (file.type && file.type.indexOf('image/') === 0) {
                        var img = document.createElement('img');
                        img.className = 'preview-thumb';
                        img.alt = file.name;
                        thumbWrap.appendChild(img);

                        var reader = new FileReader();
                        reader.onload = function (e) {
                            img.src = e.target.result;
                        };
                        reader.readAsDataURL(file);
                    } else {
                        thumbWrap.innerHTML = '<div class="small-muted">No preview</div>';
                    }

                    body.appendChild(name);
                    body.appendChild(size);

                    item.appendChild(thumbWrap);
                    item.appendChild(body);
                    previewGrid.appendChild(item);
                });
            }

            fileInput.addEventListener('change', function () {
                renderFiles(fileInput.files);
            });

            dropZone.addEventListener('click', function () {
                fileInput.click();
            });

            ['dragenter', 'dragover'].forEach(function (evtName) {
                dropZone.addEventListener(evtName, function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    dropZone.classList.add('dragover');
                });
            });

            ['dragleave', 'drop'].forEach(function (evtName) {
                dropZone.addEventListener(evtName, function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    dropZone.classList.remove('dragover');
                });
            });

            dropZone.addEventListener('drop', function (e) {
                var files = e.dataTransfer.files;
                if (!files || !files.length) return;

                try {
                    fileInput.files = files;
                } catch (err) {
                    // bazı tarayıcılarda readonly olabilir
                }
                renderFiles(files);
            });
        })();
    </script>

</asp:Content>