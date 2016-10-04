using System;
using System.Collections.Generic;
using System.Linq;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine.Managers
{
    public class CommandManager : ICommandManager
    {
        private string Command { get; set; }
        private string Prefix { get; set; }
        private List<CategoryCollection> _commandCategories;
        private List<ProbabilityScoreIndex> _commandProbabilityScoreIndices;

        public CommandManager(string command, string prefix, List<CategoryCollection> commandCategories,
                              List<ProbabilityScoreIndex> commandProbabilityScoreIndices)
        {
            Command = command;
            Prefix = prefix;
            _commandCategories = commandCategories;
            _commandProbabilityScoreIndices = commandProbabilityScoreIndices;
        }

        public List<CommandType> GetCommand()
        {
            var probableCommandTypes = new List<CommandType>();
            try
            {
                _commandCategories = FileManager.GetCategoryInstances(FileManager.GetExplicitFileNamesByPrefix(FileManager.GetTestFiles(), Prefix).ToArray(), Prefix);
                DataManager.PopulateDataToCategories(_commandCategories, FileManager.GetTestFiles(), Prefix);
                new NaiveCommandCategorization(_commandCategories).CalculateProbabilityOfSegments(Command.Split(' ').ToList(), true, out _commandProbabilityScoreIndices);
                var highestProbabilityCategories = new NaiveCommandCategorization().GetHighestProbabilityScoreIndeces(_commandProbabilityScoreIndices);
                if (highestProbabilityCategories != null)
                {
                    if (highestProbabilityCategories.Count != 1)
                    {
                        var commands =
                            DataManager.GetHighestProbableCommands<CommandType>(
                                highestProbabilityCategories);
                        highestProbabilityCategories = IdentifyLemmaConformation(Command.Split(' '), commands);
                        if (highestProbabilityCategories.Count > 1)
                        {
                            throw new Exception("Command Identification failed from the final level!");
                        }
                    }
                    probableCommandTypes.AddRange(
                        highestProbabilityCategories.Select(
                            highestProbabilityCategory =>
                            Conversions.ConvertIntegerToEnum<CommandType>(highestProbabilityCategory.ReferenceId)));
                    var fileNameWithPrefix = Prefix + "_" + probableCommandTypes.First().ToString();

                    // Acquiring unknown words for the fnc_browsr_go command is not facilitated due to that command
                    // being a special scenario where the website names are located in a different test file
                    if (fileNameWithPrefix != "fnc_brwsr_go")
                        DataManager.AcquireUnknownWordsForACommand(Command.Split(' ').ToList(), fileNameWithPrefix);
                }
                return probableCommandTypes;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        private static List<ProbabilityScoreIndex> IdentifyLemmaConformation(IEnumerable<string> command, IEnumerable<CommandType> highestProbableCommands)
        {

            try
            {
                var identifiedCategoryCollection = new List<CategoryCollection>();
                foreach (var highestProbableCommand in highestProbableCommands)
                {
                    var categoryCollection = new CategoryCollection
                        {
                            Name = highestProbableCommand.ToString(),
                            Category = Conversions.ConvertEnumToInt(
                                Conversions.ConvertStringToEnum<CommandType>(highestProbableCommand.ToString()))
                        };
                    var splittedCommand = highestProbableCommand.ToString().Split('_');
                    var newSplittedCommandList = splittedCommand.Where(cmd => !String.IsNullOrEmpty(cmd)).ToList();
                    categoryCollection.List = newSplittedCommandList;
                    identifiedCategoryCollection.Add(categoryCollection);
                }


                List<ProbabilityScoreIndex> probabilityScoreIndices;
                new NaiveCommandCategorization(identifiedCategoryCollection).CalculateProbabilityOfSegments(command, false,
                                                                                                            out
                                                                                                            probabilityScoreIndices);
                return new NaiveCommandCategorization().GetHighestProbabilityScoreIndeces(probabilityScoreIndices);
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}
