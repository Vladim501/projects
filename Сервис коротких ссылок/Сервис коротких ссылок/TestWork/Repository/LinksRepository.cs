using HashidsNet;
using TestWork.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using TestWork;
using TestWork.Interfaces;

namespace TestWork.Repository
{
    public class LinksRepository : ILinksRepository
    {

        private Hashids hashids = new Hashids("From Vladimir", 4);
        private readonly LinkDbContext _context;
        // количество ссылок на страницу
        private readonly int linksPerPage = 10;


        public LinksRepository(LinkDbContext context)
        {
            _context = context;
        }

        private static int TimeSinceMidnight()
        {
            DateTime dateTime = DateTime.Now;
            int timeMsSinceMidnight = (int)dateTime.TimeOfDay.TotalMilliseconds;
            return timeMsSinceMidnight;
        }

        public (IEnumerable<Link>, int) Get(int skip)
        {
            var links = _context.Links;
            var linksCount = links.Count();

            var paginatedLinks = links
                .OrderBy(x => x.Id)
                .Skip(skip)
                .Take(linksPerPage);

            return (paginatedLinks, linksCount);
        }

        public void AddLink(Link link)
        {
            link.ShortLink = hashids.Encode(TimeSinceMidnight());
            _context.Links.Add(link);
            _context.SaveChanges();
        }

        public void Delete(int linkId)
        {
            Link linkEntity = _context.Links.Find(linkId);
            _context.Links.Remove(linkEntity);
            _context.SaveChanges();

        }

        public void Update(Link link)
        {
            _context.Links.Attach(link);
            _context.Entry(link).State = EntityState.Modified;
            _context.SaveChanges();
        }

        public string RedirectLink(string id)
        {
            Link linkToRedirect = GetLinkFromShortLink(id);

            if (linkToRedirect != null)
            {
                return linkToRedirect.FullLink;
            }

            return "index";


        }

        private Link GetLinkFromShortLink(string id)
        {
            return _context.Links
                .Where(x => x.ShortLink == id)
                .FirstOrDefault();
        }

        public void SaveLinkClick(string id, string clientIpAddress)
        {
            var link = GetLinkFromShortLink(id);
            link.Clicks++;

            IpAddress ipAddress = GetIpAddressFromClientIp(clientIpAddress);

            bool wasLinkClickFromIp = _context.IpLink
                .Where(i => i.IpAddress.ClientIp == clientIpAddress && i.LinkId == link.Id)
                .Count() > 0;

            if (!wasLinkClickFromIp)
            {
                link.UniqueClicks++;

                var ipLinkToAdd = new IpLink
                {
                    IpAddress = ipAddress,
                    Link = link
                };

                link.IpLinks = new List<IpLink>
                {
                    ipLinkToAdd
                };

            }

            _context.Links.Attach(link);
            _context.Entry(link).State = EntityState.Modified;
            _context.SaveChanges();

        }

        private IpAddress GetIpAddressFromClientIp(string clientIpAddress)
        {

            var ipAddress = _context.IpAddresses
                .Where(x => x.ClientIp == clientIpAddress)
                .FirstOrDefault();

            if (ipAddress == null)
            {
                ipAddress = new IpAddress { ClientIp = clientIpAddress };
                _context.IpAddresses.Add(ipAddress);
            }

            return ipAddress;
        }

        public object GetSingleLink(int id)
        {
            return _context.Links.FirstOrDefault(l => l.Id == id);
        }
    }
}
