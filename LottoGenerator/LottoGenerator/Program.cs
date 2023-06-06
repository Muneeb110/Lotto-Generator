using System;
using System.Linq;

namespace LottoGenerator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int a = 49;
            int b = 6;
            string x = "44 45 46 47 48 49";
            int y = 13983816;
            Rank rank = new Rank();
            int res = rank.GetRank(a, b, Array.ConvertAll(x.Split(' '), int.Parse));
            Console.WriteLine("Rank is:" + res);

            Combination comb = new Combination();   
            int[] result = comb.GetCombination(a, b, y);
            string response = result.Aggregate(
                "", // start with empty string to handle empty list case.
                (current, next) => current + " " + next);
            Console.WriteLine("Combination is:" + response);
            Console.ReadLine();
        }
    }
    internal class Addition
    {
        public static int Add(int a, int b)
        {
            return a + b;
        }
    }

    public static class Binomials
    {
        //  /   \
        // |  n  |  from n
        // |     |
        // |  k  |  choose k
        //  \   /
        // Kudos: https://en.wikipedia.org/wiki/Combination#Example_of_counting_combinations
        /// <summary>Calculates the Binomial Coefficient (n choose k), which is the number of unique k-sized combinations in a set of n elements.</summary>
        /// <remarks>We divide before multiplying to minimise the chance of overflow.</remarks>
        public static int CalculateBinomialCoefficient(int n, int k)
        {
            if (k > n)
                return 0;

            if (k == n)
                return 1;

            double result = n;  // n choose 1
            for (double i = 1; i < k; i++)
            {
                var numerator = Subtraction.Sub(n, i);
                double factor = Division.Div(numerator, (i + 1));
                result = Multiplication.Mul(result, factor);
            }

            return (int)result;
        }



        //// Extensions


        /// <summary>Returns the number (n) choose k</summary>
        public static int Choose(this int n, int k) =>
            CalculateBinomialCoefficient(n, k);

        /// <summary>Calculates the number of unique combinations of size k from n numbers (k-combination)</summary>
        public static int CountUniqueCombinationsOfSize(this int n, int k) =>
            CalculateBinomialCoefficient(n, k);
    }

    public class Combination
    {
        /// <summary>Returns the k-combination of (n choose k) with the provided rank</summary>
        public int[] GetCombination(int n, int k, int rank)
        {
            var dualOfZero = Subtraction.Sub(n, 1);



            //Put condition if rank >= n.Choose(k) then error 
            if (rank < 1)
                return new int[] { -1 };
            if (rank > n.Choose(k))
                return new int[] { -1 };
            // Move to base 0
            rank--;
            int maxRank = GetMaxRank(n, k, rank);

            // Calculate combinadic of the dual
            var combination = CalculateCombinadic(maxRank, k, n);

            for (int i = 0; i < k; i++)
            {
                // Map to zero-based combination
                combination[i] = Subtraction.Sub(dualOfZero, combination[i]);

                // Add 1 (for base 1)
                combination[i] = Addition.Add(combination[i], 1);
            }

            return combination;

        }

        private static int GetMaxRank(int n, int k, int rank)
        {
            // Calculate the dual
            return Subtraction.Sub(n.Choose(k), 1, rank);
        }

        /// Local Functions


        /// <summary>Calculates zero-based array of c such that maxRank = (c1 choose k-1) + (c2 choose k-2) + ... (c[of k-1] choose 1) </summary>
        int[] CalculateCombinadic(int maxRank, int k, int n)
        {
            var result = new int[k];
            var diminishingRank = maxRank;
            var reducingK = k;

            for (int i = 0; i < k; i++)
            {
                result[i] = CalculateLargestRankBelowThreshold(n, reducingK, diminishingRank);

                diminishingRank -= result[i].Choose(reducingK);
                reducingK--;
            }

            return result;
        }

        /// <summary>Returns the highest rank of n2 choose k2 that is less than the threshold</summary>
        int CalculateLargestRankBelowThreshold(int n2, int k2, int threshold)
        {
            int i = Subtraction.Sub(n2, 1);

            while (i.Choose(k2) > threshold)
                i--;

            return i;
        }
    }
    internal class Division
    {
        public static double Div(double a, double b)
        {
            return a / b;
        }
    }

    internal class Multiplication
    {
        public static double Mul(double a, double b)
        {
            return a * b;
        }
    }
    internal class Rank
    {
        /// <summary>Returns the rank of the provided k-combination</summary>
        public int GetRank(int n, int k, int[] combination)
        {
            var dualOfZero = Subtraction.Sub(n, 1);

            var result = new int[k];
            combination.CopyTo(result, 0);

            var dualOfCombinadic = 0;
            var reducingK = k;
            for (int i = 0; i < k; i++)
            {
                // Take 1 (for base 0)
                result[i] -= 1;

                // Map to combinadic
                result[i] = Subtraction.Sub(dualOfZero, result[i]);

                // Calculate dual of combinadic (by accumulation)
                dualOfCombinadic += result[i].Choose(reducingK);
                reducingK--;
            }

            // Calculate the dual
            var dual = Subtraction.Sub(n.Choose(k), 1, dualOfCombinadic);

            // Add 1 (for base 1)
            dual++;

            return dual;
        }
    }

    internal class Subtraction
    {
        public static int Sub(params int[] numbers)
        {
            int result = numbers[0];
            for (int i = 1; i < numbers.Length; i++)
            {
                result = result - numbers[i];
            }
            return result;
        }

        public static double Sub(int n, double i)
        {
            return n - i;
        }
    }
}
