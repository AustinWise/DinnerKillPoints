// Write your Javascript code.
"use strict";
function ready(fn) {
    if (document.readyState !== 'loading') {
        fn();
    } else {
        document.addEventListener('DOMContentLoaded', fn);
    }
}
ready(function () {
    document.querySelectorAll('.date-adjust').forEach(el => {
        var milliOffset = new Number(el.getAttribute('data-milli'));
        var newDate = new Date(milliOffset);
        el.innerHTML = newDate.toLocaleString();
    });
});
