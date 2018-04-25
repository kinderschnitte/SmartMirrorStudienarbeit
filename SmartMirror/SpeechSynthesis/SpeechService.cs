using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;
using Api;
using Api.HelperClasses;
using Api.Joke;
using Api.Quote;
using Api.Sun;
using Api.Weather;
using DataAccessLibrary.Module;
using Speechservice.HelperClasses;

namespace Speechservice
{
    public static class SpeechService
    {

        #region Public Methods

        public static async Task BadlyUnderstood()
        {
            const string excuse = "Lückenfüller Lückenfüller <break time='300ms'/> Entschuldigung, das habe ich akustisch nicht verstanden.";

            await sayAsync(excuse);
        }

        public static async Task CountDown(int fromNumber)
        {
            StringBuilder countdownString = new StringBuilder();

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

            await sayAsync(countdownString.ToString());
        }

        public static async Task CountTo(int toNumber)
        {
            StringBuilder countToString = new StringBuilder();

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

            await sayAsync(countToString.ToString());
        }

        public static async Task SayCreator()
        {
            StringBuilder creatorString = new StringBuilder();

            creatorString.AppendLine("Lückenfüller Lückenfüller <break time='300ms'/> Gute Frage, das wüsste ich auch gerne. Ich sehe das wie Winston Churchill, er sagte einst: <break time='300ms'/> Ich bin bereit, meinem Schöpfer gegenüberzutreten. Ob mein Schöpfer ebenso bereit ist, diese Begegnung über sich ergehen zu lassen, ist eine andere Sache.");

            await sayAsync(creatorString.ToString());
        }

        public static async Task SayGender()
        {
            StringBuilder genderString = new StringBuilder();

            genderString.AppendLine("Lückenfüller Lückenfüller <break time='300ms'/> Das ist eine gute Frage. Ich muss gestehen, ich weiß es selber nicht einmal so genau.");

            await sayAsync(genderString.ToString());
        }

        public static async Task SayJoke()
        {
            //if (!ModuleData.Data.TryGetValue(Modules.JOKE, out dynamic r))
            //    return;

            //Joke joke = (Joke) r;

            Joke joke = await JokeHelper.GetJoke();

            StringBuilder jokeString = new StringBuilder();

            jokeString.Append("Lückenfüller Lückenfüller <break time='300ms'/>");
            jokeString.AppendLine($"Einen {joke.Title.Remove(joke.Title.Length - 1)} gefällig: <break time='300ms'/><prosody rate=\"-15%\">{joke.Description}</prosody>");

            await sayAsync(jokeString.ToString());
        }

        public static async Task SayLook()
        {
            Random randi = new Random();
            int randomNumber = randi.Next(0, 2);

            StringBuilder lookString = new StringBuilder();

            lookString.Append("Lückenfüller Lückenfüller <break time='300ms'/>");
            lookString.Append(randomNumber == 0 ? "Ich fürchte, dass die Beschreibung meines Aussehens einen längeren Ausflug in Themenbereiche zum Raum - Zeit - Kontinuum und zur Mode notwendig machen würde, die dir bis jetzt noch völlig unbekannt sind." : "Mal schauen. <break time='500ms'/> Dacht ich mir's doch, ich trage das gleiche wie gestern.");

            await sayAsync(lookString.ToString());
        }

        public static async Task SayMirror()
        {
            Random randi = new Random();
            int randomNumber = randi.Next(0, 2);

            StringBuilder nameString = new StringBuilder();

            nameString.Append("Lückenfüller Lückenfüller <break time='300ms'/>");
            nameString.Append(randomNumber == 0 ? "Geh mal zur Seite, <break time='300ms'/>ich kann nichts sehen!" : "Hier ein Tipp unter Freunden: <break time='500ms'/> Frag heute einfach mal nicht!");

            await sayAsync(nameString.ToString());
        }

        public static async Task SayName()
        {
            StringBuilder nameString = new StringBuilder();

            nameString.Append("Lückenfüller Lückenfüller <break time='300ms'/> Mein Name ist <prosody rate=\"-30%\">Mira</prosody>.");
            nameString.Append("<break time='300ms'/>");
            nameString.Append("Aus dem lateinischen heraus übersetzt, bedeutet mein Name soviel wie <prosody rate=\"-30%\">wunderbar</prosody>, <prosody rate=\"-30%\">die Wunderbare</prosody>.");
            nameString.Append("<break time='300ms'/>");
            nameString.Append("Eine hinduistische Legende erzählt die Geschichte von Mirabai, <break time='200ms'/>einer Prinzessin aus dem 16. Jahrhundert, die sich als Geliebte Krishnas betrachtete. <break time='200ms'/>Mirabai gilt als geistliche Liebesdichterin.");

            await sayAsync(nameString.ToString());
        }

