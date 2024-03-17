using FutuEvents.Data;
using FutuEvents.Models.ApiModels;
using FutuEvents.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace FutuEvents.Services
{
    public class EventService
    {
        public static int? CreateEvent(ApiContext context, ApiFutuEvent futuEvent)
        {
            if (futuEvent == null)
            {
                return null;
            }
            else
            {
                

                FutuEvent toBeAdded = new FutuEvent
                {
                    Name = futuEvent.Name
                };
                context.FutuEvents.Add(toBeAdded);
                context.SaveChanges();

                foreach (var item in futuEvent.Dates)
                {
                    PossibleDate dateToBeAdded = new PossibleDate
                    {
                        EventId = toBeAdded.Id,
                        SuggestedDate = item.ToUniversalTime()
                    };
                    context.PossibleDates.Add(dateToBeAdded);
                }

                context.SaveChanges();

                return (int?)toBeAdded.Id;
            }
        }
    }
}
