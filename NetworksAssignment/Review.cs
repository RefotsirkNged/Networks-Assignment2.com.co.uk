using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Review
    {
        public string review;
        public string user;
        public int community;
        public List<string> tokens;
        public Sentiment sentiment;
        public enum Sentiment {blank, positive, negative}
    }
}
