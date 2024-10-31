using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.DAL.Models;

public partial class Payment
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("user_id")]
    public Guid? UserId { get; set; }

    [Column("payment_date")]
    public DateTime? PaymentDate { get; set; }

    [Column("amount")]
    public float? Amount { get; set; }

    [Column("currency", TypeName = "character varying")]
    public string? Currency { get; set; }

    [Column("payment_method", TypeName = "character varying")]
    public string? PaymentMethod { get; set; }

    [Column("subscription_type", TypeName = "character varying")]
    public string? SubscriptionType { get; set; }

    [Column("status", TypeName = "character varying")]
    public string? Status { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Payments")]
    public virtual User? User { get; set; }
}
