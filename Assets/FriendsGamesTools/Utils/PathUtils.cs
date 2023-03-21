using System.IO;

namespace FriendsGamesTools
{
    public static partial class Utils
    {
        public static string[] GetFiles(string folder, string extension)
            => Directory.GetFiles(folder, $"*.{extension}", SearchOption.AllDirectories);
    }
}