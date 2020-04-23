using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AspCoreRestAPI.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "ErrorMessageSku")]
        public string Sku { get; set; }
        public float Price { get; set; }
        public float? DiscountPrice { get; set; }
        public string ImageUrl { get; set; }
        public string ImageList { get; set; }
        public int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsActive { get; set; }
    }
}
