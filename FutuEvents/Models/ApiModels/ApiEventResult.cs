namespace FutuEvents.Models.ApiModels
{
    public class ApiEventResult
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public List<ApiGetVote> SuitableDates { get; set; }
    }
}
