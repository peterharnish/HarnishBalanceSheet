$(document).ready(function () {   

    $("#btnCancel").click(function () {
        window.returnValue = null;
        self.close();
    });

    $("#btnOK").click(function () {
        var obj = new Object();
        obj.Name = $("#liabilityName").first().val();
        obj.Value = parseFloat($("#value").first().val());
        window.returnValue = obj;
        self.close();
    });
});