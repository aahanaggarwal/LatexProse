using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ProseTutorial
{
    public static class Semantics
    {
        public static string Replace(string v, List<Tuple<string, string>> tokens, List<string> replacements)
        {
            string modified = Regex.Replace(v, @"\s+", " ");

            for (int i = 0; i < tokens.Count; i++) {
                modified = modified.Replace(tokens[i].Item2, replacements[i]);
            }

            return modified;
        }

        public static List<Tuple<string, string>> Split(string v, List<Tuple<string, Tuple<int, int>>> range_list)
        {
            // delimiters, matches all non-words and numbers
            string delim = @"([^\w\s\^\\]+)|([_^])|(\s+)";

            // list of symbols to replace
            List<string> symbol_list = new List<string>(range_list.Select(symbol => symbol.Item1));

            // separate input into symbols, keeping delimiters but removing whitespace
            string[] token = Regex.Split(v, delim);
            token = token.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

            List<string> input = new List<string>(token);

            // this method's return value
            List<Tuple<string, string>> substrings = new List<Tuple<string, string>>();

            // loop through the input and find the substrings to replace
            for (int i = 0; i < input.Count; i++) {
                if (symbol_list.Contains(input[i])) {
                    int index = symbol_list.IndexOf(input[i]);
                    Tuple<int, int> local_range = range_list[index].Item2;

                    int start = Math.Max(i + local_range.Item1, 0);
                    int count = Math.Min(local_range.Item2, input.Count);

                    List<string> sublist = input.GetRange(start, count);
                    string substring_ws = String.Join(" ", sublist);
                    string substring_no_ws = String.Join("", sublist);
                    substrings.Add(new Tuple<string, string>(input[i], substring_ws));
                    substrings.Add(new Tuple<string, string>(input[i], substring_no_ws));
                }
            }

            return substrings;
        }

        public static List<string> Map(
            List<Tuple<string, string>> tokens, 
            List<Tuple<string, string[]>> templates, 
            List<Tuple<string, int[], bool[]>> mappings
        ) {

            // delimiters, matches all non-words and numbers
            string delim = @"([^\w\s\^\\]+)|([_^])|(\s+)";

            List<string> replacements = new List<string>();

            for (int i = 0; i < tokens.Count; i++) {
                string symbol = tokens[i].Item1;
                string token = tokens[i].Item2;

                int template_index = templates.FindIndex(elem => elem.Item1 == symbol);
                string[] template = templates[template_index].Item2;
                int[] placeholder_index = mappings[template_index].Item2;
                bool[] is_matched_out = mappings[template_index].Item3;

                string[] token_ws = Regex.Split(token, delim);
                string[] token_no_ws = token_ws.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
                List<string> input = new List<string>(token_no_ws);

                // map user input to correct position
                for (int j = 0; j < template.Length; j++) {
                    if (is_matched_out[j]) {
                        if (placeholder_index[j] < input.Count) {
                            template[j] = input[placeholder_index[j]];
                        }
                    }
                }
                replacements.Add(String.Join("", template));
            }

            return replacements;
        }
    }
}