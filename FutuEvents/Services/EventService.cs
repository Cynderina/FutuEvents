using FutuEvents.Data;
using FutuEvents.Models.ApiModels;
using FutuEvents.Models.DbModels;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FutuEvents.Services
{
    public class EventService
    {
        public static FutuEvent? CreateEvent(ApiContext context, ApiCreateFutuEvent futuEvent)
        {
            if (futuEvent == null)
            {
                throw new Exception("The event info was empty.");
            }
            else
            {
                if (string.IsNullOrEmpty(futuEvent.Name) || string.IsNullOrWhiteSpace(futuEvent.Name))
                {
                    throw new Exception("The event name was empty.");
                }

                if (!futuEvent.Dates.Any())
                {
                    throw new Exception("The event is missing suggested dates.");
                }

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

                return toBeAdded;
            }
        }

        public static ApiGetFutuEvent? GetFutuEvent(ApiContext context, long id)
        {
            ApiGetFutuEvent result = new ApiGetFutuEvent();
            FutuEvent? futuEvent = context.FutuEvents.Where(x => x.Id == id).FirstOrDefault();
            List<PossibleDate> dates = context.PossibleDates.Where(x => x.EventId == id).ToList();

            if (futuEvent == null)
            {
                return null;
            }
            
            if (dates == null)
            {
                throw new Exception("No possible dates for the event found.");
            }
            
            result.Id = futuEvent.Id;
            result.Name = futuEvent.Name;

            result.Dates = new List<string>();

            foreach (var date in dates)
            {
                result.Dates.Add(date.SuggestedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
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

                    vote.Date = date.SuggestedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);


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

        public static ApiGetFutuEvent AddVote(ApiContext context, long id, ApiCreateVote vote)
        {
            FutuEvent? futuEvent = context.FutuEvents.Where(x => x.Id == id).FirstOrDefault();
            if (futuEvent == null)
            {
                throw new Exception("No event found with the id provided.");
            }

            List<PossibleDate> dates = context.PossibleDates.Where(x => x.EventId == id).ToList();
            if (dates == null)
            {
                throw new Exception("No possible dates for the event found.");
            }

            List<DateTime> suggestedDates = dates.Select(x => x.SuggestedDate).ToList();

            if (string.IsNullOrEmpty(vote.Name) || string.IsNullOrWhiteSpace(vote.Name))
            {
                throw new Exception("The name provided with the vote is empty.");
            }

            if (!vote.Votes.Any())
            {
                throw new Exception("Vote doesn't have any suitable dates.");
            }

            Vote voteToBeAdded = new Vote { EventId = id, Name = vote.Name };

            context.Votes.Add(voteToBeAdded);
            context.SaveChanges();

            foreach (var item in vote.Votes)
            {
                if (suggestedDates.Contains(item))
                {
                    VotedDay voteDayToBeAdded = new VotedDay
                    {
                        EventId = id,
                        VoteId = voteToBeAdded.Id,
                        DateId = dates.Where(x => x.SuggestedDate == item).FirstOrDefault().Id
                    };

                    context.VotedDays.Add(voteDayToBeAdded);
                }
            }

            context.SaveChanges();

            return GetFutuEvent(context, id);
        }

        public static ApiEventResult GetResult(ApiContext context, long id)
        {
            ApiGetFutuEvent? futuEvent = GetFutuEvent(context, id);
            if (futuEvent == null)
            {
                throw new Exception("The requested event was not found.");
            }

            ApiEventResult result = new ApiEventResult();
            result.SuitableDates = new List<ApiGetVote>();

            result.Id = futuEvent.Id;
            result.Name = futuEvent.Name;

            List<Vote> votes = context.Votes.Where(x => x.EventId == futuEvent.Id).ToList();
            int voteAmount = votes.Count();

            foreach (var vote in futuEvent.Votes)
            {
                if (vote.People.Count() == voteAmount)
                {
                    result.SuitableDates.Add(vote);
                }
            }

            return result;
        }
    }
}
