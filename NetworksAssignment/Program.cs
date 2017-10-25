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

namespace NetworksAssignment
{
    class Program
    {
        static void Main(string[] args)
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

            foreach (Tuple<int, double> value in eigenVectorMapping)
            {
                Console.WriteLine(value.Item1 + ": " + value.Item2);
            }


            Console.ReadLine();
        }
    }
}
