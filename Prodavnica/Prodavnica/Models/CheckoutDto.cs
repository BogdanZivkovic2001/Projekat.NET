﻿using System.ComponentModel.DataAnnotations;

namespace Prodavnica.Models
{
    public class CheckoutDto
    {
        [Required(ErrorMessage = "Delivery Address is required")]
        [MaxLength(200)]
        public string DeliveryAddress { get; set; } = "";
        public string PaymentMethod { get; set; } = "";
    }
}
