using System;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Globalization;
using Windows.Media.SpeechRecognition;
using Windows.Storage;

namespace SmartMirror.SpeechRecognition.SpeechRecognitionManager
{
    public class SpeechRecognitionManager
    {
        public SpeechRecognitionManager(string grammarFile)
        {
            SpeechRecognizer = new SpeechRecognizer(new Language("de-DE"));

            GrammarFile = string.Format(grammarFile);
        }

        public SpeechRecognizer SpeechRecognizer { get; set; }

        private string GrammarFile { get; }

        public async Task CompileGrammar()
        {
            StorageFile grammarContentFile = await Package.Current.InstalledLocation.GetFileAsync(GrammarFile);

            SpeechRecognitionGrammarFileConstraint grammarConstraint = new SpeechRecognitionGrammarFileConstraint(grammarContentFile);

            SpeechRecognizer.Constraints.Add(grammarConstraint);

            SpeechRecognitionCompilationResult compilationResult = await SpeechRecognizer.CompileConstraintsAsync();

            if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
                return;

            await SpeechRecognizer.ContinuousRecognitionSession.StartAsync();
        }
    }
}