<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm2.aspx.cs" Inherits="Feniks.Administrator.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Refresh" content="2;url=/Administrator/Orders.aspx" />
    <title></title>
        <script type="text/javascript">
        var soundObject = null;
        function PlaySound() {
            if (soundObject != null) {
                document.body.removeChild(soundObject);
                soundObject.removed = true;
                soundObject = null;
            }
            soundObject = document.createElement("embed");
            soundObject.setAttribute("src", "sounds/chaching.wav");
            soundObject.setAttribute("hidden", true);
            soundObject.setAttribute("autostart", true);
            document.body.appendChild(soundObject);
            }
            window.onload = function () {
                PlaySound();
            }
            {
                window.setTimeout(CloseMe, 5000);
            }
        </script>
<%--            <script type="text/javascript">
                function CloseWindow() {
                    window.close();
                }
            </script>--%>
<%--     <script language="javascript" type="text/javascript">
         function PlaySound() {
             if (soundObject != null) {
                 document.body.removeChild(soundObject);
                 soundObject.removed = true;
                 soundObject = null;
             }
             soundObject = document.createElement("embed");
             soundObject.setAttribute("src", "sounds/chaching.wav");
             soundObject.setAttribute("hidden", true);
             soundObject.setAttribute("autostart", true);
             document.body.appendChild(soundObject);
         }
         var redirectTimerId = 0;
         function closeWindow() {
             window.opener = top;
             redirectTimerId = window.setTimeout('redirect()', 2000);
             window.close();
         }

         function stopRedirect() {
             window.clearTimeout(redirectTimerId);
         }

         function redirect() {
             window.location = 'default.aspx';
         }
     </script>--%>
</head>
<body>
    <asp:Label ID="Label1" runat="server" Text="The order was successfully registered." BackColor="#33CC33" Font-Names="Arial"></asp:Label>
<%--    <form id="form1" runat="server">
        <input type = "button" onclick = "PlaySound()" value = "Play Sound" />
    </form>--%>
</body>
</html>
