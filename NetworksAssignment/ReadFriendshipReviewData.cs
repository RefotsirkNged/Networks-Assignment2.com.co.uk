using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConsoleApplication1;

namespace NetworksAssignment
{
    class ReadFriendshipReviewData
    {
        public class FriendshipReviewDataType
        {
            public string productID;
            public string userID;
            public string profileName;
            public float helpfullness;
            public float score;
            public int time;
            public string summary;
            public string review;
            public List<string> friendsList;

            public Review getReview()
            {
                Review temp = new Review();
                temp.review = review.ToLower();
                if (!review.Contains("*"))
                {
                    temp.tokens = Tokenizer.SplitString(temp.review);
                }
                else
                    temp.tokens = new List<string>();
                temp.user = userID;

                return temp;
            }
        }

        public static List<Review> readFileAsReview(string filePath)
        {
            return ConvertDataToReview.convertFriendshipReviewDataToReview(readFile(filePath));
        }

        public static List<FriendshipReviewDataType> readFile(string filePath)
        {
            try
            {
                List<FriendshipReviewDataType> dataList = new List<FriendshipReviewDataType>();
                // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string lineRead = sr.ReadLine();
                    while (lineRead != null)
                    {
                        if (lineRead.Contains("user:"))
                        {
                            dataList.Add(new FriendshipReviewDataType());
                            dataList.LastOrDefault().userID = lineRead.Split(':')[1];
                        }

                        else if (lineRead.Contains("friends:"))
                            dataList.LastOrDefault().friendsList = lineRead.Split(':')[1].Split('\t').ToList();

                        else if (lineRead.Contains("summary:"))
                        {
                            dataList.LastOrDefault().summary = lineRead.Split(':')[1];
                        }
                        else if (lineRead.Contains("review:"))
                            dataList.LastOrDefault().review = lineRead.Split(':')[1];
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
