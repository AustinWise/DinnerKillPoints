"use strict";
$(document).ready(function () {
    console.log('lol');
    $('.date-adjust').each(function (ndx, el) {
        var year = new Number(el.getAttribute('data-date-year'));
        var month = new Number(el.getAttribute('data-date-month')) - 1;
        var day = new Number(el.getAttribute('data-date-day'));
        var hour = new Number(el.getAttribute('data-date-hour'));
        var minute = new Number(el.getAttribute('data-date-minute'));
        var second = new Number(el.getAttribute('data-date-second'));
        var milliOffset = new Number(el.getAttribute('data-milli'));
        //var milliOffset = Date.UTC(year, month, day, hour, minute, second, 0);
        var newDate = new Date(milliOffset);
        el.innerHTML = newDate.toLocaleString();
        //el.innerText = newDate.getDate().toString();
    });
});