using TestWork.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestWork.Models
{
    public class IpLink
    {
        public int LinkId { get; set; }
        public Link Link { get; set; }

        public int IpAddressId { get; set; }
        public IpAddress IpAddress { get; set; }

    }
}
