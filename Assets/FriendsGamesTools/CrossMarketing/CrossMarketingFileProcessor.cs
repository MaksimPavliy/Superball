using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FriendsGamesTools
{
    public class CrossMarketingFileProcessor
    {
        private static List<string> columnNames = new List<string> {
            "Index","AppName","Apple ID","Google ID","Icon Name", "Show IOS","Show Android"
        };
        private static string columnsLine => columnNames.PrintCollection(",");
        const string noId = CrossMarketingAppData.noId;
        public static bool ImportCSV(string text, List<CrossMarketingAppData> data, List<CrossMarketingShowFlag> data1) {
            var lines = text.Split('\n').ToList();
            if (lines.GetElementSafe(0) != columnsLine)
                return false;
            lines.RemoveAt(0);
            var assets = Resources.LoadAll("Icons", typeof(Sprite));
            var sprites = assets.Where(s => s is Sprite).Cast<Sprite>();
            foreach (var line in lines) {
                if (line.IsNullOrEmpty()) continue;
                var cells = SplitCSVLine(line);
                if (cells.Count != columnNames.Count) return false;
                var spr = sprites.FirstOrDefault(s => s.name.Contains(cells[4]));
                data.Add(new CrossMarketingAppData() {
                    id = int.Parse(cells[0]),
                    appName = cells[1],
                    appleID = cells[2].IsNullOrEmpty() ? noId : cells[2],
                    androidPackageId = cells[3].IsNullOrEmpty() ? noId : cells[3],
                    icon = sprites.FirstOrDefault(s => s.name.Contains(cells[4])),
                    showIOS = bool.Parse(cells[5]),
                    showAndroid = bool.Parse(cells[6])
                });
                data1.Add(new CrossMarketingShowFlag() {
                    id = int.Parse(cells[0]),
                    show = true
                });
            }
            return true;
        }

        private static List<string> SplitCSVLine(string line) {
            var cells = line.Split(',').ToList();
            for (int i = cells.Count - 1; i >= 0; i--) {
                if (!cells[i].StartsWith("\"")) continue;
                while (i + 1 < cells.Count && !cells[i].EndsWith("\"")) {
                    cells[i] += cells[i + 1];
                    cells.RemoveAt(i + 1);
                }
            }
            return cells;
        }
    }
}