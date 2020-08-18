using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OwOConverter.StringExtensions.OwOConverter
{
    public static class OwOConverter
    {
        private static readonly Random RandomInt = new Random((int)DateTime.UtcNow.Ticks);
        private static readonly List<string> Faces = new List<string> { ";w;", "owo", "UwU", ">w<", "^w^", "*w*" };//"(`・ω・´)(・`ω´・)" doesn't encode right :C
        private static readonly MatchEvaluator FaceEvaluator = _ => " "+Faces[RandomInt.Next(Faces.Count)]+" ";
        private static readonly IDictionary<string, string> Map = new Dictionary<string, string>{
            {@"(r|l)", "w"},
            {@"(R|L)", "W"},
            {"n([aeiou])", @"ny$1"},
            {"N([aeiou])", @"Ny$1"},
            {"N([AEIOU])", @"Ny$1"},
            {@"(ove)", "uv"},
            {@"(ou)", "ew"}
        };
        private static string pattern = string.Join("|", Map.Keys);
        public static string ConvertToOwO(this string input)
        {
            input = Regex.Replace(input, pattern, m => Map[m.Value], RegexOptions.Multiline,TimeSpan.FromSeconds(1));
            input = Regex.Replace(input, @"\!+", FaceEvaluator);
            return input;
        }
    }
}
