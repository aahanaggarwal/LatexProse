using System.Collections.Generic;
using System.Linq;
using Microsoft.ProgramSynthesis;
using Microsoft.ProgramSynthesis.Learning;
using Microsoft.ProgramSynthesis.Rules;
using Microsoft.ProgramSynthesis.Specifications;
using System.Text.RegularExpressions;

namespace ProseTutorial
{
    public class WitnessFunctions : DomainLearningLogic
    {
        public WitnessFunctions(Grammar grammar) : base(grammar)
        {
        }

        [WitnessFunction(nameof(Semantics.Replace), 1)]
        public ExampleSpec WitnessStartPosition(GrammarRule rule, ExampleSpec spec)
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

                // delimiters, matches (), {}, [], and whitespace
                string delim = @"[(){}\[\]\s]";

                // separate input into symbols, keeping delimiters
                string[] in_symbols = Regex.Split(input, delim);
                string[] out_symbols = Regex.Split(output, delim);

                // declare and initialize arrays to false
                bool[] is_matched_in = Enumerable.Repeat(false, in_symbols.Length).ToArray();
                bool[] is_matched_out = Enumerable.Repeat(false, out_symbols.Length).ToArray();
                bool[] is_delim_out = Enumerable.Repeat(false, out_symbols.Length).ToArray();

                int[] index_change = new int[out_symbols.Length];

                // compute index change
                for (int i = 0; i < out_symbols.Length; i++) {

                    // skip brackets
                    if (Regex.IsMatch(out_symbols[i], delim)) {
                        is_delim_out[i] = true;
                        continue;
                    }

                    for (int j = 0; j < in_symbols.Length; j++) {
                        // word matched
                         if (!is_matched_in[j] && out_symbols[i].Equals(in_symbols[j])) {
                             index_change[i] = j - i;
                             is_matched_in[j] = true;
                             is_matched_out[i] = true;
                             break;
                         }
                    }
                }

                // TODO: identify all unmatched symbols, like in Substring
                // Right now, match all unmatched out_symbol with the nearest unmatched in_symbol
                for (int i = 0; i < is_matched_out.Length; i++) {
                    if (!is_delim_out[i] && !is_matched_out[i]) {
                        for (int j = 0; j < is_matched_in.Length; j++) {
                            if (!is_matched_in[j]) {
                                index_change[i] = j - i;
                                is_matched_in[j] = true;
                                is_matched_out[i] = true;
                                break;
                            }
                        }
                    }
                }

                result[inputState] = in_symbols;




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

                result[inputState] = in_words[0];

                // var occurrences = new List<int>();

                // for (int i = input.IndexOf(output); i >= 0; i = input.IndexOf(output, i + 1)) occurrences.Add(i);

                // if (occurrences.Count == 0) return null;
                // result[inputState] = occurrences.Cast<object>();
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