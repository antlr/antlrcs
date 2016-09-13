namespace Antlr4.StringTemplate.Extensions
{
    using System;

    internal static class Arrays
    {
        public static TOutput[] ConvertAll<TInput, TOutput>(TInput[] array, Func<TInput, TOutput> transform)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (transform == null)
                throw new ArgumentNullException("transform");

            TOutput[] result = new TOutput[array.Length];
            for (int i = 0; i < array.Length; i++)
            {
                result[i] = transform(array[i]);
            }

            return result;
        }
    }
}
