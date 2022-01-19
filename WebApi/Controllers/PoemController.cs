using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Services;
using Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    [Route("api/poems")]
    [ApiController]
    public class PoemController : WebApiController
    {
        private readonly IPoemService _poemService;

        public PoemController(IHostEnvironment hostEnvironment, IPoemService poemService) : base(hostEnvironment)
        {
            _poemService = poemService;
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> AddPoem(PoemDto poem)
        {
             var result = await _poemService.AddPoem(poem, UserId);
            return Ok(result);
        }

        [HttpGet]
        [Route("{poemId}")]
        public  ActionResult<PoemDto> GetPoem(Guid poemId)
        {
            var result =  _poemService.GetPoem(poemId);
            return result;
        }

        [HttpGet]
        [Route("")]
        public ActionResult<List<PoemDto>> GetPoems()
        {
            var result = _poemService.GetPoems();
            return result;
        }

        [HttpDelete]
        [Route("{poemId}")]
        public async Task<IActionResult> DeletePoem(Guid poemId)
        {
            var result = await _poemService.DeletePoem(poemId);
            return Ok(result);
        }
    }
}
