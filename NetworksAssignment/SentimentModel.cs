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
        public class VocabularySerializable
        {
            public Dictionary<string, TokenSeializable> tokens;
            public int amountNEGReviews;
            public int amountPOSReviews;
            public double SOfEmptyNEG;
            public double SOfEmptyPOS;
            public int negWords;
            public int posWords;

            public double ProbPosDoc
            {
                get
                {
                    return (double)amountPOSReviews / (double)(amountPOSReviews + amountNEGReviews);
                }
            }

            public double ProbNegDoc
            {
                get
                {
                    return (double)amountNEGReviews / (double)(amountPOSReviews + amountNEGReviews);
                }
            }

            internal double GetNegProbability(string token)
            {
                double result = (tokens.ContainsKey(token) ? tokens[token].probabilityNEG : 1);
                return result;
            }

            internal double GetPosProbability(string token)
            {
                double result = (tokens.ContainsKey(token) ? tokens[token].probabilityPOS : 1);
                return result;
            }
        }

        public static VocabularySerializable Dezerialize(string json)
        {
            return JsonConvert.DeserializeObject<VocabularySerializable>(json);
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

            public double probabilityNEG(int negWords, int vocabularySize)
            {
                return (double)(NofXNEG + 1) / (double)(negWords + vocabularySize);
            }

            public double probabilityPOS(int posWords, int vocabularySize)
            {
                return (double)(NofXPOS + 1) / (double)(posWords + vocabularySize);
            }

            public double notXProbabilityNEG(int negWords, int vocabularySize)
            {
                return 1 - probabilityNEG(negWords, vocabularySize);
            }

            public double notXProbabilityPOS(int posWords, int vocabularySize)
            {
                return 1 - probabilityPOS(posWords, vocabularySize);
            }
        }

        public class Vocabulary
        {
            private Dictionary<string, TokenModel> _tokens = new Dictionary<string, TokenModel>();
            public List<TokenSeializable> tokens;
            public int amountNEGReviews;
            public int amountPOSReviews;
            public int negWords;
            public int posWords;

            public double ProbPosDoc
            {
                get
                {
                    return (double) amountPOSReviews / (double)(amountPOSReviews + amountNEGReviews);
                }
            }

            public double ProbNegDoc
            {
                get
                {
                    return (double)amountNEGReviews / (double)(amountPOSReviews + amountNEGReviews);
                }
            }

            private double SOfEmptyPOS
            {
                get
                {
                    double sum = 1;
                    foreach (TokenModel t in _tokens.Values)
                    {
                        sum = sum * t.notXProbabilityPOS(posWords, VocabularySize);
                    }

                    return (double)(sum * ((double)amountPOSReviews / (double)(amountNEGReviews + amountPOSReviews)));
                }
            }

            private double SOfEmptyNEG
            {
                get
                {
                    double sum = 1;
                    foreach (TokenModel t in _tokens.Values)
                    {
                        sum = sum * t.notXProbabilityNEG(negWords, VocabularySize);
                    }

                    return (double)(sum * ((double)amountNEGReviews / (double)(amountNEGReviews + amountPOSReviews)));
                }
            }

            public int VocabularySize
            {
                get
                {
                    return _tokens.Count;
                }
            }

            public string JSONSerialize()
            {
                Dictionary<string, TokenSeializable> sTokens = new Dictionary<string, TokenSeializable>();
                VocabularySerializable sVocabulary = new VocabularySerializable();

                foreach (TokenModel t in _tokens.Values)
                {
                    sTokens.Add(t.token, new TokenSeializable(t.token, t.NofXNEG, t.NofXPOS, t.probabilityNEG(negWords, _tokens.Count), t.probabilityPOS(posWords, _tokens.Count)));
                }

                sVocabulary.amountNEGReviews = amountNEGReviews;
                sVocabulary.amountPOSReviews = amountPOSReviews;
                sVocabulary.SOfEmptyNEG = SOfEmptyNEG;
                sVocabulary.SOfEmptyPOS = SOfEmptyPOS;
                sVocabulary.tokens = sTokens;
                sVocabulary.negWords = negWords;
                sVocabulary.posWords = posWords;

                return JsonConvert.SerializeObject(sVocabulary);
            }

            public bool Contains(string token)
            {
                return _tokens.ContainsKey(token);
            }

            internal void Add(string token, Review review)
            {
                TokenModel tokenModel = Get(token);
                if (tokenModel == null)
                {
                    tokenModel = new TokenModel(token);
                    _tokens[token] = tokenModel;
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
                return (_tokens.ContainsKey(token) ? _tokens[token] : null);
            }
        }

        public Vocabulary vocabulary;
        private List<Review> reviews;
        private VocabularySerializable sVocabulary;

        public SentimentModel(List<Review> reviews)
        {
            vocabulary = new Vocabulary();
            this.reviews = reviews;
            vocabulary.amountNEGReviews = 0;
            vocabulary.amountPOSReviews = 0;
            vocabulary.negWords = 0;
            vocabulary.posWords = 0;

            foreach (Review review in this.reviews)
            {
                if (review != null)
                {
                    switch (review.sentiment)
                    {
                        case Review.Sentiment.positive:
                            vocabulary.amountPOSReviews++;
                            vocabulary.posWords += review.tokens.Count;
                            break;
                        case Review.Sentiment.negative:
                            vocabulary.amountNEGReviews++;
                            vocabulary.negWords += review.tokens.Count;
                            break;
                        default:
                            break;
                    }

                    AddReviewToVocabulary(review);
                }
            }
        }

        public SentimentModel(string JSON)
        {
            sVocabulary = Dezerialize(JSON);
            
        }

        public Review AnalyseReview(Review review)
        {
            double probabilityPos = 1;
            double probabilityNeg = 1;

            if (review.tokens.Count == 0)
                return review;

            foreach (string token in review.tokens)
            {
                //probabilityNeg = probabilityNeg * (sVocabulary.GetNegProbabililty(token) / (1 - sVocabulary.GetNegProbabililty(token)));
                //probabilityPos = probabilityPos * (sVocabulary.GetPosProbability(token) / (1 - sVocabulary.GetPosProbability(token)));

                probabilityNeg *= sVocabulary.GetNegProbability(token) / sVocabulary.ProbNegDoc;
                probabilityPos *= sVocabulary.GetPosProbability(token) / sVocabulary.ProbPosDoc;
            }

            //probabilityNeg = probabilityNeg * sVocabulary.SOfEmptyNEG;
            //probabilityPos = probabilityPos * sVocabulary.SOfEmptyPOS;

            probabilityNeg = probabilityNeg * sVocabulary.ProbNegDoc;
            probabilityPos = probabilityPos * sVocabulary.ProbPosDoc;

            if (probabilityPos > probabilityNeg)
            {
                review.sentiment = Review.Sentiment.positive;
            }
            else
            {
                review.sentiment = Review.Sentiment.negative;
            }

            return review;
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
            return vocabulary.Get(token).probabilityPOS(vocabulary.posWords, vocabulary.VocabularySize);
        }

        public double GetProbabilityPOS(string token)
        {
            return vocabulary.Get(token).probabilityPOS(vocabulary.negWords, vocabulary.VocabularySize);
        }
    }
}
