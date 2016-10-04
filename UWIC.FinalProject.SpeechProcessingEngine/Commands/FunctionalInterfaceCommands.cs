using System.Collections.Generic;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine.Managers;

namespace UWIC.FinalProject.SpeechProcessingEngine.Commands
{
    public class FunctionalInterfaceCommands
    {
        private readonly List<CategoryCollection> _interfaceCommandCategories;
        private readonly List<ProbabilityScoreIndex> _interfaceCommandProbabilityScoreIndices;
        private readonly string _command;

        public FunctionalInterfaceCommands(string command)
        {
            _command = command;
            _interfaceCommandCategories = new List<CategoryCollection>();
            _interfaceCommandProbabilityScoreIndices = new List<ProbabilityScoreIndex>();
        }

        public List<CommandType> GetCommand()
        {
            try
            {
                return new CommandManager(_command, "fnc_intfc", _interfaceCommandCategories, _interfaceCommandProbabilityScoreIndices)
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
