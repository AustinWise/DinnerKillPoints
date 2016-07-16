using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace DkpWeb.Services
{
    public static class AdjustDateExtenstions
    {
        static DateTime sEpoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static IHtmlContent AdjustDate(this IHtmlHelper html, DateTime utcDate)
        {
            TagBuilder tb = new TagBuilder("span");
            tb.AddCssClass("date-adjust");
            var diff = utcDate.Subtract(sEpoc);
            tb.Attributes.Add("data-milli", ((long)diff.TotalMilliseconds).ToString(CultureInfo.InvariantCulture));
            tb.InnerHtml.SetContent(utcDate.ToString("G"));
            return tb;
        }
    }
}
