#if RESKINER
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FriendsGamesTools.Reskiner
{
    public class Reskiner : SettingsScriptable<Reskiner>
    {
        #region Create
        protected override string SubFolder => "Reskin";
        public string AssetsToReskinFolder => ParentFolder + "/AssetsToReskin";
        public string AssetsToReskinFolderResources => AssetsToReskinFolder + "/Resources";
        public override string AssetName => "Reskin";
        [MenuItem(FriendsGamesManager.MainPluginName + "/Reskin")]
        static void SelectInstance()
            => Selection.activeObject = SettingsInEditor<Reskiner>.instance;
        #endregion

        public List<ObjectToReskin> objs = new List<ObjectToReskin>();
        public bool showObjsList;
    }

    [Serializable]
    public class ObjectToReskin {        
        public UnityEngine.Object obj;

        public float modelScale;


        // State.
        public enum State { Original, Reskinned, WontChange }
        public State state = State.Original;
        public long originalHash;
    }
}

public class ChangedBackCol : IDisposable
{
    Color prevBackCol;
    public ChangedBackCol(Color col)
    {
        prevBackCol = GUI.backgroundColor;
        GUI.backgroundColor = col;
    }
    public void Dispose() => GUI.backgroundColor = prevBackCol;
}

public class EditorProgressbar : IDisposable
{
    public EditorProgressbar(string title)
    {
        EditorUtility.DisplayProgressBar(title, "", 0);
    }
    public void Dispose() => EditorUtility.ClearProgressBar();
}

//obj = EditorGUILayout.ObjectField("ignore folder", obj, typeof(Object), false);
//if (obj != null)
//GUILayout.Label(obj.GetType().Name);
#endif