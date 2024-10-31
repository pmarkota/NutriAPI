using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class ShoppingListItem
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("shopping_list_id")]
    public Guid? ShoppingListId { get; set; }

    [Column("ingredient_name", TypeName = "character varying")]
    public string? IngredientName { get; set; }

    [Column("quantity")]
    public float? Quantity { get; set; }

    [Column("unit", TypeName = "character varying")]
    public string? Unit { get; set; }

    [Column("is_checked")]
    public bool? IsChecked { get; set; }

    [ForeignKey("ShoppingListId")]
    [InverseProperty("ShoppingListItems")]
    public virtual ShoppingList? ShoppingList { get; set; }
}
