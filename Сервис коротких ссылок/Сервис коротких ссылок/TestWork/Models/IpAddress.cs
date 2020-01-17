using System.Collections.Generic;

namespace TestWork.Models
{
    public class IpAddress
    {
        public int Id { get; set; }
        public string ClientIp { get; set; }

        public List<IpLink> IpLinks { get; set; }
    }
}
