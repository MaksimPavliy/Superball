using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace FriendsGamesTools
{
    public static class StringUtils
    {
        public static string ClampLength(this string str, int maxChars) => str.Substring(0, Mathf.Min( maxChars, str.Length));
        public static string ToPrintable(this string str, string whenNullOrEmpty = "NullOrEmpty")
            => str.IsNullOrEmpty() ? whenNullOrEmpty : str;
        public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);
        public static string WithUTF32Support(this string str)
        {
            do
            {
                var startInd = str.IndexOf("0x");
                if (startInd == -1 || startInd >= str.Length - 3)
                    break;
                var endInd = startInd + 2;
                while (endInd < str.Length - 1 && str[endInd + 1].IsLetterOrDigit())
                    endInd++;
                var prefixedHex = str.Substring(startInd, endInd - startInd + 1);
                int intValue = Convert.ToInt32(prefixedHex, 16);
                str = str.Replace(prefixedHex, Char.ConvertFromUtf32(intValue));
            } while (true);
            return str;
        }
        public static string WithMaxLength(this string s, int maxLength) => s.LengthSafe() <= maxLength ? s : s.Substring(0, maxLength);
        public static int LengthSafe(this string s) => s == null ? 0 : s.Length;
        static StringBuilder sb;
        public static StringBuilder InitStringBuilder()
        {
            if (sb == null)
                sb = new StringBuilder();
            else
                sb.Clear();
            return sb;
        }

        public static string FirstLine(this string s)
        {
            var ind = s.IndexOf('\n');
            return ind == -1 ? s : s.Substring(0, ind);
        }
        public static int Count(this string s, string str)
        {
            int count = 0, ind = 0;
            while (ind < s.Length)
            {
                ind = s.IndexOf(str, ind);
                if (ind == -1)
                    break;
                ind++;
                count++;
            }
            return count;
        }
        public static char GetCharSafe(this string text, int ind)
        {
            if (ind < 0 || ind >= text.Length)
                return (char)0;
            return text[ind];
        }
        static HashSet<char> digits = new HashSet<char> { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        public static bool IsLetter(this char ch) => Char.IsLetter(ch);
        public static bool IsDigit(this char ch) => Char.IsDigit(ch);
        public static bool IsLetterOrDigit(this char ch) => Char.IsLetterOrDigit(ch);
        public static int DigitsCount(this string s) => s == null ? 0 : Regex.Matches(s, "[0-9]").Count;
        public static int LowerCaseLettersCount(this string s) => s == null ? 0 : Regex.Matches(s, "[a-z]").Count;
        public static bool HasDigits(this string s) => s.DigitsCount() > 0;
        public static string RemoveDigits(string s)
        {
            if (s.DigitsCount() == 0)
                return s;
            for (int i = s.Length - 1; i >= 0; i--)
            {
                if ((s[i].ToString()).DigitsCount() > 0)
                    s = s.Remove(i, 1);
            }
            return s;
        }
        public static string Repeat(this string s, int count)
        {
            InitStringBuilder();
            for (int i = 0; i < count; i++)
                sb.Append(s);
            return sb.ToString();
        }
        public static string GetLineWithCharInd(this string s, int characterInd)
        {
            int startInd = s.LastIndexOf("\n", characterInd);
            if (startInd == -1)
                startInd = 0;
            else
                startInd++;
            int endInd = s.IndexOf("\n", characterInd);
            if (endInd == -1)
                endInd = s.Length - 1;
            var substring = s.Substring(startInd, endInd - startInd + 1);
            substring = substring.Trim('\r', '\n');
            return substring;
        }
        public static string GetLineWith(this string s, string text)
        {
            int ind = s.IndexOf(text);
            if (ind == -1)
                return null;
            return s.GetLineWithCharInd(ind);
        }
        public static string ReplaceLineWith(this string s, string text, string replacingLine)
        {
            var line = s.GetLineWith(text);
            return s.Replace(line, replacingLine);
        }
        public static string ReplaceLineWith(this string s, string text, Func<string, string> replaceLine)
        {
            var line = s.GetLineWith(text);
            var replacingLine = replaceLine(line);
            return s.Replace(line, replacingLine);
        }
        public static string ToCrLf(this string text) => text.WithLineEndings("\r\n");
        public static string WithLineEndings(this string text, string lineEndings)
        {
            var lines = text.Split('\n').ToList();
            text = string.Join(lineEndings, lines.ConvertAll(line => line.TrimEnd('\r')));
            return text;
        }
        public static string ToLf(this string text) => text.Replace("\r", "");
        public const string OSLineEnding =
#if UNITY_EDITOR_OSX
            "\n";
#else
            "\r\n";
#endif
        private static TextEditor textEditor;
        private static void EnsureTextEditor()
        {
            if (textEditor != null)
                return;
            textEditor = new TextEditor();
            textEditor.multiline = true;
        }
        public static void CopyToClipboard(this string text)
        {
            EnsureTextEditor();
            textEditor.text = text;
            textEditor.SelectAll();
            textEditor.Copy();
        }
        public static string PasteFromClipboard()
        {
            EnsureTextEditor();
            if (!textEditor.CanPaste())
                return null;
            textEditor.text = string.Empty;
            textEditor.Paste();
            return textEditor.text;
        }
        public static string If(this string str, bool condition) => condition ? str : string.Empty;
        public static bool TryParse(string str, out float val)
            => float.TryParse(str.Replace(",", "."),
                NumberStyles.Any, CultureInfo.InvariantCulture, out val);
        public static bool TryParse(string str, out double val)
            => double.TryParse(str.Replace(",", "."),
                NumberStyles.Any, CultureInfo.InvariantCulture, out val);
        public static bool VersionGreaterEquals(string minVersion, string checkedVersion)
        {
            string[] SplitVersion(string version)
            {
                var ind = version.IndexOf("-preview");
                if (ind != -1)
                    version = version.Substring(0, ind);
                return version.Split('.');
            }
            var minV = SplitVersion(minVersion);
            var checkedV = SplitVersion(checkedVersion);
            for (int i = 0; i < minV.Length; i++)
            {
                if (checkedV.Length <= i)
                    return false;
                if (int.TryParse(minV[i], out var currMinV) && int.TryParse(checkedV[i], out var currCheckedV))
                {
                    if (currMinV > currCheckedV)
                        return false;
                    else if (currMinV < currCheckedV)
                        return true;
                }
                else
                    throw new Exception($"cant parse verisons: {minVersion} or {checkedVersion}");
            }
            return true;
        }

        static int[,] d;
        public static int LevenshteinDistance(string s, string t)
        {
            var n = s.Length;
            var m = t.Length;
            if (d == null || d.GetLength(0) < n + 1 || d.GetLength(1) < m + 1)
                d = new int[n + 1, m + 1];
            if (n == 0) return m;
            if (m == 0) return n;
            for (int i = 0; i <= n; d[i, 0] = i++) { }
            for (int j = 0; j <= m; d[0, j] = j++) { }
            for (int i = 1; i <= n; i++) {
                for (int j = 1; j <= m; j++) {
                    var cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                        d[i - 1, j - 1] + cost);
                }
            }
            return d[n, m];
        }

#if UNITY_EDITOR
        public static string EnsurePathRelative(string path)
        {
            var ind = path.IndexOf(FriendsGamesManager.AssetsFolder);
            if (ind == -1)
                return string.Empty;
            //Debug.Assert(ind!=-1, $"path {path} cant be relative to project folder, its not in assets");
            if (ind == 0)
                return path;
            path = path.Remove(0, ind);
            return path;
        }

        public static string EnsurePathFull(string path)
        {
            var ind = path.IndexOf(FriendsGamesManager.AssetsFolder);
            if (ind == 0)
                return $"{Application.dataPath}/{path.Remove(0, FriendsGamesManager.AssetsFolder.Length + 1)}";
            return path;
        }
#endif
    }
}