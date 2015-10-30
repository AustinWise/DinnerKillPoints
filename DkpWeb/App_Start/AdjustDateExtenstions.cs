using System;
using System.Globalization;
using System.Web;
using System.Web.Mvc;

namespace DkpWeb
{
    //TODO: is the App_Startup folder really the right place for this?
    public static class AdjustDateExtenstions
    {
        static DateTime sEpoc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static HtmlString AdjustDate(this HtmlHelper html, DateTime utcDate)
        {
            TagBuilder tb = new TagBuilder("span");
            tb.AddCssClass("date-adjust");
            var diff = utcDate.Subtract(sEpoc);
            tb.Attributes.Add("data-milli", ((long)diff.TotalMilliseconds).ToString(CultureInfo.InvariantCulture));
            tb.SetInnerText(utcDate.ToString("G"));
            return new HtmlString(tb.ToString(TagRenderMode.Normal));
        }
    }
}
