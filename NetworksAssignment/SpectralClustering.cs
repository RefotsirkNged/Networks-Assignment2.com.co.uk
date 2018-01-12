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
    class SpectralClustering
    {
        MatrixGenerator mg;
        Dictionary<int, List<int>> friendships;

        public SpectralClustering()
        {
            mg = new MatrixGenerator();

            friendships = mg.GenerateFriendships();

            
           

            
        }

        public List<List<int>> FindCommunities()
        {
            List<List<int>> results = new List<List<int>>();
            List<List<int>> intialCommunities;


            Matrix<double> AdjacencyArray = DenseMatrix.OfArray(mg.GetAdjancencyMatrix(friendships));
            Matrix<double> DegreeArray = DenseMatrix.OfArray(mg.GetDegreeMatrix(friendships));
            Matrix<double> unnormalizedlaplace = DegreeArray - AdjacencyArray;

            intialCommunities = FindCommunitiesHelper(unnormalizedlaplace);

            foreach (List<int> community in intialCommunities)
            {
                Dictionary<int, List<int>> communityFriendships = new Dictionary<int, List<int>>();

                foreach (int user in community)
                {
                    communityFriendships.Add(user, friendships[user]);
                }

                AdjacencyArray = DenseMatrix.OfArray(mg.GetAdjancencyMatrix(communityFriendships));
                DegreeArray = DenseMatrix.OfArray(mg.GetDegreeMatrix(communityFriendships));
                unnormalizedlaplace = DegreeArray - AdjacencyArray;

                results.AddRange(FindCommunitiesHelper(unnormalizedlaplace));
            }

            return results;
        }

        private List<List<int>> FindCommunitiesHelper(Matrix<double> unnormalizedlaplace)
        {
            List<Tuple<int, double>> eigenVectorMapping = new List<Tuple<int, double>>();
           
            Control.UseNativeMKL();
            
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

            List<List<int>> communities = new List<List<int>>();

            communities.Add(new List<int>());
            foreach (Tuple<int, double> mapping in eigenVectorMapping.Where(x => x.Item2 < 0).ToList())
            {
                communities.Last().Add(mapping.Item1);
            }

            communities.Add(new List<int>());
            foreach (Tuple<int, double> mapping in eigenVectorMapping.Where(x => x.Item2 > 0).ToList())
            {
                communities.Last().Add(mapping.Item1);
            }

            return communities;

            //for (int i = 0; i < eigenVectorMapping.Count; i++)
            //{
            //    Console.WriteLine(eigenVectorMapping[i].Item2);
            //    if (i < eigenVectorMapping.Count - 1)
            //    {
            //        if (((eigenVectorMapping[i + 1].Item2 - eigenVectorMapping[i].Item2) / eigenVectorMapping[i].Item2) * 100 >= 30)
            //        {
            //            Console.WriteLine("Cut: " + i);
            //            cuts.Add(i);
            //        }
            //    }
            //}

            //List<List<int>> communities = new List<List<int>>();

            //int j = 0;
            //foreach (int index in cuts)
            //{
            //    communities.Add(new List<int>());

            //    for (int i = j; i < index; i++)
            //    {
            //        communities.Last().Add(eigenVectorMapping[i].Item1);
            //    }

            //    j = index;
            //}

            //communities.Add(new List<int>());

            //for (int i = j; i < eigenVectorMapping.Count; i++)
            //{
            //    communities.Last().Add(eigenVectorMapping[i].Item1);
            //}
        }
    }
}
