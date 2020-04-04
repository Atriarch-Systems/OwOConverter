using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace OwOConverter.StringExtensions.OwOConverter
{
    public static class OwOConverter
    {
        public static string ConvertToOwO(this string input)
        {
            var random = new Random((int)DateTime.UtcNow.Ticks);
            var faces = new List<string>{";;w;;", "owo", "UwU", ">w<", "^w^"};//"(・`ω´・)" doesn't encode right :C

            input = Regex.Replace(input,@"(r|l)", "w", RegexOptions.Multiline);
            input = Regex.Replace(input, @"(R|L)", "W", RegexOptions.Multiline);
            input = Regex.Replace(input,"n([aeiou])", @"ny$1", RegexOptions.Multiline);
            input = Regex.Replace(input,"N([aeiou])", @"Ny$1", RegexOptions.Multiline);
            input = Regex.Replace(input,"N([AEIOU])", @"Ny$1", RegexOptions.Multiline);
            input = Regex.Replace(input,@"(ove)", "uv", RegexOptions.Multiline);
            input = Regex.Replace(input,@"\!+", " " + faces[random.Next(faces.Count)] + " ");
            return input;
        }
    }
}
