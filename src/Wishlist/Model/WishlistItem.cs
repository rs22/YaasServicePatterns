using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Wishlist.Model
{
    public class WishlistItem {
        
        [JsonProperty("product")]
        [RegularExpression("^.+"), Required]
        public string Product { get; set; }
        
        [JsonProperty("amount")]
        [Range(1, int.MaxValue), Required]
        public int Amount { get; set; }
        
        [JsonProperty("note")]
        public string Note { get; set; }
        
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
    }
}