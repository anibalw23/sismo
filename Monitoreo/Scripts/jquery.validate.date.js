$(function() {
    $.validator.methods.date = function (value, element) {
        return this.optional(element) || true;//!/Invalid|NaN/.test(new Date(value).toString()); No Validation Made
    };
});