using System;
using System.Diagnostics;
using Windows.Media.SpeechRecognition;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using SmartMirror.Enums;
using SmartMirror.Objects;
using SmartMirror.SpeechRecognitionManager;
using Type = SmartMirror.Enums.Type;

namespace SmartMirror.SpeechRecognition
{
    internal class SpeechRecognition
    {
        private SpeechRecognitionManager.SpeechRecognitionManager speechRecognizer;

        private readonly WebView browser;

        private readonly CoreDispatcher dispatcher;

        private volatile bool isOn;

        public SpeechRecognition(CoreDispatcher dispatcher, WebView browser)
        {
            this.browser = browser;
            this.dispatcher = dispatcher;

            StartRecognizing();

            isOn = false;
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
            RecognizedSpeech recognizedSpeech = setRecognizedSpeech(args.Result);

            handleRecognizedSpeech(recognizedSpeech);
        }

        private async void handleRecognizedSpeech(RecognizedSpeech recognizedSpeech)
        {
            Debug.WriteLine(recognizedSpeech.Confidence.ToString());
            if (recognizedSpeech.Confidence == SpeechRecognitionConfidence.Low) return;

            if (!isOn && recognizedSpeech.RecognizedMessage.MessageType != Type.MIRROR_LOCK) return;

            switch (recognizedSpeech.RecognizedMessage.MessageType)
            {
                case Type.HOME:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => browser.Navigate(new Uri("http://localhost/MagicMirror/home.html")));
                    break;

                case Type.TIME:
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => browser.Navigate(new Uri("http://localhost/MagicMirror/time.html")));
                    break;

                case Type.UNKNOWN:
                    break;

                case Type.SHUTDOWN:
                    break;

                case Type.MIRROR_LOCK:
                    isOn = !isOn;
                    if (isOn)
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => browser.Navigate(new Uri("http://localhost/MagicMirror/home.html")));
                    else
                        await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => browser.Navigate(new Uri("http://localhost/MagicMirror/time.html")));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            Debug.WriteLine(isOn + ", " + recognizedSpeech.RecognizedMessage.MessageType);
        }

        // ReSharper disable once SuggestBaseTypeForParameter
        private static RecognizedSpeech setRecognizedSpeech(SpeechRecognitionResult speechRecognitionResult)
        {
            RecognizedSpeech recognizedSpeech = new RecognizedSpeech();

            if (string.IsNullOrEmpty(speechRecognitionResult.Text))
            {
                recognizedSpeech.IsInvalidRecognition = true;
                return recognizedSpeech;
            }

            recognizedSpeech.Confidence = speechRecognitionResult.Confidence;
            recognizedSpeech.RawText = speechRecognitionResult.Text;

            string[] splittedRecognizedText = speechRecognitionResult.Text.Split(' ');

            switch (splittedRecognizedText[0])
            {
                case "spiegel":
                    recognizedSpeech.RecognizedMessage.MessageType = Type.MIRROR_LOCK;
                    break;

                case "smart":
                case "home":
                case "hauptseite":
                    recognizedSpeech.RecognizedMessage.MessageType = Type.HOME;
                    break;

                case "zeit":
                    recognizedSpeech.RecognizedMessage.MessageType = Type.TIME;
                    break;

                case "spiegel ausschalten":
                    recognizedSpeech.RecognizedMessage.MessageType = Type.SHUTDOWN;
                    break;

                default:
                    recognizedSpeech.RecognizedMessage.MessageType = Type.UNKNOWN;
                    break;
            }

            switch (recognizedSpeech.RecognizedMessage.MessageType)
            {
                case Type.HOME:
                    recognizedSpeech.RecognizedMessage.Message = Message.HOME;
                    break;
                case Type.TIME:
                    recognizedSpeech.RecognizedMessage.Message = Message.TIME;
                    break;

                case Type.UNKNOWN:
                    recognizedSpeech.RecognizedMessage.Message = Message.UNKNOWN;
                    recognizedSpeech.IsInvalidRecognition = true;
                    break;

                case Type.MIRROR_LOCK:
                    recognizedSpeech.RecognizedMessage.Message = Message.LOCK;
                    break;

                case Type.SHUTDOWN:
                    recognizedSpeech.RecognizedMessage.Message = Message.SHUTDOWN;
                    break;

                default:
                    recognizedSpeech.RecognizedMessage.Message = Message.UNKNOWN;
                    recognizedSpeech.IsInvalidRecognition = true;
                    break;
            }

            return recognizedSpeech;
        }
    }
}