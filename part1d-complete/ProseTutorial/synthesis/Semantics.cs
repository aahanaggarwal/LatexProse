using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ProseTutorial
{
    public static class Semantics
    {
        public static string Replace(string v, List<(string, string)> words, string[] replacements)
        {
            string modified = v;

            for (int i = 0; i < words.Count; i++) {
                modified = modified.Replace(words[i].Item2, replacements[i]);
            }

            return modified;
        }

        public static List<string> Split(string v, List<(string, (int, int))> range_list)
        {
            // delimiters, matches (), {}, [], and whitespace
            string delim = @"[(){}\[\]\s]";

            // list of symbols to replace
            List<string> symbol_list = new List<string>(range_list.Select(symbol => symbol.Item1));

            // separate input into symbols, keeping delimiters
            List<string> input = new List<string>(Regex.Split(v, delim));

            // this method's return value
            List<string> substrings = new List<string>();

            // loop through the input and find the substrings to replace
            for (int i = 0; i < input.Count; i++) {
                if (symbol_list.Contains(input[i])) {
                    int index = symbol_list.IndexOf(input[i]);
                    (int, int) local_range = range_list[index].Item2;

                    int start = Math.Max(i + local_range.Item1, 0);
                    int end = Math.Min(i + local_range.Item2, input.Count);

                    List<string> sublist = input.GetRange(start, end);
                    string substring = String.Join("", sublist);
                    substrings.Add(substring);
                }
            }

            return substrings;
        }

        public static List<string> Map(
            List<(string, string)> words, 
            List<(string, string[])> formats, 
            List<(string, int[], bool[])> mappings
        ) {

            string delim = @"[(){}\[\]\s]";
            List<string> replacements = new List<string>();

            for (int i = 0; i < words.Count; i++) {
                string symbol = words[i].Item1;
                string word = words[i].Item2;

                int format_index = formats.FindIndex(elem => elem.Item1 == symbol);
                string[] format = formats[format_index].Item2;
                int[] index_change = mappings[format_index].Item2;
                bool[] is_matched_out = mappings[format_index].Item3;

                List<string> input = new List<string>(Regex.Split(word, delim));

                // map user input to correct position
                for (int j = 0; j < format.Length; j++) {
                    if (is_matched_out[j]) {
                        format[j] = input[j + index_change[j]];
                    }
                }
                replacements.Add(String.Join("", format));
            }

            return replacements;
        }
    }
}