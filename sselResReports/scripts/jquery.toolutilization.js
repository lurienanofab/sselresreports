(function ($) {
    $.lnf = {
        round: function (x, digits) {
            var factor = Math.pow(10, digits);
            return Math.round(parseFloat(x) * factor) / factor
        },
        numberWithCommas: function (x) {
            var parts = x.toString().split(".");
            parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, ",");
            return parts.join(".");
        }
    }

    $.fn.toolutilization = function () {
        return this.each(function () {
            var $this = $(this);

            var showPercent = function () {
                return $(".show-percent", $this).val() == "true";
            }

            //converts normal table to DataTable
            $('.tool-utilization-report', $this).dataTable({
                "paging": false,
                "stateSave": true,
                "autoWidth": false,
                "order": [[0, 'asc'], [1, 'asc'], [2, 'asc']],
                "footerCallback": function (tfoot, data, start, end, display) {
                    //display is an arrray of displayed indexes (indexes of data)
                    var numberOfTotalColumns = $('.tool-utilization-report > thead > tr > th', $this).length;
                    var numberOfTextColumns = 3;
                    var numberOfValueColumns = numberOfTotalColumns - numberOfTextColumns;

                    var foot = $(tfoot);
                    var subTotals = [];

                    for (var x = 0; x < numberOfValueColumns; x++)
                        subTotals.push(0);

                    for (var x = start; x < end; x++) {
                        var index = display[x];
                        var item = data[index];
                        for (var c = 0; c < numberOfValueColumns; c++) {
                            var val = 0;
                            var raw = item[c + numberOfTextColumns].replace(",", "");
                            if (raw) val = $.lnf.round(raw, 1);
                            subTotals[c] += val;
                        }
                    }

                    for (var c = 0; c < numberOfValueColumns; c++) {
                        var cell = foot.find('td').eq(c + 1);
                        cell.html('');
                        var v = subTotals[c];
                        if (v != 0) {
                            var num = $.lnf.numberWithCommas(v.toFixed(1));
                            if (showPercent()) {
                                var total = { row: null, cell: null, value: 0 };
                                total.row = foot.closest('tfoot').find("tr.total-row");
                                total.cell = total.row.find('td').eq(c + 1);
                                total.value = parseFloat(total.cell.html().replace(",", ""));
                                var p = v / total.value;
                                var percent = $.lnf.round(p * 100, 1);
                                num += " (" + percent.toFixed(1) + "%)";
                            }
                            cell.html(num);
                        }
                    }
                }
            });

            var toggleIncludeForgivenCheckboxVisibility = function () {
                if ($('.stats-checkbox.charged input').is(':checked')) {
                    $('.include-forgiven', $this).show();
                    $(".include-forgiven-message", $this).hide();
                } else {
                    $('.include-forgiven', $this).hide();
                    $(".include-forgiven-message", $this).show();
                }
            }

            $('.stats-checkbox input', $this).change(function (event) {
                toggleIncludeForgivenCheckboxVisibility();
            });

            toggleIncludeForgivenCheckboxVisibility();
        });
    }
}(jQuery))


// This validate method is used in ResUtil and ResToolUsageSummary pages.
var ValidateAccountTypeCheckList = function (sender, args) {
    var chkControlId = sender.id.replace("validator_", "");
    var options = $("#" + chkControlId).find('input');
    var ischecked = false;
    args.IsValid = false;
    for (i = 0; i < options.length; i++) {
        var opt = options[i];
        if (opt.type == "checkbox") {
            if (opt.checked) {
                ischecked = true;
                args.IsValid = true;
            }
        }
    }
}