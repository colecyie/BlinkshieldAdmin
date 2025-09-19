using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlinkshieldAdmin.Models
{
    [Table("achievements")]
    public class Achievement
    {
        [Key]
        [Column("achievement_id")]
        public int AchievementId { get; set; }

        [Column("title")]
        public string Title { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("points")]
        public int Points { get; set; }

        [Column("requirement_type")]
        public string? RequirementType { get; set; } // ✅ New

        [Column("requirement_value")]
        public int RequirementValue { get; set; } // ✅ New

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }


    }
}
