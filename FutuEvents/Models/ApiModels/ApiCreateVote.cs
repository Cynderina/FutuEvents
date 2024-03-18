namespace FutuEvents.Models.ApiModels
{
    public class ApiCreateVote
    {
        public string Name { get; set; }
        public List<DateTime> Votes { get; set; }
    }
}
