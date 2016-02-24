$(document).ready(function () {
    var data = JSON.parse(window.dialogArguments);
    AddRow(data, "Cash");
    AddRow(data, "Precious Metals");
    AddRow(data, "Real Estate");
    AddRow(data, "Bonds");
    AddRow(data, "Stocks");

    $("#btnCancel").click(function () {
        window.returnValue = null;
        self.close();
    });

    $("#btnOK").click(function () {
        var arr = new Array(5);
        for (var i = 0; i < 5; i++){
            var obj = new Object();
            var el = $(".type").eq(i)[0];
            obj.Type = el.innerHTML;
            el = $(".fract").eq(i)[0];
            obj.Fraction = parseFloat($(el).val()) / 100;
            el = $(".value").eq(i)[0];
            obj.Value = parseFloat($(el).val());
            arr[i] = obj;
        }
        window.returnValue = arr;
        self.close();
    });
});

function AddRow(data, assetType) {
    var fract = null;
    for (var i = 0; i < data.length; i++) {
        if (data[i].Type == assetType) fract = data[i];
    }

    if (fract == null) {
        fract = new Object();
        fract.Type = assetType;
        fract.Fraction = 0;
        fract.Value = 0;
    }

    $("table thead").append("<tr><td class='type'>" + fract.Type + "</td><td><input class='fract' style='text-align:right' type='text' value='" 
        + (fract.Fraction * 100).toString()
        + "%' /></td><td><input class='value' style='text-align:right' type='text' value='"
        + fract.Value.toFixed(2)
        +"'/></td></tr>");
}
