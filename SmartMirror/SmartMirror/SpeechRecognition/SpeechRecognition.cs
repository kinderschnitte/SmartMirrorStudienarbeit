using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using SmartMirror.SpeechRecognition.SpeechRecognitionManager;

namespace SmartMirror.SpeechRecognition
{
    internal class SpeechRecognition
    {
        public SpeechRecognitionManager.SpeechRecognitionManager SpeechRecognizer { get; private set; }

        private readonly CoreDispatcher dispatcher;

        private readonly MainPage mainPage;

        public SpeechRecognition(MainPage mainPage, CoreDispatcher dispatcher)
        {
            this.mainPage = mainPage;
            this.dispatcher = dispatcher;
        }

        public async void StopRecognizing()
        {
            await SpeechRecognizer.Dispose();
        }

        public async Task StartRecognizing()
        {
            SpeechRecognizer = new SpeechRecognitionManager.SpeechRecognitionManager("Grammar.xml");

            SpeechRecognizer.SpeechRecognizer.ContinuousRecognitionSession.ResultGenerated += continuousRecognitionSessionOnResultGenerated;

            SpeechRecognizer.SpeechRecognizer.RecognitionQualityDegrading += (sender, args) =>
            {
                Debug.WriteLine(args.Problem.ToString());
            };

            SpeechRecognizer.SpeechRecognizer.ContinuousRecognitionSession.Completed += (sender, args) =>
            {
                Debug.WriteLine(args.Status.ToString());
            };

            await SpeechRecognizer.CompileGrammar();
        }

        private void continuousRecognitionSessionOnResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            Debug.WriteLine(args.Result.Confidence.ToString());
            if (args.Result.Confidence == SpeechRecognitionConfidence.Low || args.Result.Confidence == SpeechRecognitionConfidence.Medium) return;

            handleRecognizedSpeech(evaluateSpeechInput(args.Result));
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static RecognizedSpeech evaluateSpeechInput(SpeechRecognitionResult argsResult)
        {
            RecognizedSpeech recognizedSpeech = new RecognizedSpeech
            {
                RawText = argsResult.Text,
                Confidence = argsResult.Confidence
            };

            recognizedSpeech.Message = getSpeechInputMessage(argsResult.SemanticInterpretation, recognizedSpeech);

            return recognizedSpeech;
        }

        private static Message getSpeechInputMessage(SpeechRecognitionSemanticInterpretation speechRecognitionSemanticInterpretation, RecognizedSpeech recognizedSpeech)
        {
            string home = speechRecognitionSemanticInterpretation.GetInterpretation("home");
            string time = speechRecognitionSemanticInterpretation.GetInterpretation("time");
            string light = speechRecognitionSemanticInterpretation.GetInterpretation("light");
            string weather = speechRecognitionSemanticInterpretation.GetInterpretation("weather");
            string weatherforecast = speechRecognitionSemanticInterpretation.GetInterpretation("weatherforecast");
            string news = speechRecognitionSemanticInterpretation.GetInterpretation("news");
            string quote = speechRecognitionSemanticInterpretation.GetInterpretation("quote");
            string scroll = speechRecognitionSemanticInterpretation.GetInterpretation("scroll");
            string navigate = speechRecognitionSemanticInterpretation.GetInterpretation("navigate");
            string reload = speechRecognitionSemanticInterpretation.GetInterpretation("reload");
            string speech = speechRecognitionSemanticInterpretation.GetInterpretation("speech");

            if (home != null)
            {
                recognizedSpeech.SemanticText = home;
                return Message.HOME;
            }

            if (time != null)
            {
                recognizedSpeech.SemanticText = time;
                return Message.TIME;
            }

            if (light != null)
            {
                recognizedSpeech.SemanticText = light;
                return Message.LIGHT;
            }

            if (weather != null)
            {
                recognizedSpeech.SemanticText = weather;
                return Message.WEATHER;
            }

            if (weatherforecast != null)
            {
                recognizedSpeech.SemanticText = weatherforecast;
                return Message.WEATHERFORECAST;
            }

            if (news != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (news == "sport")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NEWS_SPORTS;
                }

                if (news == "business")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NEWS_BUSINESS;
                }

                if (news == "entertainment")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NEWS_ENTERTAINMENT;
                }

