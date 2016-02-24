$(document).ready(function () {
    $('a.edit').click(function () {
        var hidden = $(this).parent().first().next().children()[1];
        var retValue = showModalDialog("../editasset.html", $(hidden).val(), "dialogWidth:450px; dialogHeight:225px;");

        if (retValue != null) {
            $(hidden).val(JSON.stringify(retValue));
        }
    });
    $('#editcoins').click(function () {
        var hidden = $(this).parent().first().next().children()[2];
        var retValue = showModalDialog("../editcoins.html", $(hidden).val(), "dialogWidth:375px; dialogHeight:225px;");

        if (retValue != null) {
            $(hidden).val(JSON.stringify(retValue));
        }
    });
    $('#addAsset').click(function () {
        var retValue = showModalDialog("../addasset.html", null, "dialogWidth:650px;dialogHeight:275px;");

        if (retValue != null) {
            $("<tr><td class='hundred'><label for='" + retValue.Name + "'>" + retValue.Name + "</label>"
                + "<input id='asset_Name' name='asset.Name' type='hidden' value='" + retValue.Name + "'/></td>"
                + "<td class='right'><input id='asset_FormattedValue' name='asset.FormattedValue' style='text-align:right' type='text' value='" + retValue.Value.toFixed(2) + "'/></td>"
                + "<td>&nbsp;&nbsp;<a href='#' class='edit'>Edit</a></td>"
                + "<td><input id='asset_Type' name='asset.Type' type='hidden' value='" + retValue.Type + "'/>"
                + "<input id='asset_FormattedFractions' name='asset.FormattedFractions' type='hidden' value='" + JSON.stringify(retValue.Fract) + "'/>"
                + "<input id='asset_FormattedElements' name='asset.FormattedElements' type='hidden' value='null' /></td></tr>").insertBefore("#rwAddAsset");
        }
    });
    $('#addLiability').click(function () {
        var retValue = showModalDialog("../addliability.html", null, "dialogWidth:500px;dialogHeight:125px;");

        if (retValue != null) {
            $("<tr><td class='hundred'><label for='" + retValue.Name + "'>" + retValue.Name + "</label>"
                + "<input id='liability_Name' name='liability.Name' type='hidden' value='" + retValue.Name + "'/></td>"
                + "<td class='right'><input id='liability_Value' name='liability.Value' style='text-align:right' type='text' value='" + retValue.Value.toFixed(2) + "'/></td></tr>").insertBefore("#rwAddLiability");
        }
    });
});
