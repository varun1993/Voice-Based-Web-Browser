using System.Collections.Generic;
using UWIC.FinalProject.Common;

namespace UWIC.FinalProject.SpeechProcessingEngine.Managers
{
    interface ICommandManager
    {
        List<CommandType> GetCommand();
    }
}
