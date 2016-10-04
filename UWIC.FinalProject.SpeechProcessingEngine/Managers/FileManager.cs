using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine.Managers
{
    public class FileManager
    {
        /// <summary>
        /// this method will acquire test files from the data folder
        /// </summary>
        public static string[] GetTestFiles()
        {
            try
            {
                return Directory.GetFiles(VbwFileManager.FilePath(), "*" + VbwFileManager.FileExtension());
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will acquire the final level category instances
        /// </summary>
        /// <param name="fileNames">File paths</param>
        /// <param name="prefix">prefix of the file of the related command type</param>
        /// <returns>a category collection list</returns>
        public static List<CategoryCollection> GetCategoryInstances(string[] fileNames, string prefix)
        {
            try
            {
                return
                    (from nameWithoutPrefix in
                         fileNames.Where(row => row.Contains(prefix))
                                  .Select(testFileName => testFileName.Replace(prefix + "_", String.Empty))
                     where nameWithoutPrefix != "websites"
                     select new CategoryCollection
                         {
                             Name = nameWithoutPrefix,
                             List = new List<string>(),
                             Category =
                                 Conversions.ConvertEnumToInt(
                                     Conversions.ConvertStringToEnum<CommandType>(nameWithoutPrefix))
                         }).ToList();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will return explicit file names which starts with the provided value from a given file paths list
        /// </summary>
        /// <param name="fileNames">file paths list</param>
        /// <param name="startsWithValue">starting value</param>
        /// <returns></returns>
        public static List<string> GetExplicitFileNamesByPrefix(string[] fileNames, string startsWithValue)
        {
            try
            {
                return fileNames.Select(item => item.Replace(VbwFileManager.FilePath(), String.Empty).Replace(VbwFileManager.FileExtension(), String.Empty)).Where(explicitFileName => explicitFileName.StartsWith(startsWithValue)).ToList();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will return the content of a given file name
        /// </summary>
        /// <param name="file">name of the file</param>
        /// <returns>File content as a list of string</returns>
        public static List<string> GetContentOfAFile(string file)
        {
            try
            {
                return DataManager.GetFileData(VbwFileManager.FilePath() + file + VbwFileManager.FileExtension());
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}
