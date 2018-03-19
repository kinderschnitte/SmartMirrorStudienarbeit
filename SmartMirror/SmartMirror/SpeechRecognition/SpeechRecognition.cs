using System;
using System.Diagnostics;
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

        public SpeechRecognition(CoreDispatcher dispatcher)
        {
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
        private RecognizedSpeech evaluateSpeechInput(SpeechRecognitionResult argsResult)
        {
            RecognizedSpeech recognizedSpeech = new RecognizedSpeech
            {
                RawText = argsResult.Text,
                Confidence = argsResult.Confidence,
                MessageType = getSpeechInputMessageType(),
                Message = getSpeechInputMessage()
            };

            return recognizedSpeech;
        }

        private Message getSpeechInputMessage()
        {
            throw new NotImplementedException();
        }

        private Type getSpeechInputMessageType()
        {
            throw new NotImplementedException();
        }

        private async void handleRecognizedSpeech(RecognizedSpeech recognizedSpeech)
        {

        }
    }
}