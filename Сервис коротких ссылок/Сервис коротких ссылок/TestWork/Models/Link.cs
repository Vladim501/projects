using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TestWork.Models;

namespace TestWork.Models
{
    public class Link
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Url]
        public string FullLink { get; set; }
        public string ShortLink { get; set; }
        public int Clicks { get; set; }
        public int UniqueClicks { get; set; }

        public List<IpLink> IpLinks { get; set; }
    }
}
