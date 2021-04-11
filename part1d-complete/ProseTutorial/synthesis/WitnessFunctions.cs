using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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

        private (string[], string[], bool[], bool[], bool[], int[]) 
        computePartialMapping(string input, string output) {
            
            // delimiters, matches (), {}, [], and whitespace
            string delim = @"[(){}\[\]\s]";

            // separate input into symbols, keeping delimiters
            string[] word = Regex.Split(input, delim);
            string[] replacement = Regex.Split(output, delim);

            // declare and initialize arrays to false
            bool[] is_word_matched = Enumerable.Repeat(false, word.Length).ToArray();
            bool[] is_replacement_matched = Enumerable.Repeat(false, replacement.Length).ToArray();
            bool[] is_replacement_delim = Enumerable.Repeat(false, replacement.Length).ToArray();

            int[] placeholder_index = new int[replacement.Length];

            // compute index change at every symbol in replacement
            for (int i = 0; i < replacement.Length; i++) {

                // skip brackets
                if (Regex.IsMatch(replacement[i], delim)) {
                    is_replacement_delim[i] = true;
                    continue;
                }

                // TODO: this is greedy matching. should do something like Substring where we
                // send in all possible choices and let PROSE take care of choosing the correct one

                // if word = replacement, then (j - i) is the index of this symbol in `word`,
                // relative to the index in `replacement`
                for (int j = 0; j < word.Length; j++) {
                    if (!is_word_matched[j] && replacement[i].Equals(word[j])) {
                        placeholder_index[i] = j;
                        is_word_matched[j] = true;
                        is_replacement_matched[i] = true;
                        break;
                    }
                }
            }

            return (
                word, 
                replacement, 
                is_word_matched, 
                is_replacement_matched, 
                is_replacement_delim, 
                placeholder_index
            );
        }


        /* Split */

        [WitnessFunction(nameof(Semantics.Split), 1)]
        public ExampleSpec WitnessSplitRange(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;
                var output = example.Value as string;

                (
                    string[] word, 
                    string[] replacement, 
                    bool[] is_word_matched, 
                    bool[] is_replacement_matched, 
                    bool[] is_replacement_delim, 
                    int[] placeholder_index
                ) = computePartialMapping(input, output);

                List<Tuple<string, Tuple<int, int>>> range_list = new List<Tuple<string, Tuple<int, int>>>();

                // TODO: identify all unmatched symbols, like in Substring
                // Right now, match all unmatched `replacement` with the nearest unmatched `word`
                for (int i = 0; i < is_replacement_matched.Length; i++) {
                    if (!is_replacement_delim[i] && !is_replacement_matched[i]) {
                        for (int j = 0; j < is_word_matched.Length; j++) {
                            if (!is_word_matched[j]) {
                                string symbol = word[j];
                                int range_start = -j;
                                int range_end = replacement.Length - j;
                                is_word_matched[j] = true;

                                Tuple<int, int> range = new Tuple<int, int>(range_start, range_end);
                                range_list.Add(new Tuple<string, Tuple<int, int>>(symbol, range));
                                break;
                            }
                        }
                    }
                }

                result[inputState] = range_list;
            }
            return new ExampleSpec(result);
        }


        
        /* Map */

        [WitnessFunction(nameof(Semantics.Map), 1)]
        public ExampleSpec WitnessMapFormats(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;
                var output = example.Value as string;

                (
                    string[] word, 
                    string[] replacement, 
                    bool[] is_word_matched, 
                    bool[] is_replacement_matched, 
                    bool[] is_replacement_delim, 
                    int[] placeholder_index
                ) = computePartialMapping(input, output);

                List<Tuple<string, string[]>> formats = new List<Tuple<string, string[]>>();

                // TODO: identify all unmatched symbols, like in Substring
                // Right now, match all unmatched `replacement` with the nearest unmatched `word`
                for (int i = 0; i < is_replacement_matched.Length; i++) {
                    if (!is_replacement_delim[i] && !is_replacement_matched[i]) {
                        for (int j = 0; j < is_word_matched.Length; j++) {
                            if (!is_word_matched[j]) {
                                string symbol = word[j];
                                is_word_matched[j] = true;
                                formats.Add(new Tuple<string, string[]>(symbol, replacement));
                                break;
                            }
                        }
                    }
                }

                result[inputState] = formats;
            }
            return new ExampleSpec(result);
        }


        [WitnessFunction(nameof(Semantics.Map), 2)]
        public ExampleSpec WitnessMapMappings(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;
                var output = example.Value as string;

                (
                    string[] word, 
                    string[] replacement, 
                    bool[] is_word_matched, 
                    bool[] is_replacement_matched, 
                    bool[] is_replacement_delim, 
                    int[] placeholder_index
                ) = computePartialMapping(input, output);

                List<Tuple<string, int[], bool[]>> mappings = new List<Tuple<string, int[], bool[]>>();

                // TODO: identify all unmatched symbols, like in Substring
                // Right now, match all unmatched `replacement` with the nearest unmatched `word`
                for (int i = 0; i < is_replacement_matched.Length; i++) {
                    if (!is_replacement_delim[i] && !is_replacement_matched[i]) {
                        for (int j = 0; j < is_word_matched.Length; j++) {
                            if (!is_word_matched[j]) {
                                string symbol = word[j];
                                placeholder_index[i] = j;
                                is_word_matched[j] = true;

                                // deliberately skip `is_replacement_matched = true`
                                // otherwise the LaTeX command will be replaced by user symbol
                                // e.g. "\frac" will be replaced by "/" 

                                // is_replacement_matched = true;

                                mappings.Add(new Tuple<string, int[], bool[]>(symbol, placeholder_index, is_replacement_matched));
                                break;
                            }
                        }
                    }
                }

                result[inputState] = mappings;
            }
            return new ExampleSpec(result);
        }
    }
}
