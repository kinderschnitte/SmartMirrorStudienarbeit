using Windows.Media.SpeechRecognition;
using SmartMirror.Enums;

namespace SmartMirror.Objects
{
    internal class RecognizedSpeech
    {
        public string RawText { get; set; }

        public bool IsInvalidRecognition { get; set; }

        public SpeechRecognitionConfidence Confidence { get; set; }

        public RecognizedMessage RecognizedMessage { get; }

        public RecognizedSpeech()
        {
            RecognizedMessage = new RecognizedMessage();
        }
    }

    internal class RecognizedMessage
    {
        public Type MessageType { get; set; }

        public Message Message { get; set; }
    }
}
