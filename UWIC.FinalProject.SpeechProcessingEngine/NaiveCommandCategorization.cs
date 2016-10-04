using System;
using System.Collections.Generic;
using System.Linq;
using UWIC.FinalProject.Common;
using UWIC.FinalProject.SpeechProcessingEngine.Managers;

namespace UWIC.FinalProject.SpeechProcessingEngine
{
    public class NaiveCommandCategorization
    {
        private readonly List<CategoryCollection> _categoryCollection;
        private List<ProbabilityScoreIndex> _probabilityScoreIndices; 

        public NaiveCommandCategorization(){}

        public NaiveCommandCategorization(List<CategoryCollection> categoryCollection)
        {
            _categoryCollection = categoryCollection;
        }

        public void CalculateProbabilityOfSegments(IEnumerable<string> speechCommand, bool assignUnknownCommands, out List<ProbabilityScoreIndex> probabilityScoreIndices)
        {
            try
            {
                LearningManager.UnIdentifiedWords = new List<string>(); 
                // Add Probability Score Classes to the Respective Sets before going through the loop
                // So that their values can be concatenated after each loop
                _probabilityScoreIndices = new List<ProbabilityScoreIndex>();
                foreach (var category in _categoryCollection)
                {
                    _probabilityScoreIndices.Add(new ProbabilityScoreIndex
                        {
                            ProbabilityScore = 0,
                            ReferenceId = category.Category
                        });
                }

                // For each speech segment
                foreach (var segment in speechCommand)
                {
                    var booleanProbabilities = (from category in _categoryCollection
                                                where category.List.Contains(segment.ToLower())
                                                select new BooleanProbabilityIndex
                                                    {
                                                        Available = true, 
                                                        ReferenceId = category.Category
                                                    }).ToList(); // foreach list class, add boolean probability classes to the list

                    if (assignUnknownCommands)
                    {
                        if (booleanProbabilities.Count == 0) 
                            LearningManager.UnIdentifiedWords.Add(segment);
                    }
                    var availableCount = booleanProbabilities.Count(rec => rec.Available);
                    var probabilityOfBelongness = availableCount > 0 ? (1.0/Convert.ToDouble(availableCount)) : 0;
                    foreach (var obj in from booleanProbability in booleanProbabilities where booleanProbability.Available select _probabilityScoreIndices.FirstOrDefault(
                        rec => rec.ReferenceId == booleanProbability.ReferenceId) into obj where obj != null select obj)
                    {
                        obj.ProbabilityScore += probabilityOfBelongness;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
            finally
            {
                probabilityScoreIndices = _probabilityScoreIndices;
            }
        }

        public List<ProbabilityScoreIndex> GetHighestProbabilityScoreIndeces(List<ProbabilityScoreIndex> probabilityScoreIndices)
        {
            try
            {
                var maximumValue = probabilityScoreIndices.Select(row => row.ProbabilityScore).Max();
                return probabilityScoreIndices.Where(row => Math.Abs(row.ProbabilityScore - maximumValue) < Double.Epsilon).ToList();
            }
            catch (Exception ex)
            {
                Log.ErrorLog(ex);
                throw;
            }
            
        }
    }
}
