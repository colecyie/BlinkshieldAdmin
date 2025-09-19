using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlinkshieldAdmin.Models
{
    [Table("eye_tips")] // Match your actual table name
    public class EyeCareTip
    {
        [Key]
        [Column("tip_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TipId { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
        [Column("title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [Column("content")]
        public string Content { get; set; }
        public string Category { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        [Column("link")]
        public string? Link { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}