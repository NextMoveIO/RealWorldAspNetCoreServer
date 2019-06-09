using System;

namespace RealWordServer.Models
{
    public class Token
    {
        public int TokenId { get; set; }
        public string Secret { get; set; }
        public int UserId { get; set; }
        public DateTime Expires { get; set; }
    }
}
