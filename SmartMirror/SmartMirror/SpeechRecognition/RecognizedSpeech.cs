using Windows.Media.SpeechRecognition;

namespace SmartMirror.SpeechRecognition
{
    internal class RecognizedSpeech
    {
        public string RawText { get; set; }

        public SpeechRecognitionConfidence Confidence { get; set; }

        public Message Message { get; set; }

        public string SemanticText { get; set; }
    }
}
