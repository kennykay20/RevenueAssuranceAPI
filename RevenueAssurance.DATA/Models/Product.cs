

using System.ComponentModel.DataAnnotations;

namespace anythingGoodApi.AnythingGood.DATA.Models
{
    public class Product
    {
         [Key]
         public int AId {get; set;}
         public int service_id {get; set;}
    }
}