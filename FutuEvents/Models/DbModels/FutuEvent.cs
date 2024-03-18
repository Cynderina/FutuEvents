using System.ComponentModel.DataAnnotations.Schema;

namespace FutuEvents.Models.DbModels
{
    [Table("futu_event")]
    public class FutuEvent
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
    }
}
