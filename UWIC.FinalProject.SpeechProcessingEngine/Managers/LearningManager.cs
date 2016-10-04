using System;
using System.Collections.Generic;
using System.Linq;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine.Managers
{
    class LearningManager
    {
        public static List<string> UnIdentifiedWords { get; set; }

        /// <summary>
        /// This method will compare the command type and append unidnetified words to the respective text file
        /// </summary>
        /// <param name="commandType">identified command type</param>
        public static void AddUnidentifiedWordsToCommandText(CommandType commandType)
        {
            try
            {
                var testFiles = FileManager.GetTestFiles();
                var textFile = "";
                // get the text file by first removing the file extension, and it's checking whether it's ending 
                // with the provided command type
                foreach (
                    var testFile in
                        testFiles.Where(
                            testFile =>
                            testFile.Remove(testFile.IndexOf(VbwFileManager.FileExtension(), StringComparison.Ordinal))
                                    .EndsWith(commandType.ToString())))
                {
                    // if so, set that particular testFile as the file name
                    textFile = testFile;
                }
                // assign the unidentified words to the test file
                DataManager.AppendToFile(textFile, UnIdentifiedWords);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}
