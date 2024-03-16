using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FutuEvents.Data;
using FutuEvents.Models.DbModels;

namespace FutuJunior.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FutuEventController : ControllerBase
    {
        private readonly ApiContext _context;

        public FutuEventController(ApiContext context)
        {
            _context = context;
        }

        // Create
        [HttpPost]
        public JsonResult Create(FutuEvent futuEvent)
        {
            if (futuEvent.Id == 0)
            {
                _context.FutuEvents.Add(futuEvent);
            }

            _context.SaveChanges();

            return new JsonResult(Ok(futuEvent.Id));
        }

        // Get
        [HttpGet]
        public JsonResult Get()
        {
            var result = _context.FutuEvents;

            var testi = result.ToList();

            if (result == null)
            {
                return new JsonResult(NotFound("There were no events to return"));
            }

            return new JsonResult(Ok(result));
        }
    }
}
