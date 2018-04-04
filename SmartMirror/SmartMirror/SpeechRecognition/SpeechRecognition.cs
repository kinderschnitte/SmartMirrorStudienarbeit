using System;
using System.Diagnostics;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using SmartMirror.Enums;
using SmartMirror.Objects;
using SmartMirror.SpeechRecognitionManager;

namespace SmartMirror.SpeechRecognition
{
    internal class SpeechRecognition
    {
        private SpeechRecognitionManager.SpeechRecognitionManager speechRecognizer;

        private readonly CoreDispatcher dispatcher;

        private readonly MainPage mainPage;

        public SpeechRecognition(MainPage mainPage, CoreDispatcher dispatcher)
        {
            this.mainPage = mainPage;
            this.dispatcher = dispatcher;

            StartRecognizing();
        }

        public async void StopRecognizing()
        {
            await speechRecognizer.Dispose();
        }

        public async void StartRecognizing()
        {
            speechRecognizer = new SpeechRecognitionManager.SpeechRecognitionManager("Grammar.xml");

            speechRecognizer.SpeechRecognizer.ContinuousRecognitionSession.ResultGenerated += continuousRecognitionSessionOnResultGenerated;

            await speechRecognizer.CompileGrammar();
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
                Confidence = argsResult.Confidence,
                Message = getSpeechInputMessage(argsResult.SemanticInterpretation)
            };

            return recognizedSpeech;
        }

        private static Message getSpeechInputMessage(SpeechRecognitionSemanticInterpretation speechRecognitionSemanticInterpretation)
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
                return Message.HOME;

            if (time != null)
                return Message.TIME;

            if (light != null)
                return Message.LIGHT;

            if (weather != null)
                return Message.WEATHER;

            if (weatherforecast != null)
                return Message.WEATHERFORECAST;

            if (news != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (news == "sport")
                    return Message.NEWS_SPORTS;

                if (news == "business")
                    return Message.NEWS_BUSINESS;

                if (news == "entertainment")
                    return Message.NEWS_ENTERTAINMENT;

                if (news == "health")
                    return Message.NEWS_HEALTH;

                if (news == "science")
                    return Message.NEWS_SCIENCE;

                if (news == "technology")
                    return Message.NEWS_TECHNOLOGY;
            }

            if (quote != null)
                return Message.QUOTE;

            if (scroll != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (scroll == "up")
                    return Message.SCROLL_UP;
                if (scroll == "down")
                    return Message.SCROLL_DOWN;
            }

            if (navigate != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (navigate == "back")
                    return Message.NAVIGATE_BACKWARDS;

                if (navigate == "forward")
                    return Message.NAVIGATE_FOREWARDS;
            }

            if (reload != null)
                return Message.RELOAD;

            if (speech != null)
            {
                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (speech == "clock")
                    return Message.SPEECH_TIME;

                if (speech == "weather today")
                    return Message.SPEECH_WEATHER_TOMORROW;

                if (speech == "weather tomorrow")
                    return Message.SPEECH_WEATHER_DAYAFTERTOMORROW;

                if (speech == "temperature")
                    return Message.SPEECH_WEATHER_TEMPERATURE;

                if (speech == "sunrise")
                    return Message.SPEECH_SUNRISE;

                if (speech == "sunset")
                    return Message.SPEECH_SUNSET;

                if (speech == "name")
                    return Message.SPEECH_NAME;

                if (speech == "look")
                    return Message.SPEECH_NAME;

                if (speech == "gender")
                    return Message.SPEECH_NAME;
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

                case Message.SPEECH_WEATHER_TOMORROW:
                    break;
                case Message.SPEECH_WEATHER_DAYAFTERTOMORROW:
                    break;
                case Message.SPEECH_WEATHER_TEMPERATURE:
                    break;
                case Message.SPEECH_SUNRISE:
                    break;
                case Message.SPEECH_SUNSET:
                    break;
                case Message.UNKNOWN:
                    break;
            }
        }
    }
}