﻿using System;
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
        public Sentiment sentiment;
        public enum Sentiment { positive, negative}
    }
}
