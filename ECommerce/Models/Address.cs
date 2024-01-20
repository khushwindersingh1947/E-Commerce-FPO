﻿using System.ComponentModel.DataAnnotations;

namespace ECommerce.Models
{
    public class Address
    {
        public Guid Id { get; set; }
        [Required]
        public string? StreetAddress { get; set; }
        public string? UnitApt { get; set; }
        [Required]
        public string? City { get; set; }
        [Required]
        public string? Province { get; set; }
        [Required]
        public string? PostalCode { get; set; }
        [Required]
        public string? Country { get; set; }
    }
}
