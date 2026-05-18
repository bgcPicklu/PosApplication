$('#VATPercentage').keyup(function (event) {
    if ($(this).val().length > 0) {
        if (!$.isNumeric($(this).val())) {
            $(this).notify('Please enter only numeric value!', 'warn');
            $(this).val('');
        }
        else if (event.keyCode == 13) {
            $(this).val(parseFloat($(this).val()));
            $('#TotalVatAmount').focus();
        }
    }
});

$('#TotalVatAmount').keyup(function (event) {
    if ($(this).val().length > 0) {
        if (!$.isNumeric($(this).val())) {
            $(this).notify('Please enter only numeric value!', 'warn');
            $(this).val('');
        }
        else if (event.keyCode == 13) {
            $(this).val(parseFloat($(this).val()));
            $('#SupplierId').focus();
        }
    }
});

$('#luQty').keyup(function (event) {
    if ($(this).val().length > 0) {
        if (!$.isNumeric($(this).val())) {
            $(this).notify('Please enter only numeric value!', 'warn');
            $(this).val('');
        }
        else if (event.keyCode == 13) {
            $(this).val(parseFloat($(this).val()));
            $('#suQty').focus();
        }
    }
});

$('#suQty').keyup(function (event) {
    if ($(this).val().length > 0) {
        if (!$.isNumeric($(this).val())) {
            $(this).notify('Please enter only numeric value!', 'warn');
            $(this).val('');
        }
        else if (event.keyCode == 13) {
            $(this).val(parseFloat($(this).val()));
            $('#vAmount').focus();
        }
    }
});

$('#vAmount').keyup(function (event) {
    if ($(this).val().length > 0) {
        if (!$.isNumeric($(this).val())) {
            $(this).notify('Please enter only numeric value!', 'warn');
            $(this).val('');
        }
        else if (event.keyCode == 13) {
            $(this).val(parseFloat($(this).val()));
            $('#discount').focus();
        }
    }
});

$('#discount').keyup(function (event) {
    if ($(this).val().length > 0) {
        if (!$.isNumeric($(this).val())) {
            $(this).notify('Please enter only numeric value!', 'warn');
            $(this).val('');
        }
        else if (event.keyCode == 13) {
            $(this).val(parseFloat($(this).val()));
            $('#btnAddEdit').focus();
        }
    }
});

//function renderDistributionLocation(element, distribution_Location) {
//    var $ele = $(element);
//    $ele.empty();
//    $ele.append($('<option/>').val('0').text('--Select--'));
//    $.each(distribution_Location, function (i, val) {
//        $ele.append($('<option/>').val(val.vDistributionLocationID).text(val.vDistributionLocation));
//    });
//}

//function renderDealerName(element, dealerName) {
//    var $ele = $(element);
//    $ele.empty();
//    $ele.append($('<option/>').val('0').text('--Select--'));
//    $.each(dealerName, function (i, val) {
//        $ele.append($('<option/>').val(val.vDealerID).text(val.vDealerName));
//    });
//}

function renderProductName(element, productName) {
    var $ele = $(element);
    $ele.empty();
    $ele.append($('<option/>').val('0').text('--Select--'));
    $.each(productName, function (i, val) {
        $ele.append($('<option/>').val(val.vProductID).text(val.vProductName));
    });
}

//var duplicateRow = [];
//$('#btnAdd').click(function () {
//    var isAllValid = true;
//    if ($('#productID').val() == "0") {
//        isAllValid = false;
//        $.notify('Select Product Name', 'warn');
//        $('#productID').focus();
//    }
//    else if ($('#quantity').val().trim() == '' || parseInt($('#quantity').val().trim()) <= 0) {
//        isAllValid = false;
//        $.notify('Provide Quantity', 'warn');
//        $('#quantity').focus();
//    }
//    $.each(duplicateRow, function (i, value) {
//        if (value.vProductID == $('#productID').val()) {
//            isAllValid = false;
//            $('#productID').notify('Product already exists in the list.', 'warn');
//            $('#productID').val('0');
//            $('#productUnitID').text('');
//            $('#productUnit').text('');
//            $('#quantity').val('');
//            $('#nAmount').text('');
//            $('#productID').focus();
//        }
//    });
//    if (isAllValid) {
//        var $newRow = $('#mainRow').clone().removeAttr('id');
//        $('#productID', $newRow).addClass('mainProductID').val($('#productID').val());
//        $('#productRate', $newRow).val($('#productRate').val());
//        $('#quantity', $newRow).addClass('mainQuantity').val($('#quantity').val());
//        $('#quantityMinimum', $newRow).val($('#quantityMinimum').val());
//        $('#productUnitID', $newRow).text($('#productUnitID').text());
//        $('#productUnit', $newRow).text($('#productUnit').text());
//        $('#nAmount', $newRow).text($('#nAmount').text());
//        $('#btnAdd', $newRow).addClass('remove').val('Remove').removeClass('btn-success').addClass('btn-danger');
//        $('#productID', '#quantity').removeAttr('id'); duplicateRow.push({ vProductID: $('#productID').val().trim() });
//        $('#productID').val('0'); $('#productUnitID').text('');
//        $('#productUnit').text('');
//        $('#quantity').val('');
//        $('#productRate').text('');
//        $('#quantityMinimum').text('');
//        $('#nAmount').text('');
//        $('#productID', $newRow).prop('disabled', true);
//    }
//    $('#tbProduct').append($newRow);
//});

$('#tbProduct').on('click', '.remove', function () {
    duplicateRow = [];
    var removeAmount = parseFloat($(this).parents('tr').find('label.nAmount').text());
    $(this).parents('tr').remove();
    $('#tbProduct tbody tr').each(function (index, ele) {
        if (index > 0) {
            if ($('#productID', this).val() != "") duplicateRow.push({ vProductID: $('#productID', this).val() });
        }
    });
    var length = duplicateRow.length;
    $('#productID').focus();
    $('#totalAmount').text((parseFloat($('#totalAmount').text()) - removeAmount).toFixed(2));
});

$('#tbProduct').on('keyup', '.mainQuantity', function () {
    var totalAmount = 0.0;
    var i = 0;
    $('#tbProduct tbody tr').each(function (ind, ele) {/*if (ind > 0) {*/
        if (parseFloat($('#productRate', this).text()) > 0.0 && parseFloat($('#quantityMinimum', this).text()) > 0.0 && parseFloat($('#quantity', this).val()) > 0.0) {
            $('#nAmount', this).text((parseFloat($('#productRate', this).text()) * parseFloat($('#quantityMinimum', this).text()) * parseFloat($('#quantity', this).val())).toFixed(2));
            totalAmount += parseFloat($('#productRate', this).text()) * parseFloat($('#quantityMinimum', this).text()) * parseFloat($('#quantity', this).val()); $('#totalAmount').text(totalAmount.toFixed(2));
            i++;
        }
        else {/*totalAmount = parseFloat($('#totalAmount').text());$('#totalAmount').text((totalAmount - parseFloat($('#nAmount',this).text())).toFixed(2));*/
            $('#nAmount', this).text('');
        }/*}*/
        if (i == 0) {
            $('#totalAmount').text('');
        }
    });
});