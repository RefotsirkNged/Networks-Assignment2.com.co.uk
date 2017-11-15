using NetworksAssignment;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class ReadSentimentTrainingData
    {
        public class SentimentDataType
        {
            public string productID;
            public string userID;
            public string profileName;
            public float helpfullness;
            public float score;
            public int time;
            public string summory;
            public string text;

            public Review getReview()
            {
                Review temp = new Review();
                if (score <= 20)
                    temp.sentiment = Review.Sentiment.negative;
                else if (score >= 40)
                    temp.sentiment = Review.Sentiment.positive;
                temp.review = text;
                temp.user = userID;
                temp.tokens = Tokenizer.SplitString(temp.review);
                return temp;
            }
        }

        public static List<Review> readFileAsReview(string filePath)
        {
            return ConvertDataToReview.convertSentimentDataToReview(readFile(filePath));
        }

        public static List<SentimentDataType> readFile(string filePath)
        {
            try
            {
                List<SentimentDataType> dataList = new List<SentimentDataType>();
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string lineRead = sr.ReadLine();
                    float resualt;
                    while(lineRead != null)
                    {
                        if (lineRead.Contains("product/productId"))
                        {
                            SentimentDataType temp = new SentimentDataType();
                            temp.productID = lineRead.Split(':')[1];
                            dataList.Add(temp);
                        }
                        else if (lineRead.Contains("review/userId"))
                            dataList.LastOrDefault().userID = lineRead.Split(':')[1];

                        else if (lineRead.Contains("review/profileName"))
                            dataList.LastOrDefault().profileName = lineRead.Split(':')[1];

                        else if (lineRead.Contains("review/helpfulness")){
                            string temp = lineRead.Split(':')[1];
                            dataList.LastOrDefault().helpfullness = float.Parse(temp.Split('/')[0]);
                            dataList.LastOrDefault().helpfullness /= float.Parse(temp.Split('/')[1]);
                        }
                        else if (lineRead.Contains("review/score") && float.TryParse(lineRead.Split(':')[1], out resualt))
                            dataList.LastOrDefault().score = resualt;

                        else if (lineRead.Contains("review/time"))
                            dataList.LastOrDefault().time = Int32.Parse(lineRead.Split(':')[1]);

                        else if (lineRead.Contains("review/summary"))
                            dataList.LastOrDefault().summory = lineRead.Split(':')[1];

                        else if (lineRead.Contains("review/text"))
                            dataList.LastOrDefault().text = lineRead.Split(':')[1];
                        lineRead = sr.ReadLine();
                    }
                }
                return dataList;
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
