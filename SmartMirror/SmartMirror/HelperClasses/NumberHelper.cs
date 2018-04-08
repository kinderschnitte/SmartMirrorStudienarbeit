using System;

namespace SmartMirror.HelperClasses
{
    public static class NumberHelper
    {
        public static string NumberToWords(int number)
        {
            if (number == 0)
                return "null";

            if (number < 0)
                return "minus " + NumberToWords(Math.Abs(number));

            string words = "";

            if (number / 1000000 > 0)
            {
                words += NumberToWords(number / 1000000) + " millionen ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += NumberToWords(number / 1000) + " tausend ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += NumberToWords(number / 100) + " hundert ";
                number %= 100;
            }

            if (number <= 0)
                return words;

            if (words != "")
                words += "und ";

            string[] unitsMap = { "null", "eins", "zwei", "drei", "vier", "fünf", "sechs", "sieben", "acht", "neun", "zehn", "elf", "zwölf", "dreizehn", "vierzehn", "fünfzehn", "sechzehn", "siebzehn", "achtzehn", "neunzehn" };
            string[] tensMap = { "null", "zehn", "zwanzig", "dreißig", "vierzig", "fünfzig", "sechzig", "siebzig", "achtzig", "neunzig" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if (number % 10 > 0)
                    words += "-" + unitsMap[number % 10];
            }

            return words;
        }
    }
}