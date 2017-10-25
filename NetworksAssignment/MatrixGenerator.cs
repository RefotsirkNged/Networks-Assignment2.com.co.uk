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
        private List<List<double>> matrix;
        public Dictionary<string, int> nameMapping;

        public MatrixGenerator()
        {
            matrix = new List<List<double>>();
            nameMapping = new Dictionary<string, int>();

            using (StreamReader reader = new StreamReader(File.OpenRead("friendships.txt")))
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

            using (StreamReader reader = new StreamReader(File.OpenRead("friendships.txt")))
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
                                if(friend != string.Empty)
                                    matrix[nameMapping[name]][nameMapping[friend.Trim().ToLower()]] = 1;
                            }
                        }
                    }
                }
            }
        }


        public double[,] GetAdjancencyMatrix()
        {
            double[,] adjacencyMatrix = new double[matrix.Count, matrix.Count];

            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix.Count; j++)
                {
                    adjacencyMatrix[i, j] = matrix[i][j];
                }
            }

            return adjacencyMatrix;
        }

        public double[,] GetDegreeMatrix()
        {
            double[,] degreeMatrix = new double[matrix.Count, matrix.Count];

            for (int i = 0; i < matrix.Count; i++)
            {
                for (int j = 0; j < matrix.Count; j++)
                {
                    degreeMatrix[i,j] = 0;
                }
            }

            int userIndex = 0;
            foreach (List<double> user in matrix)
            {
                foreach (double friend in user)
                {
                    degreeMatrix[userIndex, userIndex] += friend;
                }

                userIndex++;
            }

            return degreeMatrix;
        }
    }
}
