using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using DataAccessLibrary;
using DataAccessLibrary.Module;
using SmartMirror.Features.Joke;
using SmartMirror.Features.Quote;
using SmartMirror.HelperClasses;
using SmartMirror.SpeechRecognition;
using SmartMirrorServer.Features.SunTimes;
using SmartMirrorServer.Features.Weather;

namespace SmartMirror.SpeechSynthesis
{
    internal class SpeechService
    {

        #region Private Fields

        private readonly MediaPlayer speechPlayer;
        private readonly SpeechSynthesizer speechSynthesizer;

        #endregion Private Fields

        #region Public Constructors

        public SpeechService()
        {
            speechSynthesizer = createSpeechSynthesizer();

            speechPlayer = new MediaPlayer { AudioCategory = MediaPlayerAudioCategory.Speech, AutoPlay = false };

            #pragma warning disable 4014
            startup();
            #pragma warning restore 4014
        }

        #endregion Public Constructors

        #region Public Methods

        public async Task CountDown(int fromNumber)
        {
            StringBuilder countdownString = new StringBuilder();

            countdownString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            countdownString.AppendLine(@"<sentence>");

            for (int i = fromNumber; i >= 0; i--)
            {
                if (i == 0)
                {
                    countdownString.AppendLine($"<prosody rate =\"-30%\">{NumberHelper.NumberToWords(i)}</prosody>.");
                    countdownString.AppendLine("<break time='500ms'/>");
                    countdownString.AppendLine("Die Zeit ist abgelaufen. Countdown beendet.");
                }
                else
                {
                    countdownString.AppendLine(NumberHelper.NumberToWords(i));
                    countdownString.AppendLine("<break time='1000ms'/>");
                }
            }

            countdownString.AppendLine(@"</sentence>");
            countdownString.AppendLine(@"</speak>");

            await sayAsyncSsml(countdownString.ToString());
        }

        public async Task CountTo(int toNumber)
        {
            StringBuilder countToString = new StringBuilder();

            countToString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            countToString.AppendLine(@"<sentence>");

            for (int i = 0; i >= toNumber; i--)
            {
                if (i == toNumber)
                {
                    countToString.AppendLine($"<prosody rate =\"-30%\">{NumberHelper.NumberToWords(i)}</prosody>.");
                    countToString.AppendLine("<break time='500ms'/>");
                    countToString.AppendLine("Zielnummer erreicht.");
                }
                else
                {
                    countToString.AppendLine(NumberHelper.NumberToWords(i));
                    countToString.AppendLine("<break time='1000ms'/>");
                }
            }

            countToString.AppendLine(@"</sentence>");
            countToString.AppendLine(@"</speak>");

            await sayAsyncSsml(countToString.ToString());
        }

        public async Task SayCreator()
        {
            StringBuilder weatherTodayString = new StringBuilder();

            weatherTodayString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            weatherTodayString.AppendLine(@"<sentence>");

            weatherTodayString.AppendLine("Gute Frage, das wüsste ich auch gerne. Ich sehe das wie Winston Churchill, er sagte einst: <break time='300ms'/> Ich bin bereit, meinem Schöpfer gegenüberzutreten. Ob mein Schöpfer ebenso bereit ist, diese Begegnung über sich ergehen zu lassen, ist eine andere Sache.");

            weatherTodayString.AppendLine(@"</sentence>");
            weatherTodayString.AppendLine(@"</speak>");

            await sayAsyncSsml(weatherTodayString.ToString());
        }

        public async Task SayGender()
        {

            StringBuilder genderString = new StringBuilder();

            genderString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            genderString.AppendLine(@"<sentence>");

            genderString.AppendLine("Das ist eine gute Frage. Ich muss gestehen, ich weiß es selber nicht einmal so genau.");

            genderString.AppendLine(@"</sentence>");
            genderString.AppendLine(@"</speak>");

            await sayAsyncSsml(genderString.ToString());
        }

