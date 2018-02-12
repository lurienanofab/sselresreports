<%@ Page Title="" Language="C#" MasterPageFile="~/ResReportsMaster.Master" AutoEventWireup="true" CodeBehind="AccChrono.aspx.cs" Inherits="sselResReports.AccChrono" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .chrono-room-access-report {
            border-collapse: collapse;
        }

            .chrono-room-access-report th,
            .chrono-room-access-report td {
                padding: 3px;
            }
    </style>
    <script type="text/javascript">
        $(document).ready(function () {
            $('.chrono-room-access').each(function () {
                var $this = $(this);
                $('.datepicker', $this).datepicker();
            });
        })
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="chrono-room-access">
        <div class="section">
            <h2>Chronological Room Access</h2>
            <div class="criteria">
                <table class="criteria-table">
                    <tr>
                        <td>Room:</td>
                        <td>
                            <asp:DropDownList runat="server" ID="ddlRoom" Height="24" AutoPostBack="false">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td>Start Date:</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtStartDate" Width="90" Height="24" CssClass="datepicker"></asp:TextBox>
                            <span style="margin-left: 20px;">Number of days:</span>
                            <asp:TextBox runat="server" ID="txtNumDays" Width="32" Height="24"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <div class="criteria-item">
                    <asp:Button runat="server" ID="btnReport" Text="Retrieve Data" OnClick="btnReport_Click" CssClass="report-button" />
                    <asp:Label ID="lblMsg" runat="server" ForeColor="Red" Visible="false"></asp:Label>
                </div>
                <div class="criteria-item">
                    <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
                </div>
            </div>
        </div>
        <div class="section">
            <table class="chrono-room-access-report">
                <tr>
                    <td>
                        <asp:DataGrid ID="dgInLab" runat="server" AutoGenerateColumns="false" OnItemDataBound="dgInLab_ItemDataBound">
                            <HeaderStyle Font-Bold="true" BackColor="#E0E0E0" />
                            <Columns>
                                <asp:BoundColumn DataField="EvtTime" HeaderText="Time" DataFormatString="{0: h:mm tt, d MMMM yyyy}">
                                    <HeaderStyle Width="200" />
                                    <ItemStyle VerticalAlign="Middle" />
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="Count" HeaderText="Count" DataFormatString="{0:#; ; }">
                                    <HeaderStyle HorizontalAlign="Center" Width="50" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </asp:BoundColumn>
                                <asp:BoundColumn DataField="InLabUsers" HeaderText="Users in lab">
                                    <HeaderStyle Width="250" />
                                </asp:BoundColumn>
                            </Columns>
                        </asp:DataGrid>
                    </td>
                </tr>
            </table>
        </div>
    </div>
</asp:Content>
