namespace ProseTutorial
{
    public static class Semantics
    {
        public static string Replace(string v, int start, int end)
        {
            return v.Replace(v.Substring(start, end - start), "xxx");
        }

        public static int? AbsPos(string v, int k)
        {
            return k > 0 ? k - 1 : v.Length + k + 1;
        }
    }
}