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
        public MatrixGenerator mg;
        public Dictionary<int, List<int>> friendships;

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



            Dictionary<int, List<int>> communityFriendships1 = new Dictionary<int, List<int>>();

            foreach (int user in intialCommunities[0])
            {
                communityFriendships1.Add(user, friendships[user]);
            }

            AdjacencyArray = DenseMatrix.OfArray(mg.GetAdjancencyMatrix(communityFriendships1));
            DegreeArray = DenseMatrix.OfArray(mg.GetDegreeMatrix(communityFriendships1));
            unnormalizedlaplace = DegreeArray - AdjacencyArray;
            results.AddRange(FindCommunitiesHelper(unnormalizedlaplace));

            Dictionary<int, List<int>> communityFriendships2 = new Dictionary<int, List<int>>();

            foreach (int user in intialCommunities[1])
            {
                communityFriendships2.Add(user, friendships[user]);
            }

            AdjacencyArray = DenseMatrix.OfArray(mg.GetAdjancencyMatrix(communityFriendships2));
            DegreeArray = DenseMatrix.OfArray(mg.GetDegreeMatrix(communityFriendships2));
            unnormalizedlaplace = DegreeArray - AdjacencyArray;
            results.AddRange(FindCommunitiesHelper(unnormalizedlaplace));

            //foreach (List<int> community in intialCommunities)
            //{
            //    Dictionary<int, List<int>> communityFriendships = new Dictionary<int, List<int>>();

            //    foreach (int user in community)
            //    {
            //        communityFriendships.Add(user, friendships[user]);
            //    }

            //    AdjacencyArray = DenseMatrix.OfArray(mg.GetAdjancencyMatrix(communityFriendships));
            //    DegreeArray = DenseMatrix.OfArray(mg.GetDegreeMatrix(communityFriendships));
            //    unnormalizedlaplace = DegreeArray - AdjacencyArray;
            //    FindCommunitiesHelper(unnormalizedlaplace);
            //    //results.AddRange();
            //}

            return results;
        }

        private List<List<int>> FindCommunitiesHelper(Matrix<double> unnormalizedlaplace)
        {
            List<Tuple<int, double>> eigenVectorMapping = new List<Tuple<int, double>>();
           
            Control.UseNativeMKL();
            
            Console.WriteLine("unnormalized");
            Console.WriteLine(unnormalizedlaplace);
            Evd<Double> evdmat = unnormalizedlaplace.Evd();
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
        }
    }
}
