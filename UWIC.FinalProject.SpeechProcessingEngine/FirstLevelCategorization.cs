using System;
using System.Collections.Generic;
using System.Linq;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine.Commands;
using UWIC.FinalProject.SpeechProcessingEngine.Managers;

namespace UWIC.FinalProject.SpeechProcessingEngine
{
    public class FirstLevelCategorization
    {
        private List<CategoryCollection> _categoryCollection;
        private List<ProbabilityScoreIndex> _probabilityScoreIndices; 

        /// <summary>
        /// Constructor
        /// </summary>
        public FirstLevelCategorization()
        {
            // invoke the method to instantiate the first level category
            DefinePrimaryCategories();
            AcquireTestFiles();
        }

        /// <summary>
        /// This method will define the first level Categories for the Naive Command Categorization
        /// </summary>
        private void DefinePrimaryCategories()
        {
            try
            {
                // add first level category details to the category collection
                _categoryCollection = new List<CategoryCollection>
                {
                    new CategoryCollection
                        {
                            Category = Conversions.ConvertEnumToInt(FirstLevelCategory.FunctionalCommand),
                            List = new List<string>(),
                            Name = "FunctionalCommandList"
                        },
                    new CategoryCollection
                        {
                            Category = Conversions.ConvertEnumToInt(FirstLevelCategory.KeyboardCommand),
                            List = new List<string>(),
                            Name = "KeyboardCommandList"
                        },
                    new CategoryCollection
                        {
                            Category = Conversions.ConvertEnumToInt(FirstLevelCategory.MouseCommand),
                            List = new List<string>(),
                            Name = "MouseCommandList"
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
                // get the files
                var testFiles = FileManager.GetTestFiles();
                // if there are no files, throw an exception
                if (testFiles == null) throw new Exception("No Test Files Found");
                // foreach test file
                foreach (var testFile in testFiles)
                {
                    // get test data of the test file
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
                // get the file data of a given file path
                var tempList = DataManager.GetFileData(filepath);
                // get the explicit file name by removing the filepath
                var explicitFileName = filepath.Replace(VbwFileManager.FilePath(), String.Empty);
                // get the prefix of the file name
                var prefix = explicitFileName.Substring(0, 3);
                // invoke the method to store the file data according to the category
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
                // switch by the prefix (category) of a command
                switch (prefix)
                {
                    // if prefix is "fnc", add the command details to the functional command list
                    case "fnc":
                        var funcCommand =
                            _categoryCollection.FirstOrDefault(
                                rec => rec.Category == Conversions.ConvertEnumToInt(FirstLevelCategory.FunctionalCommand));
                        if (funcCommand != null)
                            DataManager.AssignDataToTestSet(funcCommand.List, testData);
                        break;
                    // if prefix is "key", add the command details to the keyboard command list
                    case "key":
                        var keyCommand =
                            _categoryCollection.FirstOrDefault(
                                rec => rec.Category == Conversions.ConvertEnumToInt(FirstLevelCategory.KeyboardCommand));
                        if (keyCommand != null)
                            DataManager.AssignDataToTestSet(keyCommand.List, testData);
                        break;
                    // if prefix is "mse", add the command details to the mouse command list
                    case "mse":
                        var mouseCommand =
                            _categoryCollection.FirstOrDefault(
                                rec => rec.Category == Conversions.ConvertEnumToInt(FirstLevelCategory.MouseCommand));
                        if (mouseCommand != null)
                            DataManager.AssignDataToTestSet(mouseCommand.List, testData);
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
        /// This method is used to calculate the First level of probability of a given command
        /// </summary>
        /// <param name="command"></param>
        public Dictionary<CommandType, object> CalculateProbabilityOfCommand(string command)
        {
            try
            {
                var probableCommands = new List<CommandType>();
                new NaiveCommandCategorization(_categoryCollection).CalculateProbabilityOfSegments(
                    command.Split(' ').ToList(), true, out _probabilityScoreIndices);
                if (_probabilityScoreIndices == null) return null;
                var highestProbabilityCategories = new NaiveCommandCategorization().GetHighestProbabilityScoreIndeces(_probabilityScoreIndices);
                if (highestProbabilityCategories.Count == 1)
                {
                    var firstLevelHighestProbabilityCategory = highestProbabilityCategories.First();
                    if (firstLevelHighestProbabilityCategory.ReferenceId ==
                        Conversions.ConvertEnumToInt(FirstLevelCategory.FunctionalCommand))
                        probableCommands = new SecondLevelCategorization().CalculateSecondLevelProbabilityOfCommand(command);
                    else if (firstLevelHighestProbabilityCategory.ReferenceId ==
                             Conversions.ConvertEnumToInt(FirstLevelCategory.KeyboardCommand))
                        probableCommands = new KeyboardCommands(command).GetCommand();
                    else if (firstLevelHighestProbabilityCategory.ReferenceId ==
                             Conversions.ConvertEnumToInt(FirstLevelCategory.MouseCommand))
                        probableCommands = new MouseCommands(command).GetCommand();
                }
                else
                {
                    throw new Exception("Command Identification Failed From the First Level. There are " +
                                        highestProbabilityCategories.Count + " probable categories which are " + DataManager.GetHighestProbableCommandTypesForException<FirstLevelCategory>(highestProbabilityCategories));
                }
                var returnDict = GetCommandDetails(probableCommands, command);
                if (returnDict.Count == 1)
                {
                    var identifiedCommandType = returnDict.First().Key;
                    if (LearningManager.UnIdentifiedWords.Count > 0)
                        LearningManager.AddUnidentifiedWordsToCommandText(identifiedCommandType);
                    DataManager.AddToCommandCounter(identifiedCommandType);
                }
                return returnDict;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will return the command type with/without parameters to be executed
        /// </summary>
        /// <param name="commandTypes"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        private static Dictionary<CommandType, object> GetCommandDetails(IReadOnlyCollection<CommandType> commandTypes, string command)
        {
            try
            {
                var resultDictionary = new Dictionary<CommandType, object>();
                if (commandTypes == null) return null;
                if (commandTypes.Count == 1)
                {
                    var commandType = commandTypes.FirstOrDefault();
                    switch (commandType)
                    {
                        case CommandType.go:
                            {
                                resultDictionary.Add(commandType, CommandParametersManager.GetWebsiteNameForGoCommand(command.Split(' ').ToList()));
                                break;
                            }
                        case CommandType.go_to_tab:
                            {
                                resultDictionary.Add(commandType, CommandParametersManager.GetTabIndexForGoToTabCommand(command));
                                break;
                            }
                        case CommandType.move:
                            {
                                resultDictionary.Add(commandType, CommandParametersManager.GetxyValuesToMouseMoveCommand(command));
                                break;
                            }
                        default:
                            {
                                resultDictionary.Add(commandType, "");
                                break;
                            }
                    }
                }
                return resultDictionary;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}