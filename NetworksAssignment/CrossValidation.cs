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
            if (!File.Exists("brain.json"))
            {
                ReadTestData();
            }
            SentimentModel brain = new SentimentModel(File.ReadAllText("brain.json"));

            List<List<Review>> folds = FindFoldsFromTrainingData();

            AnalyseUsingTestData(folds);

        }



        private void ReadTestData()
        {
            SentimentModel model = new SentimentModel(ReadSentimentTrainingData.readFileAsReview("SentimentTestingData.txt"));

            FileStream brainFile = File.Create("brain.json");

            using (System.IO.StreamWriter file =
                new System.IO.StreamWriter(brainFile))
            {
                file.Write(model.vocabulary.JSONSerialize());
            }
        }

        private void AnalyseUsingTestData(List<List<Review>> testData)
        {
            SentimentModel brain = new SentimentModel(File.ReadAllText("brain.json"));
            List<Review> reviews = ReadFriendshipReviewData.readFileAsReview("friendships.reviews.txt").Where(r => r != null).ToList();

            for (int i = 0; i < reviews.Count; i++)
            {
                reviews[i] = brain.AnalyseReview(reviews[i]);
            }
            reviews = reviews.Where(r => r.sentiment != Review.Sentiment.blank).ToList();
        }

        private List<List<Review>> FindFoldsFromTrainingData()
        {
            List<Review> reviews = ReadFriendshipReviewData.readFileAsReview("friendships.reviews.txt")
                .Where(r => r != null).ToList();
            List<List<Review>> folds = new List<List<Review>>();

            int foldLength = (int) reviews.Count / 10;
            for (int i = 0; i < 10; i++)
            {
                int foldMin = i * foldLength;
                int foldMax = i * foldLength + foldLength;
                folds.Add(new List<Review>(reviews.GetRange(foldMin, foldMax)));
            }
            return new List<List<Review>>();
        }



    }
}
