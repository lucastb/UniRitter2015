using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace UniRitter.UniRitter2015.Models
{
    public class PostModel : Model
    {
        [Required]
        [MaxLength(4000)]
        public string body { get; set; }

        [Required]
        [MaxLength(100)]
        public string title { get; set; }

        public Guid authorId { get; set; }

        public IEnumerable<string> tags { get; set; }

        public override bool Equals(Model model)
        {
            if (model == null) return false;

            var post = (PostModel) model;

            return
                id == post.id
                && body == post.body
                && title == post.title
                && authorId == post.authorId
                && tags == post.tags;
        }
    }
}