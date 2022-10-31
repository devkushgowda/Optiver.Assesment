using System;
using System.Collections.Generic;

namespace Optiver.Assesment
{
    public class SExpressionValidator
    {
        public class ExpressionException : Exception
        {
            public ExpressionException(string message) : base(message)
            {

            }
        }

        public const char FirstAlphabet = 'A';
        public const int MaxLength = 26;
        public const string E1 = "E1";
        public const string E2 = "E2";
        public const string E3 = "E3";
        public const string E4 = "E4";
        public const string E5 = "E5";

        /// <summary>
        /// Performs the given task.
        /// </summary>
        /// <param name="sExpression"></param>
        /// <returns></returns>
        public static string ValidateSExpression(string sExpression)
        {
            var binaryTree = new bool[MaxLength, MaxLength];
            HashSet<char> uniqueNodes = new HashSet<char>();

            try
            {
                //E1 & E2
                ValidateFormatAndDuplicatePair(sExpression, binaryTree, uniqueNodes);

                //E3
                ValidateBinaryTree(binaryTree);

                bool containsCycle = false;
                int rootNums = 0;
                char root = ' ';

                foreach (var n in uniqueNodes)
                {
                    for (int i = 0; i < MaxLength; i++)
                    {
                        if (binaryTree[i, n - FirstAlphabet])
                        {
                            break;
                        }

                        if (i == MaxLength - 1)
                        {
                            rootNums++;
                            if (rootNums > 1)
                                break;
                            root = n;
                            containsCycle = ContainsCycle(n, binaryTree, new bool[MaxLength]);
                        }
                    }
                }

                if (rootNums > 1)
                    throw new ExpressionException(E4);

                if (containsCycle)
                    throw new ExpressionException(E5);

                return BuildSExpression(root, binaryTree);
            }
            catch (ExpressionException ex)
            {
                return ex.Message;
            }
        }

        /// <summary>
        /// Performs char addition.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private static char IntToAlphabet(int x, char c = FirstAlphabet)
        {
            return (char)(c + x);
        }

        /// <summary>
        /// Validates whether the given tree is binary or not.
        /// </summary>
        /// <param name="binaryTree"></param>
        private static void ValidateBinaryTree(bool[,] binaryTree)
        {
            for (int i = 0; i < MaxLength; i++)
            {
                int count = 0;

                for (int j = 0; j < MaxLength; j++)
                {
                    if (binaryTree[i, j])
                    {
                        count++;
                    }
                }

                if (count > 2)
                {
                    throw new ExpressionException(E3);
                }
            }
        }

        /// <summary>
        /// Validates the E1 & E2
        /// </summary>
        /// <param name="sExpression"></param>
        /// <param name="binaryTree"></param>
        /// <param name="uniqueNodes"></param>
        private static void ValidateFormatAndDuplicatePair(string sExpression, bool[,] binaryTree, HashSet<char> uniqueNodes)
        {
            if (sExpression == null || sExpression.Length < 5)
                throw new ExpressionException(E1);

            try
            {
                for (int i = 1; i < sExpression.Length; i += 6)
                {
                    char firstChar, secondChar;

                    ValidateFormat(sExpression, i, out firstChar, out secondChar);

                    int parent = firstChar - FirstAlphabet;
                    int child = secondChar - FirstAlphabet;

                    if (binaryTree[parent, child])
                    {
                        throw new ExpressionException(E2);
                    }

                    binaryTree[parent, child] = true;
                    uniqueNodes.Add(IntToAlphabet(parent));
                    uniqueNodes.Add(IntToAlphabet(parent));
                }
            }
            catch (InvalidOperationException)
            {
                throw new ExpressionException(E1);
            }
        }

        /// <summary>
        /// Validates the input string format.
        /// </summary>
        /// <param name="sExpression"></param>
        /// <param name="i"></param>
        /// <param name="firstChar"></param>
        /// <param name="secondChar"></param>
        private static void ValidateFormat(string sExpression, int i, out char firstChar, out char secondChar)
        {
            var openBrace = sExpression[i - 1];
            firstChar = sExpression[i];
            var comma = sExpression[i + 1];
            secondChar = sExpression[i + 2];
            var closeBrace = sExpression[i + 3];
            var space = i + 4 < sExpression.Length ? sExpression[i + 4] : ' ';  //For last pair there wont be a space
            //Regex can be used but this is efficient in no extra time.
            if (openBrace != '(' || !char.IsUpper(firstChar) || !char.IsUpper(secondChar) ||
                    closeBrace != ')' || comma != ',' || space != ' ')
                throw new InvalidOperationException();
        }

        /// <summary>
        /// Returns whether the tree contains cycle.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="binaryTree"></param>
        /// <param name="visited"></param>
        /// <returns></returns>
        private static bool ContainsCycle(char node, bool[,] binaryTree, bool[] visited)
        {
            if (visited[node - FirstAlphabet])
            {
                return true;
            }

            visited[node - FirstAlphabet] = true;

            for (int i = 0; i < MaxLength; i++)
            {
                if (binaryTree[node - FirstAlphabet, i])
                {
                    if (ContainsCycle(IntToAlphabet(i), binaryTree, visited))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns lexicographically smallest S-Expression.
        /// </summary>
        /// <param name="root"></param>
        /// <param name="binaryTree"></param>
        /// <returns></returns>
        private static string BuildSExpression(char root, bool[,] binaryTree)
        {
            string lVal = "";
            string rVal = "";

            for (int i = 0; i < MaxLength; i++)
            {
                if (binaryTree[root - FirstAlphabet, i])
                {
                    lVal = BuildSExpression(IntToAlphabet(i), binaryTree);

                    for (int j = i + 1; j < MaxLength; j++)
                    {
                        if (binaryTree[root - FirstAlphabet, j])
                        {
                            rVal = BuildSExpression(IntToAlphabet(j), binaryTree);
                            break;
                        }
                    }
                    break;
                }
            }
            return $"({root}{lVal}{rVal})";
        }


        /// <summary>
        /// Method to test different edge cases.
        /// </summary>
        internal static void Test()
        {
            /*
            E1      Invalid Input Format
            E2      Duplicate Pair
            E3      Parent Has More than Two Children
            E4      Multiple Roots
            E5      Input Contains Cycle
            
            */
            var expressions = new List<string> { "(A,B) (B,D) (D,E) (A,C) (C,F) (E,G)",
                "(A,B) (A,C) (B,D) (D,C)", "XYZ", "(A,B) (B,C)", "(A,BX", "(A,b)", "(A, b)" , "(A, B)" , "(A,B)|(B,C)", "(A,B) (X,C)",
            "", "A", "A,B", "(A)", "(A,B)", "[A,B]","(a,b)","(A,B) (B,C)","(A,B) (B,C) (B,C)","(A,B) (A,C) (B,D) (B,E) (B,F)","(A,B) (A,C) (B,D) (B,E) (F,C)","(A,B) (A,C) (B,D) (B,E) (E,C)"};
            expressions.ForEach(e => Console.WriteLine($"{e}\n{ValidateSExpression(e)}\n\n"));
        }
    }
}
