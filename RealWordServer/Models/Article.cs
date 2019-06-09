using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RealWordServer.Models
{
    public class Article
    {
        public int ArticleId { get; set; }
        public int UserId { get; set; }
        public PublishState State { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? VisibleFrom { get; set; }
    }
}
