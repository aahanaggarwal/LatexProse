using System;
using System.Collections.Generic;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.AST;
using Microsoft.ProgramSynthesis.Features;

namespace ProseTutorial
{
    public class RankingScore : Feature<double>
    {
        public RankingScore(Grammar grammar) : base(grammar, "Score")
        {
        }

        protected override double GetFeatureValueForVariable(VariableNode variable)
        {
            return 0;
        }

        [FeatureCalculator(nameof(Semantics.Replace))]
        public static double RankingReplace(double v, double tokens, double replacements)
        {
            return 0;
        }

        [FeatureCalculator(nameof(Semantics.Split))]
        public static double RankingSplit(double v, double range)
        {
            return range;
        }

        [FeatureCalculator(nameof(Semantics.Map))]
        public static double RankingMap(double tokens, double templates, double mappings)
        {
            return 0;
        }

        [FeatureCalculator("range", Method = CalculationMethod.FromLiteral)]
        public static double RankingRange(List<Tuple<string, Tuple<int, int>>> range)
        {
            return 0;
        }

        [FeatureCalculator("templates", Method = CalculationMethod.FromLiteral)]
        public static double RankingTemplates(List<Tuple<string, string[]>> templates)
        {
            return 0;
        }

        [FeatureCalculator("mappings", Method = CalculationMethod.FromLiteral)]
        public static double RankingMappings(List<Tuple<string, int[], bool[]>> mappings)
        {
            return 0;
        }
    }
}