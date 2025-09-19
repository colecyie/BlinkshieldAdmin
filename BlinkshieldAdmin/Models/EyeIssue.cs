using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlinkshieldAdmin.Models
{
    [Table("eye_issues")]
    public class EyeIssue
    {
        [Key]
        [Column("issue_id")]
        public int IssueId { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("title")]
        public string Title { get; set; } = string.Empty; // Prevent CS8618

        [Required]
        [Column("description")]
        public string Description { get; set; } = string.Empty; // Prevent CS8618

        [Column("suggested_action")]
        public string? SuggestedAction { get; set; }

        [MaxLength(255)]
        [Column("link")]
        public string? Link { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Default to now
    }
}
