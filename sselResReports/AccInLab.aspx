<%@ Page Title="" Language="C#" MasterPageFile="~/ResReportsMaster.Master" AutoEventWireup="true" CodeBehind="AccInLab.aspx.cs" Inherits="sselResReports.AccInLab" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style type="text/css">
        .table-container {
            font-family: Arial;
            font-size: 10pt;
        }

        .datatable {
            width: 100%;
        }
    </style>

    <script type="text/javascript">
        $(document).ready(function () {
            $(".datatable").dataTable({
                "columns": [
                    null,
                    { 'width': '180px' },
                    { 'width': '200px' },
                    { 'width': '110px' },
                ],
                "paging": false
            });
        });
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Users currently in lab</h2>
        <h4>All the users below are in the lab at the time:&nbsp;
            <asp:Label ID="lblTime" runat="server"></asp:Label></h4>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Select room:</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlRoom" Height="24" AutoPostBack="true" OnSelectedIndexChanged="DdlRoom_SelectedIndexChanged" DataTextField="Room">
                        </asp:DropDownList>
                        <span style="color: #BBBBBB;">&larr; Retrieve the data by changing room</span>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:Button runat="server" ID="btnReport" Text="Refresh" OnClick="BtnReport_Click" CssClass="report-button" />
            </div>
            <div class="criteria-item">
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">
        <div style="width: 800px;" class="table-container">
            <asp:Repeater runat="server" ID="rptUsers">
                <HeaderTemplate>
                    <table class="datatable">
                        <thead>
                            <tr>
                                <th>Last Name</th>
                                <th>First Name</th>
                                <th>Time of Entrance</th>
                                <th>Time in Room</th>
                            </tr>
                        </thead>
                        <tbody>
                </HeaderTemplate>
                <ItemTemplate>
                    <tr>
                        <td><%#GetValue(Eval("LastName"), "{0}", "Error") %></td>
                        <td><%#GetValue(Eval("FirstName"), "{0}", "Error") %></td>
                        <td><%#GetValue(Eval("EventDateTime"), "{0:h:mm tt on d MMMM yyyy}", "Error") %></td>
                        <td style="text-align: center;"><%#GetValue(Eval("Duration"), "{0:#.00 hours}", "Error") %></td>
                    </tr>
                </ItemTemplate>
                <FooterTemplate>
                    </tbody>
                </table>
                </FooterTemplate>
            </asp:Repeater>
        </div>

        <asp:GridView ID="gvUsers" runat="server" AutoGenerateColumns="false" AllowSorting="false" GridLines="None" CssClass="data-table" Visible="false">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <EmptyDataTemplate>
                -- No users in this room at this time --
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField HeaderText="User Name" DataField="FullName" NullDisplayText="Error" ItemStyle-Width="180" ItemStyle-HorizontalAlign="Center" SortExpression="FullName" />
                <asp:BoundField HeaderText="Time of Entrance" DataField="EventDateTime" ItemStyle-Width="200" DataFormatString="{0: h:mm tt on d MMMM yyyy}" NullDisplayText="Error" ItemStyle-HorizontalAlign="Center" SortExpression="EventTime" HtmlEncode="false" />
                <asp:BoundField HeaderText="Access Method" DataField="AccessMethod" NullDisplayText="N/A" ItemStyle-Width="130" ItemStyle-HorizontalAlign="Center" SortExpression="CardNumber" />
                <asp:BoundField HeaderText="Time in Room" DataField="Duration" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="110" DataFormatString="{0: #.00 hours}" SortExpression="Duration" HtmlEncode="false" />
            </Columns>
        </asp:GridView>
    </div>
</asp:Content>
