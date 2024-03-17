using FutuEvents.Data;
using FutuEvents.Models.ApiModels;
using FutuEvents.Models.DbModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FutuEvents.Services;

namespace FutuEvents.Controllers.v1
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ApiContext _context;

        public EventController(ApiContext context)
        {
            _context = context;
        }

        // Get
        [HttpGet]
        public JsonResult Get()
        {
            var result = _context.FutuEvents;

            if (result == null)
            {
                return new JsonResult(NotFound("There were no events to return"));
            }

            return new JsonResult(Ok(result));
        }

        // Create
        [HttpPost]
        public JsonResult Create(ApiFutuEvent futuEvent)
        {
            var id = EventService.CreateEvent(_context, futuEvent);

            if (id == null)
            {
                return new JsonResult(BadRequest("Saving the event failed. Check the sent event info"));
            }

            return new JsonResult(Ok($"id:  { id }"));
        }
    }
}
