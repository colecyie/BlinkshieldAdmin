using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlinkshieldAdmin.Models
{
    [Table("users")]
    public class acccounts
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("nickname")]
        public string Nickname { get; set; } = string.Empty;

        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("password")]
        public string Password { get; set; } = string.Empty;

        [Column("profile_picture")]
        public string? ProfilePicture { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        [Column("is_verified")]
        public bool? IsVerified { get; set; }

        [Column("verification_code")]
        public string? VerificationCode { get; set; }

        [Column("points")]
        public int? Points { get; set; }

        [Column("screen_time_minutes")]
        public int? ScreenTimeMinutes { get; set; }

        [Column("breaks_taken")]
        public int? BreaksTaken { get; set; }

    }
}