                if (news == "health")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NEWS_HEALTH;
                }

                if (news == "science")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NEWS_SCIENCE;
                }

                if (news == "technology")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NEWS_TECHNOLOGY;
                }
            }

            if (quote != null)
            {
                recognizedSpeech.SemanticText = quote;
                return Message.QUOTE;
            }

            if (scroll != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (scroll == "up")
                {
                    recognizedSpeech.SemanticText = scroll;
                    return Message.SCROLL_UP;
                }

                if (scroll == "down")
                {
                    recognizedSpeech.SemanticText = scroll;
                    return Message.SCROLL_DOWN;
                }
            }

            if (navigate != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (navigate == "back")
                {
                    recognizedSpeech.SemanticText = navigate;
                    return Message.NAVIGATE_BACKWARDS;
                }

                if (navigate == "forward")
                {
                    recognizedSpeech.SemanticText = navigate;
                    return Message.NAVIGATE_FOREWARDS;
                }
            }

            if (reload != null)
            {
                recognizedSpeech.SemanticText = reload;
                return Message.RELOAD;
            }

            if (speech != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (speech == "clock")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_TIME;
                }

                if (speech == "weather")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_WEATHER;
                }

                if (speech.Contains("weatherforecast"))
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_WEATHERFORECAST;
                }

                if (speech == "temperature")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_WEATHER_TEMPERATURE;
                }

                if (speech == "sunrise")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_SUNRISE;
                }

                if (speech == "sunset")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_SUNSET;
                }

                if (speech == "name")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_NAME;
                }

                if (speech == "look")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_LOOK;
                }

                if (speech == "gender")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_GENDER;
                }

                if (speech == "mirror")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_MIRROR;
                }

                if (speech == "quote")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_QUOTE;
                }

                if (speech == "joke")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_JOKE;
                }

                if (speech == "creator")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SPEECH_CREATOR;
                }
            }

            return Message.UNKNOWN;
        }

        private async void handleRecognizedSpeech(RecognizedSpeech recognizedSpeech)
        {
            switch (recognizedSpeech.Message)
            {
                case Message.HOME:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/home.html"));
                    });
                    break;

                case Message.TIME:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/time.html"));
                    });
                    break;

                case Message.WEATHER:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/weather.html"));
                    });
                    break;

                case Message.WEATHERFORECAST:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/weatherforecast.html"));
                    });
                    break;

                case Message.LIGHT:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/light.html"));
                    });
                    break;

                case Message.NEWS_SPORTS:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/news.html?Sports"));
                    });
                    break;

                case Message.NEWS_BUSINESS:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/news.html?Business"));
                    });
                    break;

                case Message.NEWS_ENTERTAINMENT:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/news.html?Entertainment"));
                    });
                    break;

                case Message.NEWS_HEALTH:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/news.html?Health"));
                    });
                    break;

                case Message.NEWS_SCIENCE:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/news.html?Science"));
                    });
                    break;

                case Message.NEWS_TECHNOLOGY:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/news.html?Technology"));
                    });
                    break;

                case Message.QUOTE:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/quote.html"));
                    });
                    break;

                case Message.RELOAD:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Refresh();
                    });
                    break;

                case Message.NAVIGATE_FOREWARDS:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (mainPage.Browser.CanGoForward)
                            mainPage.Browser.GoForward();
                    });
                    break;

                case Message.NAVIGATE_BACKWARDS:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (mainPage.Browser.CanGoBack)
                            mainPage.Browser.GoBack();
                    });
                    break;

                case Message.SCROLL_UP:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.Browser.InvokeScriptAsync("eval", new[] { "window.scrollBy(0, 50);" });
                    });
                    break;

                case Message.SCROLL_DOWN:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.Browser.InvokeScriptAsync("eval", new[] { "window.scrollBy(0, -50);" });
                    });
                    break;

                case Message.SPEECH_TIME:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SayTime();
                    });
                    break;

                case Message.SPEECH_NAME:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SayName();
                    });
                    break;

                case Message.SPEECH_LOOK:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SayLook();
                    });
                    break;

                case Message.SPEECH_GENDER:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SayGender();
                    });
                    break;

                case Message.SPEECH_MIRROR:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SayMirror();
                    });
                    break;

                case Message.SPEECH_COUNT:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        int.TryParse(recognizedSpeech.SemanticText, out int number);
                        await mainPage.SpeechService.CountTo(number);
                    });
                    break;

                case Message.SPEECH_COUNTDOWN:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        int.TryParse(recognizedSpeech.SemanticText, out int number);
                        await mainPage.SpeechService.CountDown(number);
                    });
                    break;

                case Message.SPEECH_RANDOM:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        int.TryParse(recognizedSpeech.SemanticText.Split(' ')[0], out int from);
                        int.TryParse(recognizedSpeech.SemanticText.Split(' ')[1], out int to);
                        await mainPage.SpeechService.SayRandom(from, to);
                    });
                    break;

                case Message.SPEECH_JOKE:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SayJoke();
                    });
                    break;

                case Message.SPEECH_QUOTE:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SayQuote();
                    });
                    break;

                case Message.SPEECH_WEATHER:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SayWeatherToday();
                    });
                    break;

                case Message.SPEECH_CREATOR:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SayCreator();
                    });
                    break;

                case Message.SPEECH_WEATHERFORECAST:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SayWeatherforecast(recognizedSpeech);
                    });
                    break;

                case Message.SPEECH_WEATHER_TEMPERATURE:
                    break;

                case Message.SPEECH_SUNRISE:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SaySunrise();
                    });
                    break;

                case Message.SPEECH_SUNSET:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.SpeechService.SaySunset();
                    });
                    break;

                case Message.UNKNOWN:
                    break;
            }
        }
    }
}