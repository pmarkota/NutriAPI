using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class Notification
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("message")]
    public string? Message { get; set; }

    [Column("type", TypeName = "character varying")]
    public string? Type { get; set; }

    [Column("scheduled_at", TypeName = "timestamp without time zone")]
    public DateTime? ScheduledAt { get; set; }

    [Column("is_sent")]
    public bool? IsSent { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Notifications")]
    public virtual User? User { get; set; }
}
