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

namespace NetworksAssignment
{
    class Program
    {
        static void Main(string[] args)
        {

            
            SentimentModel model = new SentimentModel(ReadSentimentTrainingData.readFileAsReview("small.txt"));

            Console.ReadLine();
        }

        private static void FindCommunities()
        {
            MatrixGenerator mg = new MatrixGenerator();
            List<Tuple<int, double>> eigenVectorMapping = new List<Tuple<int, double>>();

            Matrix<double> AdjacencyArray = DenseMatrix.OfArray(mg.GetAdjancencyMatrix());
            Matrix<double> DegreeArray = DenseMatrix.OfArray(mg.GetDegreeMatrix());

            Control.UseNativeMKL();
            var unnormalizedlaplace = DegreeArray - AdjacencyArray;
            Console.WriteLine("unnormalized");
            Console.WriteLine(unnormalizedlaplace);
            var evdmat = unnormalizedlaplace.Evd();
            Console.WriteLine("Eigenvectors");
            Console.WriteLine(evdmat.EigenVectors);

            Console.WriteLine("Result:");

            for (int i = 0; i < evdmat.EigenVectors.RowCount; i++)
            {
                eigenVectorMapping.Add(new Tuple<int, double>(i, evdmat.EigenVectors[i, 1]));
            }

            eigenVectorMapping = eigenVectorMapping.OrderBy(t => t.Item2).ToList();
            

            List<int> cuts = new List<int>();

            for (int i = 0; i < eigenVectorMapping.Count; i++)
            {
                if (i < eigenVectorMapping.Count - 1)
                {
                    if (((eigenVectorMapping[i + 1].Item2 - eigenVectorMapping[i].Item2) / eigenVectorMapping[i].Item2) * 100 >= 30)
                    {
                        Console.WriteLine("Cut: " + i);
                        cuts.Add(i);
                    }
                }
            }

            List<List<int>> communities = new List<List<int>>();

            int j = 0;
            foreach (int index in cuts)
            {
                communities.Add(new List<int>());

                for (int i = j; i < index; i++)
                {
                    communities.Last().Add(eigenVectorMapping[i].Item1);
                }

                j = index;
            }

            communities.Add(new List<int>());

            for (int i = j; i < eigenVectorMapping.Count; i++)
            {
                communities.Last().Add(eigenVectorMapping[i].Item1);
            }

            foreach (List<int> c in communities)
            {
                Console.WriteLine(c.Count);
            }
        }
    }
}
