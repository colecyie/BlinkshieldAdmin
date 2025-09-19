using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlinkshieldAdmin.Models
{
    [Table("wardrobe_items")]
    public class WardrobeItem
    {
        [Key]
        [Column("item_id")]
        public int ItemId { get; set; }

        [Required]
        [StringLength(100)]
        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("type")]
        public string Type { get; set; } = string.Empty;  // PascalCase for property

        [Required]
        [Column("price")]
        public decimal Price { get; set; }   // Use decimal for money instead of int

        [Column("image", TypeName = "LONGBLOB")]
        public byte[]? Image { get; set; }   // Real binary image

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
