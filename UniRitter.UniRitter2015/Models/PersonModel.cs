using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace UniRitter.UniRitter2015.Models
{
    public class PersonModel: Model
    {
        [Required]
        [MaxLength(100)]
        public string firstName { get; set; }

        [Required]
        public string lastName { get; set; }

        [Required]
        [EmailAddress]
        public string email { get; set; }

        [Url]
        public string url { get; set; }
        
        public override bool Equals(Model model)
        {
            if (model == null) return false;

            var person = (PersonModel) model;

            return
                id == person.id
                && firstName == person.firstName
                && lastName == person.lastName
                && email == person.email
                && url == person.url;
        }
    }
}