namespace FutuEvents.Models.ApiModels
{
    public class ApiEventResult : ApiEventBase
    {
        public string Name { get; set; }
        public List<ApiGetVote> SuitableDates { get; set; }
    }
}
