using TestWork.Models;
using System.Collections.Generic;

namespace TestWork.Models
{
    public class QueryResult
    {
        public IEnumerable<LinkResult> Items { get; set; }
        public PageInfo PageInfo { get; set; }
    }

    public class PageInfo
    {
        public int CurrentPage { get; set; }
        public int MaxPage { get; set; }

    }

    public class LinkResult
    {
        public LinkResult(Link link)
        {
            Id = link.Id;
            FullLink = link.FullLink;
            ShortLink = link.ShortLink;
            Clicks = link.Clicks;
            UniqueClicks = link.UniqueClicks;

        }

        public int Id { get; set; }
        public string FullLink { get; set; }
        public string ShortLink { get; set; }
        public int Clicks { get; set; }
        public int UniqueClicks { get; set; }
    }
}
