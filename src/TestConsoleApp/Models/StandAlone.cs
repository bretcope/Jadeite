using System.Collections.Generic;

namespace TestConsoleApp.Models
{
    public class NavItem
    {
        public string Name { get; set; }
        public string Url { get; set; }
    }

    public class StandAlone
    {
        public string SiteName { get; set; }
        public string Title { get; set; }
        public List<string> Scripts { get; set; }
        public List<NavItem> NavItems { get; set; }
        public Dictionary<string, string> UnsortedStuff { get; set; }
    }
}