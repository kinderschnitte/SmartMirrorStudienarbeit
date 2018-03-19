using Windows.Media.SpeechRecognition;
using SmartMirror.Enums;

namespace SmartMirror.Objects
{
    internal class RecognizedSpeech
    {
        public string RawText { get; set; }

        public bool IsInvalidRecognition { get; set; }

        public SpeechRecognitionConfidence Confidence { get; set; }

        public Type MessageType { get; set; }

        public Message Message { get; set; }
    }
}