        public static async Task SayQuote()
        {
            //if (!ModuleData.Data.TryGetValue(Modules.QUOTE, out dynamic r))
            //    return;

            //Quote quote = (Quote) r;

            Quote quote = await ApiData.GetQuoteOfDay();

            StringBuilder quoteString = new StringBuilder();

            quoteString.Append("Lückenfüller Lückenfüller <break time='300ms'/>");
            quoteString.Append($"{(quote.Author != string.Empty ? quote.Author : "Ein kluge Frau oder ein kluger Mann")} sagte einstmal: <break time='400ms'/> {quote.Text}");

            await sayAsync(quoteString.ToString());
        }

        public static async Task SayRandom(int from, int to)
        {
            Random randi = new Random();
            int randomNumber = randi.Next(from, to);

            StringBuilder nameString = new StringBuilder();

            nameString.Append($"Lass mich nachdenken. <break time='1500ms'/> Ich sage einfach mal <break time='300ms'/><prosody rate=\"-35%\">{NumberHelper.NumberToWords(randomNumber)}</prosody>.");

            await sayAsync(nameString.ToString());
        }

        public static async Task SaySunrise()
        {
            //if (!ModuleData.Data.TryGetValue(Modules.TIME, out dynamic r))
            //    return;

            //Sun sun = (Sun)r;

            Module module = DataAccessLibrary.DataAccess.GetModule(Modules.TIME);

            Sun sun = new Sun(module);

            StringBuilder sunriseString = new StringBuilder();

            sunriseString.Append("Lückenfüller Lückenfüller <break time='300ms'/>");

            int time = DateTime.Compare(DateTime.ParseExact(sun.Sunrise, "H:mm", null, DateTimeStyles.None), DateTime.Now);

            if (time != -1)
            {
                if (time != 0)
                {
                    if (time == 1)
                        sunriseString.Append($"Um {sun.Sunrise} geht die Sonne auf.");
                }
                else
                    sunriseString.Append("In diesem Moment geht die Sonne auf.");
            }
            else
                sunriseString.Append($"Um {sun.Sunrise} ist die Sonne aufgegangen.");

            await sayAsync(sunriseString.ToString());
        }

        public static async Task SaySunset()
        {
            //if (!ModuleData.Data.TryGetValue(Modules.TIME, out dynamic r))
            //    return;

            //Sun sun = (Sun) r;

            Module module = DataAccessLibrary.DataAccess.GetModule(Modules.TIME);

            Sun sun = new Sun(module);

            StringBuilder sunsetString = new StringBuilder();

            sunsetString.Append("Lückenfüller Lückenfüller <break time='300ms'/>");

            int time = DateTime.Compare(DateTime.ParseExact(sun.Sunset, "H:mm", null, DateTimeStyles.None), DateTime.Now);

            if (time != -1)
            {
                if (time != 0)
                {
                    if (time == 1)
                        sunsetString.Append($"Um {sun.Sunset} geht die Sonne unter.");
                }
                else
                    sunsetString.Append("In diesem Moment geht die Sonne unter.");
            }
            else
                sunsetString.Append($"Um {sun.Sunset} ist die Sonne untergegangen.");

            await sayAsync(sunsetString.ToString());
        }

        public static async Task SayTime()
        {
            DateTime now = DateTime.Now;

            //string time = $"Lückenfüller Lückenfüller Es ist {now.Hour} Uhr {now.Minute}.";
            string time = $"Lückenfüller Lückenfüller <break time='300ms'/> Meine innere Uhr sagt mir, dass es {now.Hour} Uhr {now.Minute} ist.";

            await sayAsync(time);
        }

