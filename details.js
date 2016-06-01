$(document).ready(function () {
    $("#datepicker").datepicker({
        onSelect: function (dateText) {
            window.location.href = "../BalanceSheet/Details?date=" + dateText;
        }
    });     // select a previous month's balance sheet

    var array = new Array();
    var dif = $(".diff");
    var index = 0;
    for (var i = 0; i < dif.length; i++) { array[i] = +($(dif[i]).text().split(' ')[0]); }
    var value = array[0];

    for (var i = 1; i < array.length; i++) {
        if (array[i] < value) {
            value = array[i];
            index = i;
        }
    }       // select the asset class which has the lowest % difference from target

    $('.target').eq(index).css("color", "red");

    var lastYear = new Date();
    lastYear.setDate(lastYear.getDate() - 365);
    $.get("../BalanceSheet/Liabilities?start=" + dateString(lastYear) + "&end=" + dateString(new Date()))
    .done(function (d) {
        var returnedData = JSON.parse(d);
        var data = {
            labels: returnedData.labels,
            datasets: [
                {
                    label: "Liabilities",
                    fillColor: "rgba(0,0,220,0.2)",
                    strokeColor: "rgba(0,0,0,1)",
                    pointColor: "rgba(0,0,0,1)",
                    pointStrokeColor: "#fff",
                    pointHighlightFill: "#fff",
                    pointHighlightStroke: "rgba(0,0,0,1)",
                    data: returnedData.data
                }
            ]
        };
        var ctx = $("#chart1").get(0).getContext("2d");
        var options = null;
        var lChart = new Chart(ctx).Line(data, options);
    });     // fill liabilities line graph

    $.get("../BalanceSheet/NetWorth?start=" + dateString(lastYear) + "&end=" + dateString(new Date()))
    .done(function (d) {
        var returnedData = JSON.parse(d);
        var data = {
            labels: returnedData.labels,
            datasets: [
                {
                    label: "Net Worth",
                    fillColor: "rgba(220,0,0,0.2)",
                    strokeColor: "rgba(0,0,0,1)",
                    pointColor: "rgba(0,0,0,1)",
                    pointStrokeColor: "#fff",
                    pointHighlightFill: "#fff",
                    pointHighlightStroke: "rgba(0,0,0,1)",
                    data: returnedData.data
                }
            ]
        };
        var ctx = $("#chart2").get(0).getContext("2d");
        var options = null;
        var lChart = new Chart(ctx).Line(data, options);
    });     // fill net worth chart

    

});

function dateString(date){
    return (date.getMonth() + 1).toString() + "/" + date.getDate().toString() + "/" + date.getFullYear();
}