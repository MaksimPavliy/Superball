using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace FriendsGamesTools.Integrations
{
    public abstract class GoogleMobileAdsMediation
    {
        protected GoogleMobileAdsModuleSettings settings => SettingsInEditor<GoogleMobileAdsModuleSettings>.instance;
        public abstract string sourceURL { get; }
        public abstract string downloadURL { get; }
        public abstract string version { get; }
        public virtual string SomeClassNameWithNamespace { get; }
        public virtual bool isInProject => ReflectionUtils.DoesClassExist(SomeClassNameWithNamespace);
        public string name => GetType().Name.Replace("GMA", "").Replace("Mediation", "");

        static List<Type> disabledMediations = new List<Type> { };// typeof(AdColonyMediationSettings), typeof(IronSourceMediationSettings) };
        static List<GoogleMobileAdsMediation> _instances;
        public static List<GoogleMobileAdsMediation> instances
        {
            get
            {
                if (_instances == null)
                {
                    _instances = new List<GoogleMobileAdsMediation>();
                    ReflectionUtils.ForEachDerivedType<GoogleMobileAdsMediation>(t =>
                    {
                        if (!disabledMediations.Contains(t))
                            Activator.CreateInstance(t);
                    });
                }
                return _instances;
            }
        }
        public GoogleMobileAdsMediation() => instances.Add(this);

        static bool mediationsShown = true;
        public static void ShowAllMediations(ref bool changed)
        {
            GUILayout.BeginHorizontal();
            EditorGUIUtils.ShowOpenClose(ref mediationsShown);
            var validCount = ValidCount();
            EditorGUIUtils.ShowValid(validCount==instances.Count);
            EditorGUIUtils.RichMultilineLabel($"mediations <b>{validCount}/{instances.Count}</b>");
            GUILayout.EndHorizontal();
            if (!mediationsShown)
                return;
            foreach (var m in instances)
                m.Show(ref changed);
        }
        static int ValidCount(StringBuilder sb  = null) => instances.Sum(m => m.Valid(sb) ? 1 : 0);
        public static bool AllValid(StringBuilder sb = null) => ValidCount(sb) == instances.Count;
        public virtual bool Valid(StringBuilder sb = null) {
            if (!isInProject)
            {
                sb?.AppendLine($"{name} mediation is not in project");
                return false;
            }
            return true;
        }
        bool detailsShown;
        public void Show(ref bool changed)
        {
            GUILayout.BeginHorizontal();
            EditorGUIUtils.ShowOpenClose(ref detailsShown);
            if (isInProject)
                EditorGUIUtils.ShowValid(Valid());
            else
                EditorGUIUtils.ShowValidEmpty();
            EditorGUIUtils.RichMultilineLabel($"<b>{name}</b>", 80);
            EditorGUIUtils.RichMultilineLabel(version, 40);
            EditorGUIUtils.RichMultilineLabel($"<i>{statusText}</i>", 300);
            GUILayout.EndHorizontal();
            if (detailsShown)
                ShowDetails(ref changed);
        }
        static StringBuilder sb = new StringBuilder();
        string statusText
        {
            get
            {
                sb.Clear();
                return Valid(sb) ? "ok" : sb.ToString().FirstLine();
            }
        }
        protected virtual void ShowDetails(ref bool changed)
        {
            if (GUILayout.Button("go to sdk site"))
                Application.OpenURL(sourceURL);
            if (GUILayout.Button($"download sdk v{version}"))
                DownloadSDK();
        }
        void DownloadSDK()
        {
            LibrarySetupUtils.DownloadUnzipAndImport(downloadURL, true);
        }
    }
}