using System.Collections.Generic;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine.Managers;

namespace UWIC.FinalProject.SpeechProcessingEngine.Commands
{
    public class MouseCommands
    {
        private readonly List<CategoryCollection> _mouseCommandCategories;
        private readonly List<ProbabilityScoreIndex> _mouseCommandProbabilityScoreIndices;
        private readonly string _command;

        public MouseCommands(string command)
        {
            _command = command;
            _mouseCommandCategories = new List<CategoryCollection>();
            _mouseCommandProbabilityScoreIndices = new List<ProbabilityScoreIndex>();
        }

        public List<CommandType> GetCommand()
        {
            try
            {
                return new CommandManager(_command, "mse", _mouseCommandCategories, _mouseCommandProbabilityScoreIndices)
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
