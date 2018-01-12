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
using System.IO;

namespace NetworksAssignment
{
    class MatrixGenerator
    {
        public Dictionary<string, int> nameMapping;
        int size;

        public MatrixGenerator()
        {

        }

        public Dictionary<int, List<int>> GenerateFriendships()
        {
            Dictionary<int, List<int>> friendships = new Dictionary<int, List<int>>();

            nameMapping = new Dictionary<string, int>();
            size = nameMapping.Count;

            int userID = 0;
            using (StreamReader reader = new StreamReader(File.OpenRead("friendships.reviews.txt")))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    //if line contains user, check if user exists and add if not else continue
                    if (line.Contains("user:"))
                    {
                        string name = line.Split(':').Last().Trim().ToLower();

                        if (!nameMapping.Keys.Contains(name) && name != string.Empty)
                        {
                            nameMapping.Add(name, userID++);
                            friendships.Add(nameMapping[name], new List<int>());
                        }
                    }
                }
            }

            using (StreamReader reader = new StreamReader(File.OpenRead("friendships.reviews.txt")))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    //if line contains user, check if user exists and add if not else continue
                    if (line.Contains("user:"))
                    {
                        string name = line.Split(':').Last().Trim().ToLower();
                        line = reader.ReadLine();

                        if (line.Contains("friends:"))
                        {
                            foreach (string friend in line.Split(':').Last().Split('\t'))
                            {
                                if (friend != string.Empty)
                                    friendships[nameMapping[name]].Add(nameMapping[friend.Trim().ToLower()]);
                            }
                        }
                    }
                }
            }

            return friendships;
        }

        public List<List<double>> GenerateMatrix()
        {
            List<List<double>> matrix = new List<List<double>>();
            nameMapping = new Dictionary<string, int>();

            using (StreamReader reader = new StreamReader(File.OpenRead("friendships.reviews.txt")))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    //if line contains user, check if user exists and add if not else continue
                    if (line.Contains("user:"))
                    {
                        string name = line.Split(':').Last().Trim().ToLower();

                        if (!nameMapping.Keys.Contains(name) && name != string.Empty)
                        {
                            matrix.Add(new List<double>());
                            nameMapping.Add(name, matrix.Count - 1);
                        }
                    }
                }
            }

            foreach (List<double> user in matrix)
            {
                for (int i = 0; i < matrix.Count; i++)
                {
                    user.Add(0);
                }
            }

            using (StreamReader reader = new StreamReader(File.OpenRead("friendships.reviews.txt")))
            {
                string line = "";
                while ((line = reader.ReadLine()) != null)
                {
                    //if line contains user, check if user exists and add if not else continue
                    if (line.Contains("user:"))
                    {
                        string name = line.Split(':').Last().Trim().ToLower();
                        line = reader.ReadLine();

                        if (line.Contains("friends:"))
                        {
                            foreach (string friend in line.Split(':').Last().Split('\t'))
                            {
                                if (friend != string.Empty)
                                    matrix[nameMapping[name]][nameMapping[friend.Trim().ToLower()]] = 1;
                            }
                        }
                    }
                }
            }

            return matrix;
        }

        public double[,] GetAdjancencyMatrix(Dictionary<int, List<int>> friendships)
        {
            double[,] adjacencyMatrix = new double[friendships.Keys.Count, friendships.Keys.Count];
            
            for (int i = 0; i < friendships.Keys.Count; i++)
            {
                for (int j = 0; j < friendships.Keys.Count; j++)
                {
                    if (friendships[i].Contains(j))
                        adjacencyMatrix[i, j] = 1;
                    else
                        adjacencyMatrix[i, j] = 0;
                }
            }

            return adjacencyMatrix;
        }

        public double[,] GetDegreeMatrix(Dictionary<int, List<int>> friendships)
        {
            double[,] degreeMatrix = new double[friendships.Keys.Count, friendships.Keys.Count];

            for (int i = 0; i < friendships.Keys.Count; i++)
            {
                for (int j = 0; j < friendships.Keys.Count; j++)
                {
                    degreeMatrix[i, j] = 0;
                }
            }
            
            foreach (int user in friendships.Keys)
            {
                degreeMatrix[user, user] += friendships[user].Count;
            }

            return degreeMatrix;
        }
    }
}
