using System.Collections.Generic;

namespace UWIC.FinalProject.SpeechProcessingEngine
{
    public class CategoryCollection
    {
        public string Name { get; set; }
        public int Category { get; set; }
        public List<string> List { get; set; }
    }

    public class BooleanProbabilityIndex
    {
        public int ReferenceId { get; set; }
        public bool Available { get; set; }
    }

    public class ProbabilityScoreIndex
    {
        public int ReferenceId { get; set; }
        public double ProbabilityScore { get; set; }
    }
}