        public async Task SayJoke()
        {
            Joke joke = JokeHelper.GetJoke();

            StringBuilder jokeString = new StringBuilder();

            jokeString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            jokeString.AppendLine(@"<sentence>");

            jokeString.AppendLine($"Einen {joke.Title.Remove(joke.Title.Length - 1)} gefällig: <break time='300ms'/><prosody rate=\"-15%\">{joke.Description}</prosody>");

            jokeString.AppendLine(@"</sentence>");
            jokeString.AppendLine(@"</speak>");

            await sayAsyncSsml(jokeString.ToString());
        }

        public async Task SayLook()
        {
            Random randi = new Random();
            int randomNumber = randi.Next(2);

            StringBuilder nameString = new StringBuilder();

            nameString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            nameString.AppendLine(@"<sentence>");

            nameString.AppendLine(randomNumber == 0 ? "Ich fürchte, dass die Beschreibung meines Aussehens einen längeren Ausflug in Themenbereiche zum Raum - Zeit - Kontinuum und zur Mode notwendig machen würde, die dir bis jetzt noch völlig unbekannt sind." : "Mal schauen. <break time='500ms'/> Dacht ich mir's doch, das gleiche wie gestern.");

            nameString.AppendLine(@"</sentence>");
            nameString.AppendLine(@"</speak>");

            await sayAsyncSsml(nameString.ToString());
        }

