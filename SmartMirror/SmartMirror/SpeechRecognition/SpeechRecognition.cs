using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;
using Windows.UI;
using Windows.UI.Core;
using SmartMirror.SpeechRecognition.SpeechRecognitionManager;
using Speechservice;

namespace SmartMirror.SpeechRecognition
{
    internal class SpeechRecognition
    {

        #region Private Fields

        private readonly CoreDispatcher dispatcher;
        private readonly MainPage mainPage;
        private SpeechRecognitionManager.SpeechRecognitionManager speechRecognizer;

        #endregion Private Fields

        #region Public Constructors

        public SpeechRecognition(MainPage mainPage, CoreDispatcher dispatcher)
        {
            this.mainPage = mainPage;
            this.dispatcher = dispatcher;
        }

        #endregion Public Constructors

        #region Public Methods

        public async void StartRecognizing()
        {
            speechRecognizer = new SpeechRecognitionManager.SpeechRecognitionManager("Grammar.xml");

            speechRecognizer.SpeechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSessionOnResultGenerated;

            speechRecognizer.SpeechRecognizer.RecognitionQualityDegrading += (sender, args) =>
            {
                Debug.WriteLine("Quality" + args.Problem.ToString());
            };

            speechRecognizer.SpeechRecognizer.ContinuousRecognitionSession.Completed += (sender, args) =>
            {
                Debug.WriteLine("Completed" + args.Status.ToString());
            };

            await speechRecognizer.CompileGrammar();

            Debug.WriteLine("Speech recognition started.");
            Debug.WriteLine(speechRecognizer.SpeechRecognizer.State);
        }

        public async void StopRecognizing()
        {
            await speechRecognizer.Dispose();
        }

        #endregion Public Methods

        #region Private Methods

        private static RecognizedSpeech EvaluateSpeechInput(SpeechRecognitionResult argsResult)
        {
            RecognizedSpeech recognizedSpeech = new RecognizedSpeech
            {
                RawText = argsResult.Text,
                Confidence = argsResult.Confidence
            };

            recognizedSpeech.Message = GetSpeechInputMessage(argsResult.SemanticInterpretation, recognizedSpeech);

            return recognizedSpeech;
        }

        private static Message GetSpeechInputMessage(SpeechRecognitionSemanticInterpretation speechRecognitionSemanticInterpretation, RecognizedSpeech recognizedSpeech)
        {
            string home = speechRecognitionSemanticInterpretation.GetInterpretation("home");
            string help = speechRecognitionSemanticInterpretation.GetInterpretation("help");
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
            string power = speechRecognitionSemanticInterpretation.GetInterpretation("power");

            if (home != null)
            {
                recognizedSpeech.SemanticText = home;
                return Message.Home;
            }

            if (help != null)
            {
                recognizedSpeech.SemanticText = help;
                return Message.Help;
            }

            if (time != null)
            {
                recognizedSpeech.SemanticText = time;
                return Message.Time;
            }

            if (light != null)
            {
                recognizedSpeech.SemanticText = light;
                return Message.Light;
            }

            if (weather != null)
            {
                recognizedSpeech.SemanticText = weather;
                return Message.Weather;
            }

            if (weatherforecast != null)
            {
                recognizedSpeech.SemanticText = weatherforecast;
                return Message.Weatherforecast;
            }

            if (news != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (news == "sport")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NewsSports;
                }

                if (news == "business")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NewsBusiness;
                }

                if (news == "entertainment")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NewsEntertainment;
                }

