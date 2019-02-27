using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Reflection.Tasks
{
    public class CodeGeneration
    {
        /// <summary>
        /// Returns the functions that returns vectors' scalar product:
        /// (a1, a2,...,aN) * (b1, b2, ..., bN) = a1*b1 + a2*b2 + ... + aN*bN
        /// Generally, CLR does not allow to implement such a method via generics to have one function for various number types:
        /// int, long, float. double.
        /// But it is possible to generate the method in the run time! 
        /// See the idea of code generation using Expression Tree at: 
        /// http://blogs.msdn.com/b/csharpfaq/archive/2009/09/14/generating-dynamic-methods-with-expression-trees-in-visual-studio-2010.aspx
        /// </summary>
        /// <typeparam name="T">number type (int, long, float etc)</typeparam>
        /// <returns>
        ///   The function that return scalar product of two vectors
        ///   The generated dynamic method should be equal to static MultuplyVectors (see below).   
        /// </returns>
        public static Func<T[], T[], T> GetVectorMultiplyFunction<T>() where T : struct
        {
            ParameterExpression t1 = Expression.Parameter(typeof(T[]), "t1");
            ParameterExpression t2 = Expression.Parameter(typeof(T[]), "t2");

            ParameterExpression result = Expression.Variable(typeof(T), "result");
            ParameterExpression i = Expression.Parameter(typeof(int), "i");

            LabelTarget label = Expression.Label(typeof(T));

            Expression expression = 
                Expression.Block(new[] { result, i },
                                 Expression.Loop(
                                     Expression.IfThenElse(
                                        Expression.GreaterThan(Expression.ArrayLength(t1), i),
                                        Expression.Block(Expression.AddAssign(result,
                                            Expression.Multiply(Expression.ArrayAccess(t1, i), 
                                                                Expression.ArrayAccess(t2, i))),
                                                                Expression.PostIncrementAssign(i)),
                                        Expression.Break(label, result)),
                                     label));
            return Expression.Lambda<Func<T[], T[], T>>(expression, t1, t2).Compile();
        }


        // Static solution to check performance benchmarks
        public static int MultuplyVectors(int[] first, int[] second)
        {
            int result = 0;
            for (int i = 0; i < first.Length; i++)
            {
                result += first[i] * second[i];
            }
            return result;
        }

    }
}
