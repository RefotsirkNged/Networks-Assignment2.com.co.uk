using ConsoleApplication1;
using Newtonsoft.Json;
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

        
        public class TokenSeializable
        {
            public string token;
            public int NofXNEG;
            public int NofXPOS;
            public double probabilityNEG;
            public double probabilityPOS;

            public TokenSeializable(string token, int NofXNEG, int NofXPOS, double probabilityNEG, double probabilityPOS)
            {
                this.token = token;
                this.NofXNEG = NofXNEG;
                this.NofXPOS = NofXPOS;
                this.probabilityNEG = probabilityNEG;
                this.probabilityPOS = probabilityPOS;
            }
        }

        public class TokenModel
        {
            public string token;

            public int NofXNEG;

            public int NofXPOS;

            

            public TokenModel(string token)
            {
                this.token = token;
                NofXNEG = 0;

                NofXPOS = 0;
            }

            public double probabilityNEG(int amountNEGReviews)
            {
                return (double)NofXNEG / (double)amountNEGReviews;
            }

            public double probabilityPOS(int amountPOSReviews)
            {
                return (double)NofXPOS / (double)amountPOSReviews;
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

        public class Vocabulary
        {
            private List<TokenModel> _tokens = new List<TokenModel>();
            public List<TokenSeializable> tokens;
            public int amountNEGReviews;
            public int amountPOSReviews;

            public string JSONSerialize()
            {
                List<TokenSeializable> sTokens = new List<TokenSeializable>();

                foreach (TokenModel t in _tokens)
                {
                    sTokens.Add(new TokenSeializable(t.token, t.NofXNEG, t.NofXPOS, t.probabilityNEG(amountNEGReviews), t.probabilityPOS(amountPOSReviews)));
                }

                return JsonConvert.SerializeObject(sTokens);
            }

            public void JSONDeserialize(string json)
            {
                tokens = (List<TokenSeializable>)JsonConvert.DeserializeObject(json);
            }

            public bool Contains(string token)
            {
                foreach (TokenModel tokenModel in _tokens)
                {
                    if (tokenModel.token == token)
                        return true;
                }

                return false;
            }

            internal void Add(string token, Review review)
            {
                TokenModel tokenModel = Get(token);
                if (tokenModel == null)
                {
                    tokenModel = new TokenModel(token);
                }

                _tokens.Add(tokenModel);

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
                foreach (TokenModel t in _tokens)
                {
                    if (t.token == token)
                        return t;
                }

                return null;
            }
        }

        public Vocabulary vocabulary;
        private List<Review> reviews;

        

        private double SOfEmptyNEG;
        private double SOfEmptyPOS;

        public SentimentModel(List<Review> reviews)
        {
            vocabulary = new Vocabulary();
            this.reviews = reviews;
            vocabulary.amountNEGReviews = 0;
            vocabulary.amountPOSReviews = 0;
            

            foreach (Review review in this.reviews)
            {
                switch (review.sentiment)
                {
                    case Review.Sentiment.positive:
                        vocabulary.amountPOSReviews++;
                        break;
                    case Review.Sentiment.negative:
                        vocabulary.amountNEGReviews++;
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
                vocabulary.Add(token, review);
            }
        }

        public double GetProbabilityNEG(string token)
        {
            return vocabulary.Get(token).probabilityPOS(vocabulary.amountNEGReviews);
        }

        public double GetProbabilityPOS(string token)
        {
            return vocabulary.Get(token).probabilityPOS(vocabulary.amountPOSReviews);
        }
    }
}
