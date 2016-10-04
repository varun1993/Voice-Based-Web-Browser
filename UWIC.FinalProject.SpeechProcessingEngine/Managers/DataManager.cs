using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine.Managers
{
    public class DataManager
    {
        /// <summary>
        /// This method is used to assign test data to each category without redundancy
        /// </summary>
        /// <param name="set">The training set to which the data should be added</param>
        /// <param name="currentList">the current data list acquired from the local file</param>
        public static void AssignDataToTestSet(ICollection<string> set, IEnumerable<string> currentList)
        {
            try
            {
                if (currentList != null)
                    foreach (var item in currentList.Where(item => !set.Contains(item)))
                    {
                        set.Add(item);
                    }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will populate data available in a give file list to given category Collection
        /// This method is used for final level data categorization
        /// </summary>
        /// <param name="categoryCollections">List of categories</param>
        /// <param name="fileNames"> list of file names</param>
        /// <param name="prefix">prefix of the command category</param>
        public static void PopulateDataToCategories(List<CategoryCollection> categoryCollections, string[] fileNames, string prefix)
        {
            try
            {
                foreach (var fileName in fileNames)
                {
                    var explicitFileName = fileName.Replace(VbwFileManager.FilePath(), String.Empty)
                                                   .Replace(VbwFileManager.FileExtension(), String.Empty); // Get the proper file name, without extensions or the directory
                    if (!explicitFileName.StartsWith(prefix)) continue; // if the file doesn't belong to the command category meant by the given prefix, continue to the next item of the loop
                    var fileData = GetFileData(fileName); // else get the file data
                    var fileNameWithoutPrefix = explicitFileName.Replace(prefix + "_", String.Empty); // get the file name without the prefix
                    if (fileNameWithoutPrefix != "websites") // if the file name without prefix not equals to websites
                    {
                        var categoryObject = GetCategoryCollectionObjectFromFileName(categoryCollections,
                                                                                     fileNameWithoutPrefix); //get the category object by using the file name without the prefix
                        if (categoryObject != null && fileData != null) // if the category object is not null and the file data is not null
                            AssignDataToTestSet(categoryObject.List, fileData); // assign file data to the list available in the category object
                    }
                    else
                    {
                        // this is a special scenario where website names will be taken into account as go command keywords
                        var goCommandCategoryObject =
                            GetCategoryCollectionObjectFromFileName(categoryCollections, "go");
                        if (goCommandCategoryObject != null)
                            AssignDataToTestSet(goCommandCategoryObject.List, fileData);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// this method is used to get the category collection from a given category collection list
        /// using the file name
        /// </summary>
        /// <param name="categoryCollections">category collection list</param>
        /// <param name="fileName">file name</param>
        /// <returns>category collection object</returns>
        private static CategoryCollection GetCategoryCollectionObjectFromFileName(IEnumerable<CategoryCollection> categoryCollections, string fileName)
        {
            try
            {
                return categoryCollections.FirstOrDefault(
                                row =>
                                row.Category ==
                                Conversions.ConvertEnumToInt(Conversions.ConvertStringToEnum<CommandType>(fileName)));
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will return data available in a given file as a string array;
        /// </summary>
        /// <param name="filePath">file path</param>
        /// <returns>File data as a list of string</returns>
        public static List<string> GetFileData(string filePath)
        {
            return VbwFileManager.GetTextFileData(filePath);
        }

        /// <summary>
        /// this method will add counter to the counter text file
        /// </summary>
        /// <param name="commandType"></param>
        public static void AddToCommandCounter(CommandType commandType)
        {
            try
            {
                var fileName = VbwFileManager.FilePath() + "count_commandExecutionCounter" + VbwFileManager.FileExtension();
                var data = GetFileData(fileName);
                var valueToBeRemoved = "";
                var valueToBeAdded = "";
                foreach (var value in from value in data let command = value.Split('|').First() where command == commandType.ToString() select value)
                {
                    valueToBeRemoved = value;
                    var count = Convert.ToInt32(value.Split('|').Last());
                    valueToBeAdded = commandType + "|" + ++count;
                }

                data.Remove(valueToBeRemoved);
                data.Add(valueToBeAdded);

                using (var writer = new StreamWriter(fileName))
                {
                    foreach (var content in data)
                    {
                        writer.WriteLine(content);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Get the highest probability categories for printing the exception
        /// </summary>
        /// <param name="highestProbabilityCategories"></param>
        /// <returns></returns>
        public static string GetHighestProbableCommandTypesForException<T>(IEnumerable<ProbabilityScoreIndex> highestProbabilityCategories) where T : struct, IConvertible
        {
            try
            {
                var highProbableCategories = "";
                var counter = 0;
                var probabilityScoreIndices = highestProbabilityCategories as ProbabilityScoreIndex[] ?? highestProbabilityCategories.ToArray();

                foreach (var highestProbabilityCategory in probabilityScoreIndices)
                {
                    counter++;
                    if (counter < probabilityScoreIndices.Count())
                        highProbableCategories += (T)Enum.ToObject(typeof(T), highestProbabilityCategory.ReferenceId) +
                                                  " - Probability Score : " + highestProbabilityCategory.ProbabilityScore +
                                                  ", ";
                    else
                        highProbableCategories += (T)Enum.ToObject(typeof(T), highestProbabilityCategory.ReferenceId) +
                                                  " - Probability Score : " + highestProbabilityCategory.ProbabilityScore;
                }
                return highProbableCategories;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// Get Highest Probability commands for a given category
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="highestProbabilityCategories"></param>
        /// <returns></returns>
        public static List<T> GetHighestProbableCommands<T>(IEnumerable<ProbabilityScoreIndex> highestProbabilityCategories) where T : struct, IConvertible
        {
            try
            {
                var probabilityScoreIndices = highestProbabilityCategories as ProbabilityScoreIndex[] ?? highestProbabilityCategories.ToArray();
                return probabilityScoreIndices.Select(highestProbabilityCategory => (T)Enum.ToObject(typeof(T), highestProbabilityCategory.ReferenceId)).ToList();
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
        public static void AppendToFile(string fileName, List<string> data)
        {
            try
            {
                VbwFileManager.AppendToTextFile(fileName, data);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will get the unknown words associated a particular command and assign them to the UnidentifiedWords list of the Learning Manager
        /// </summary>
        /// <param name="commandSegments">command segments</param>
        /// <param name="identifiedCommandWithPrefix">identified command name with prefix</param>
        public static void AcquireUnknownWordsForACommand(List<string> commandSegments, string identifiedCommandWithPrefix)
        {
            try
            {
                // get the content of the file
                var fileContent = FileManager.GetContentOfAFile(identifiedCommandWithPrefix);
                //  assign the command segments into another list, from which they can be removed
                var removableCommandSegments = new List<string>();
                removableCommandSegments.AddRange(commandSegments);
                // foreach command segment
                foreach (var command in commandSegments)
                {
                    // if the file content includes the command
                    if (fileContent.Contains(command))
                        // remove them from the removableCommandSegments list
                        removableCommandSegments.Remove(command);
                }
                // finally if there are any segments available in the removableCommandSegments list
                if (removableCommandSegments.Count > 0)
                {
                    // add each of the commands to the unidentified words list
                    foreach (var removableCommandSegment in removableCommandSegments)
                    {
                        if (!String.IsNullOrEmpty(removableCommandSegment))
                            LearningManager.UnIdentifiedWords.Add(removableCommandSegment);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}
