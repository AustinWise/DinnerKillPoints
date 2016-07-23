using System;
using System.Net;

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
                    return WebUtility.UrlEncode(mStr);
                default:
                    return mStr;
            }
        }
    }
}
