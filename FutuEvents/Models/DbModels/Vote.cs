using System.ComponentModel.DataAnnotations.Schema;

namespace FutuEvents.Models.DbModels
{
    [Table("futu_event_vote")]
    public class Vote
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("event_id")]
        public long EventId { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
