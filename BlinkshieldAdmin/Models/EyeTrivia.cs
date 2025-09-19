using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlinkshieldAdmin.Models
{
    [Table("eye_trivias")] // Explicit table name matching database
    public class EyeTrivia
    {
        [Key]
        [Column("trivia_id")] // Explicit column name
        public int TriviaId { get; set; } // Consider PascalCase for C# convention

        [Required(ErrorMessage = "Title is required")]
        [StringLength(255, ErrorMessage = "Title cannot exceed 255 characters")]
        [Column("title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [Column("description")]
        public string Description { get; set; }

        [Url(ErrorMessage = "Please enter a valid URL")]
        [StringLength(500, ErrorMessage = "Link cannot exceed 500 characters")]
        [Column("link")]
        public string? Link { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
       

    }
}