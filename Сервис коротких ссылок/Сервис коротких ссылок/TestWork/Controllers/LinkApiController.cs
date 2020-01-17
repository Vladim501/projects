using TestWork.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using TestWork.Interfaces;


namespace TestWork.Controllers
{


    [Produces("application/json")]
    [Route("api/LinkApi")]
    public class LinkApiController : Controller
    {
        private ILinksRepository _repository;
        // количество ссылок на странице
        private int itemPerPage = 10;

        public LinkApiController(ILinksRepository linksRepository)
        {
            _repository = linksRepository;
        }

        
        [HttpGet]
        public IActionResult Get([FromQuery] int page = 1)
        {
            var (links, count) = _repository
                                 .Get((page - 1) * itemPerPage);

            var result = new QueryResult
            {

                PageInfo = new PageInfo
                {
                    CurrentPage = page,
                    MaxPage = count % itemPerPage == 0 ? count / itemPerPage : count / itemPerPage + 1
                },
                Items = links.Select(x => new LinkResult(x))
            };

            return Ok(result);
        }

        
        [HttpPost]
        public IActionResult Post([FromBody]CreateLinkRequest createLink)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(@"Не верный формат ссылки, используйте http:// или https://");
            }

            _repository.AddLink(createLink.GetLink());
            return Ok();
        }

       
        [HttpPut]
        public IActionResult Put([FromBody]Link link)
        {
            _repository.Update(link);
            return Ok();
        }

      
        [Route("{id}")]
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            _repository.Delete(id);
            return new NoContentResult();
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult GetSingleLink(int id)
        {
            return Ok(_repository.GetSingleLink(id));
        }
    }
}
