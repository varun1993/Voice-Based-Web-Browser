using System;
using System.Collections.Generic;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine.Managers;

namespace UWIC.FinalProject.SpeechProcessingEngine.Commands
{
    public class FunctionalBrowserCommands
    {
        private readonly List<CategoryCollection> _browserCommandCategories;
        private readonly List<ProbabilityScoreIndex> _browserCommandProbabilityScoreIndices;
        private readonly string _command;

        public FunctionalBrowserCommands(string command)
        {
            _command = command;
            _browserCommandCategories = new List<CategoryCollection>();
            _browserCommandProbabilityScoreIndices = new List<ProbabilityScoreIndex>();
        }

        public List<CommandType> GetCommand()
        {
            try
            {
                return new CommandManager(_command, "fnc_brwsr", _browserCommandCategories, _browserCommandProbabilityScoreIndices)
                    .GetCommand();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
    }
}
