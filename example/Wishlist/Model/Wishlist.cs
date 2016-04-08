using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace Wishlist.Model
{
    public class Wishlist {
        
        [JsonProperty("id")]
        [Required]
        public string Id { get; set; }
        
        [JsonProperty("url")]
        [Url]
        public string Url { get; set; }
        
        [JsonProperty("owner")]
        [RegularExpression("^.+"), Required]
        public string Owner { get; set; }
        
        [JsonProperty("title"), Required]
        [RegularExpression("^.+")]
        public string Title { get; set; }
        
        // TODO: This does not match the localized schema
        [JsonProperty("description")]
        public string Description { get; set; }
        
        [JsonProperty("createdAt")]
        public DateTime CreatedAt { get; set; }
        
        [JsonProperty("items")]
        public IEnumerable<WishlistItem> Items { get; set; }
    }
}