﻿using System.ComponentModel.DataAnnotations;

namespace API.Requests.Users;

public class UserDietaryPreferences
{
    [StringLength(100)] public string? DietaryPreference { get; set; }

    [StringLength(100)] public string? Goal { get; set; }

    [Range(0, long.MaxValue)] public long? CaloricGoal { get; set; }
}