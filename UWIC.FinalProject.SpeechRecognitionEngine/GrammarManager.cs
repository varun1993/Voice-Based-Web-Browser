using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Speech.Recognition;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechRecognitionEngine
{
    public class GrammarManager
    {
        public GrammarManager()
        {
            Settings.CultureInfo = "en-GB";
        }

        public GrammarBuilder GetSpellGrammar()
        {
            try
            {
                var dictationBuilder = new GrammarBuilder // creating a new grammar builder
                    {
                        Culture = new CultureInfo(Settings.CultureInfo)
                    };
                dictationBuilder.AppendDictation(); // append dictation to the created grammar builder

                var dictaphoneGb = new GrammarBuilder { Culture = new CultureInfo(Settings.CultureInfo) };
                dictaphoneGb.Append(new Choices("A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"));

                var dictation = new GrammarBuilder { Culture = new CultureInfo(Settings.CultureInfo) };
                var length = Convert.ToInt32(ConfigurationManager.AppSettings.Get("SpellGrammarLength"));
                for (var i = 0; i < length; i++)
                {
                    dictation.Append(dictaphoneGb, 0, 200);
                    dictation.Append(dictationBuilder, 0 /* minimum repeat */, 10 /* maximum repeat*/ );
                }
                return dictation;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        public GrammarBuilder GetWebsiteNamesGrammar()
        {
            try
            {
                Settings.CultureInfo = "en-GB";
                var webSiteNames = new List<string>();
                using (var fs = File.Open(VbwFileManager.FilePath() + "fnc_brwsr_websites" + VbwFileManager.FileExtension(), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var bs = new BufferedStream(fs))
                using (var sr = new StreamReader(bs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        webSiteNames.Add(line);
                    }
                }

                var dictationBuilder = new GrammarBuilder // creating a new grammar builder
                    {
                        Culture = new CultureInfo(Settings.CultureInfo)
                    };
                dictationBuilder.AppendDictation(); // append dictation to the created grammar builder

                var dictaphoneGb = new GrammarBuilder { Culture = new CultureInfo(Settings.CultureInfo) };
                dictaphoneGb.Append(dictationBuilder, 0 /* minimum repeat */, 10 /* maximum repeat*/ );
                dictaphoneGb.Append(new Choices(webSiteNames.ToArray()));
                dictaphoneGb.Append(dictationBuilder, 0 /* minimum repeat */, 10 /* maximum repeat*/ );
                return dictaphoneGb;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}
