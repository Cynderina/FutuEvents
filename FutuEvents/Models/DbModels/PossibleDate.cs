using FutuEvents.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace FutuEvents.Models.DbModels
{
    [Table("possible_date")]
    public class PossibleDate
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("event_id")]
        public long EventId { get; set; }
        [Column("suggested_date")]
        public DateTime SuggestedDate { get; set; }
    }
}
