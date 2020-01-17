using TestWork.Models;
using System.Collections.Generic;

namespace TestWork.Interfaces
{
    public interface ILinksRepository
    {
        (IEnumerable<Link>, int) Get(int skip);
        void AddLink(Link link);
        void Delete(int linkId);
        void Update(Link link);
        string RedirectLink(string id);
        void SaveLinkClick(string id, string clientIpAddress);
        object GetSingleLink(int id);
    }
}
