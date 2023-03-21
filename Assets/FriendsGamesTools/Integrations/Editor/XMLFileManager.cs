using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public class XMLFileManager
    {
        string path;
        public string contents { get; protected set; }
        public bool exists { get; private set; }
        protected virtual string defaultContents { get; }
        protected virtual string lineEndings => "\n";
        protected virtual string saveWithLineEndings => "\r\n";
        public XMLFileManager(string path)
        {
            this.path = path;
            exists = File.Exists(path);
            if (exists)
            {
                contents = File.ReadAllText(path);
                contents = contents.WithLineEndings(lineEndings);
            }
            else
            {
                var defaultContents = this.defaultContents;
                if (!string.IsNullOrEmpty(defaultContents))
                {
                    contents = defaultContents;
                    exists = true;
                } else
                    contents = string.Empty;
            }
        }
        public void Save()
        {
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var contentsToSave = contents;
            if (!string.IsNullOrEmpty(saveWithLineEndings))
                contentsToSave = contentsToSave.WithLineEndings(saveWithLineEndings);
            File.WriteAllText(path, contentsToSave, System.Text.Encoding.UTF8);
            AssetDatabase.Refresh();
        }

        public XMLFileManager RemoveLineWith(string text)
        {
            var lines = contents.Split('\n').ToList();
            lines.RemoveAll(line => line.Contains(text));
            contents = string.Join("\n", lines);
            return this;
        }
        public XMLFileManager RemoveTagWith(string text)
        {
            int ind = contents.IndexOf(text);
            if (ind != -1)
            {
                var openInd = contents.LastIndexOf("<", ind);
                var tagName = contents.Substring(openInd + 1, contents.IndexOf(" ", openInd) - openInd - 1);
                openInd = contents.LastIndexOf("\n", openInd) + 1;
                int closeInd = contents.IndexOf($">", ind);
                int simpleNoContentsCloseInd = contents.IndexOf($"/>", ind);
                bool simpleNoContentsTag = simpleNoContentsCloseInd < closeInd && simpleNoContentsCloseInd != -1;
                if (simpleNoContentsTag)
                    closeInd = simpleNoContentsCloseInd + 1;
                else
                    closeInd = contents.IndexOf($"{tagName}>", ind) + tagName.Length;
                closeInd = contents.IndexOf("\n", closeInd) + 1; // To end of line.
                contents = contents.Remove(openInd, closeInd - openInd);
            }
            return this;
        }
        public string GetParam(string paramName, params string[] tagPath)
        {
            var (startInd, endInd) = FindParamValueBounds(paramName, false, false, tagPath);
            if (startInd == -1)
                return string.Empty;
            var val = contents.Substring(startInd, endInd - startInd + 1);
            return val;
        }
        public XMLFileManager ReplaceParam(string newValue, string paramName, params string[] tagPath)
        {
            var (startInd, endInd) = FindParamValueBounds(paramName, true, false, tagPath);
            contents = contents.Substring(0, startInd) + newValue + contents.Substring(endInd + 1, contents.Length - endInd - 1);
            return this;
        }
        public bool TagExists(params string[] tagPath)
        {
            var (startInd, endInd) = FindTagBounds(tagPath);
            return startInd != -1 && endInd != -1;
        }
        public void AddTagLong(string tag, string tagParams, params string[] tagPath)
        {
            var (startInd, endInd) = FindTagBounds(tagPath);
            startInd = contents.IndexOf(">", startInd);
            Debug.Assert(startInd!=-1);
            startInd++;
            var indent = "\t".Repeat(tagPath.Length - 1);
            contents = contents.Insert(startInd,
                $"\n{indent}<{tag} {tagParams}>" +
                $"\n{indent}</{tag}>");
        }
        public void EnsureExistsInLongTag(string text, params string[] tagPath)
        {
            var (startInd, endInd) = FindTagBounds(tagPath);
            var searchedString = contents.Substring(startInd, endInd - startInd + 1);
            if (searchedString.Contains(text))
                return;
            startInd = contents.IndexOf(">", startInd);
            Debug.Assert(startInd != -1);
            startInd++;
            contents = contents.Insert(startInd, text);
        }
        (int startInd, int endInd) FindTagBounds(params string[] tagPath)
        {
            int startInd, endInd;
            startInd = 0;
            endInd = contents.Length - 1;
            for (int i = 0; i < tagPath.Length; i++)
                (startInd, endInd) = FindTagBounds(tagPath[i], startInd, endInd);
            return (startInd, endInd);
        }
        (int startInd, int endInd) FindParamValueBounds(string paramName, bool addParamIfNotExists, bool errorIfNotExists, params string[] tagPath)
        {
            var (startInd, endInd) = FindTagBounds(tagPath);
            Debug.Assert(startInd != -1 || !errorIfNotExists, $"{paramName} not found in {tagPath.PrintCollection("->")}");
            if (startInd == -1) return (-1, -1);
            var newStartInd = contents.IndexOf(paramName, startInd);
            if (newStartInd == -1)
            {
                if (addParamIfNotExists)
                {
                    // Add param.
                    startInd += 1 + tagPath.Last().Length + 1;
                    var added = $"{paramName}=\"\" ";
                    contents = contents.Insert(startInd, added);
                    endInd += added.Length;
                } else
                    return (-1, -1);
            }
            else
                startInd = newStartInd;
            Debug.Assert(startInd != -1);
            startInd = contents.IndexOf("=", startInd);
            Debug.Assert(startInd != -1);
            startInd = contents.IndexOf("\"", startInd);
            Debug.Assert(startInd != -1);
            startInd++;
            endInd = contents.IndexOf("\"", startInd);
            Debug.Assert(endInd != -1);
            endInd--;
            return (startInd, endInd);
        }
        (int startInd, int endInd) FindTagBounds(string tag, int startInd, int endInd)
        {
            if (startInd == -1 && endInd == -1)
                return (-1, -1);
            startInd = contents.IndexOf($"<{tag}", startInd);
            if (startInd == -1)
                return (-1, -1);
            endInd = FindTagClosing(tag, startInd, endInd);
            return (startInd, endInd);
        }
        bool CanBeVariableName(char c) => c != ' ' && c != '<' && c != '>' && c != '/' && c != '\r' && c != '\n';
        int FindTagClosing(string tag, int startInd, int endInd)
        {
            var currInd = startInd + 1;
            do
            {
                var closeShort = "/>";
                var closeLong = $"</{tag}>";
                var count = endInd - currInd + 1;
                var subTagOpenInd = contents.IndexOf("<", currInd, count);
                var closeTagShortInd = contents.IndexOf(closeShort, currInd, count);
                var closeTagLongInd = contents.IndexOf(closeLong, currInd, count);
                if (subTagOpenInd == -1) subTagOpenInd = int.MaxValue;
                if (closeTagShortInd == -1) closeTagShortInd = int.MaxValue;
                if (closeTagLongInd == -1) closeTagLongInd = int.MaxValue;
                if (subTagOpenInd < closeTagShortInd && subTagOpenInd < closeTagLongInd)
                {
                    int subTagCloseInd = subTagOpenInd + 1;
                    while (CanBeVariableName(contents[subTagCloseInd]))
                        subTagCloseInd++;
                    subTagCloseInd--;
                    var subTag = contents.Substring(subTagOpenInd + 1, subTagCloseInd - subTagOpenInd);
                    subTagCloseInd = FindTagClosing(subTag, subTagOpenInd, endInd);
                    currInd = subTagCloseInd + 1;
                    continue;
                }
                else if (closeTagShortInd < closeTagLongInd)
                    endInd = closeTagShortInd + closeShort.Length - 1;
                else
                    endInd = closeTagLongInd + closeLong.Length - 1;
            } while (false);
            return endInd;
        }
    }
}