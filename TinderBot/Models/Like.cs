using System;
using System.Collections.Generic;
using System.Text;

namespace TinderBot.Models
{
    public class Like
    {
        public int status { get; set; }
        public bool match { get; set; }
        public int likes_remaining { get; set; }
        public string XPadding { get; set; }
    }

}
