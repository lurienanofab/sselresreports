<%@ Page Title="" Language="C#" MasterPageFile="~/ResReportsMaster.Master" AutoEventWireup="true" CodeBehind="ResToolUsageSummary.aspx.cs" Inherits="sselResReports.ResToolUsageSummary" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script>
        function validate() {
            $(".account-types-message").html("");

            var checked = $(".account-types input[type='checkbox']:checked");

            if (checked.length == 0) {
                $(".account-types-message").html("You must select at least one account type.");
                return false;
            }

            return true;
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <script type="text/javascript" src="scripts/jquery.toolutilization.js"></script>
    <div class="section">
        <h2>Tool Usage Summary</h2>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Start period:</td>
                    <td>
                        <lnf:PeriodPicker runat="server" ID="pp1" StartPeriod="2006-01-01" />
                        <span style="padding-left: 20px;">Months:</span>
                        <asp:TextBox runat="server" ID="txtNumMonths" Width="30" Text="1" CssClass="report-text"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>Select tool:</td>
                    <td>
                        <asp:DropDownList ID="ddlTool" runat="server" DataSourceID="odsTool" DataValueField="Value" DataTextField="Text" CssClass="report-select"></asp:DropDownList>
                        <asp:ObjectDataSource ID="odsTool" runat="server" TypeName="sselResReports.AppCode.DAL.SchedulerTool" SelectMethod="GetToolSelectItems">
                            <SelectParameters>
                                <asp:Parameter Name="includeSelectItem" DefaultValue="true" />
                            </SelectParameters>
                        </asp:ObjectDataSource>
                    </td>
                </tr>
                <tr>
                    <td style="vertical-align: top; padding-top: 14px;">Select Account Type:</td>
                    <td class="account-types">
                        <asp:CheckBoxList runat="server" ID="cblAccountType" RepeatDirection="Horizontal" DataTextField="Text" DataValueField="Value"></asp:CheckBoxList>
                        <div class="account-types-message" style="color: #ff0000; padding-left: 10px;"></div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <asp:Button runat="server" ID="btnReport" Text="Retrieve Data" OnClick="btnReport_Click" CssClass="report-button" OnClientClick="return validate();" />
                        <span style="color: #bbbbbb;">&larr; You must click here to retrieve the data</span><br />
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>
        </div>
    </div>
    <asp:Panel runat="server" ID="panSummary" Visible="false">
        <div class="section summary">
            <h3>Summary</h3>
            <table>
                <tr>
                    <td style="width: 180px;"><strong>Resource:</strong></td>
                    <td>
                        <asp:Literal runat="server" ID="litResource"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td style="width: 180px;"><strong>Total Reservations:</strong></td>
                    <td>
                        <asp:Literal runat="server" ID="litTotalReservations"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td><strong>Total Scheduled Hours:</strong></td>
                    <td>
                        <asp:Literal runat="server" ID="litTotalSchedHours"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td><strong>Total Actual Hours:</strong></td>
                    <td>
                        <asp:Literal runat="server" ID="litTotalActualHours"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <hr />
                        <div><strong>Hourly Rates:</strong></div>
                        <asp:Literal runat="server" ID="litHourlyRates"></asp:Literal>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <hr />
                        <asp:Repeater runat="server" ID="rptCombined">
                            <HeaderTemplate>
                                <table class="summary-table">
                                    <tr>
                                        <th class="edge"></th>
                                        <th class="edge" colspan="3">Hours</th>
                                        <th class="edge" colspan="3">Charges</th>
                                        <th class="edge" colspan="3">%</th>
                                    </tr>
                                    <tr>
                                        <th class="edge"></th>
                                        <th class="normal">Gross</th>
                                        <th class="normal">Forgiven</th>
                                        <th class="edge">Net</th>
                                        <th class="normal">Gross</th>
                                        <th class="normal">Forgiven</th>
                                        <th class="edge">Net</th>
                                        <th class="edge">Forgiven</th>
                                    </tr>
                            </HeaderTemplate>
                            <ItemTemplate>
                                <tr>
                                    <td colspan="8" class="section-title no-border">&bull;&nbsp;<%#Eval("ChargeTypeName") %></td>
                                </tr>
                                <tr>
                                    <td class="edge"><strong>Normal:</strong></td>
                                    <td class="normal"><%#SetStyle(GetHours(Container.DataItem, "Normal", false), "#0.0000")%></td>
                                    <td class="normal"><%#SetStyle(GetForgivenHours(Container.DataItem, "Normal"), "#0.0000")%></td>
                                    <td class="edge"><%#SetStyle(GetHours(Container.DataItem, "Normal", true), "#0.0000")%></td>
                                    <td class="normal"><%#SetStyle(GetAmount(Container.DataItem, "Normal", false), "C")%></td>
                                    <td class="normal"><%#SetStyle(GetForgivenAmount(Container.DataItem, "Normal"), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetAmount(Container.DataItem, "Normal", true), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetForgivenPercent(Container.DataItem, "Normal"), "0.000%")%></td>
                                </tr>
                                <tr>
                                    <td class="edge"><strong>Over Time:</strong></td>
                                    <td class="normal"><%#SetStyle(GetHours(Container.DataItem, "OverTime", false), "#0.0000")%></td>
                                    <td class="normal"><%#SetStyle(GetForgivenHours(Container.DataItem, "OverTime"), "#0.0000")%></td>
                                    <td class="edge"><%#SetStyle(GetHours(Container.DataItem, "OverTime", true), "#0.0000")%></td>
                                    <td class="normal"><%#SetStyle(GetAmount(Container.DataItem, "OverTime", false), "C")%></td>
                                    <td class="normal"><%#SetStyle(GetForgivenAmount(Container.DataItem, "OverTime"), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetAmount(Container.DataItem, "OverTime", true), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetForgivenPercent(Container.DataItem, "OverTime"), "0.000%")%></td>
                                </tr>
                                <tr>
                                    <td class="edge"><strong>Booking Fee:</strong></td>
                                    <td class="normal center">-</td>
                                    <td class="normal center">-</td>
                                    <td class="edge center">-</td>
                                    <td class="normal"><%#SetStyle(GetBookingFee(Container.DataItem, false), "C")%></td>
                                    <td class="normal"><%#SetStyle(GetBookingFeeForgiven(Container.DataItem), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetBookingFee(Container.DataItem, true), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetBookingFeeForgivenPercent(Container.DataItem), "0.000%")%></td>
                                </tr>
                            </ItemTemplate>
                            <FooterTemplate>
                                <tr>
                                    <td colspan="8" class="section-title no-border">&bull;&nbsp;Total</td>
                                </tr>
                                <tr>
                                    <td class="edge"><strong>Usage Charges:</strong></td>
                                    <td class="normal"><%#SetStyle(GetTotalUsageHours(false), "#0.0000")%></td>
                                    <td class="normal"><%#SetStyle(GetTotalUsageHoursForgiven(), "#0.0000")%></td>
                                    <td class="edge"><%#SetStyle(GetTotalUsageHours(true), "#0.0000")%></td>
                                    <td class="normal"><%#SetStyle(GetTotalUsageAmount(false), "C")%></td>
                                    <td class="normal"><%#SetStyle(GetTotalUsageAmountForgiven(), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetTotalUsageAmount(true), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetTotalForgivenPercentage(), "0.000%")%></td>
                                </tr>
                                <tr>
                                    <td class="edge"><strong>Booking Fee:</strong></td>
                                    <td class="normal center">-</td>
                                    <td class="normal center">-</td>
                                    <td class="edge center">-</td>
                                    <td class="normal"><%#SetStyle(GetTotalBookingFee(false), "C")%></td>
                                    <td class="normal"><%#SetStyle(GetTotalBookingFeeForgiven(), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetTotalBookingFee(true), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetTotalBookingFeeForgivenPercentage(), "0.000%")%></td>
                                </tr>
                                <tr>
                                    <td class="edge"><strong>Billed Charges:</strong></td>
                                    <td class="normal center">-</td>
                                    <td class="normal center">-</td>
                                    <td class="edge center">-</td>
                                    <td class="normal"><%#SetStyle(GetTotalBilledAmount(false), "C")%></td>
                                    <td class="normal"><%#SetStyle(GetTotalBilledAmountForgiven(), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetTotalBilledAmount(true), "C")%></td>
                                    <td class="edge"><%#SetStyle(GetTotalBilledAmountForgivenPercentage(), "0.000%")%></td>
                                </tr>
                                </table>
                            </FooterTemplate>
                        </asp:Repeater>
                    </td>
                </tr>
            </table>
            <asp:Literal runat="server" ID="litSummaryExport"></asp:Literal>
        </div>
    </asp:Panel>
    <div class="section activated detail-parent">
        <h3>Activated Reservations</h3>
        <asp:GridView runat="server" ID="gvActivated" DataSourceID="odsActivated" AutoGenerateColumns="false" AllowSorting="true" OnRowDataBound="gvActivated_RowDataBound" OnDataBound="gvActivated_DataBound" GridLines="None" CssClass="gridview highlight">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <EmptyDataTemplate>
                -- No records for this tool in this period --
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField HeaderText="User Name" DataField="DisplayName" NullDisplayText="Error" ItemStyle-HorizontalAlign="Center" SortExpression="DisplayName" />
                <asp:BoundField HeaderText="Account Name" DataField="AccountName" NullDisplayText="Error" ItemStyle-HorizontalAlign="Center" SortExpression="AccountName" />
                <asp:BoundField HeaderText="Uses" DataField="TotalUses" NullDisplayText="0" ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:0.00}" HtmlEncode="false">
                    <HeaderStyle CssClass="edge" />
                    <ItemStyle CssClass="edge" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Scheduled" DataField="TotalSchedHours" ItemStyle-HorizontalAlign="Center" SortExpression="TotalSchedHours" DataFormatString="{0:0.0000}" HtmlEncode="false" />
                <asp:BoundField HeaderText="Actual" DataField="TotalActHours" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="TotalActHours" DataFormatString="{0:0.0000}" HtmlEncode="false">
                    <HeaderStyle CssClass="edge" />
                    <ItemStyle CssClass="edge" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Gross" DataField="NormalHoursGross" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalHoursGross" DataFormatString="{0:0.0000}" HtmlEncode="false" HeaderStyle-CssClass="hours" ItemStyle-CssClass="hours" />
                <asp:BoundField HeaderText="Forgiven" DataField="NormalHoursForgiven" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalHoursForgiven" DataFormatString="{0:0.0000}" HtmlEncode="false" HeaderStyle-CssClass="hours" ItemStyle-CssClass="hours" />
                <asp:BoundField HeaderText="Net" DataField="NormalHoursNet" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalHoursNet" DataFormatString="{0:0.0000}" HtmlEncode="false">
                    <HeaderStyle CssClass="edge hours" />
                    <ItemStyle CssClass="edge hours" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Gross" DataField="OverTimeHoursGross" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="OverTimeHoursGross" DataFormatString="{0:0.0000}" HtmlEncode="false" HeaderStyle-CssClass="hours" ItemStyle-CssClass="hours" />
                <asp:BoundField HeaderText="Forgiven" DataField="OverTimeHoursForgiven" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="OverTimeHoursForgiven" DataFormatString="{0:0.0000}" HtmlEncode="false" HeaderStyle-CssClass="hours" ItemStyle-CssClass="hours" />
                <asp:BoundField HeaderText="Net" DataField="OverTimeHoursNet" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="OverTimeHoursNet" DataFormatString="{0:0.0000}" HtmlEncode="false">
                    <HeaderStyle CssClass="edge hours" />
                    <ItemStyle CssClass="edge hours" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Gross" DataField="NormalAmountGross" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalAmountGross" DataFormatString="{0:c2}" HtmlEncode="False" HeaderStyle-CssClass="amount" ItemStyle-CssClass="amount" />
                <asp:BoundField HeaderText="Forgiven" DataField="NormalAmountForgiven" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalAmountForgiven" DataFormatString="{0:c2}" HtmlEncode="False" HeaderStyle-CssClass="amount" ItemStyle-CssClass="amount" />
                <asp:BoundField HeaderText="Net" DataField="NormalAmountNet" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalAmountNet" DataFormatString="{0:c2}" HtmlEncode="False">
                    <HeaderStyle CssClass="edge amount" />
                    <ItemStyle CssClass="edge amount" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Gross" DataField="OverTimeAmountGross" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="OverTimeAmountGross" DataFormatString="{0:c2}" HtmlEncode="False" HeaderStyle-CssClass="amount" ItemStyle-CssClass="amount" />
                <asp:BoundField HeaderText="Forgiven" DataField="OverTimeAmountForgiven" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="OverTimeAmountForgiven" DataFormatString="{0:c2}" HtmlEncode="False" HeaderStyle-CssClass="amount" ItemStyle-CssClass="amount" />
                <asp:BoundField HeaderText="Net" DataField="OverTimeAmountNet" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="OverTimeAmountNet" DataFormatString="{0:c2}" HtmlEncode="False">
                    <HeaderStyle CssClass="edge amount" />
                    <ItemStyle CssClass="edge amount" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Gross" DataField="BilledAmountGross" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BilledAmountGross" DataFormatString="{0:c2}" HtmlEncode="False" />
                <asp:BoundField HeaderText="Forgiven" DataField="BilledAmountForgiven" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BilledAmountForgiven" DataFormatString="{0:c2}" HtmlEncode="false" />
                <asp:BoundField HeaderText="Net" DataField="BilledAmountNet" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BilledAmountNet" DataFormatString="{0:c2}" HtmlEncode="False">
                    <HeaderStyle CssClass="edge" />
                    <ItemStyle CssClass="edge" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        <asp:Literal runat="server" ID="litActivatedExport"></asp:Literal>
        <asp:ObjectDataSource ID="odsActivated" runat="server" TypeName="sselResReports.AppCode.DAL.ToolUsageSummary" SelectMethod="GetActivatedReservations">
            <SelectParameters>
                <asp:ControlParameter ControlID="pp1" Type="Int32" PropertyName="SelectedMonth" Name="Month" />
                <asp:ControlParameter ControlID="pp1" Type="Int32" PropertyName="SelectedYear" Name="Year" />
                <asp:ControlParameter ControlID="txtNumMonths" Type="Int32" PropertyName="Text" Name="NumMonths" />
                <asp:ControlParameter ControlID="ddlTool" Type="Int32" PropertyName="SelectedValue" Name="ResourceID" />
                <asp:ControlParameter ControlID="hidVarAccountTypes" PropertyName="Value" Type="String" Name="selectedAccountTypes" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
    <div class="section unactivated detail-parent">
        <h3>Unactivated Reservations</h3>
        <asp:GridView ID="gvUnactivated" runat="server" DataSourceID="odsUnactivated" AutoGenerateColumns="false" AllowSorting="true" OnRowDataBound="gvUnactivated_RowDataBound" OnDataBound="gvUnactivated_DataBound" GridLines="None" CssClass="gridview highlight">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <EmptyDataTemplate>
                -- No records for this tool in this period --
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField HeaderText="User Name" DataField="DisplayName" NullDisplayText="Error" ItemStyle-HorizontalAlign="Center" SortExpression="DisplayName" />
                <asp:BoundField HeaderText="Account Name" DataField="AccountName" NullDisplayText="Error" ItemStyle-HorizontalAlign="Center" SortExpression="AccountName" />
                <asp:BoundField HeaderText="Uses" DataField="TotalUses" NullDisplayText="0" ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:0.00}" HtmlEncode="false">
                    <HeaderStyle CssClass="edge" />
                    <ItemStyle CssClass="edge" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Scheduled" DataField="TotalSchedHours" ItemStyle-HorizontalAlign="Center" SortExpression="TotalSchedHours" DataFormatString="{0:0.0000}" HtmlEncode="false">
                    <HeaderStyle CssClass="edge" />
                    <ItemStyle CssClass="edge" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Gross" DataField="NormalHoursGross" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalHoursGross" DataFormatString="{0:0.0000}" HtmlEncode="false" HeaderStyle-CssClass="hours" ItemStyle-CssClass="hours" />
                <asp:BoundField HeaderText="Forgiven" DataField="NormalHoursForgiven" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalHoursForgiven" DataFormatString="{0:0.0000}" HtmlEncode="false" HeaderStyle-CssClass="hours" ItemStyle-CssClass="hours" />
                <asp:BoundField HeaderText="Net" DataField="NormalHoursNet" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalHoursNet" DataFormatString="{0:0.0000}" HtmlEncode="false">
                    <HeaderStyle CssClass="edge hours" />
                    <ItemStyle CssClass="edge hours" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Gross" DataField="NormalAmountGross" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalAmountGross" DataFormatString="{0:c2}" HtmlEncode="False" HeaderStyle-CssClass="amount" ItemStyle-CssClass="amount" />
                <asp:BoundField HeaderText="Forgiven" DataField="NormalAmountForgiven" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalAmountForgiven" DataFormatString="{0:c2}" HtmlEncode="False" HeaderStyle-CssClass="amount" ItemStyle-CssClass="amount" />
                <asp:BoundField HeaderText="Net" DataField="NormalAmountNet" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="NormalAmountNet" DataFormatString="{0:c2}" HtmlEncode="False">
                    <HeaderStyle CssClass="edge amount" />
                    <ItemStyle CssClass="edge amount" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Gross" DataField="BookingFeeGross" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BookingFeeGross" DataFormatString="{0:c2}" HtmlEncode="False" />
                <asp:BoundField HeaderText="Forgiven" DataField="BookingFeeForgiven" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BookingFeeForgiven" DataFormatString="{0:c2}" HtmlEncode="false" />
                <asp:BoundField HeaderText="Net" DataField="BookingFeeNet" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BookingFeeNet" DataFormatString="{0:c2}" HtmlEncode="False">
                    <HeaderStyle CssClass="edge" />
                    <ItemStyle CssClass="edge" />
                </asp:BoundField>
                <asp:BoundField HeaderText="Gross" DataField="BilledAmountGross" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BilledAmountGross" DataFormatString="{0:c2}" HtmlEncode="False" />
                <asp:BoundField HeaderText="Forgiven" DataField="BilledAmountForgiven" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BilledAmountForgiven" DataFormatString="{0:c2}" HtmlEncode="false" />
                <asp:BoundField HeaderText="Net" DataField="BilledAmountNet" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BilledAmountNet" DataFormatString="{0:c2}" HtmlEncode="False">
                    <HeaderStyle CssClass="edge" />
                    <ItemStyle CssClass="edge" />
                </asp:BoundField>
            </Columns>
        </asp:GridView>
        <asp:Literal runat="server" ID="litUnactivatedExport"></asp:Literal>
        <asp:ObjectDataSource ID="odsUnactivated" runat="server" TypeName="sselResReports.AppCode.DAL.ToolUsageSummary" SelectMethod="GetUnactivatedReservations">
            <SelectParameters>
                <asp:ControlParameter ControlID="pp1" Type="Int32" PropertyName="SelectedMonth" Name="Month" />
                <asp:ControlParameter ControlID="pp1" Type="Int32" PropertyName="SelectedYear" Name="Year" />
                <asp:ControlParameter ControlID="txtNumMonths" Type="Int32" PropertyName="Text" Name="NumMonths" />
                <asp:ControlParameter ControlID="ddlTool" Type="Int32" PropertyName="SelectedValue" Name="ResourceID" />
                <asp:ControlParameter ControlID="hidVarAccountTypes" PropertyName="Value" Type="String" Name="selectedAccountTypes" />
            </SelectParameters>
        </asp:ObjectDataSource>
    </div>
    <div class="section forgiven">
        <h3>Forgiven Reservations</h3>
        <asp:GridView ID="gvForgiven" runat="server" DataSourceID="odsForgiven" AutoGenerateColumns="false" AllowSorting="true" OnRowDataBound="gvForgiven_RowDataBound" GridLines="None" CssClass="gridview highlight">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <EmptyDataTemplate>
                &nbsp;-- No records for this tool in this period --
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField HeaderText="User Name" DataField="DisplayName" NullDisplayText="Error" ItemStyle-HorizontalAlign="Center" SortExpression="DisplayName" />
                <asp:BoundField HeaderText="Account Name" DataField="AccountName" NullDisplayText="Error" ItemStyle-HorizontalAlign="Center" SortExpression="AccountName" />
                <asp:BoundField HeaderText="Uses" DataField="TotalUses" NullDisplayText="0" ItemStyle-Width="40" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:0.00}" HtmlEncode="False" />
                <asp:BoundField HeaderText="Scheduled Hours" DataField="TotalSchedHours" ItemStyle-HorizontalAlign="Center" SortExpression="TotalSchedHours" DataFormatString="{0:0.00}" HtmlEncode="False" />
                <asp:BoundField HeaderText="Actual Hours" DataField="TotalActHours" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="TotalActHours" DataFormatString="{0:0.00}" HtmlEncode="False" />
                <asp:BoundField HeaderText="Billed Gross" DataField="BilledAmountGross" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BilledAmountGross" DataFormatString="{0:c2}" HtmlEncode="False" />
                <asp:BoundField HeaderText="Forgiven" DataField="BilledAmountForgiven" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BilledAmountForgiven" DataFormatString="{0:c2}" HtmlEncode="false" />
                <asp:BoundField HeaderText="Billed Net" DataField="BilledAmountNet" NullDisplayText="0" ItemStyle-HorizontalAlign="Center" SortExpression="BilledAmountNet" DataFormatString="{0:c2}" HtmlEncode="False" />
            </Columns>
        </asp:GridView>
        <asp:Literal runat="server" ID="litForgivenExport"></asp:Literal>
        <asp:ObjectDataSource ID="odsForgiven" runat="server" TypeName="sselResReports.AppCode.DAL.ToolUsageSummary" SelectMethod="GetForgivenReservations">
            <SelectParameters>
                <asp:ControlParameter ControlID="pp1" Type="Int32" PropertyName="SelectedMonth" Name="Month" />
                <asp:ControlParameter ControlID="pp1" Type="Int32" PropertyName="SelectedYear" Name="Year" />
                <asp:ControlParameter ControlID="txtNumMonths" Type="Int32" PropertyName="Text" Name="NumMonths" />
                <asp:ControlParameter ControlID="ddlTool" Type="Int32" PropertyName="SelectedValue" Name="ResourceID" />
                <asp:ControlParameter ControlID="hidVarAccountTypes" PropertyName="Value" Type="String" Name="selectedAccountTypes" />
            </SelectParameters>
        </asp:ObjectDataSource>

        <asp:HiddenField  runat="server"  ID="hidVarAccountTypes" Value="1,2,3" />

    </div>
</asp:Content>
