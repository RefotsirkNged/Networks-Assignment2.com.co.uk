﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using MathNet.Numerics.LinearAlgebra.Factorization;
using ConsoleApplication1;
using System.IO;

namespace NetworksAssignment
{
    class SentimentPredictions
    {
        SpectralClustering sc;
        public List<Review> reviews;

        public SentimentPredictions()
        {
            sc = new SpectralClustering();
            List<List<int>> rawCommunities = sc.FindCommunities();
            SentimentModel model = new SentimentModel(File.ReadAllText("brain.json"));
            reviews = ReadFriendshipReviewData.readFileAsReview("friendships.reviews.txt");//.Where(r => r != null).ToList();

            for (int i = 0; i < reviews.Count; i++)
            {
                reviews[i] = model.AnalyseReview(reviews[i]);
            }

            int communityIndex = 0;
            foreach (List<int> community in rawCommunities)
            {
                foreach (int user in community)
                {
                    reviews[user].community = communityIndex;
                }

                communityIndex++;
            }

            foreach (Review user in reviews.Where(u => u.sentiment == Review.Sentiment.blank))
            {
                PredictSentiment(user);
            }
        }

        private void PredictSentiment(Review user)
        {
            int average = 0;
            foreach (int friend in sc.friendships[sc.mg.nameMapping[user.user.Trim()]])
            {
                Review friendReview = reviews.Where(r => r.user.Trim() == sc.mg.idMapping[friend].Trim()).First();

                int multiplier = 1;

                if (friendReview.user == "kyle")
                    multiplier *= 10;
                if (user.community != friendReview.community)
                    multiplier *= 10;

                switch (friendReview.sentiment)
                {
                    case Review.Sentiment.blank:
                        break;
                    case Review.Sentiment.positive:
                        average += 1 * multiplier;
                        break;
                    case Review.Sentiment.negative:
                        average -= 1 * multiplier;
                        break;
                    default:
                        break;
                }
            }

            user.sentiment = (average > 0 ? Review.Sentiment.positive : Review.Sentiment.negative);
        }
    }
}
