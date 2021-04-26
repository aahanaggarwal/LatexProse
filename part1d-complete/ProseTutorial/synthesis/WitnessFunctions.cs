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

        // map all placeholders to the correct symbol index in the token
        private (string[], string[], bool[], bool[], bool[], int[]) 
        computePartialMapping(string input, string output) {
            
            // delimiters, matches (), {}, [], and whitespace
            string delim = @"[(){}\[\]\s]";

            // separate input into symbols, keeping delimiters
            string[] token = Regex.Split(input, delim);
            string[] replacement = Regex.Split(output, delim);

            // declare and initialize arrays to false
            bool[] is_token_matched = Enumerable.Repeat(false, token.Length).ToArray();
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

                for (int j = 0; j < token.Length; j++) {
                    if (!is_token_matched[j] && replacement[i].Equals(token[j])) {
                        placeholder_index[i] = j;
                        is_token_matched[j] = true;
                        is_replacement_matched[i] = true;
                        break;
                    }
                }
            }

            return (
                token, 
                replacement, 
                is_token_matched, 
                is_replacement_matched, 
                is_replacement_delim, 
                placeholder_index
            );
        }


        /* Replace */

        [WitnessFunction(nameof(Semantics.Replace), 1)]
        public ExampleSpec WitnessReplaceTokens(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;
                var output = example.Value as string;

                result[inputState] = output;
            }
            return new ExampleSpec(result);
        }

        [WitnessFunction(nameof(Semantics.Replace), 2)]
        public ExampleSpec WitnessReplaceReplacements(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = inputState[rule.Body[0]] as string;
                var output = example.Value as string;

                result[inputState] = output;
            }
            return new ExampleSpec(result);
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
                    string[] token, 
                    string[] replacement, 
                    bool[] is_token_matched, 
                    bool[] is_replacement_matched, 
                    bool[] is_replacement_delim, 
                    int[] placeholder_index
                ) = computePartialMapping(input, output);

                List<Tuple<string, Tuple<int, int>>> range_list = new List<Tuple<string, Tuple<int, int>>>();

                // TODO: identify all unmatched symbols, like in Substring
                // Right now, match all unmatched `replacement` with the nearest matched `token`
                for (int i = 0; i < is_replacement_matched.Length; i++) {
                    if (!is_replacement_delim[i] && !is_replacement_matched[i]) {
                        for (int j = 0; j < is_token_matched.Length; j++) {
                            if (!is_token_matched[j]) {
                                string symbol = token[j];
                                int range_start = j - 1;
                                int range_end = j + 2;
                                is_token_matched[j] = true;

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

        [WitnessFunction(nameof(Semantics.Map), 0)]
        public ExampleSpec WitnessMapTokens(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var output = example.Value as string;

                result[inputState] = output;
            }
            return new ExampleSpec(result);
        }


        [WitnessFunction(nameof(Semantics.Map), 1)]
        public ExampleSpec WitnessMapTemplates(GrammarRule rule, ExampleSpec spec)
        {
            var result = new Dictionary<State, object>();
            foreach (KeyValuePair<State, object> example in spec.Examples)
            {
                State inputState = example.Key;
                var input = inputState[rule.Grammar.InputSymbol] as string;
                var output = example.Value as string;

                (
                    string[] token, 
                    string[] replacement, 
                    bool[] is_token_matched, 
                    bool[] is_replacement_matched, 
                    bool[] is_replacement_delim, 
                    int[] placeholder_index
                ) = computePartialMapping(input, output);

                List<Tuple<string, string[]>> templates = new List<Tuple<string, string[]>>();

                // TODO: identify all unmatched symbols, like in Substring
                // Right now, match all unmatched `replacement` with the nearest matched `token`
                for (int i = 0; i < is_replacement_matched.Length; i++) {
                    if (!is_replacement_delim[i] && !is_replacement_matched[i]) {
                        for (int j = 0; j < is_token_matched.Length; j++) {
                            if (!is_token_matched[j]) {
                                string symbol = token[j];
                                is_token_matched[j] = true;
                                templates.Add(new Tuple<string, string[]>(symbol, replacement));
                                break;
                            }
                        }
                    }
                }

                result[inputState] = templates;
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
                var input = inputState[rule.Grammar.InputSymbol] as string;
                var output = example.Value as string;

                (
                    string[] token, 
                    string[] replacement, 
                    bool[] is_token_matched, 
                    bool[] is_replacement_matched, 
                    bool[] is_replacement_delim, 
                    int[] placeholder_index
                ) = computePartialMapping(input, output);

                List<Tuple<string, int[], bool[]>> mappings = new List<Tuple<string, int[], bool[]>>();

                // TODO: identify all unmatched symbols, like in Substring
                // Right now, match all unmatched `replacement` with the nearest unmatched `token`
                for (int i = 0; i < is_replacement_matched.Length; i++) {
                    if (!is_replacement_delim[i] && !is_replacement_matched[i]) {
                        for (int j = 0; j < is_token_matched.Length; j++) {
                            if (!is_token_matched[j]) {
                                string symbol = token[j];
                                placeholder_index[i] = j;
                                is_token_matched[j] = true;

                                // deliberately skip `is_replacement_matched[j] = true`
                                // otherwise the LaTeX command will be replaced by user symbol
                                // e.g. "\frac" will be replaced by "/" 

                                // is_replacement_matched[j] = true;

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
