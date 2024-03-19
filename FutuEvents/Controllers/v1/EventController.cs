using FutuEvents.Data;
using FutuEvents.Models.ApiModels;
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

        // Get list of all the events
        [Route("list")]
        [HttpGet]
        public JsonResult Get()
        {
            try
            {
                // Could also include limits and pagination
                // Fetching all the events from db
                var result = _context.FutuEvents;

                if (result == null)
                {
                    return new JsonResult(NotFound("There were no events to return."));
                }

                return new JsonResult(Ok(result));
            }
            catch (Exception ex)
            {

                return new JsonResult(BadRequest($"There was an error getting the event list. {ex.Message}"));
            }
            
        }

        // Create new event
        [HttpPost]
        public JsonResult Create(ApiCreateFutuEvent futuEvent)
        {
            // Making sure that the API is actually receiving an event to create
            if (futuEvent == null)
            {
                return new JsonResult(BadRequest("The event info is null. There is nothing to save."));
            }

            try
            {
                // Proceeding to service and creating event
                var createdEvent = EventService.CreateEvent(_context, futuEvent);

                // Checking, that an actual event was created
                if (createdEvent == null)
                {
                    return new JsonResult(BadRequest($"There was an error creating the event."));
                }

                // Generating response object
                ApiEventBase result = new ApiEventBase { Id = createdEvent.Id };

                // Returning result as Json
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
                // Getting the event requested
                var result = EventService.GetFutuEvent(_context, id);

                // Making sure that result actually has an event to return
                if (result == null)
                {
                    return new JsonResult(NotFound("There were no event to return."));
                }

                // Returning the API response model as Json
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
                // Adding vote and getting API result for response
                var result = EventService.AddVote(_context, id, vote);

                // Making sure that a result was gotten
                if (result == null)
                {
                    return new JsonResult(BadRequest("There was error in adding the vote"));
                }

                // Returning whole event info in response as json
                return new JsonResult(Ok(result));
            }
            catch (Exception ex)
            {
                return new JsonResult(BadRequest($"There was an error adding the vote. {ex.Message}"));
            }
            
        }

        // Show all suitable dates for all the voters for requested event
        [Route("{id}/results")]
        [HttpGet]
        public JsonResult Results(long id)
        {
            try
            {
                // Getting the event info with suitable date. The date needs to fit everyone who has voted.
                var result = EventService.GetResult(_context, id);

                // Checking that there is a result to turn
                if (result == null)
                {
                    return new JsonResult(NotFound("No event was found"));
                }

                // Returning API result model as json
                return new JsonResult(Ok(result));
            }
            catch (Exception ex)
            {

                return new JsonResult(BadRequest($"There was an error generating the event results. {ex.Message}"));
            }
            
        }
    }
}
