using ConsoleApplication1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworksAssignment
{
    class SentimentModel
    {
        class _ReviewSentiment
        {

        }

        class TokenModel
        {
            public string token;

            public int NofXNEG;

            public int NofXPOS;

            public TokenModel()
            {
                NofXNEG = 0;

                NofXPOS = 0;
            }

            public double probabilityNEG(int amountNEGReviews)
            {
                return NofXNEG / amountNEGReviews;
            }

            public double probabilityPOS(int amountPOSReviews)
            {
                return NofXPOS / amountPOSReviews;
            }

            public double notXProbabilityNEG(int amountNEGReviews)
            {
                return 1 - probabilityNEG(amountNEGReviews);
            }

            public double notXProbabilityPOS(int amountPOSReviews)
            {
                return 1 - probabilityPOS(amountPOSReviews);
            }
        }

        class Vocabulary
        {
            List<TokenModel> tokens;

            public bool Contains(string token)
            {
                foreach (TokenModel tokenModel in tokens)
                {
                    if (tokenModel.token == token)
                        return true;
                }

                return false;
            }

            public void Add(string token, Review review)
            {
                TokenModel tokenModel = Get(token);
                if (tokenModel == null)
                {
                    tokenModel = new TokenModel();
                }

                switch (review.sentiment)
                {
                    case Review.Sentiment.positive:
                        tokenModel.NofXPOS++;
                        break;
                    case Review.Sentiment.negative:
                        tokenModel.NofXNEG++;
                        break;
                    default:
                        break;
                }
            }

            public TokenModel Get(string token)
            {
                foreach (TokenModel t in tokens)
                {
                    if (t.token == token)
                        return t;
                }

                return null;
            }
        }

        private Vocabulary vocabulary;
        private List<Review> reviews;

        private int amountNEGReviews;
        private int amountPOSReviews;

        private double SOfEmptyNEG;
        private double SOfEmptyPOS;

        public SentimentModel(List<Review> reviews)
        {
            this.reviews = reviews;
            amountNEGReviews = 0;
            amountPOSReviews = 0;

            foreach (Review review in this.reviews)
            {
                switch (review.sentiment)
                {
                    case Review.Sentiment.positive:
                        amountNEGReviews++;
                        amountPOSReviews++;
                        break;
                    case Review.Sentiment.negative:
                        break;
                    default:
                        break;
                }

                AddReviewToVocabulary(review);
            }
        }


        private void AddReviewToVocabulary(Review review)
        {
            foreach (string token in review.tokens)
            {
                if (!vocabulary.Contains(token))
                {
                    vocabulary.Add(token, review);
                }
            }
        }
    }
}
