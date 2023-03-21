using System.Collections.Generic;
using System.Linq;

namespace FriendsGamesTools
{
    public class DefinesSettings : SettingsScriptable<DefinesSettings>
    {
        public List<string> defines = new List<string>();
        public static bool Exists(string define) 
	    => instance.defines.Contains(define);
#if UNITY_EDITOR
        public void SetDefinesInEditMode(string[] currDefinesArray)
        {
            var currDefines = currDefinesArray.ToList();
            currDefines.Remove("UNITY_POST_PROCESSING_STACK_V2"); // Ignore platform-dependant define.
            if (currDefines.All(d => defines.Contains(d)) && defines.All(d => currDefines.Contains(d)))
                return;
            defines = currDefines;
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }
}