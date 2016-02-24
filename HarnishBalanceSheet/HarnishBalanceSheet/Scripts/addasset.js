$(document).ready(function () {   

    $("#btnCancel").click(function () {
        window.returnValue = null;
        self.close();
    });

    $("#btnOK").click(function () {
        var obj = new Object();
        obj.Name = $("#assetName").first().val();
        obj.Value = parseFloat($("#value").first().val());
        obj.Type = $("#assetType option:selected").text();
        var arr = new Array(5);
        for (var i = 0; i < 5; i++) {
            var fract = new Object();
            var el = $(".type").eq(i)[0];
            fract.Type = el.innerHTML;
            el = $(".fract").eq(i)[0];
            fract.Fraction = parseFloat($(el).val()) / 100;
            el = $(".value").eq(i)[0];
            fract.Value = parseFloat($(el).val());
            arr[i] = fract;
        }
        obj.Fract = arr;
        window.returnValue = obj;
        self.close();
    });
});