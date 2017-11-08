﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ConvertDataToReview
    {
        public static List<Review> convertSentimentDataToReview(List<ReadSentimentTrainingData.SentimentDataType> dataList)
        {
            List<Review> newReviewList = new List<Review>();

            Parallel.ForEach(dataList, currentReview =>
            {
                newReviewList.Add(currentReview.getReview());
            });

            //foreach(ReadSentimentTrainingData.SentimentDataType element in dataList)
            //{
            //    newReviewList.Add(element.getReview());
            //}

            return newReviewList;
        }
    }
}
