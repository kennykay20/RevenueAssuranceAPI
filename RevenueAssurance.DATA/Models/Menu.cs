using System.ComponentModel.DataAnnotations;
namespace anythingGoodApi.AnythingGood.DATA.Models
{
    public class Menu
    {
            [Key]
            public string AId {get; set;}
            public string Id{ get; set;}
            public string Title {get; set;}
            public string RouterLink{get; set;}
            public string Href{get; set;}
            public string Icon{get; set;}
            public string Target{get; set;}
            public string HasSubMenu{get; set;}
            public string ParentId{get; set;}
            public string Status{get; set;}
            public string PriorityOrder{get; set;}
    }
}