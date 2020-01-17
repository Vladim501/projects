using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestWork.Interfaces;

namespace TestWork.Controllers
{
    public class RedirectController : Controller
    {

        private ILinksRepository _repository;
        private IHttpContextAccessor _accessor;


        public RedirectController(ILinksRepository linksRepository, IHttpContextAccessor accessor)
        {
            _repository = linksRepository;
            _accessor = accessor;
        }

        [HttpGet("/{id}")]
        public IActionResult Index(string id)
        {

            var linkToRedirect = _repository.RedirectLink(id);
            if (linkToRedirect != "index")
            {
                string clientIpAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();

                _repository.SaveLinkClick(id, clientIpAddress);
            }
            return Redirect(linkToRedirect);
        }
    }
}