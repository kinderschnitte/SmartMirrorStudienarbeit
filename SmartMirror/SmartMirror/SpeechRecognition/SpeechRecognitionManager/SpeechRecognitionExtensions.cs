using System;
using System.Threading.Tasks;
using Windows.Media.SpeechRecognition;

namespace SmartMirror.SpeechRecognition.SpeechRecognitionManager
{
    public static class SpeechRecognitionExtensions
    {
        public static async Task Dispose(this SpeechRecognitionManager speechRecognitionManager)
        {
            await speechRecognitionManager.SpeechRecognizer.ContinuousRecognitionSession.StopAsync();

            speechRecognitionManager.SpeechRecognizer.Dispose();

            speechRecognitionManager.SpeechRecognizer = null;
        }

        public static string GetInterpretation(this SpeechRecognitionSemanticInterpretation interpretation, string key)
        {
            return interpretation.Properties.ContainsKey(key) ? interpretation.Properties[key][0] : null;
        }

        public static bool IsRecognisedWithHighConfidence(this SpeechRecognitionResult result)
        {
            return result.Confidence == SpeechRecognitionConfidence.High;
        }

        public static bool IsRecognisedWithMediumConfidence(this SpeechRecognitionResult result)
        {
            return result.Confidence == SpeechRecognitionConfidence.Medium;
        }

        public static bool IsRecognisedWithLowConfidence(this SpeechRecognitionResult result)
        {
            return result.Confidence == SpeechRecognitionConfidence.Low;
        }
    }
}