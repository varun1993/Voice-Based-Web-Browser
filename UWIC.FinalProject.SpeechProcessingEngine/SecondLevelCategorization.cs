using System;
using System.Collections.Generic;
using System.Linq;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine.Commands;
using UWIC.FinalProject.SpeechProcessingEngine.Managers;

namespace UWIC.FinalProject.SpeechProcessingEngine
{
    public class SecondLevelCategorization
    {
        private List<CategoryCollection> _secondLevelCategoryCollection;
        private List<ProbabilityScoreIndex> _secondLevelProbabilityScoreIndices; 

        /// <summary>
        /// Constructor
        /// </summary>
        public SecondLevelCategorization()
        {
            DefineSecondaryCategories();
            AcquireTestFiles();
        }

        /// <summary>
        /// This method will define the Second level Categories for the Naive Command Categorization
        /// </summary>
        private void DefineSecondaryCategories()
        {
            try
            {
                _secondLevelCategoryCollection = new List<CategoryCollection>
                {
                    new CategoryCollection
                        {
                            Category = Conversions.ConvertEnumToInt(SecondLevelCategory.BrowserCommand),
                            List = new List<string>(),
                            Name = "BrowserCommandList"
                        },
                    new CategoryCollection
                        {
                            Category = Conversions.ConvertEnumToInt(SecondLevelCategory.InterfaceCommand),
                            List = new List<string>(),
                            Name = "InterfaceCommandList"
                        },
                    new CategoryCollection
                        {
                            Category = Conversions.ConvertEnumToInt(SecondLevelCategory.WebPageCommand),
                            List = new List<string>(),
                            Name = "WebPageCommand"
                        }
                };
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// this method will acquire test files from the data folder
        /// </summary>
        private void AcquireTestFiles()
        {
            try
            {
                var testFiles = FileManager.GetTestFiles();
                foreach (var testFile in testFiles)
                {
                    GetTestSet(testFile);
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// this method is used to acquire and assign test data to relevant test sets by passing the file path
        /// </summary>
        /// <param name="filepath">the path of the file</param>
        private void GetTestSet(string filepath)
        {
            try
            {
                var explicitFileName = filepath.Replace(VbwFileManager.FilePath(), String.Empty);
                if (explicitFileName.Substring(0, 3) != "fnc") return;
                var tempList = DataManager.GetFileData(filepath);
                var prefix = explicitFileName.Substring(4, 5);
                CheckFunctionalCategory(prefix, tempList);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will check the functional category of the test data according to the prefix and assign them to 
        /// relevant test sets
        /// </summary>
        /// <param name="prefix">Prefix</param>
        /// <param name="testData">Test data set acquired from the local folder</param>
        private void CheckFunctionalCategory(string prefix, IEnumerable<string> testData)
        {
            try
            {
                switch (prefix)
                {
                    case "brwsr":
                        var browserCommand = _secondLevelCategoryCollection.FirstOrDefault(rec => rec.Category == Conversions.ConvertEnumToInt(SecondLevelCategory.BrowserCommand));
                        if (browserCommand != null)
                            DataManager.AssignDataToTestSet(browserCommand.List, testData);
                        break;
                    case "intfc":
                        var interfaceCommand = _secondLevelCategoryCollection.FirstOrDefault(rec => rec.Category == Conversions.ConvertEnumToInt(SecondLevelCategory.InterfaceCommand));
                        if (interfaceCommand != null)
                            DataManager.AssignDataToTestSet(interfaceCommand.List, testData);
                        break;
                    case "wpage":
                        var webPageCommand = _secondLevelCategoryCollection.FirstOrDefault(rec => rec.Category == Conversions.ConvertEnumToInt(SecondLevelCategory.WebPageCommand));
                        if (webPageCommand != null)
                            DataManager.AssignDataToTestSet(webPageCommand.List, testData);
                        break;
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
        
        /// <summary>
        /// This method is used to calculate the Second level of probability of a given command
        /// </summary>
        /// <param name="command"></param>
        public List<CommandType> CalculateSecondLevelProbabilityOfCommand(string command)
        {
            try
            {
                new NaiveCommandCategorization(_secondLevelCategoryCollection).CalculateProbabilityOfSegments(
                        command.Split(' ').ToList(), true, out _secondLevelProbabilityScoreIndices);
                if (_secondLevelProbabilityScoreIndices == null) return null;
                var highestProbabilityCategories = new NaiveCommandCategorization().GetHighestProbabilityScoreIndeces(_secondLevelProbabilityScoreIndices);
                if (highestProbabilityCategories == null) return null;
                if (highestProbabilityCategories.Count != 1)
                {
                    throw new Exception("Command Identification Failed From the Second Level. There are " +
                                        highestProbabilityCategories.Count + " probable categories which are " + DataManager.GetHighestProbableCommandTypesForException<SecondLevelCategory>(highestProbabilityCategories));
                }
                var secondLevelHighestProbabilityCategory = highestProbabilityCategories.First();
                var secondLevelCategory =
                    Conversions.ConvertIntegerToEnum<SecondLevelCategory>(
                        secondLevelHighestProbabilityCategory.ReferenceId);
                return GetCommand(command, secondLevelCategory);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will get the most probable command types by command category
        /// </summary>
        /// <param name="command">command</param>
        /// <param name="category">category</param>
        /// <returns>probable command list</returns>
        private static List<CommandType> GetCommand(string command, SecondLevelCategory category)
        {
            try
            {
                switch (category)
                {
                    case SecondLevelCategory.BrowserCommand:
                        {
                            return new FunctionalBrowserCommands(command).GetCommand();
                        }
                    case SecondLevelCategory.InterfaceCommand:
                        {
                            return new FunctionalInterfaceCommands(command).GetCommand();
                        }
                    case SecondLevelCategory.WebPageCommand:
                        {
                            return new FunctionalWebpageCommands(command).GetCommand();
                        }
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}
