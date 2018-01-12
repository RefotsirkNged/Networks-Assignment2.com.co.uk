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

        public void TenFoldCrossvalidate()
        {
            List<List<Review>> folds = FindFoldsFromTrainingData();
            List<double> AccuracyList = new List<double>();
            //For each fold
            for (int i = 0; i < 10; i++)
            {
                //Train brain
                string trainingBrain = TrainFoldBrain(folds, i);

                //predict on the unused fold using that brain (returns in percentage)
                AccuracyList.Add(AnalyseUsingTestData(trainingBrain, folds[i]));

            }
            Console.WriteLine("Accuracy results:");
            foreach (int result in AccuracyList)
            {

                Console.WriteLine(result);
            }
        }

        private string TrainFoldBrain(List<List<Review>> folds, int foldBeingTested)
        {
            List<Review> brainFolds = new List<Review>();
            for (int i = 0; i < 10; i++)
            {
                if (i != foldBeingTested)
                {
                    brainFolds.AddRange(folds[i]);
                }
            }
            string testBrainJSON = new SentimentModel(brainFolds).vocabulary.JSONSerialize();
            return testBrainJSON;
        }
        private List<List<Review>> FindFoldsFromTrainingData()
        {
            List<Review> reviews = ReadSentimentTrainingData.readFileAsReview("SentimentTestingData.txt")
                .Where(r => r != null).ToList();
            List<List<Review>> folds = new List<List<Review>>();

            int foldLength = (int)reviews.Count / 10;
            for (int i = 0; i < 10; i++)
            {
                int foldIndex = i * foldLength;

                folds.Add(new List<Review>(reviews.Skip(foldIndex).Take(foldLength)));
            }
            return folds;
        }

        private double AnalyseUsingTestData(string testBrainJSON, List<Review> foldBeingTested)
        {
            SentimentModel testBrain = new SentimentModel(testBrainJSON);
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
            }
            return (accuracy/(double)foldBeingTested.Count)*100;
        }
    }
}
