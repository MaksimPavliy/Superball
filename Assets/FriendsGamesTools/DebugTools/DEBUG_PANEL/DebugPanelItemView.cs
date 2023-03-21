using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.DebugTools
{
    [ExecuteAlways]
    public abstract class DebugPanelItemView : MonoBehaviour
    {
        public abstract (string tab, string name) whereToShow { get; }
        public virtual bool wholeTab => string.IsNullOrEmpty(whereToShow.name);
        public virtual float sortPriority => 0;
        public virtual bool showInDebugPanel => true;
        public virtual void OnDebugPanelAwake() { }
        protected virtual void Update()
        {
            UpdateAddToSettings();
            if (Application.isPlaying) UpdatePlaying();
        }
        protected virtual void UpdatePlaying() { }
        protected virtual void OnEnable() {
            if (Application.isPlaying) OnEnablePlaying();
        }
        protected virtual void OnEnablePlaying() {
            UpdateTogglesView();
            UpdateInputsView();
        }
        protected virtual void Awake()
        {
            if (Application.isPlaying) AwakePlaying();
        }
        protected virtual void AwakePlaying() { }

        public static DebugPanelSettings settings => DebugPanelSettings.instance;
        #region Add itself to enabled items
        void UpdateAddToSettings()
        {
#if UNITY_EDITOR
            if (Utils.IsPrefabOpened() && Utils.PrefabChangesAllowed(gameObject))
            {
                var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<DebugPanelItemView>(
                    UnityEditor.Experimental.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().prefabAssetPath);
                UpdateAddToSettings(prefab);
            }
#endif
        }
        public static void UpdateAddToSettings(DebugPanelItemView prefab)
        {
#if UNITY_EDITOR
            if (Application.isPlaying || prefab == null || settings == null) return;
            var currentlyShown = settings.itemViews.Contains(prefab);
            if (prefab.showInDebugPanel == currentlyShown) return;
            if (prefab.showInDebugPanel)
                settings.itemViews.Add(prefab);
            else
                settings.itemViews.Remove(prefab);
            UnityEditor.EditorUtility.SetDirty(settings);
#endif
        }
        #endregion

        #region Toggles
        private class ToggleSetup
        {
            Toggle toggle;
            Func<bool> get;
            Action<bool> set;
            public ToggleSetup(Toggle toggle, Func<bool> get, Action<bool> set)
            {
                this.toggle = toggle;
                this.get = get;
                this.set = set;
                toggle.onValueChanged.AddListener(onValueChanged);
                //onValueChanged(toggle.isOn);
            }
            private void onValueChanged(bool val) => set(val);
            public void UpdateView() => toggle.isOn = get();
        }
        List<ToggleSetup> toggles = new List<ToggleSetup>();
        protected void AddToggle(Toggle toggle, Func<bool> get, Action<bool> set)
            => toggles.Add(new ToggleSetup(toggle, get, set));
        void UpdateTogglesView() => toggles.ForEach(t => t.UpdateView());
        protected void AddEnumToggles<T>(Func<T> get, Action<T> set, params Toggle[] toggles) where T: Enum {
            var i = 0;
            foreach (T t in Enum.GetValues(typeof(T))) {
                AddToggle(toggles[i], () => get().CompareTo(t) == 0, value => {
                    if (value)
                        set(t);
                    UpdateItemsView();
                });
                i++;
            }
            void UpdateItemsView() {
                var selected = get();
                var j = 0;
                foreach (T t in Enum.GetValues(typeof(T))) {
                    toggles[j].isOn = t.CompareTo(selected) == 0;
                    j++;
                }
            }
        }
        #endregion

        #region TextFields
        private class TextFieldSetup
        {
            TMP_InputField input;
            Func<string> get;
            Action<string> set;
            public TextFieldSetup(TMP_InputField input, Func<string> get, Action<string> set) {
                this.input = input;
                this.get = get;
                this.set = set;
                input.onValueChanged.AddListener(onValueChanged);
                //onValueChanged(toggle.isOn);
            }
            private void onValueChanged(string val) => set(val);
            public void UpdateView() => input.text = get();
        }
        List<TextFieldSetup> inputs = new List<TextFieldSetup>();
        protected void AddInput(TMP_InputField input, Func<string> get, Action<string> set)
            => inputs.Add(new TextFieldSetup(input, get, set));
        protected void AddNumberInput(TMP_InputField input, Func<int> get, Action<int> set)
            => inputs.Add(new TextFieldSetup(input, () => get().ToString(), str => set(int.Parse(str))));
        protected void AddNumberInput(TMP_InputField input, Func<float> get, Action<float> set)
            => inputs.Add(new TextFieldSetup(input, () => get().ToString(), str => set(float.Parse(str))));
        void UpdateInputsView() => inputs.ForEach(t => t.UpdateView());
        #endregion
    }
}