using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlinkshieldAdmin.Models
{
    [Table("admin_history")]
    public class AdminHistory
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("action")]
        public string? Action { get; set; } = string.Empty;

        [Column("Description")]
        public string? Description { get; set; }

        [Column("table_name")]
        public string? TableName { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("record_id")]
        public int? RecordId { get; set; }

        [Column("old_values")]
        public string? OldValues { get; set; } // JSON format

        [Column("new_values")]
        public string? NewValues { get; set; } // JSON format
    }

    }

