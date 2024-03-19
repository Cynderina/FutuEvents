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
            // Controller had already check for this, but as this method is in service, considering
            // further uses of this method and if it's called elsewhere here is also null check that there
            // is an actual event to be created.
            if (futuEvent == null)
            {
                throw new Exception("The event info was empty.");
            }
            else
            {
                // Checking that the event has an actual name
                if (string.IsNullOrEmpty(futuEvent.Name) || string.IsNullOrWhiteSpace(futuEvent.Name))
                {
                    throw new Exception("The event name was empty.");
                }

                // Checking that the event has suggested dates
                if (!futuEvent.Dates.Any())
                {
                    throw new Exception("The event is missing suggested dates.");
                }

                //Proceeding to first add event in events' table
                FutuEvent toBeAdded = new FutuEvent
                {
                    Name = futuEvent.Name
                };
                context.FutuEvents.Add(toBeAdded);
                context.SaveChanges();

                // After save we got the EventId to use for saving the suggested dates in their own table
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

                // Returning just the saved event as API response needs id from the object
                return toBeAdded;
            }
        }

        public static ApiGetFutuEvent? GetFutuEvent(ApiContext context, long id)
        {
            // Initializing result object
            ApiGetFutuEvent result = new ApiGetFutuEvent();
            // Fetching the event and suggested dates from database
            FutuEvent? futuEvent = context.FutuEvents.Where(x => x.Id == id).FirstOrDefault();
            List<PossibleDate> dates = context.PossibleDates.Where(x => x.EventId == id).ToList();

            // Checking that event and the suggested dates were found
            if (futuEvent == null)
            {
                return null;
            }
            
            if (dates == null)
            {
                throw new Exception("No possible dates for the event found.");
            }
            
            // Adding data from db model to API result model
            result.Id = futuEvent.Id;
            result.Name = futuEvent.Name;

            result.Dates = new List<string>();

            foreach (var date in dates)
            {
                result.Dates.Add(date.SuggestedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }

            // Fetching vote data from db
            List<Vote> votes = context.Votes.Where(x => x.EventId == futuEvent.Id).ToList();
            List<VotedDay> votedDays = context.VotedDays.Where(x => x.EventId == futuEvent.Id).ToList();
            var votedDaysIds = votedDays.Select(x => x.DateId);

            
            // Initializing the API result models' vote listing and inserting data if there are votes registered
            result.Votes = new List<ApiGetVote>();

            // Looping the suggested days for the event
            foreach (var date in dates)
            {
                // Making sure that the suggested date is added to voted days only if there actual votes for the day.
                if (votedDaysIds.Contains(date.Id))
                {
                    // Initializing the vote and people info for API result
                    ApiGetVote vote = new ApiGetVote();
                    vote.People = new List<string>();

                    // Inserting the suggested date info in requested format
                    vote.Date = date.SuggestedDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

                    // Gathering the votes for the suggested day
                    var votesForTheDay = votedDays.Where(x => x.DateId == date.Id);

                    // Looping the votes for the day to add the people info for the votes
                    foreach (var voteForTheDay in votesForTheDay)
                    {
                        vote.People.Add(votes.Where(x => x.Id == voteForTheDay.VoteId).FirstOrDefault().Name);
                    }

                    // Adding the build vote for the vote listing for the API response model
                    result.Votes.Add(vote);
                }
            }

            return result;
        }

        public static ApiGetFutuEvent AddVote(ApiContext context, long id, ApiCreateVote vote)
        {
            // Fetching event from db and checking it was found
            FutuEvent? futuEvent = context.FutuEvents.Where(x => x.Id == id).FirstOrDefault();
            if (futuEvent == null)
            {
                throw new Exception("No event found with the id provided.");
            }

            // Fetching suggested dates and checking that they were found
            List<PossibleDate> dates = context.PossibleDates.Where(x => x.EventId == id).ToList();
            if (dates == null)
            {
                throw new Exception("No possible dates for the event found.");
            }

            // Data validation for the vote that there is actual name and vote for days
            if (string.IsNullOrEmpty(vote.Name) || string.IsNullOrWhiteSpace(vote.Name))
            {
                throw new Exception("The name provided with the vote is empty.");
            }

            if (!vote.Votes.Any())
            {
                throw new Exception("Vote doesn't have any suitable dates.");
            }

            // Gathering the suggested dates in list
            List<DateTime> suggestedDates = dates.Select(x => x.SuggestedDate).ToList();

            // Checking the dates if there are dates not suggested for event
            foreach (var item in vote.Votes)
            {
                // Making sure that the vote and voted days are added only if they fir suggested dates
                if (!suggestedDates.Contains(item))
                {
                    throw new Exception("Vote contained dates that are not suggested to event.");
                }
            }

            // Adding vote to vote table and getting back VoteId after save
            Vote voteToBeAdded = new Vote { EventId = id, Name = vote.Name };

            context.Votes.Add(voteToBeAdded);
            context.SaveChanges();

            // Adding vote info for voted days table
            foreach (var item in vote.Votes)
            {
                // Making sure that the voted day is added only if it is in suggested dates of the event
                // so users can't add votes for days that are not suggested
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

            // After vote has been added then getting again the current event info and returning it as response
            return GetFutuEvent(context, id);
        }

        public static ApiEventResult GetResult(ApiContext context, long id)
        {
            // First going to get the API result version for the event, for we just want to remove
            // the extra voted days and just show the suitable dates for every voter
            ApiGetFutuEvent? futuEvent = GetFutuEvent(context, id);

            // Again cheking that event was found
            if (futuEvent == null)
            {
                throw new Exception("The requested event was not found.");
            }

            // Initializing the API result variable which will be delivered in response
            ApiEventResult result = new ApiEventResult();
            result.SuitableDates = new List<ApiGetVote>();

            // Gatherin ID and name info
            result.Id = futuEvent.Id;
            result.Name = futuEvent.Name;

            // Fetching votes for event to come up with the suitable date
            List<Vote> votes = context.Votes.Where(x => x.EventId == futuEvent.Id).ToList();
            // Taking count on the votes. For in this case it is considered that everyone gets to vote only once
            // so if there are 8 voters for the event then the suggested date having 8 voters is a suitable date
            int voteAmount = votes.Count();

            // Looping through the voted dates
            foreach (var vote in futuEvent.Votes)
            {
                if (vote.People.Count() == voteAmount)
                {
                    result.SuitableDates.Add(vote);
                }
            }

            // Returning Api result model for response
            return result;
        }
    }
}
