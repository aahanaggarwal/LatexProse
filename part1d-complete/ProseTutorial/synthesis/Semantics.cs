using System.Collections.Generic;

namespace ProseTutorial
{
    public static class Semantics
    {
        public static string Replace(string v, List<string> in_words, List<string> out_words)
        {
            string modified = v;

            for (int i = 0; i < in_words.Count; i++) {
                modified = modified.Replace(in_words[i], out_words[i]);
            }

            return modified;
        }
    }
}