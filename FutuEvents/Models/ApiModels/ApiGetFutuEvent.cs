namespace FutuEvents.Models.ApiModels
{
    public class ApiGetFutuEvent
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<DateTime> Dates { get; set; }
        public List<ApiGetVote> Votes { get; set; }
    }
}