        public static async Task SayWeather()
        {
            //if (!ModuleData.Data.TryGetValue(Modules.WEATHER, out dynamic r))
            //    return;

            //SingleResult<CurrentWeatherResult> result = (SingleResult<CurrentWeatherResult>) r;

            Module module = DataAccessLibrary.DataAccess.GetModule(Modules.WEATHER);

            SingleResult<CurrentWeatherResult> result = await ApiData.GetCurrentWeatherByCityName(module);

            StringBuilder weatherTodayString = new StringBuilder();

            weatherTodayString.Append("Lückenfüller Lückenfüller <break time='300ms'/>");
            weatherTodayString.Append($"{result.Item.Description} in {result.Item.City}. Momentan werden {result.Item.Temp.ToString(CultureInfo.InvariantCulture).Replace(".", ",")} Grad Außentemperatur, bei einer Luftfeuchtigkeit von {result.Item.Humidity} Prozent gemessen. <break time='300ms'/>");
            weatherTodayString.Append(Math.Abs(result.Item.WindSpeed - double.Epsilon) < 0 ? "Zur Zeit weht kein Wind." : $"Ein Wind mit der Geschwindigkeit von {(Math.Abs(result.Item.WindSpeed - 1) < 0 ? "eins" : result.Item.WindSpeed.ToString(CultureInfo.InvariantCulture).Replace(".", ","))} Meter pro Sekunde weht aus Richtung {WindDirectionHelper.GetWindDirection(result.Item.WindDegree)}");

            await sayAsync(weatherTodayString.ToString());
        }

        public static async Task SayWeatherforecast(string days)
        {
            //if (!ModuleData.Data.TryGetValue(Modules.WEATHERFORECAST, out dynamic r))
            //    return;

            //List<List<FiveDaysForecastResult>> result = (List<List<FiveDaysForecastResult>>) r;

            Module module = DataAccessLibrary.DataAccess.GetModule(Modules.WEATHERFORECAST);

            List<List<FiveDaysForecastResult>> result = await ApiData.GetFiveDaysForecastByCityName(module);

            // Infos zu heutigen Tag löschen
            if (result.Count > 5)
                result.RemoveAt(0);

            StringBuilder weatherforecastString = new StringBuilder();

            weatherforecastString.Append("Lückenfüller Lückenfüller <break time='300ms'/>");

            switch (days)
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
                    int weekDay = result.IndexOf(result.First(x => x[0].Date.DayOfWeek == DateTimeHelper.GetNextWeekday(DateTime.Now, DayOfWeekHelper.GetDayOfWeek(days)).DayOfWeek));

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

            await sayAsync(weatherforecastString.ToString());
        }
        public static async Task Startup()
        {
            StringBuilder startupString = new StringBuilder();

            startupString.AppendLine("Darf ich mich vorstellen ?");
            startupString.AppendLine("<break time='500ms'/>");
            startupString.AppendLine("Mein Name ist <prosody rate=\"-30%\">Mira</prosody>.");
            startupString.AppendLine("<break time='1000ms'/>");
            startupString.AppendLine("Sprachbefehle, sowie weitere Information kannst du dir mit dem Sprachbefehl <break time='250ms'/> <prosody rate=\"-25%\">Mira zeige Hilfe</prosody> anzeigen lassen.");

            await sayAsync(startupString.ToString());
        }

        #endregion Public Methods

        #region Private Methods

        private static async Task noDataAvailable()
        {
            StringBuilder nodata = new StringBuilder();

            nodata.AppendLine("");

            await sayAsync(nodata.ToString());
        }

        private static async Task sayAsync(string ssml)
        {
            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append(@"<speak version='1.0' xmlns='http://www.w3.org/2001/10/synthesis' xml:lang='de-DE'>");
            stringBuilder.Append(@"<sentence>");
            stringBuilder.Append(@"<break time='500ms'/>");
            stringBuilder.Append(ssml);
            stringBuilder.Append(@"<break time='500ms'/>");
            stringBuilder.Append(@"</sentence>");
            stringBuilder.Append(@"</speak>");

            using (SpeechSynthesizer synth = new SpeechSynthesizer())
            {
                synth.Voice = SpeechSynthesizer.AllVoices.SingleOrDefault(i => i.DisplayName == "Microsoft Katja") ?? SpeechSynthesizer.DefaultVoice;

                using (MediaPlayer mediaPlayer = new MediaPlayer())
                {
                    TaskCompletionSource<bool>[] source = { null };

                    mediaPlayer.MediaEnded += (s, e) =>
                    {
                        source[0].SetResult(true);
                    };

                    source[0] = new TaskCompletionSource<bool>();

                    using (SpeechSynthesisStream speechStream = await synth.SynthesizeSsmlToStreamAsync(stringBuilder.ToString()))
                    {
                        mediaPlayer.Source = MediaSource.CreateFromStream(speechStream, speechStream.ContentType);
                        mediaPlayer.Play();
                    }

                    await source[0].Task;
                }
            }
        }

        #endregion Private Methods
    }
}