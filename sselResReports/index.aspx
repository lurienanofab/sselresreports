<%@ Page Title="" Language="C#" MasterPageFile="~/ResReportsMaster.Master" AutoEventWireup="true" CodeBehind="index.aspx.cs" Inherits="sselResReports.Index" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <table class="button-table">
            <tr>
                <td colspan="3" class="PageHeader">Resource Reports</td>
            </tr>
            <tr>
                <td colspan="3">You are logged in as
                <asp:Label ID="lblName" runat="server"></asp:Label></td>
            </tr>
            <tr>
                <td class="ButtonGroupHeader">
                    <asp:Label runat="server" ID="lblRes" Visible="false">Resource Reports</asp:Label>
                </td>
                <td class="ButtonGroupHeader">
                    <asp:Label runat="server" ID="lblAcc" Visible="false">Access Control Reports</asp:Label>
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnResStatus" CssClass="CommandButton" Visible="false" Text="Tool Status" OnCommand="Button_Command" CommandName="ResStatus" CommandArgument="" />
                </td>
                <td>
                    <asp:Button runat="server" ID="btnAccInLab" CssClass="CommandButton" Visible="false" Text="Users Currently in Lab" OnCommand="Button_Command" CommandName="AccInLab" CommandArgument="" />
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnResDetailed" CssClass="CommandButton" Visible="false" Text="Tool Usage Detail" OnCommand="Button_Command" CommandName="ResDetailed" CommandArgument="" />
                </td>
                <td>
                    <asp:Button runat="server" ID="btnAccChrono" CssClass="CommandButton" Visible="false" Text="Chronological Lab Access" OnCommand="Button_Command" CommandName="AccChrono" CommandArgument="" />
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnResUtil" CssClass="CommandButton" Text="Tool Utilization" Visible="false" OnCommand="Button_Command" CommandName="ResUtil" CommandArgument="" />
                </td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnResToolUsageSummary" CssClass="CommandButton" Width="209" Text="Tool Usage Summary" Visible="false" OnCommand="Button_Command" CommandName="ResToolUsageSummary" CommandArgument="" />
                </td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnResReservationDetails" CssClass="CommandButton" Text="Reservation Details" Visible="false" OnCommand="Button_Command" CommandName="ResReservationDetails" CommandArgument="" />
                </td>
                <td>
                    <asp:Button runat="server" ID="btnResToolReport" CssClass="CommandButton" Text="Reservation Abnormality" Visible="false" OnCommand="Button_Command" CommandName="ResToolReport" CommandArgument="" />
                </td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td class="ButtonGroupHeader">Application Control</td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td>
                    <asp:Button runat="server" ID="btnLogout" Width="209" CssClass="CommandButton" Text="Exit Application" OnClick="BtnLogout_Click" />
                </td>
                <td>&nbsp;</td>
                <td>&nbsp;</td>
            </tr>
        </table>
    </div>
</asp:Content>
