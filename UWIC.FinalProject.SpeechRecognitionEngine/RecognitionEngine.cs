using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace UWIC.FinalProject.SpeechRecognitionEngine
{
    public class RecognitionEngine
    {
        public static string getNavigationCommand(string word)
        {
            var websiteName = "";
            var words = "";
            Match match = null;
            match = Regex.Match(word, @"go to [a-zA-Z]*", RegexOptions.IgnoreCase);
            if (String.IsNullOrEmpty(match.Value))
            {
                match = Regex.Match(word, @"move to [a-zA-Z]*", RegexOptions.IgnoreCase);
                words = match.Value.ToString();
                websiteName = words.Replace("move to ", String.Empty);
            }
            else
            {
                words = match.Value.ToString();
                websiteName = words.Replace("go to ", String.Empty);
            }
            return websiteName;
        }
    }
}
