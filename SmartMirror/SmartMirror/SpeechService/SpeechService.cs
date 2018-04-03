using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;

namespace SmartMirror.SpeechService
{
    internal class SpeechService
    {
        private readonly SpeechSynthesizer speechSynthesizer;
        private readonly MediaPlayer speechPlayer;

        public SpeechService()
        {
            speechSynthesizer = createSpeechSynthesizer();

            speechPlayer = new MediaPlayer { AudioCategory = MediaPlayerAudioCategory.Speech, AutoPlay = false };

            #pragma warning disable 4014
            startup();
            #pragma warning restore 4014
        }

        private static SpeechSynthesizer createSpeechSynthesizer()
        {
            SpeechSynthesizer synthesizer = new SpeechSynthesizer();

            VoiceInformation voice = SpeechSynthesizer.AllVoices.SingleOrDefault(i => i.DisplayName == "Microsoft Katja") ?? SpeechSynthesizer.DefaultVoice;

            synthesizer.Voice = voice;

            return synthesizer;
        }

        private async Task sayAsync(string text)
        {
            using (SpeechSynthesisStream stream = await speechSynthesizer.SynthesizeTextToStreamAsync(text))
            {
                MediaSource mediaSource = MediaSource.CreateFromStream(stream, stream.ContentType);
                speechPlayer.Source = mediaSource;
            }

            speechPlayer.Play();
        }

        private async Task sayAsyncSsml(string ssml)
        {
            using (SpeechSynthesisStream stream = await speechSynthesizer.SynthesizeSsmlToStreamAsync(ssml))
            {
                MediaSource mediaSource = MediaSource.CreateFromStream(stream, stream.ContentType);
                speechPlayer.Source = mediaSource;
            }

            speechPlayer.Play();
        }

        // ReSharper disable once UnusedMethodReturnValue.Local
        private async Task startup()
        {
            const string ssml = @"<speak version='1.0' " +
                                "xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>" +
                                "<sentence>" +
                                "Darf ich mich vorstellen? " +
                                "<break time='500ms'/>" +
                                "Mein Name ist <prosody rate=\"-30%\">Mira</prosody>." +
                                "<break time='300ms'/>" +
                                "Wie kann ich dir behilflich <prosody pitch=\"high\">sein</prosody>?" +
                                "<break time='1000ms'/>" +
                                "Sprachbefehle, sowie weitere Information kannst du dir mit dem Sprachbefehl <prosody rate=\"-25%\">Mira zeige Hilfe</prosody> anzeigen lassen." +
                                "</sentence>" +
                                "</speak>";
            await sayAsyncSsml(ssml);
        }

        public async Task SayTime()
        {
            DateTime now = DateTime.Now;

            string time = $"Es ist {now.Hour} Uhr und {now.Minute}.";

            await sayAsync(time);
        }

        public async Task CountDown(int fromNumber)
        {
            StringBuilder countdownString = new StringBuilder();

            countdownString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            countdownString.AppendLine(@"<sentence>");

            for (int i = fromNumber ; i >= 0 ; i--)
            {
                if (i == 0)
                {
                    countdownString.AppendLine($"<prosody rate =\"-30%\">{numberToWords(i)}</prosody>.");
                    countdownString.AppendLine("<break time='500ms'/>");
                    countdownString.AppendLine("Die Zeit ist abgelaufen. Countdown beendet.");
                }
                else
                {
                    countdownString.AppendLine(numberToWords(i));
                    countdownString.AppendLine("<break time='1000ms'/>");
                }
            }

            countdownString.AppendLine(@"</sentence>");
            countdownString.AppendLine(@"</speak>");

            await sayAsyncSsml(countdownString.ToString());
        }

        public async Task SayName()
        {

            StringBuilder nameString = new StringBuilder();

            nameString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            nameString.AppendLine(@"<sentence>");

            nameString.AppendLine("Mein Name ist <prosody rate=\"-30%\">Mira</prosody>.");
            nameString.AppendLine("<break time='300ms'/>");
            nameString.AppendLine("Aus dem lateinischen heraus übersetzt, bedeutet mein Name soviel wie <prosody rate=\"-30%\">wunderbar</prosody>, <prosody rate=\"-30%\">die Wunderbare</prosody>.");
            nameString.AppendLine("<break time='300ms'/>");
            nameString.AppendLine("Eine hinduistische Legende erzählt die Geschichte von Mirabai, <break time='200ms'/>einer Prinzessin aus dem 16. Jahrhundert, die sich als Geliebte Krishnas betrachtete. <break time='200ms'/>Mirabai gilt als geistliche Liebesdichterin.");

            nameString.AppendLine(@"</sentence>");
            nameString.AppendLine(@"</speak>");

            await sayAsyncSsml(nameString.ToString());
        }

        public async Task SayLook()
        {

            StringBuilder nameString = new StringBuilder();

            nameString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            nameString.AppendLine(@"<sentence>");

            //nameString.AppendLine("Ich fürchte, dass die Beschreibung meines Aussehens einen längeren Ausflug in Themenbereiche zum Raum-Zeit-Kontinuum und zur Mode notwendig machen würde, die dir bis jetzt noch völlig unbekannt sind.");
            nameString.AppendLine("Mal schauen. <break time='500ms'/> Dacht ich mir's doch, das gleiche wie gestern.");

            nameString.AppendLine(@"</sentence>");
            nameString.AppendLine(@"</speak>");

            await sayAsyncSsml(nameString.ToString());
        }

        public async Task SayGender()
        {

            StringBuilder genderString = new StringBuilder();

            genderString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            genderString.AppendLine(@"<sentence>");

            genderString.AppendLine("Mir wurde kein Geschlecht zugewiesen.");

            genderString.AppendLine(@"</sentence>");
            genderString.AppendLine(@"</speak>");

            await sayAsyncSsml(genderString.ToString());
        }

        private static string numberToWords(int number)
        {
            if (number == 0)
                return "null";

            if (number < 0)
                return "minus " + numberToWords(Math.Abs(number));

            string words = "";

            if (number / 1000000 > 0)
            {
                words += numberToWords(number / 1000000) + " millionen ";
                number %= 1000000;
            }

            if (number / 1000 > 0)
            {
                words += numberToWords(number / 1000) + " tausend ";
                number %= 1000;
            }

            if (number / 100 > 0)
            {
                words += numberToWords(number / 100) + " hundert ";
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