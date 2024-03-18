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
            try
            {
                var result = _context.FutuEvents;

                if (result == null)
                {
                    return new JsonResult(NotFound("There were no events to return"));
                }

                return new JsonResult(Ok(result));
            }
            catch (Exception ex)
            {

                return new JsonResult(BadRequest($"There was an error getting the event list. {ex.Message}"));
            }
            
        }

        // Create
        [HttpPost]
        public JsonResult Create(ApiCreateFutuEvent futuEvent)
        {
            if (futuEvent == null)
            {
                return new JsonResult(BadRequest("The event info is null. There is nothing to save."));
            }

            try
            {
                var createdEvent = EventService.CreateEvent(_context, futuEvent);

                ApiEventBase result = new ApiEventBase { Id = createdEvent.Id };

                return new JsonResult(Ok(result));
            }
            catch (Exception ex)
            {

                return new JsonResult(BadRequest($"There was an error creating the event. {ex.Message}"));
            }
        }

        // Get one event
        [Route("{id}")]
        [HttpGet]
        public JsonResult Get(long id)
        {
            try
            {
                var result = EventService.GetFutuEvent(_context, id);

                if (result == null)
                {
                    return new JsonResult(NotFound("There were no events to return"));
                }

                return new JsonResult(Ok(result));
            }
            catch (Exception ex)
            {

                return new JsonResult(BadRequest($"There was an error getting the event. {ex.Message}"));
            }
        }

        // Add vote for an event
        [Route("{id}/vote")]
        [HttpPost]
        public JsonResult Vote(long id, ApiCreateVote vote)
        {
            try
            {
                var result = EventService.AddVote(_context, id, vote);

                if (result == null)
                {
                    return new JsonResult(BadRequest("There was error in adding the vote"));
                }

                return new JsonResult(Ok(result));
            }
            catch (Exception ex)
            {
                return new JsonResult(BadRequest($"There was an error adding the vote. {ex.Message}"));
            }
            
        }

        // Show results of an event
        [Route("{id}/results")]
        [HttpGet]
        public JsonResult Results(long id)
        {
            try
            {
                var result = EventService.GetResult(_context, id);

                if (result == null)
                {
                    return new JsonResult(NotFound("No event was found"));
                }

                return new JsonResult(Ok(result));
            }
            catch (Exception ex)
            {

                return new JsonResult(BadRequest($"There was an error generating the event results. {ex.Message}"));
            }
            
        }
    }
}
