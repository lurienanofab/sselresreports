<%@ Page Title="" Language="C#" MasterPageFile="~/ResReportsMaster.Master" AutoEventWireup="true" CodeBehind="ResReservationDetails.aspx.cs" Inherits="sselResReports.ResReservationDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Reservation Details</h2>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Select period:</td>
                    <td>
                        <lnf:PeriodPicker runat="server" ID="pp1" AutoPostBack="true" />
                    </td>
                </tr>
                <tr>
                    <td>Select user:</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlUser" DataTextField="DisplayName" DataValueField="ClientID" DataSourceID="odsUser" Height="24">
                        </asp:DropDownList>
                        <asp:ObjectDataSource runat="server" ID="odsUser" TypeName="sselResReports.AppCode.DAL.ClientDA" SelectMethod="GetAllClientsbyPeriod">
                            <SelectParameters>
                                <asp:ControlParameter ControlID="pp1" PropertyName="SelectedYear" Type="Int32" Name="Year" />
                                <asp:ControlParameter ControlID="pp1" PropertyName="SelectedMonth" Type="Int32" Name="Month" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:Button runat="server" ID="btnReport" Text="Retrieve Data" OnClick="btnReport_Click" CssClass="report-button" />
            </div>
            <div class="criteria-item">
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">
        <asp:GridView ID="gvRes" runat="server" AutoGenerateColumns="false" AllowSorting="true" GridLines="None" CssClass="gridview highlight">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <Columns>
                <asp:BoundField DataField="ResourceName" HeaderText="Tool" SortExpression="ResourceName" HeaderStyle-Width="280" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="AccountName" HeaderText="Account" SortExpression="AccountName" HeaderStyle-Width="220" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="BeginDateTime" HeaderText="Begin" SortExpression="BeginDateTime" ItemStyle-Width="140" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:MM/dd/yyyy HH:mm:ss}" HtmlEncode="False" />
                <asp:BoundField DataField="EndDateTime" HeaderText="End" SortExpression="EndDateTime" ItemStyle-Width="140" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:MM/dd/yyyy HH:mm:ss}" HtmlEncode="False" />
                <asp:BoundField DataField="ActualBeginDateTime" HeaderText="Actual Begin" SortExpression="ActualBeginDateTime" ItemStyle-Width="140" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:MM/dd/yyyy HH:mm:ss}" HtmlEncode="False" />
                <asp:BoundField DataField="ActualEndDateTime" HeaderText="Actual End" SortExpression="ActualEndDateTime" ItemStyle-Width="140" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:MM/dd/yyyy HH:mm:ss}" HtmlEncode="False" />
                <asp:BoundField DataField="DurationHour" HeaderText="Duration (hours)" HtmlEncode="false" DataFormatString="{0:0.00}" HeaderStyle-Width="100px" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="ActivityName" HeaderText="Activity" SortExpression="ActivityName" HeaderStyle-Width="150" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="ChargeMultiplier" HeaderText="Charge Multiplier" SortExpression="ChargeMultiplier" HeaderStyle-Width="10px" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField DataField="IsStarted" HeaderText="Is Started?" SortExpression="IsStarted" ItemStyle-HorizontalAlign="Center"></asp:BoundField>
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="odsRes" TypeName="sselResReports.AppCode.DAL.SchedulerTool" SelectMethod="GetReservationsByClientID" runat="server">
            <SelectParameters>
                <asp:ControlParameter ControlID="pp1" PropertyName="SelectedYear" Type="Int32" Name="Year" />
                <asp:ControlParameter ControlID="pp1" PropertyName="SelectedMonth" Type="Int32" Name="Month" />
                <asp:ControlParameter ControlID="ddlUser" PropertyName="SelectedValue" Type="Int32" Name="ClientID" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
</asp:Content>
