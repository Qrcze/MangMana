using System;
using System.IO;

namespace MangMana.Helpers
{
    internal static class SimpleUID
    {
        public static string Generate(Predicate<string> condition)
        {
            string uid;

            do
            {
                uid = Path.GetRandomFileName().Replace(".", "");
            } while (!condition(uid));

            return uid;
        }
    }
}