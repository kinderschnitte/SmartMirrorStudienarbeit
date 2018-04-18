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
        #region Public Constructors

        public SpeechRecognitionManager(string grammarFile)
        {
            SpeechRecognizer = new SpeechRecognizer(new Language("de-DE"));

            GrammarFile = string.Format(grammarFile);
        }

        #endregion Public Constructors

        #region Public Properties

        public SpeechRecognizer SpeechRecognizer { get; set; }

        #endregion Public Properties

        #region Private Properties

        private string GrammarFile { get; }

        #endregion Private Properties

        #region Public Methods

        public async Task CompileGrammar()
        {
            try
            {
                StorageFile grammarContentFile = await Package.Current.InstalledLocation.GetFileAsync(GrammarFile);

                SpeechRecognitionGrammarFileConstraint grammarConstraint = new SpeechRecognitionGrammarFileConstraint(grammarContentFile);

                SpeechRecognizer.Constraints.Add(grammarConstraint);

                SpeechRecognitionCompilationResult compilationResult = await SpeechRecognizer.CompileConstraintsAsync();

                if (compilationResult.Status != SpeechRecognitionResultStatus.Success)
                    return;

                await SpeechRecognizer.ContinuousRecognitionSession.StartAsync();
            }
            catch (Exception)
            {
                // ignored
            }
        }

        #endregion Public Methods
    }
}