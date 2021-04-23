<%@ Page Title="" Language="C#" MasterPageFile="~/ResReportsMaster.Master" AutoEventWireup="true" CodeBehind="ResUtil.aspx.cs" Inherits="sselResReports.ResUtil" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .tool-utilization {
            font-family: Arial;
            font-size: 10pt;
        }

        .tool-utilization-report {
            border-collapse: collapse;
            width: 100%;
        }

            .tool-utilization-report > thead > tr > th,
            .tool-utilization-report > tbody > tr > td {
                padding: 3px;
                min-height: 35px;
                border: solid 1px #CFCFCF;
            }

            .tool-utilization-report > thead > tr > th {
                white-space: normal;
                text-align: center !important;
                padding-right: 20px;
            }

                .tool-utilization-report > thead > tr > th.activity-column {
                    width: 80px;
                }

            .tool-utilization-report > tbody > tr > td.numeric,
            .tool-utilization-report > tfoot > tr > td.numeric {
                text-align: right;
            }

            .tool-utilization-report > tfoot {
                font-weight: bold;
                background-color: #e6f7e6;
            }

        .report-criteria {
            padding-left: 20px;
            padding-right: 20px;
            padding-bottom: 10px;
            margin-bottom: 10px;
            border-bottom: solid 1px #c4c4c4;
        }

        .dataTables_filter {
            margin-bottom: 10px;
        }

            .dataTables_filter input {
                padding: 4px;
                width: 300px;
            }
    </style>

    <script src="scripts/jquery.toolutilization.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:HiddenField runat="server" ID="hidUserData" />
    <div class="tool-utilization">
        <div class="section">
            <h2>Tool Utilization Report</h2>
            <div class="criteria">
                <table class="criteria-table">
                    <tr>
                        <td>Starting Period:</td>
                        <td>
                            <lnf:PeriodPicker runat="server" ID="pp1" />
                            <span style="padding-left: 20px;">Number of months</span>
                            <asp:TextBox runat="server" ID="txtMonths" Width="32" Text="1" CssClass="report-text"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>Stats Based On:</td>
                        <td>
                            <asp:RadioButton runat="server" ID="rdoCharged" Text="Charged" GroupName="StatsBasedOn" CssClass="stats-checkbox charged" Checked="true" />
                            <asp:RadioButton runat="server" ID="rdoScheduled" Text="Scheduled" GroupName="StatsBasedOn" CssClass="stats-checkbox scheduled" />
                            <asp:RadioButton runat="server" ID="rdoActual" Text="Actual" GroupName="StatsBasedOn" CssClass="stats-checkbox actual" />
                        </td>
                    </tr>
                    <tr>
                        <td>Account Types:</td>
                        <td>
                            <asp:CheckBoxList runat="server" ID="cblAccountType" RepeatDirection="Horizontal" DataTextField="Text" DataValueField="Value" RepeatLayout="Flow"></asp:CheckBoxList>
                            <asp:PlaceHolder runat="server" ID="phAccountTypeMessage" Visible="false">
                                <span style="color: #ff0000;">* Select at least one Account Type</span>
                            </asp:PlaceHolder>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="include-forgiven" style="display: none;">
                                <asp:CheckBox runat="server" ID="chkIncludeForgiven" Text="Include Forgiven" Checked="false" />
                            </div>
                            <div class="include-forgiven-message" style="color: #999; font-style: italic; display: none;">
                                <input type="checkbox" disabled="disabled" />
                                Include Forgiven is only available for Stats Based On Charged.
                            </div>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <div class="show-percent-of-total">
                                <asp:CheckBox runat="server" ID="chkShowPercentOfTotal" Text="Show Percent of Total" Checked="false" />
                                <input runat="server" type="hidden" id="hidShowPercentOfTotal" class="show-percent" />
                            </div>
                        </td>
                    </tr>
                </table>
                <div class="criteria-item">
                    <asp:Button ID="btnReport" runat="server" OnClick="BtnReport_Click" Text="Retrieve Data" CssClass="report-button" />
                    <asp:Button ID="btnExport" runat="server" OnClick="BtnExport_Click" Text="Export" CssClass="report-button" />
                </div>
                <div class="criteria-item">
                    <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
                </div>
            </div>
            <hr />
        </div>
        <div class="section tool-utilization-report-container">
            <asp:Repeater runat="server" ID="rptToolUtilizationReport" OnItemDataBound="RptToolUtilizationReport_ItemDataBound">
                <HeaderTemplate>
                    <table class="tool-utilization-report">
                        <thead>
                            <tr>
                                <th style="width: 40px;">Lab</th>
                                <th>Process Tech</th>
                                <th>Resource</th>
                                <th>ID</th>
                                <asp:Repeater runat="server" ID="rptHeaderColumns">
                                    <ItemTemplate>
                                        <th class="activity-column"><%#Eval("ActivityName")%></th>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <th class="activity-column">IdleTime</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#GetLabName(Eval("ResourceID"))%></td>
                        <td><%#GetProcessTechName(Eval("ResourceID"))%></td>
                        <td><%#Eval("ResourceName")%></td>
                        <td style="text-align: right;"><%#Eval("ResourceID")%></td>
                        <asp:Repeater runat="server" ID="rptItemColumns">
                            <ItemTemplate>
                                <td class="numeric">
                                    <%#GetCellText(((RepeaterItem)Container.Parent.Parent).DataItem, Eval("ActivityName").ToString())%>
                                </td>
                            </ItemTemplate>
                        </asp:Repeater>
                        <td class="numeric"><%#GetCellText(Container.DataItem, "IdleTime")%></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                    <tfoot>
                        <tr class="subtotal-row">
                            <td colspan="4">Subtotal</td>
                            <asp:Repeater runat="server" ID="rptSubtotalColumns">
                                <ItemTemplate>
                                    <td class="numeric"></td>
                                </ItemTemplate>
                            </asp:Repeater>
                            <td class="numeric"></td>
                        </tr>
                        <tr class="total-row">
                            <td colspan="4">Total</td>
                            <asp:Repeater runat="server" ID="rptTotalColumns">
                                <ItemTemplate>
                                    <td class="numeric">
                                        <%#GetTotal(Eval("ActivityName").ToString()) %>
                                    </td>
                                </ItemTemplate>
                            </asp:Repeater>
                            <td class="numeric">
                                <%#GetTotal("IdleTime")%>
                            </td>
                        </tr>
                    </tfoot>
                    </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>
    </div>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="scripts">
    <script>
        $('.tool-utilization').toolutilization();
    </script>
</asp:Content>