        public async Task SayMirror()
        {
            Random randi = new Random();
            int randomNumber = randi.Next(2);

            StringBuilder nameString = new StringBuilder();

            nameString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            nameString.AppendLine(@"<sentence>");

            nameString.AppendLine(randomNumber == 0 ? "Geh mal zur Seite, <break time='300ms'/>ich kann nichts sehen!" : "Hier ein Tipp unter Freunden: <break time='500ms'/> Frag heute einfach mal nicht!");

            nameString.AppendLine(@"</sentence>");
            nameString.AppendLine(@"</speak>");

            await sayAsyncSsml(nameString.ToString());
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

        public async Task SayQuote()
        {
            Quote quote = QuoteHelper.GetQuoteOfDay();

            StringBuilder quoteString = new StringBuilder();

            quoteString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            quoteString.AppendLine(@"<sentence>");

            quoteString.AppendLine($"<break time='1500ms'/>{(quote.Author != string.Empty ? quote.Author : "Ein kluge Frau oder ein kluger Mann")} sagte einstmal: <break time='400ms'/> {quote.Text}");

            quoteString.AppendLine(@"</sentence>");
            quoteString.AppendLine(@"</speak>");

            await sayAsyncSsml(quoteString.ToString());
        }

        public async Task SayRandom(int from, int to)
        {
            Random randi = new Random();
            int randomNumber = randi.Next(from, to);

            StringBuilder nameString = new StringBuilder();

            nameString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            nameString.AppendLine(@"<sentence>");

            nameString.AppendLine($"Lass mich nachdenken. <break time='1500ms'/> Ich sage einfach mal <break time='300ms'/><prosody rate=\"-35%\">{NumberHelper.NumberToWords(randomNumber)}</prosody>.");

            nameString.AppendLine(@"</sentence>");
            nameString.AppendLine(@"</speak>");

            await sayAsyncSsml(nameString.ToString());
        }

        public async Task SaySunrise()
        {
            Sun sun = new Sun(DataAccess.GetModule(Modules.TIME));

            StringBuilder sunriseString = new StringBuilder();

            sunriseString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            sunriseString.AppendLine(@"<sentence>");

            int time = DateTime.Compare(DateTime.ParseExact(sun.Sunrise, "H:mm", null, DateTimeStyles.None), DateTime.Now);

            if (time != -1)
            {
                if (time != 0)
                {
                    if (time == 1)
                        sunriseString.AppendLine($"Um {sun.Sunrise} geht die Sonne auf.");
                }
                else
                    sunriseString.AppendLine("In diesem Moment geht die Sonne auf.");
            }
            else
                sunriseString.AppendLine($"Um {sun.Sunrise} ist die Sonne aufgegangen.");


            sunriseString.AppendLine(@"</sentence>");
            sunriseString.AppendLine(@"</speak>");

            await sayAsyncSsml(sunriseString.ToString());
        }

        public async Task SaySunset()
        {
            Sun sun = new Sun(DataAccess.GetModule(Modules.TIME));

            StringBuilder sunsetString = new StringBuilder();

            sunsetString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            sunsetString.AppendLine(@"<sentence>");

            int time = DateTime.Compare(DateTime.ParseExact(sun.Sunset, "H:mm", null, DateTimeStyles.None), DateTime.Now);

            if (time != -1)
            {
                if (time != 0)
                {
                    if (time == 1)
                        sunsetString.AppendLine($"Um {sun.Sunset} geht die Sonne unter.");
                }
                else
                    sunsetString.AppendLine("In diesem Moment geht die Sonne unter.");
            }
            else
                sunsetString.AppendLine($"Um {sun.Sunset} ist die Sonne untergegangen.");

            sunsetString.AppendLine(@"</sentence>");
            sunsetString.AppendLine(@"</speak>");

            await sayAsyncSsml(sunsetString.ToString());
        }

        public async Task SayTime()
        {
            DateTime now = DateTime.Now;

            string time = $"Es ist {now.Hour} Uhr und {now.Minute} Minuten.";

            await sayAsync(time);
        }

        public async Task SayWeatherforecast(RecognizedSpeech recognizedSpeech)
        {
            Module weatherModule = DataAccess.GetModule(Modules.WEATHERFORECAST);

            List<List<FiveDaysForecastResult>> result = await FiveDaysForecast.GetByCityName(weatherModule.City, weatherModule.Country, weatherModule.Language, "metric");

            if (result.Count == 0)
                return;

            // Infos zu heutigen Tag löschen
            if (result.Count > 5)
                result.RemoveAt(0);

            StringBuilder weatherforecastString = new StringBuilder();

            weatherforecastString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            weatherforecastString.AppendLine(@"<sentence>");

            switch (recognizedSpeech.SemanticText.Split(' ')[1])
            {
                case "tomorrow":
                    weatherforecastString.AppendLine($"Morgen wird das Wetter in {result[0][0].City} morgens {result[0][3].Description} mit einer mittleren Temperatur von {Math.Round(result[0].Skip(2).Take(2).Average(innerList => innerList.Temp), 1).ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad.");
                    weatherforecastString.AppendLine($"Mittags {result[0][5].Description} mit einer mittleren Temperatur von {Math.Round(result[0].Skip(4).Take(2).Average(innerList => innerList.Temp), 1).ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad.");
                    weatherforecastString.AppendLine($"Abends {result[0][7].Description} mit einer mittleren Temperatur von {Math.Round(result[0].Skip(6).Take(2).Average(innerList => innerList.Temp), 1).ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad.");
                    break;

                case "dayaftertomorrow":
                    weatherforecastString.AppendLine($"Übermorgen wird das Wetter in {result[1][0].City} morgens {result[1][3].Description} mit einer mittleren Temperatur von {Math.Round(result[1].Skip(2).Take(2).Average(innerList => innerList.Temp), 1).ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad.");
                    weatherforecastString.AppendLine($"Mittags {result[1][5].Description} mit einer mittleren Temperatur von {Math.Round(result[1].Skip(4).Take(2).Average(innerList => innerList.Temp), 1).ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad.");
                    weatherforecastString.AppendLine($"Abends {result[1][7].Description} mit einer mittleren Temperatur von {Math.Round(result[1].Skip(6).Take(2).Average(innerList => innerList.Temp), 1).ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad.");
                    break;

                case "monday":
                case "tuesday":
                case "wednesday":
                case "thursday":
                case "friday":
                case "saturday":
                case "sunday":
                    int weekDay = result.IndexOf(result.First(x => x[0].Date.DayOfWeek == DateTimeHelper.GetNextWeekday(DateTime.Now, DayOfWeekHelper.GetDayOfWeek(recognizedSpeech.SemanticText.Split(' ')[1])).DayOfWeek));

                    if (result[weekDay].Count < 4)
                    {
                        weatherforecastString.AppendLine($"Über den {result[weekDay][0].Date:dddd} liegen leider zu wenig Information vor.");
                        break;
                    }

                    weatherforecastString.AppendLine($"Am {result[weekDay][0].Date:dddd} wird das Wetter in {result[1][0].City} morgens {result[weekDay][3].Description} mit einer mittleren Temperatur von {Math.Round(result[weekDay].Skip(2).Take(2).Average(innerList => innerList.Temp), 1).ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad.");

                    if (result[weekDay].Count < 6)
                    {
                        weatherforecastString.AppendLine($"Weitere Informationen über den {result[weekDay][0].Date:dddd} liegen nicht vor.");
                        break;
                    }

                    weatherforecastString.AppendLine($"Mittags {result[weekDay][5].Description} mit einer mittleren Temperatur von {Math.Round(result[weekDay].Skip(4).Take(2).Average(innerList => innerList.Temp), 1).ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad.");

                    if (result[weekDay].Count < 8)
                    {
                        weatherforecastString.AppendLine($"Weitere Informationen über den {result[weekDay][0].Date:dddd} liegen nicht vor.");
                        break;
                    }

                    weatherforecastString.AppendLine($"Abends {result[weekDay][7].Description} mit einer mittleren Temperatur von {Math.Round(result[weekDay].Skip(6).Take(2).Average(innerList => innerList.Temp), 1).ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad.");
                    break;

                case "all":
                    List<ForecastDays> forecastDays = result.Select(fiveDaysForecastResult => new ForecastDays { City = fiveDaysForecastResult[0].City, CityId = fiveDaysForecastResult[0].CityId, Date = fiveDaysForecastResult[0].Date, Temperature = Math.Round(fiveDaysForecastResult.Average(innerList => innerList.Temp), 1), MinTemp = Math.Round(fiveDaysForecastResult.Min(innerList => innerList.TempMin), 1), MaxTemp = Math.Round(fiveDaysForecastResult.Min(innerList => innerList.TempMax), 1), Icon = fiveDaysForecastResult.GroupBy(x => x.Icon).OrderByDescending(x => x.Count()).First().Key, Description = fiveDaysForecastResult.GroupBy(x => x.Description).OrderByDescending(x => x.Count()).First().Key }).ToList();

                    weatherforecastString.AppendLine($"Morgen wird das Wetter in {forecastDays[0].City} überwiegend {forecastDays[0].Description}. Eine durchschnittliche Tagestemperatur von {forecastDays[0].Temperature} Grad ist zu erwarten.");
                    weatherforecastString.AppendLine($"übermorgen wird das Wetter überwiegend {forecastDays[1].Description}. Eine durchschnittliche Tagestemperatur von {forecastDays[1].Temperature} Grad ist zu erwarten.");
                    weatherforecastString.AppendLine($"Am {forecastDays[2].Date:dddd} wird das Wetter überwiegend {forecastDays[2].Description}. Eine durchschnittliche Tagestemperatur von {forecastDays[2].Temperature} Grad ist zu erwarten.");
                    weatherforecastString.AppendLine($"Am {forecastDays[3].Date:dddd} wird das Wetter überwiegend {forecastDays[3].Description}. Eine durchschnittliche Tagestemperatur von {forecastDays[3].Temperature} Grad ist zu erwarten.");

                    if (forecastDays.Count >= 5)
                        weatherforecastString.AppendLine($"Am {forecastDays[4].Date:dddd} wird das Wetter überwiegend {forecastDays[4].Description}. Eine durchschnittliche Tagestemperatur von {forecastDays[4].Temperature} Grad ist zu erwarten.");
                    break;

                default:
                    List<ForecastDays> allDays = result.Select(fiveDaysForecastResult => new ForecastDays { City = fiveDaysForecastResult[0].City, CityId = fiveDaysForecastResult[0].CityId, Date = fiveDaysForecastResult[0].Date, Temperature = Math.Round(fiveDaysForecastResult.Average(innerList => innerList.Temp), 1), MinTemp = Math.Round(fiveDaysForecastResult.Min(innerList => innerList.TempMin), 1), MaxTemp = Math.Round(fiveDaysForecastResult.Min(innerList => innerList.TempMax), 1), Icon = fiveDaysForecastResult.GroupBy(x => x.Icon).OrderByDescending(x => x.Count()).First().Key, Description = fiveDaysForecastResult.GroupBy(x => x.Description).OrderByDescending(x => x.Count()).First().Key }).ToList();

                    weatherforecastString.AppendLine($"Morgen wird das Wetter in {allDays[0].City} überwiegend {allDays[0].Description}. Eine durchschnittliche Tagestemperatur von {allDays[0].Temperature.ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad ist zu erwarten.");
                    weatherforecastString.AppendLine($"übermorgen wird das Wetter überwiegend {allDays[1].Description}. Eine durchschnittliche Tagestemperatur von {allDays[1].Temperature.ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad ist zu erwarten.");
                    weatherforecastString.AppendLine($"Am {allDays[2].Date:dddd} wird das Wetter überwiegend {allDays[2].Description}. Eine durchschnittliche Tagestemperatur von {allDays[2].Temperature.ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad ist zu erwarten.");
                    weatherforecastString.AppendLine($"Am {allDays[3].Date:dddd} wird das Wetter überwiegend {allDays[3].Description}. Eine durchschnittliche Tagestemperatur von {allDays[3].Temperature.ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad ist zu erwarten.");

                    if (allDays.Count >= 5)
                        weatherforecastString.AppendLine($"Am {allDays[4].Date:dddd} wird das Wetter überwiegend {allDays[4].Description}. Eine durchschnittliche Tagestemperatur von {allDays[4].Temperature.ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad ist zu erwarten.");
                    break;
            }

            weatherforecastString.AppendLine(@"</sentence>");
            weatherforecastString.AppendLine(@"</speak>");

            await sayAsyncSsml(weatherforecastString.ToString());
        }

        public async Task SayWeatherToday()
        {
            Module weatherModule = DataAccess.GetModule(Modules.WEATHER);

            SingleResult<CurrentWeatherResult> result = await CurrentWeather.GetByCityName(weatherModule.City, weatherModule.Country, weatherModule.Language, "metric");

            if (!result.Success)
                return;

            StringBuilder weatherTodayString = new StringBuilder();

            weatherTodayString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            weatherTodayString.AppendLine(@"<sentence>");

            weatherTodayString.AppendLine($"{result.Item.Description} in {result.Item.City}. Momentan werden {result.Item.Temp.ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad Außentemperatur, bei einer Luftfeuchtigkeit von {result.Item.Humidity} Prozent gemessen.");
            weatherTodayString.AppendLine(Math.Abs(result.Item.WindSpeed - double.Epsilon) < 0 ? "Zur Zeit weht kein Wind." : $"Ein Wind mit der Geschwindigkeit von {(Math.Abs(result.Item.WindSpeed - 1) < 0 ? "eins" : result.Item.WindSpeed.ToString(CultureInfo.InvariantCulture).Replace(".", ","))} Meter pro Sekunde weht aus Richtung {WindDirectionHelper.GetWindDirection(result.Item.WindDegree)}");

            weatherTodayString.AppendLine(@"</sentence>");
            weatherTodayString.AppendLine(@"</speak>");

            await sayAsyncSsml(weatherTodayString.ToString());
        }

        #endregion Public Methods

        #region Private Methods

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
            StringBuilder startupString = new StringBuilder();

            startupString.AppendLine(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            startupString.AppendLine(@"<sentence>");

            startupString.AppendLine("Darf ich mich vorstellen ?");
            startupString.AppendLine("<break time='500ms'/>");
            startupString.AppendLine("Mein Name ist <prosody rate=\"-30%\">Mira</prosody>.");
            startupString.AppendLine("<break time='300ms'/>");
            startupString.AppendLine("Wie kann ich dir behilflich <prosody pitch=\"high\">sein</prosody>?");
            startupString.AppendLine("<break time='1000ms'/>");
            startupString.AppendLine("Sprachbefehle, sowie weitere Information kannst du dir mit dem Sprachbefehl <break time='250ms'/> <prosody rate=\"-25%\">Mira zeige Hilfe</prosody> anzeigen lassen.");

            startupString.AppendLine(@"</sentence>");
            startupString.AppendLine(@"</speak>");

            await sayAsyncSsml(startupString.ToString());
        }

        #endregion Private Methods

    }
}