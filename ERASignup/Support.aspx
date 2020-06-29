<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Support.aspx.cs" Inherits="ERASignup.Support" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Support | ERAConnect</title>
    <style>
        * {
            margin: 0 auto;
            font-family: Poppins, 'Segoe UI';
        }

        #container {
            width: 100%;
            height: 100%;
            position: absolute;
        }

        #form {
            border: 3px dashed #ccc;
            padding: 15px 30px;
            padding: 1em 4em;
        }

        select, input {
            padding: 8px 20px;
            font-size: 18px;
        }

        #responseRow {
            background-color: #fbf9f2;
            border-top: 3px dashed #ccc;
            position: absolute;
            bottom: 0;
            width: 100%;
            text-align: center;
            padding: 1.5em 0;
        }

        select, input[type=text] {
            width: 250px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div id="container">
            <br />
            <table cellpadding="10" id="form">
                <tr>
                    <td>SubDomain</td>
                    <td>
                        <asp:DropDownList ID="ddSubDomains" runat="server" Style="width: 100%;" required></asp:DropDownList></td>
                </tr>
                <tr>
                    <td>Key</td>
                    <td>
                        <asp:TextBox ID="txtKey" runat="server" required></asp:TextBox></td>
                </tr>
                <tr>
                    <td>Secret</td>
                    <td>
                        <asp:TextBox ID="txtSecret" runat="server" required></asp:TextBox></td>
                </tr>
                <tr>
                    <td></td>
                    <td>
                        <asp:Button ID="btnAdd" runat="server" OnClick="btnAdd_Click" Text="Submit" Width="49%" />
                        <asp:Button ID="btnRefresh" runat="server" OnClick="btnRefresh_Click" Text="Refresh" Width="49%" UseSubmitBehavior="false" /></td>
                </tr>
            </table>
            <div id="responseRow" runat="server" visible="false">
                <asp:Label ID="txtResponse" runat="server"></asp:Label>
            </div>
        </div>
    </form>
</body>
</html>
