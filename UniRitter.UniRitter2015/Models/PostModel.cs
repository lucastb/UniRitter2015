using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UniRitter.UniRitter2015.Models
{
    public class PostModel
    {
        public Guid? id { get; set; }
        public string title { get; set; }
        public string body { get; set; }
    }
}