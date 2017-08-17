// Write your Javascript code.
"use strict";
$(document).ready(function () {
    $('.date-adjust').each(function (ndx, el) {
        var milliOffset = new Number(el.getAttribute('data-milli'));
        var newDate = new Date(milliOffset);
        el.innerHTML = newDate.toLocaleString();
    });
});
