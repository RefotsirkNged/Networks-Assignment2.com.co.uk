using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Review : ICloneable
    {
        public string review;
        public string user;
        public int community;
        public List<string> tokens;
        public Sentiment sentiment;
        public enum Sentiment {blank, positive, negative}

        public object Clone()
        {
            Review rw = new Review();
            rw.review = this.review;
            rw.user = this.user;
            rw.community = this.community;
            rw.tokens = this.tokens;
            rw.sentiment = this.sentiment;
            return rw;
        }
    }
}
