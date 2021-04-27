using System;
using System.Linq;
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
            // prefer a uniform Split output
            return 1.0 / (tokens - replacements);
        }

        [FeatureCalculator(nameof(Semantics.Split))]
        public static double RankingSplit(double v, double range_list)
        {
            return range_list;
        }

        [FeatureCalculator(nameof(Semantics.Map))]
        public static double RankingMap(double tokens, double templates, double mappings)
        {
            return tokens;
        }

        [FeatureCalculator("range_list", Method = CalculationMethod.FromLiteral)]
        public static double RankingRange(List<Tuple<string, Tuple<int, int>>> range_list)
        {
            // minimum distance from any operator
            List<int> pos_values = range_list.ConvertAll<int>(symbol => Math.Abs(symbol.Item2.Item1));
            int pos_score = pos_values.Min();

            // size of range
            int count = range_list[0].Item2.Item2;

            // prefer smaller count greater than 1
            if (count == 1) {
                count *= 10;
            }

            // prefer smaller range closer to operator
            return 1.0 / (count * pos_score);
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