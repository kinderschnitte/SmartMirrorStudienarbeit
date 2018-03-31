using System;
using System.Diagnostics;
using System.Text;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using SmartMirror.Enums;
using SmartMirror.Objects;
using SmartMirror.SpeechRecognitionManager;
using Type = SmartMirror.Enums.Type;

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
                MessageType = getSpeechInputMessageType(argsResult.Text)
            };

            recognizedSpeech.Message = getSpeechInputMessage(recognizedSpeech);

            return recognizedSpeech;
        }

        private static Message getSpeechInputMessage(RecognizedSpeech recognizedSpeech)
        {
            switch (recognizedSpeech.MessageType)
            {
                case Type.HOME:
                    return Message.HOME;

                case Type.TIME:
                    return Message.TIME;

                case Type.WEATHER:
                    return Message.WEATHER;

                case Type.WEATHERFORECAST:
                    return Message.WEATHERFORECAST;

                case Type.LIGHT:
                    return Message.LIGHT;

                case Type.NEWS:
                    if (recognizedSpeech.RawText.Contains("sport"))
                        return Message.NEWS_SPORTS;
                    else if (recognizedSpeech.RawText.Contains("branchen") || recognizedSpeech.RawText.Contains("unternehmens") || recognizedSpeech.RawText.Contains("geschäfts") || recognizedSpeech.RawText.Contains("handels"))
                        return Message.NEWS_BUSINESS;
                    else if (recognizedSpeech.RawText.Contains("unterhaltungs") || recognizedSpeech.RawText.Contains("entertainment"))
                        return Message.NEWS_ENTERTAINMENT;
                    else if (recognizedSpeech.RawText.Contains("gesundheits"))
                        return Message.NEWS_HEALTH;
                    else if (recognizedSpeech.RawText.Contains("wissenschafts") || recognizedSpeech.RawText.Contains("naturwissenschafts"))
                        return Message.NEWS_SCIENCE;
                    else if (recognizedSpeech.RawText.Contains("technologie") || recognizedSpeech.RawText.Contains("technik"))
                        return Message.NEWS_TECHNOLOGY;
                    else return Message.UNKNOWN;

                case Type.QUOTE:
                    return Message.QUOTE;

                case Type.UNKNOWN:
                    return Message.UNKNOWN;

                default:
                    return Message.UNKNOWN;
            }
        }

        private static Type getSpeechInputMessageType(string message)
        {
            StringBuilder stringBuilder = new StringBuilder(message);

            stringBuilder.Replace("spiegel", "");
            stringBuilder.Replace("zeige", "");
            stringBuilder.Replace("bitte", "");
            stringBuilder.Replace("die", "");
            stringBuilder.Replace("das", "");
            stringBuilder.Replace("den", "");
            stringBuilder.Replace("ein", "");
            stringBuilder.Replace("einen", "");

            switch (stringBuilder.ToString().Trim())
            {
                case "übersicht":
                case "module":
                case "modulübersicht":
                case "hauptseite":
                    return Type.HOME;

                case "zeit":
                case "uhrzeit":
                case "sonnenaufgang":
                case "sonnenuntergang":
                case "sonne":
                case "datum":
                case "jahr":
                case "monat":
                case "tag":
                    return Type.TIME;

                case "licht":
                    return Type.LIGHT;

                case "wetter":
                case "temperatur":
                case "wetter heute":
                case "aktuelles wetter":
                    return Type.WEATHER;

                case "wettervorhersage":
                case "vorhersage":
                case "wetter morgen":
                case "wetter übermorgen":
                    return Type.WEATHERFORECAST;

                case "sport nachrichten":
                case "sport news":
                case "branchen nachrichten":
                case "branchen news":
                case "unternehmens nachrichten":
                case "unternehmens news":
                case "geschäfts nachrichten":
                case "geschäfts news":
                case "handels nachrichten":
                case "handels news":
                case "unterhaltungs nachrichten":
                case "unterhaltungs news":
                case "entertainment nachrichten":
                case "entertainment news":
                case "gesundheits nachrichten":
                case "gesundheits news":
                case "wissenschafts nachrichten":
                case "wissenschafts news":
                case "naturwissenschafts nachrichten":
                case "naturwissenschafts news":
                case "technologie nachrichten":
                case "technologie news":
                case "technik nachrichten":
                case "technik news":
                    return Type.NEWS;

                case "zitat":
                case "spruch":
                    return Type.QUOTE;

                default:
                    return Type.UNKNOWN;
            }
        }

        private async void handleRecognizedSpeech(RecognizedSpeech recognizedSpeech)
        {
            switch (recognizedSpeech.MessageType)
            {
                case Type.HOME:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/home.html"));
                    });
                    break;
                case Type.TIME:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/time.html"));
                    });
                    break;
                case Type.WEATHER:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/weather.html"));
                    });
                    break;
                case Type.WEATHERFORECAST:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/weatherforecast.html"));
                    });
                    break;
                case Type.LIGHT:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/light.html"));
                    });
                    break;
                case Type.NEWS:
                    switch (recognizedSpeech.Message)
                    {
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
                        case Message.UNKNOWN:
                            break;
                    }
                    break;
                case Type.QUOTE:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        mainPage.Browser.Navigate(new Uri("http://localhost/quote.html"));
                    });
                    break;
                case Type.UNKNOWN:
                    return;
            }
        }
    }
}