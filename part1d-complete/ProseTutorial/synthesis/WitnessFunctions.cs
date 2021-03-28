using System.Collections.Generic;
using System.Linq;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.Learning;
using Microsoft.ProgramSynthesis.Rules;
using Microsoft.ProgramSynthesis.Specifications;

namespace ProseTutorial
{
    public class WitnessFunctions : DomainLearningLogic
    {
        public WitnessFunctions(Grammar grammar) : base(grammar)
        {
        }

        [WitnessFunction(nameof(Semantics.Replace), 1)]
        public DisjunctiveExampleSpec WitnessStartPosition(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();

            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;
                var output = example.Value as string;

                // Method 1:
                // 1. separate `input` and `output` into array of words, `in_words` and `out_words`
                // 2. for each word in `in_words`, find the corresponding match in  `out_words`
                // 3. every word will have a mapping, whether or not that word is a content or a symbol


                // Method 2:
                // 1. separate `input` and `output` into array of words, `in_words` and `out_words`
                // 2. remove words that are the same in the same index of both arrays
                // 3. we will be left with direct mappings of only the relevant commands at the same index

                // We choose Method 2.

                // 1. separate input into words delimited by whitespace
                List<string> in_words = input.Split(' ').ToList();
                List<string> out_words = output.Split(' ').ToList();

                // in_words and out_words should have the same length

                // 2. remove words that are the same in the same index of both arrays
                // going backwards ensures that we don't miss any elements
                for (int i = in_words.Count - 1; i >= 0 ; i--) {
                    if (in_words[i].Equals(out_words[i])) {
                        in_words.RemoveAt(i);
                        out_words.RemoveAt(i);
                    }
                }

                // result[inputState] = in_words[0];

                var occurrences = new List<string>();

                for (int i = 0; i < in_words.Count; i++) {
                    occurrences.Add(out_words[i]);
                }

                if (occurrences.Count == 0) return null;
                result[inputState] = occurrences.Cast<object>();
            }
            return new ExampleSpec(result);
        }

        [WitnessFunction(nameof(Semantics.Replace), 2)]
        public ExampleSpec WitnessEndPosition(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;
                var output = example.Value as string;

                // We choose Method 2.

                // 1. separate input into words delimited by whitespace
                List<string> in_words = input.Split(' ').ToList();
                List<string> out_words = output.Split(' ').ToList();

                // in_words and out_words should have the same length

                // 2. remove words that are the same in the same index of both arrays
                // going backwards ensures that we don't miss any elements
                for (int i = in_words.Count - 1; i >= 0 ; i--) {
                    if (in_words[i].Equals(out_words[i])) {
                        in_words.RemoveAt(i);
                        out_words.RemoveAt(i);
                    }
                }

                result[inputState] = out_words[0];
            }
            return new ExampleSpec(result);
        }
    }
}