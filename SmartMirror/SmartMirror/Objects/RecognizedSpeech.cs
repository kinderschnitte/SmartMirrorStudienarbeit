﻿using Windows.Media.SpeechRecognition;
using SmartMirror.Enums;

namespace SmartMirror.Objects
{
    internal class RecognizedSpeech
    {
        public string RawText { get; set; }

        public SpeechRecognitionConfidence Confidence { get; set; }

        public Message Message { get; set; }

        public string SemanticText { get; set; }
    }
}
