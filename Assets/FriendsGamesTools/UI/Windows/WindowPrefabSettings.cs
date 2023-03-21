using FriendsGamesTools.UI;
using System;

namespace FriendsGamesTools.UI
{
    [Serializable]
    public abstract class WindowPrefabSettings<T> where T : Window
    {
        public abstract string title { get; }
        public abstract string defaultPath { get; }
        public T prefab;
    }    
}