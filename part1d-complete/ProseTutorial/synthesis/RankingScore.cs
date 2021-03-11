using System;
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
        public static double Substring(double v, double start, double end)
        {
            // return start * end;
            return 0;
        }

        // [FeatureCalculator(nameof(Semantics.AbsPos))]
        // public static double AbsPos(double v, double k)
        // {
        //     return k;
        // }

        [FeatureCalculator("symbol", Method = CalculationMethod.FromLiteral)]
        public static double K(string symbol)
        {
            // return 1.0 / symbol.Length;
            return 0;
        }
    }
}