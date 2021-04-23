<%@ Page Title="" Language="C#" MasterPageFile="~/ResReportsMaster.Master" AutoEventWireup="true" CodeBehind="ResToolReport.aspx.cs" Inherits="sselResReports.ResToolReport" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Reservation Abnormality</h2>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Tools:</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlTool" DataTextField="Text" DataValueField="Value" AutoPostBack="True" Height="24" OnSelectedIndexChanged="DdlTool_SelectedIndexChanged">
                        </asp:DropDownList>
                        <span style="color: #BBBBBB;">&larr; Retrieve data by selecting tool</span>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
                <asp:Label runat="server" ID="lblMsg" ForeColor="#FF0000" Visible="false"></asp:Label>
            </div>
        </div>
    </div>
    <div class="section">
        <asp:GridView runat="server" ID="gv" GridLines="None" CssClass="gridview highlight">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <EmptyDataTemplate>
                -- No abnormal reservations found --
            </EmptyDataTemplate>
        </asp:GridView>
    </div>
</asp:Content>
