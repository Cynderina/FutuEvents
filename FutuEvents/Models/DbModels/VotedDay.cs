using System.ComponentModel.DataAnnotations.Schema;

namespace FutuEvents.Models.DbModels
{
    [Table("voted_day")]
    public class VotedDay
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("event_id")]
        public long EventId { get; set; }
        [Column("date_id")]
        public long DateId { get; set; }
        [Column("vote_id")]
        public long VoteId { get; set; }
    }
}
