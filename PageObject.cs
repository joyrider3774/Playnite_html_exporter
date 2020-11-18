using System.Security.Permissions;
using Newtonsoft.Json;

namespace HtmlExporterPlugin
{
    public class PageObject
    {
        public string Pagetitle { get; set; }
        [JsonIgnore]
        public string Pagefilename
        {
            get =>
                Groupfield == Constants.NotGroupedField ?
                Templatefoldername + "_" + (Sortfield.ToLower() + "_" + (SortAscending ? Constants.AscendingText : Constants.DescendingText) + ".html") :
                (Groupfield == Constants.NameField ? "index.html" : Templatefoldername + "_" + Groupfield.ToLower() + "_" + (GroupAscending ? Constants.AscendingText : Constants.DescendingText) + "_" +
                Sortfield.ToLower() + "_" + (SortAscending ? Constants.AscendingText : Constants.DescendingText) + ".html");
        }
        public string Templatefoldername { get; set; }
        public string Groupfield { get; set; }
        public bool GroupAscending { get; set; }
        public string Sortfield { get; set; }
        public bool SortAscending { get; set; }

        public PageObject(string apagetitle, string atemplatefoldername, string agroupfield,
            bool agroupascending, string asortfield, bool aSortAscending)
        {
            Pagetitle = apagetitle;
            Templatefoldername = atemplatefoldername;
            Groupfield = agroupfield;
            Sortfield = asortfield;
            SortAscending = aSortAscending;
            GroupAscending = agroupascending;
        }
    }

}
