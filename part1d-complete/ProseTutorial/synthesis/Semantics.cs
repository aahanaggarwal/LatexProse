using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ProseTutorial
{
    public static class Semantics
    {
        public static string Replace(string v, List<Tuple<string, string>> words, List<string> replacements)
        {
            string modified = v;

            for (int i = 0; i < words.Count; i++) {
                modified = modified.Replace(words[i].Item2, replacements[i]);
            }

            return modified;
        }

        public static List<Tuple<string, string>> Split(string v, List<Tuple<string, Tuple<int, int>>> range_list)
        {
            // delimiters, matches (), {}, [], and whitespace
            string delim = @"[(){}\[\]\s]";

            // list of symbols to replace
            List<string> symbol_list = new List<string>(range_list.Select(symbol => symbol.Item1));

            // separate input into symbols, keeping delimiters
            List<string> input = new List<string>(Regex.Split(v, delim));

            // this method's return value
            List<Tuple<string, string>> substrings = new List<Tuple<string, string>>();

            // loop through the input and find the substrings to replace
            for (int i = 0; i < input.Count; i++) {
                if (symbol_list.Contains(input[i])) {
                    int index = symbol_list.IndexOf(input[i]);
                    Tuple<int, int> local_range = range_list[index].Item2;

                    int start = Math.Max(i + local_range.Item1, 0);
                    int end = Math.Min(i + local_range.Item2, input.Count);

                    List<string> sublist = input.GetRange(start, end);
                    string substring = String.Join("", sublist);
                    substrings.Add(new Tuple<string, string>(input[i], substring));
                }
            }

            return substrings;
        }

        public static List<string> Map(
            List<Tuple<string, string>> words, 
            List<Tuple<string, string[]>> formats, 
            List<Tuple<string, int[], bool[]>> mappings
        ) {

            string delim = @"[(){}\[\]\s]";
            List<string> replacements = new List<string>();

            for (int i = 0; i < words.Count; i++) {
                string symbol = words[i].Item1;
                string word = words[i].Item2;

                int format_index = formats.FindIndex(elem => elem.Item1 == symbol);
                string[] format = formats[format_index].Item2;
                int[] placeholder_index = mappings[format_index].Item2;
                bool[] is_matched_out = mappings[format_index].Item3;

                List<string> input = new List<string>(Regex.Split(word, delim));

                // map user input to correct position
                for (int j = 0; j < format.Length; j++) {
                    if (is_matched_out[j]) {
                        format[j] = input[placeholder_index[j]];
                    }
                }
                replacements.Add(String.Join("", format));
            }

            return replacements;
        }
    }
}