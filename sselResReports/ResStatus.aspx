<%@ Page Title="Tool Status" Language="C#" MasterPageFile="~/ResReportsMaster.Master" AutoEventWireup="true" CodeBehind="ResStatus.aspx.cs" Inherits="sselResReports.ResStatus" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .tool-status {
            border-collapse: collapse;
        }

            .tool-status td {
                border: solid 1px #000;
                padding: 3px;
                height: 18px;
            }

                .tool-status td.proctech {
                    padding-left: 20px;
                }

                .tool-status td.resource {
                    padding-left: 40px;
                }

                .tool-status td.status {
                    text-align: center;
                }

                    .tool-status td.status.pending {
                        color: #999;
                        font-style: italic;
                    }

                    .tool-status td.status.tool-on {
                        color: #008000;
                    }

                    .tool-status td.status.tool-off {
                        color: #800000;
                    }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="section">
        <h2>Current Tool Status</h2>
        <div class="criteria">
            <table class="criteria-table">
                <tr>
                    <td>Report type:</td>
                    <td>
                        <asp:DropDownList runat="server" ID="ddlReportType" Height="24" AutoPostBack="true" OnSelectedIndexChanged="DdlReportType_SelectedIndexChanged">
                            <asp:ListItem Value="Current" Text="Current Tool Status"></asp:ListItem>
                            <asp:ListItem Value="Future" Text="Tool Repair/Maintenance 7 Days In Advance"></asp:ListItem>
                        </asp:DropDownList>
                        <span style="color: #BBBBBB;">&larr; Retrieve data by selecting report type</span>
                    </td>
                </tr>
            </table>
            <div class="criteria-item">
                <asp:LinkButton runat="server" ID="btnBack" Text="&larr; Back to Main Page" OnClick="BackButton_Click" CssClass="back-link"></asp:LinkButton>
            </div>
        </div>
    </div>
    <div class="section">

        <asp:Table runat="server" ID="tblToolStatus" CssClass="tool-status outline">
        </asp:Table>

        <asp:DataGrid runat="server" ID="dgToolStatus" AutoGenerateColumns="false" CssClass="outline" OnItemDataBound="DgToolStatus_ItemDataBound">
            <HeaderStyle CssClass="GridHeader" />
            <Columns>
                <asp:TemplateColumn HeaderText="Resource Name">
                    <ItemStyle Width="350" />
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblResourceName"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Status">
                    <ItemStyle Width="75" />
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblStatus"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Current Acitivity">
                    <ItemStyle Width="350" />
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblActivity"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
                <asp:TemplateColumn HeaderText="Interlock State">
                    <ItemStyle Width="400" />
                    <ItemTemplate>
                        <asp:Label runat="server" ID="lblInterlock"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateColumn>
            </Columns>
        </asp:DataGrid>
        <asp:GridView ID="gvFuture" runat="server" AutoGenerateColumns="false" GridLines="None" CssClass="gridview highlight">
            <HeaderStyle CssClass="header" />
            <RowStyle CssClass="row" />
            <AlternatingRowStyle CssClass="altrow" />
            <EmptyDataTemplate>
                -- No repair or maintenance in this period --
            </EmptyDataTemplate>
            <Columns>
                <asp:BoundField HeaderText="Tool" DataField="ResourceName" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField HeaderText="Activity" DataField="ActivityName" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField HeaderText="Tool Engineer" DataField="ToolEngineer" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField HeaderText="Begin DateTime" DataField="BeginDateTime" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField HeaderText="End DateTime" DataField="EndDateTime" ItemStyle-HorizontalAlign="Center" />
                <asp:BoundField HeaderText="Duration (hr)" DataField="Hours" ItemStyle-HorizontalAlign="Center" DataFormatString="{0:0.00}" HtmlEncode="false" />
                <asp:BoundField HeaderText="Duration (min)" DataField="Minutes" ItemStyle-HorizontalAlign="Center" />
            </Columns>
        </asp:GridView>
    </div>

    <script>
        var getBlocks = function () {
            $.ajax({
                "url": "ajax/?command=blocks"
            }).done(function (data) {
                $("td.status").addClass("pending").removeClass("tool-on tool-off");

                $.each(data.Points, function (i, point) {
                    var td = $("td.status[data-point='" + point.PointID + "']");
                    if (point.InterlockState){
                        td.html("Tool On");
                        td.addClass("tool-on");
                        td.removeClass("tool-off");
                    }else{
                        td.html("Tool Off");
                        td.addClass("tool-off");
                        td.removeClass("tool-on");
                    }
                    td.removeClass("pending");
                });

                $("td.status.pending").html("No Interlock");
            });
        };

        getBlocks();
    </script>
</asp:Content>
