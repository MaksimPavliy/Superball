#if DEBUG_CONFIG
using FriendsGamesTools.EditorTools.BuildModes;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.DebugTools
{
    public abstract class BalanceSettings<TSelf> : BalanceSettings
        where TSelf : BalanceSettings<TSelf>
    {
        public static TSelf instance { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            instance = (TSelf)this;
            LoadDebugBalanceIfNeeded();
        }
        protected virtual void OnDestroy()
           => instances.Remove(this);
        public virtual string tabName => gameObject.name;

        static string loadedText;
        void LoadDebugBalanceIfNeeded()
        {
            if (BuildModeSettings.release) return;
            if (string.IsNullOrEmpty(loadedText))
                loadedText = ConfigExportImport.TryReadFile();
            var count = ConfigExportImport.GetConfigsCount(loadedText);
            if (count == instances.Count)
                ConfigExportImport.Import(loadedText);
        }
    }
    public class BalanceSettings : MonoBehaviour
    {
        public static List<BalanceSettings> instances { get; private set; } = new List<BalanceSettings>();
        protected virtual void Awake() => instances.Add(this);
    }
}
#else
    public class BalanceSettings : UnityEngine.MonoBehaviour { }
#endif