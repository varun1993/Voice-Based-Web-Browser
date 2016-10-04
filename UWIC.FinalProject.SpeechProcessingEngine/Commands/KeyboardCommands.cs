using System.Collections.Generic;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine.Managers;

namespace UWIC.FinalProject.SpeechProcessingEngine.Commands
{
    public class KeyboardCommands : ICommandManager
    {
        private readonly List<CategoryCollection> _keyboardCommandCategories;
        private readonly List<ProbabilityScoreIndex> _keyCommandProbabilityScoreIndices;
        private readonly string _command;

        public KeyboardCommands(string command)
        {
            _command = command;
            _keyboardCommandCategories = new List<CategoryCollection>();
            _keyCommandProbabilityScoreIndices = new List<ProbabilityScoreIndex>();
        }

        public List<CommandType> GetCommand()
        {
            try
            {
                return new CommandManager(_command, "key", _keyboardCommandCategories, _keyCommandProbabilityScoreIndices)
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
