using FutuEvents.Data;
using FutuEvents.Models.ApiModels;
using FutuEvents.Models.DbModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FutuEvents.Services;

namespace FutuEvents.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        private readonly ApiContext _context;

        public EventController(ApiContext context)
        {
            _context = context;
        }

        // Get
        [Route("list")]
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
        public JsonResult Create(ApiCreateFutuEvent futuEvent)
        {
            var id = EventService.CreateEvent(_context, futuEvent);

            if (id == null)
            {
                return new JsonResult(BadRequest("Saving the event failed. Check the sent event info"));
            }

            return new JsonResult(Ok($"id:  { id }"));
        }

        // Get one event
        [Route("{id}")]
        [HttpGet]
        public JsonResult Get(long id)
        {
            var result = EventService.GetFutuEvent(_context, id);

            if (result == null)
            {
                return new JsonResult(NotFound("There were no events to return"));
            }

            return new JsonResult(Ok(result));
        }

        // Add vote for an event
        [Route("{id}/vote")]
        [HttpPost]
        public JsonResult Vote(long id, ApiCreateVote vote)
        {
            var result = EventService.AddVote(_context, id, vote);

            if (result == null)
            {
                return new JsonResult(BadRequest("There was error in adding the vote"));
            }

            return new JsonResult(Ok(result));
        }

        // Show results of an event
        [Route("{id}/results")]
        [HttpGet]
        public JsonResult Results(long id)
        {
            var result = EventService.GetResult(_context, id);

            if (result == null)
            {
                return new JsonResult(NotFound("No event was found"));
            }

            return new JsonResult(Ok(result));
        }
    }
}
