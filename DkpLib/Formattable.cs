using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Austin.DkpLib
{
    public class Formattable : IFormattable
    {
        readonly string mStr;

        public Formattable(string str)
        {
            mStr = str;
        }

        public override string ToString()
        {
            return mStr;
        }

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                format = string.Empty;
            else
                format = format.ToLowerInvariant();

            switch (format)
            {
                case "urlencode":
                    return HttpUtility.UrlEncode(mStr);
                default:
                    return mStr;
            }
        }
    }
}
