namespace SmartMirror.HelperClasses
{
    public static class SpeechHelper
    {
        public static int GetNumberFromWords(string number)
        {
            string[] words = number.Split(' ');

            switch (words.Length)
            {
                case 1:
                    return getSingleWordNumber(words[0]);

                case 2:
                    return getDoubleWordNumber(words[0], words[1]);

                case 3:
                    return getThreeWordNumber(words[0], words[2]);

                case 4:
                    return getFourWordNumber(words[0], words[4]);

                default:
                    return 0;
            }
        }

        private static int getFourWordNumber(string lowNumber, string number)
        {
            string num = getNumber(lowNumber);
            string lastNum = getSingleWordNumber(number).ToString();

            return trygetNumberFromString($"{num}{(lastNum.Length == 1 ? $"0{lastNum}" : lastNum)}");
        }

        private static int getThreeWordNumber(string number, string tenner)
        {
            string firstNumber = getNumber(number);
            string tennerNumber = getTenner(tenner);

            return trygetNumberFromString($"{tennerNumber}{firstNumber}");
        }

        private static int getDoubleWordNumber(string number, string hundert)
        {
            string num = getNumber(number);

            switch (hundert)
            {
                case "hundert":
                    num += "00";
                    break;

                case "tausend":
                    num += "000";
                    break;

                default:
                    return 0;
            }

            return trygetNumberFromString(num);
        }

        private static int trygetNumberFromString(string num)
        {
            int.TryParse(num, out int number);
            return number;
        }

        private static int getSingleWordNumber(string word)
        {
            switch (word)
            {
                case "null":
                    return 0;

                case "eins":
                    return 1;

                case "zwei":
                    return 2;

                case "drei":
                    return 3;

                case "vier":
                    return 4;

                case "fünf":
                    return 5;

                case "sechs":
                    return 6;

                case "sieben":
                    return 7;

                case "acht":
                    return 8;

                case "neun":
                    return 9;

                case "zehn":
                    return 10;

                case "elf":
                    return 11;

                case "zwölf":
                    return 12;

                case "dreizehn":
                    return 13;

                case "vierzehn":
                    return 14;

                case "fünfzehn":
                    return 15;

                case "sechzehn":
                    return 16;

                case "siebzehn":
                    return 17;

                case "achtzehn":
                    return 18;

                case "neunzehn":
                    return 19;

                case "zwanzig":
                    return 20;

                case "dreißig":
                    return 30;

                case "vierzig":
                    return 40;

                case "fünfzig":
                    return 50;

                case "sechzig":
                    return 60;

                case "siebzig":
                    return 70;

                case "achtzig":
                    return 80;

                case "neunzig":
                    return 90;

                case "hundert":
                    return 100;

                case "tausend":
                    return 1000;

                default:
                    return 0;
            }
        }

        private static string getNumber(string number)
        {
            switch (number)
            {
                case "ein":
                    return "1";

                case "eins":
                    return "1";

                case "zwei":
                    return "2";

                case "drei":
                    return "3";
                    
                case "vier":
                    return "4";
                    
                case "fünf":
                    return "5";
                    
                case "sechs":
                    return "6";
                    
                case "sieben":
                    return "7";
                    
                case "acht":
                    return "8";
                    
                case "neun":
                    return "9";
                    
                default:
                    return string.Empty;
            }
        }

        private static string getTenner(string tenner)
        {
            switch (tenner)
            {
                case "zehn":
                    return "1";

                case "zwanzig":
                    return "2";

                case "dreißig":
                    return "3";

                case "vierzig":
                    return "4";

                case "fünfzig":
                    return "5";

                case "sechzig":
                    return "6";

                case "siebzig":
                    return "7";

                case "achtzig":
                    return "8";

                case "neunzig":
                    return "9";

                default:
                    return string.Empty;
            }
        }
    }
}