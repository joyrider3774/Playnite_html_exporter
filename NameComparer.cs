using System;
using System.Collections.Generic;
using System.Linq;

namespace HtmlExporterPlugin
{
    public class NameComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return String.Compare(Convert(x), Convert(y), StringComparison.OrdinalIgnoreCase);
        }

        public string Convert(string value)
        {
            string newvalue = value.Trim();
            if (String.IsNullOrEmpty(newvalue))
            {
                return "1";
            }
            else
            {
                var firstChar = Char.ToUpper(newvalue[0]);
                return Char.IsLetter(firstChar) ? "2" + newvalue : "1" + newvalue;
            }
        }
    }
}
