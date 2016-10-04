using System;
using System.Collections.Generic;
using System.Linq;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine.Managers;

namespace UWIC.FinalProject.SpeechProcessingEngine.Commands
{
    public class CommandParametersManager
    {
        # region Functional Command : Go to Website
        /// <summary>
        /// this method will acquire the website name from a given command which has been identified
        /// as a go to website command.
        /// </summary>
        /// <param name="commandSegments">command segments as a list (to remove problems that may occur when there are spaces)</param>
        /// <returns>identified website name</returns>
        public static string GetWebsiteNameForGoCommand(ICollection<string> commandSegments)
        {
            try
            {
                var fileContent = FileManager.GetContentOfAFile("fnc_brwsr_go"); // get the contents of the respective file as a list
                // the file content above do not comprise of the website names
                // thus the underneath foreach loop will iterate through the file content
                // and if the any of the content is available in the command segments, it will remove them
                // so that the website name will be remaining at the end
                foreach (var content in fileContent.Where(commandSegments.Contains))
                {
                    commandSegments.Remove(content);
                }
                // from the remaining command segment, if the user has provided the command as 
                // for instance google dot com, the "dot" will be replaced by a "." and finally the command
                // segments will be concatenated and returned to the user
                return commandSegments.Aggregate("", (current, segment) => current + (segment == "dot" ? "." : segment));
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        #region Functional Command : Go to Tab Index
        /// <summary>
        /// This method will return the tab index of a command identified 
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>tab index value</returns>
        public static string GetTabIndexForGoToTabCommand(string command)
        {
            try
            {
                var tabIndex = "";
                var gotoCommandContent = FileManager.GetContentOfAFile("fnc_intfc_go_to_tab"); // get content of the given file
                var indexValues = FileManager.GetContentOfAFile("data_indices"); // get the indeces that may include in the command from one to ten, to zeroth to ninth
                var indexValueMappings = FileManager.GetContentOfAFile("data_indexValueMapping"); // get the value mapping for each index (ex: zero , zero = 0)

                // aggregate through the command content, and if available in the go to tab command and not available in the data indeces (which are words such as go ,to ,tab)
                // remove them from the command, which will make the index value mentioned in the command remaining. (ex: two)
                command = gotoCommandContent.Where(content => !indexValues.Contains(content)).Aggregate(command, (current, content) => current.Replace(content, String.Empty));

                // finally iterate through the index value mappings and get the actual value for the value available in the command
                // ex : (9 for nine)
                foreach (var indexValueMapping in indexValueMappings.Where(indexValueMapping => indexValueMapping.Contains(command.Trim())))
                {
                    tabIndex = GetValueForMapping(indexValueMapping);
                }
                //return the identified tab index
                return tabIndex;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        /// <summary>
        /// This method will return the index value mapping for a given row
        /// </summary>
        /// <param name="row">row</param>
        /// <returns></returns>
        private static string GetValueForMapping(string row)
        {
            try
            {
                // split the row by | and return the last value
                return row.Split('|').Last();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
        #endregion

        # region Mouse Command : Move Mouse to X Y
        public static string GetxyValuesToMouseMoveCommand(string command)
        {
            try
            {
                var mouseMoveCommandContent = FileManager.GetContentOfAFile("mse_move"); // get the content of the given file
                var indexValues = FileManager.GetContentOfAFile("data_mouseMoveIndices"); // get the indeces that may include in the command from 0 to 9 and zero to nine
                if (command == null) return null;
                foreach (var content in mouseMoveCommandContent)
                {
                    if ((command.Contains(content) && !indexValues.Contains(content)))
                        command = command.Replace(content, String.Empty);
                }
                var commandIndex = command.Trim();
                var value = GetValueMappingsForEachCommand(commandIndex.Split(' '));
                var xCoordinate = value.Substring(0, 4);
                var yCoordinate = value.Substring(4, 4);
                return xCoordinate + "," + yCoordinate;
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }

        private static string GetValueMappingsForEachCommand(IEnumerable<string> indeces)
        {
            try
            {
                var mouseIndexValueMappings = FileManager.GetContentOfAFile("data_indexValueMapping");
                return (from index in indeces
                        where
                            !index.Equals("y", StringComparison.OrdinalIgnoreCase) &&
                            !index.Equals("x", StringComparison.OrdinalIgnoreCase)
                        from mouseIndexValueMapping in mouseIndexValueMappings
                        where mouseIndexValueMapping.Contains(index)
                        select mouseIndexValueMapping).Aggregate("",
                                                                 (current, mouseIndexValueMapping) =>
                                                                 current + mouseIndexValueMapping.Split('|').Last());
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
        }
        #endregion
    }
}
