using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace UWIC.FinalProject.Common
{
    public class VbwFileManager
    {
        /// <summary>
        /// This method will return data available in a given file as a string array;
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <returns>File data as a list of string</returns>
        public static List<string> GetTextFileData(string filePath)
        {
            try
            {
                var tempList = new List<string>();
                using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var bs = new BufferedStream(fs))
                using (var sr = new StreamReader(bs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        tempList.Add(line.ToLower());
                    }
                }
                return tempList;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will append a given list of strings to a text file
        /// </summary>
        /// <param name="fileName">Name of the Text File</param>
        /// <param name="data">List of strings</param>
        public static void AppendToTextFile(string fileName, List<string> data)
        {
            try
            {
                foreach (var item in data)
                {
                    using (var writer = File.AppendText(fileName))
                    {
                        writer.WriteLine(item);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        public static string FileExtension()
        {
            return ConfigurationManager.AppSettings.Get("FileExtension");
        }

        public static string FilePath()
        {
            return ConfigurationManager.AppSettings.Get("FilePath");
        }
    }
}
