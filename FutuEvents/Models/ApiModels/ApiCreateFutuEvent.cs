namespace FutuEvents.Models.ApiModels
{
    public class ApiCreateFutuEvent
    {
        public string Name { get; set; }
        public List<DateTime> Dates { get; set; }
    }
}
