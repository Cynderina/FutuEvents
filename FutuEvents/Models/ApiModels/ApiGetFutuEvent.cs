using System.Runtime.Serialization;

namespace FutuEvents.Models.ApiModels
{
    public class ApiGetFutuEvent : ApiEventBase
    {
        public string Name { get; set; }
        public List<string> Dates { get; set; }
        public List<ApiGetVote> Votes { get; set; }
    }
}
