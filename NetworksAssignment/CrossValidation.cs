using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1;

namespace NetworksAssignment
{
    class CrossValidation
    {

        public CrossValidation()
        {

        }

        public void CrossValidate(int foldcount)
        {
            List<List<Review>> folds = FindFoldsFromTrainingData(foldcount);
            List<double> AccuracyList = new List<double>();
            //For each fold
            for (int i = 0; i < foldcount; i++)
            {
                //Train brain
                string trainingBrain = TrainFoldBrain(folds, i);

                //predict on the unused fold using that brain (returns in percentage)
                AccuracyList.Add(AnalyseUsingTestData(trainingBrain, folds[i]));

            }
            Console.WriteLine("Accuracy results:");
            foreach (double result in AccuracyList)
            {

                Console.WriteLine(result);
            }
        }

        private string TrainFoldBrain(List<List<Review>> folds, int foldBeingTested)
        {
            List<Review> brainFolds = new List<Review>();
            for (int i = 0; i < folds.Count; i++)
            {
                if (i != foldBeingTested)
                {
                    brainFolds.AddRange(folds[i]);
                }
            }
            string testBrainJSON = new SentimentModel(brainFolds).vocabulary.JSONSerialize();
            return testBrainJSON;
        }
        private List<List<Review>> FindFoldsFromTrainingData(int foldcount)
        {
            List<Review> reviews = ReadSentimentTrainingData.readFileAsReview("SentimentTrainingData.txt")
                .Where(r => r != null).ToList();
            List<List<Review>> folds = new List<List<Review>>();





            List<Review> negReviews = reviews.Where(r => r.sentiment == Review.Sentiment.negative).ToList();
            List<Review> posReviews = reviews.Where(r => r.sentiment == Review.Sentiment.positive).ToList();
            List<Review> blankReviews = reviews.Where(r => r.sentiment == Review.Sentiment.blank).ToList();
            int res = negReviews.Count + posReviews.Count;

            //neg
            int foldLengthNeg = (int)negReviews.Count / foldcount;
            for (int i = 0; i < foldcount; i++)
            {
                int foldIndex = i * foldLengthNeg;
                folds.Add(new List<Review>(negReviews.Skip(foldIndex).Take(foldLengthNeg)));
            }

            //pos
            int foldLengthPos = (int)posReviews.Count / foldcount;
            for (int i = 0; i < foldcount; i++)
            {
                int foldIndex = i * foldLengthPos;
                folds[i].AddRange(posReviews.Skip(foldIndex).Take(foldLengthPos));
            }
            return folds;
        }


        private double AnalyseUsingTestData(string testBrainJSON, List<Review> foldBeingTested)
        {
            SentimentModel testBrain = new SentimentModel(testBrainJSON);
            //SentimentModel testBrain = new SentimentModel(File.ReadAllText("brain.json"));
            List<Review> originalFold = new List<Review>();
            foreach (Review review in foldBeingTested)
            {
                originalFold.Add((Review)review.Clone());
            }

            for (int i = 0; i < foldBeingTested.Count; i++)
            {
                foldBeingTested[i].sentiment = Review.Sentiment.blank;
            }

            for (int i = 0; i < foldBeingTested.Count; i++)
            {
                foldBeingTested[i] = testBrain.AnalyseReview(foldBeingTested[i]);
            }

            int accuracy = 0;

            for (int i = 0; i < foldBeingTested.Count; i++)
            {
                if (foldBeingTested[i].sentiment == originalFold[i].sentiment)
                {
                    accuracy++;
                }
                else
                {
                    
                }
            }
            return (accuracy/(double)foldBeingTested.Count)*100;
        }
    }
}
