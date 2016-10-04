using System.Collections.Generic;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine.Managers;

namespace UWIC.FinalProject.SpeechProcessingEngine.Commands
{
    public class FunctionalWebpageCommands
    {
        private readonly List<CategoryCollection> _functionalWebpageCommandCategories;
        private readonly List<ProbabilityScoreIndex> _functionalWebpageCommandProbabilityScoreIndices;
        private readonly string _command;

        public FunctionalWebpageCommands(string command)
        {
            _command = command;
            _functionalWebpageCommandCategories = new List<CategoryCollection>();
            _functionalWebpageCommandProbabilityScoreIndices = new List<ProbabilityScoreIndex>();
        }

        public List<CommandType> GetCommand()
        {
            try
            {
                return new CommandManager(_command, "fnc_wpage", _functionalWebpageCommandCategories, _functionalWebpageCommandProbabilityScoreIndices)
                        .GetCommand();
            }
            catch (System.Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}