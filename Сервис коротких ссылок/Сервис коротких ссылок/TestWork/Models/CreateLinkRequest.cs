using TestWork.Models;
using System.ComponentModel.DataAnnotations;

namespace TestWork.Models
{
    public class CreateLinkRequest
    {
        [Url]
        [Required]
        public string FullLink { get; set; }

        public Link GetLink()
        {
            var link = new Link
            {
                FullLink = FullLink
            };

            return link;
        }
    }
}
