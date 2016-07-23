using Microsoft.AspNetCore.Html;
using System;
using System.Globalization;

namespace Microsoft.AspNetCore.Mvc.Rendering
{
    //TODO: move this file to a better place
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
