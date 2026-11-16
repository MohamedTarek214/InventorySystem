// Client-side validation rules that DataAnnotations cannot generate on their own.
// They mirror the server-side rules (NotInThePastAttribute and the Grocery rule in ProductService).
$(function () {
    if (!$.validator) {
        return;
    }

    $.validator.addMethod("notinthepast", function (value) {
        if (!value) {
            return true;
        }

        // <input type="date"> always gives yyyy-MM-dd.
        var parts = value.split("-");
        var chosen = new Date(parts[0], parts[1] - 1, parts[2]);
        var today = new Date();
        today.setHours(0, 0, 0, 0);

        return chosen >= today;
    }, "Expiry date cannot be in the past.");

    $.validator.addMethod("requiredforgrocery", function (value) {
        return $("#Category").val() !== "Grocery" || !!value;
    }, "Expiry date is required for Grocery products.");

    var $expiry = $("#ExpiryDate");
    $expiry.rules("add", { notinthepast: true, requiredforgrocery: true });

    // Re-check the expiry date as soon as the category changes.
    $("#Category").on("change", function () {
        $expiry.valid();
    });
});