                if (news == "health")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NewsHealth;
                }

                if (news == "science")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NewsScience;
                }

                if (news == "technology")
                {
                    recognizedSpeech.SemanticText = news;
                    return Message.NewsTechnology;
                }
            }

            if (quote != null)
            {
                recognizedSpeech.SemanticText = quote;
                return Message.Quote;
            }

            if (scroll != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (scroll == "up")
                {
                    recognizedSpeech.SemanticText = scroll;
                    return Message.ScrollUp;
                }

                if (scroll == "down")
                {
                    recognizedSpeech.SemanticText = scroll;
                    return Message.ScrollDown;
                }
            }

            if (navigate != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (navigate == "back")
                {
                    recognizedSpeech.SemanticText = navigate;
                    return Message.NavigateBackwards;
                }

                if (navigate == "forward")
                {
                    recognizedSpeech.SemanticText = navigate;
                    return Message.NavigateForewards;
                }
            }

            if (reload != null)
            {
                recognizedSpeech.SemanticText = reload;
                return Message.Reload;
            }

            if (speech != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (speech == "clock")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechTime;
                }

                if (speech == "weather")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechWeather;
                }

                if (speech.Contains("weatherforecast"))
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechWeatherforecast;
                }

                if (speech == "temperature")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechWeatherTemperature;
                }

                if (speech == "sunrise")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechSunrise;
                }

                if (speech == "sunset")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechSunset;
                }

                if (speech == "name")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechName;
                }

                if (speech == "look")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechLook;
                }

                if (speech == "gender")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechGender;
                }

                if (speech == "mirror")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechMirror;
                }

                if (speech == "quote")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechQuote;
                }

                if (speech == "joke")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechJoke;
                }

                if (speech == "creator")
                {
                    recognizedSpeech.SemanticText = speech;
                    return Message.SpeechCreator;
                }
            }

            if (power != null)
            {
                recognizedSpeech.SemanticText = power;
                return Message.Power;
            }

            return Message.Unknown;
        }

        private async void ContinuousRecognitionSessionOnResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            try
            {
                Debug.WriteLine(args.Result.Confidence.ToString());

                if (args.Result.Confidence == SpeechRecognitionConfidence.Low) return;

                if (args.Result.Confidence == SpeechRecognitionConfidence.Medium)
                {
                    //await SpeechService.BadlyUnderstood();
                    mainPage.StartColorAnimation(mainPage.RecognitionLight, "(RecognitionLight.Background).Color", Colors.Black, Colors.Red, 2);
                }
                else if(args.Result.Confidence == SpeechRecognitionConfidence.High)
                {
                    await speechRecognizer.SpeechRecognizer.ContinuousRecognitionSession.PauseAsync();

                    Debug.WriteLine("Speech Recognition stopped");

                    mainPage.StartColorAnimation(mainPage.RecognitionLight, "(RecognitionLight.Background).Color", Colors.Black, Colors.Green);
                    await HandleRecognizedSpeech(EvaluateSpeechInput(args.Result));

                    speechRecognizer.SpeechRecognizer.ContinuousRecognitionSession.Resume();

                    Debug.WriteLine("Speech Recognition started");
                }
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task HandleRecognizedSpeech(RecognizedSpeech recognizedSpeech)
        {
            switch (recognizedSpeech.Message)
            {
                case Message.Home:
                    ShowSite("http://localhost/home.html");
                    break;

                case Message.Help:
                    ShowSite("http://localhost/help.html");
                    break;

                case Message.Time:
                    ShowSite("http://localhost/time.html");
                    break;

                case Message.Weather:
                    ShowSite("http://localhost/weather.html");
                    break;

                case Message.Weatherforecast:
                    ShowSite("http://localhost/weatherforecast.html");
                    break;

                case Message.Light:
                    ShowSite("http://localhost/light.html");
                    break;

                case Message.NewsSports:
                    ShowSite("http://localhost/news.html?Sports");
                    break;

                case Message.NewsBusiness:
                    ShowSite("http://localhost/news.html?Business");
                    break;

                case Message.NewsEntertainment:
                    ShowSite("http://localhost/news.html?Entertainment");
                    break;

                case Message.NewsHealth:
                    ShowSite("http://localhost/news.html?Health");
                    break;

                case Message.NewsScience:
                    ShowSite("http://localhost/news.html?Science");
                    break;

                case Message.NewsTechnology:
                    ShowSite("http://localhost/news.html?Technology");
                    break;

                case Message.Quote:
                    ShowSite("http://localhost/quote.html");
                    break;

                case Message.Reload:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Refresh();
                    });
                    break;

                case Message.NavigateForewards:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (mainPage.Browser.CanGoForward)
                            mainPage.Browser.GoForward();
                    });
                    break;

                case Message.NavigateBackwards:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        if (mainPage.Browser.CanGoBack)
                            mainPage.Browser.GoBack();
                    });
                    break;

                case Message.ScrollUp:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.Browser.InvokeScriptAsync("eval", new[] { "window.scrollBy(0, 50);" });
                    });
                    break;

                case Message.ScrollDown:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        await mainPage.Browser.InvokeScriptAsync("eval", new[] { "window.scrollBy(0, -50);" });
                    });
                    break;

                case Message.SpeechTime:
                    ShowSite("http://localhost/time.html");
                    await SpeechService.SayTime();
                    break;

                case Message.SpeechName:
                    await SpeechService.SayName();
                    break;

                case Message.SpeechLook:
                    await SpeechService.SayLook();
                    break;

                case Message.SpeechGender:
                    await SpeechService.SayGender();
                    break;

                case Message.SpeechMirror:
                    await SpeechService.SayMirror();
                    break;

                case Message.SpeechCount:
                    int.TryParse(recognizedSpeech.SemanticText, out int count);
                    await SpeechService.CountTo(count);
                    break;

                case Message.SpeechCountdown:
                    int.TryParse(recognizedSpeech.SemanticText, out int countdown);
                    await SpeechService.CountDown(countdown);
                    break;

                case Message.SpeechRandom:
                    // TODO
                    //await dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    //{
                    //    int.TryParse(recognizedSpeech.SemanticText.Split(' ')[0], out int from);
                    //    int.TryParse(recognizedSpeech.SemanticText.Split(' ')[1], out int to);
                    //    //await mainPage.SpeechService.SayRandom(from, to);
                    //});
                    //break;

                case Message.SpeechJoke:
                    await SpeechService.SayJoke();
                    break;

                case Message.SpeechQuote:
                    ShowSite("http://localhost/quote.html");
                    await SpeechService.SayQuote();
                    break;

                case Message.SpeechWeather:
                    ShowSite("http://localhost/weather.html");
                    await SpeechService.SayWeather();
                    break;

                case Message.SpeechCreator:
                    await SpeechService.SayCreator();
                    break;

                case Message.SpeechWeatherforecast:
                    ShowSite("http://localhost/weatherforecast.html");
                    await SpeechService.SayWeatherforecast(recognizedSpeech.SemanticText.Split(' ')[1]);
                    break;

                case Message.SpeechWeatherTemperature:
                    //TODO
                    break;

                case Message.SpeechSunrise:
                    await SpeechService.SaySunrise();
                    break;

                case Message.SpeechSunset:
                    await SpeechService.SaySunset();
                    break;

                case Message.Power:
                    await RaspberryPiGpio.RaspberryPiGpio.TriggerOnOffButton();
                    break;

                case Message.Unknown:
                    break;
            }
        }

        private async void ShowSite(string url)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Uri uri = new Uri(url);

                if (mainPage.Browser.Source.OriginalString != uri.OriginalString)
                    mainPage.Browser.Navigate(uri);
            });
        }

        #endregion Private Methods

    }
}