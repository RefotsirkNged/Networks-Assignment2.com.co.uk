using System;
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
    class Program
    {
        static void Main(string[] args)
        {
            bool done = false;
            while (true)
            {
                
                Console.WriteLine("What do you want to do:");
                Console.WriteLine("1: Train from file");
                Console.WriteLine("2: Analyse file");
                Console.WriteLine("3: Find communities");
                Console.WriteLine("4: Make predictions");
                Console.WriteLine("5: Perform crossvalidation");
                Console.WriteLine("0: Exit");

                string answer = Console.ReadLine();

                switch (answer)
                {
                    case "0":
                        done = true;
                        break;
                    case "1":
                        Console.WriteLine("Training...");
                        Train();
                        Console.WriteLine("Done!");
                        break;
                    case "2":
                        Console.WriteLine("Analysing...");
                        Analyse();
                        Console.WriteLine("Done!");
                        break;
                    case "3":
                        Console.WriteLine("Looking for communities...");
                        FindCommunities();
                        Console.WriteLine("Done!");
                        break;
                    case "4":
                        Console.WriteLine("Making predictions...");
                        PredictSentiment();
                        Console.WriteLine("Done!");
                        break;
                    case "5":
                        Console.WriteLine("Performing crossvalidation");
                        CrossValidation cv = new CrossValidation();
                        cv.CrossValidate(10);
                        Console.WriteLine("Done!");
                        break;
                    default:
                        break;
                }

                if (done)
                    break;
            }

            
        }

        private static void PredictSentiment()
        {
            SentimentPredictions predictions = new SentimentPredictions();

            List<Review> willBuy = predictions.reviews.Where(r => r.sentiment == Review.Sentiment.positive && r.review.Trim() == "*").ToList();
            List<Review> willNotBuy = predictions.reviews.Where(r => r.sentiment == Review.Sentiment.negative && r.review.Trim() == "*").ToList();
        }

        private static void Analyse()
        {
            SentimentModel model = new SentimentModel(File.ReadAllText("brain.json"));
            List<Review> reviews = ReadFriendshipReviewData.readFileAsReview("friendships.reviews.txt").Where(r => r != null).ToList();

            for (int i = 0; i < reviews.Count; i++)
            {
                reviews[i] = model.AnalyseReview(reviews[i]);
            }
            reviews = reviews.Where(r => r.sentiment != Review.Sentiment.blank).ToList();
        }

        private static void Train()
        {
            SentimentModel model = new SentimentModel(ReadSentimentTrainingData.readFileAsReview("SentimentTrainingData.txt"));

            FileStream brain = File.Create("brain.json");

            using (System.IO.StreamWriter file =
           new System.IO.StreamWriter(brain))
            {
                file.Write(model.vocabulary.JSONSerialize());
            }
        }

        private static void FindCommunities()
        {
            SpectralClustering sc = new SpectralClustering();

            foreach (List<int> community in sc.FindCommunities())
            {
                Console.WriteLine(community.Count);
            }
        }
    }
}
