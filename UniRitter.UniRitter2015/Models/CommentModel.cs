using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace UniRitter.UniRitter2015.Models
{
    public class CommentModel : Model
    {
        [Required]
        [MaxLength(4000)]
        public string body { get; set; }

        [Required]
        [MaxLength(100)]
        public string title { get; set; }

        public Guid authorId { get; set; }

        public override bool Equals(Model model)
        {
            if (model == null) return false;

            var comment = (CommentModel)model;

            return
                id == comment.id
                && title == comment.title
                && authorId == comment.authorId;
        }
    }
}