using FutuEvents.Data;
using FutuEvents.Models.ApiModels;
using FutuEvents.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FutuEvents.Services
{
    public class EventService
    {
        public static int? CreateEvent(ApiContext context, ApiCreateFutuEvent futuEvent)
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

        public static ApiGetFutuEvent GetFutuEvent(ApiContext context, long id)
        {
            ApiGetFutuEvent result = new ApiGetFutuEvent();
            // Think about exception handling
            FutuEvent? futuEvent = context.FutuEvents.Where(x => x.Id == id).FirstOrDefault();
            List<PossibleDate> dates = context.PossibleDates.Where(x => x.EventId == id).ToList();
            
            result.Id = futuEvent.Id;
            result.Name = futuEvent.Name;

            result.Dates = new List<DateTime>();

            foreach (var date in dates)
            {
                result.Dates.Add(date.SuggestedDate);
            }

            List<Vote> votes = context.Votes.Where(x => x.EventId == futuEvent.Id).ToList();
            List<VotedDay> votedDays = context.VotedDays.Where(x => x.EventId == futuEvent.Id).ToList();
            var votedDaysIds = votedDays.Select(x => x.DateId);

            

            result.Votes = new List<ApiGetVote>();

            foreach (var date in dates)
            {
                if (votedDaysIds.Contains(date.Id))
                {
                    ApiGetVote vote = new ApiGetVote();
                    vote.People = new List<string>();

                    vote.Date = date.SuggestedDate;


                    var votesForTheDay = votedDays.Where(x => x.DateId == date.Id);

                    foreach (var voteForTheDay in votesForTheDay)
                    {
                        vote.People.Add(votes.Where(x => x.Id == voteForTheDay.VoteId).FirstOrDefault().Name);
                    }

                    result.Votes.Add(vote);
                }
            }




            return result;
        }

        public static ApiGetVote GetVotes(ApiContext context, FutuEvent futuEvent)
        {
            ApiGetVote result = new ApiGetVote();
            

            return result;
        }
    }
}
