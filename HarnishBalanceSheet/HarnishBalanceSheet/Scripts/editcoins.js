$(document).ready(function () {
    var data = JSON.parse(window.dialogArguments);

    $.each(data, function (index, value) {
        $("table thead").append("<tr><td class='metal'>" + value.Element + "</td><td><input class='numounces' style='text-align:right;width:75px' type='text' value='"
            + value.Ounces
            + "'/></td><td><input class='price' style='text-align:right;width:110px' type='text' value='"
            + value.Price.toFixed(2)
            + "'/></td><td class='total' style='text-align:right'>" + value.TotalValue.toFixed(2) + "</td></tr>");
    });    

    $("#btnCancel").click(function () {
        window.returnValue = null;
        self.close();
    });

    $("#btnOK").click(function () {
        var arr = new Array(5);
        for (var i = 0; i < 5; i++) {
            var obj = new Object();
            var el = $(".metal").eq(i)[0];
            obj.Element = el.innerHTML;
            el = $(".numounces").eq(i)[0];
            obj.Ounces = parseFloat($(el).val());
            el = $(".price").eq(i)[0];
            obj.Price = parseFloat($(el).val());
            el = $(".total").eq(i)[0];
            obj.TotalValue = parseFloat(el.innerHTML);
            arr[i] = obj;
        }
        window.returnValue = arr;
        self.close();
    });
});