using TestWork.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TestWork.Interfaces;


namespace TestWork.Controllers
{
    public class LinkController : Controller
    {
        private ILinksRepository _repository;
        private int itemPerPage = 10;

        public LinkController(ILinksRepository linksRepository)
        {
            _repository = linksRepository;
        }

        [HttpGet]
        public IActionResult Index([FromQuery] int page = 1)
        {
            var (links, count) = _repository.Get((page - 1) * itemPerPage);

            var result = new QueryResult
            {

                PageInfo = new PageInfo
                {
                    CurrentPage = page,
                    MaxPage = count % itemPerPage == 0 ? count / itemPerPage : count / itemPerPage + 1
                },
                Items = links.Select(x => new LinkResult(x))
            };

            return View(links.Select(x => x).ToList());
        }

        [HttpPost]
        public IActionResult Create(Link link)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "<script>alert('Ссылка  имеет не корректный вид, используйте http:// или https://');</script>";
                return Redirect("Index");
            }

            _repository.AddLink(link);
            return Redirect("Index");
        }

        [HttpGet]
        public IActionResult Delete(Link link)
        {
            var linkId = link.Id;
            _repository.Delete(linkId);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(Link link)
        {
            return View(link);
        }

        [HttpPost]
        public IActionResult Update(Link link)
        {
            if (!ModelState.IsValid)
            {
                TempData["msg"] = "<script>alert('Ссылка  имеет не корректный вид, используйте http:// или https://');</script>";
                return Redirect("Index");
            }

            _repository.Update(link);

            return Redirect("Index");
        }

    }
}