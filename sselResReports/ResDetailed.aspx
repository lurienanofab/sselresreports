<%@ Page Title="" Language="C#" MasterPageFile="~/ResReportsMaster.Master" AutoEventWireup="true" CodeBehind="ResDetailed.aspx.cs" Inherits="sselResReports.ResDetailed" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Tool Usage Detail</h2>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Select period:</td>
                    <td>
                        <lnf:PeriodPicker runat="server" ID="pp1" OnSelectedPeriodChanged="Pp1_SelectedPeriodChanged" AutoPostBack="true" />
                    </td>
                </tr>
                <tr>
                    <td>Select tool:</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlTool" AutoPostBack="true" Height="24" OnSelectedIndexChanged="DdlTool_SelectedIndexChanged">
                        </asp:DropDownList>
                        <span style="color: #BBBBBB;">&larr; Retreive data by selecting tool</span>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:LinkButton runat="server" ID="btnBack" Text="&laquo; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">
        <asp:Label runat="server" ID="lblMsg" ForeColor="#FF0000" Visible="false"></asp:Label>
        <asp:DataGrid runat="server" ID="dgActDate" AutoGenerateColumns="false" OnItemDataBound="dgActDate_ItemDataBound">
            <AlternatingItemStyle BackColor="Azure" />
            <HeaderStyle CssClass="GridHeader" />
            <Columns>
                <asp:BoundColumn DataField="ActDate" HeaderText="Date" DataFormatString="{0:d MMMM yyyy}">
                    <HeaderStyle Font-Bold="true" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" Width="150" VerticalAlign="Middle" />
                </asp:BoundColumn>
                <asp:TemplateColumn HeaderText="Activity Detail">
                    <ItemStyle BackColor="White" />
                    <ItemTemplate>
                        <asp:DataGrid runat="server" ID="dgActivity" AutoGenerateColumns="false" HeaderStyle-BackColor="#D4D0C8">
                            <AlternatingItemStyle BackColor="Azure" />
                            <Columns>
                                <asp:BoundColumn DataField="ActTime" HeaderText="Time" ItemStyle-Width="75" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Font-Italic="true"></asp:BoundColumn>
                                <asp:BoundColumn DataField="Descrip" HeaderText="Activity" ItemStyle-Width="650" HeaderStyle-Font-Italic="true"></asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
    </div>
</asp:Content>
