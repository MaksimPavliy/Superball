#if LOCALIZATION

using UnityEditor;
using UnityEngine;

namespace FriendsGamesTools
{
    [CustomEditor(typeof(LocalizableView))]
    public class LocalizableViewEditor : Editor
    {
        LocalizableView tgt => (LocalizableView)target;
        LocalizationSettings settings => SettingsInEditor<LocalizationSettings>.instance;
        public override void OnInspectorGUI()
        {
            //base.OnInspectorGUI();
            var changed = false;
            const int labelWidth = 90;
            if (!tgt.okWithoutLocalization)
            {
                EditorGUIUtils.TextField("localization key", ref tgt.localizationKey, ref changed, labelWidth: labelWidth);
                tgt.localizationKey = tgt.localizationKey?.ToUpper();
            }
            if (tgt.localizationKey.IsNullOrEmpty())
            {
                // no localization key set.
                EditorGUIUtils.Toggle("ok without localization?", ref tgt.okWithoutLocalization, ref changed);
                if (!tgt.okWithoutLocalization)
                    EditorGUIUtils.Error("localization key not set");
            } else
            {
                // localization key exists.
                var currSettings = settings.Get(tgt.localizationKey);
                if (currSettings==null)
                {
                    // key is absent in localizations.
                    EditorGUIUtils.Error($"localization key {tgt.localizationKey} does not exist");
                    if (GUILayout.Button("Create localization"))
                        settings.CreateLocalization(tgt.localizationKey, tgt.shownText);
                } else
                {
                    // localization exists.
                    var translation = currSettings.GetTranslation(settings.playerLanguage);
                    if (translation==null)
                    {
                        translation = new TranslationData { language = settings.playerLanguage, localizedText = tgt.shownText };
                        currSettings.translations.Add(translation);
                        changed = true;
                    } else
                    {
                        // translation to dev language exists.
                        var localizedText = translation.localizedText;
                        if (localizedText == tgt.shownText)
                            GUILayout.Label("saved");
                        else
                        {
                            EditorGUIUtils.TextFieldReadOnly("new text", tgt.shownText, labelWidth: labelWidth);
                            if (GUILayout.Button($"save"))
                            {
                                translation.localizedText = tgt.shownText;
                                changed = true;
                            }
                            EditorGUIUtils.TextFieldReadOnly("old text", translation.localizedText, labelWidth: labelWidth);
                            if (GUILayout.Button("restore"))
                            {
                                tgt.ShowKeyText(tgt.localizationKey, translation.localizedText);
                                changed = true;
                            }
                            EditorGUIUtils.Error("localization changed");
                        }
                    }
                }
            }

            EditorGUIUtils.Toggle("localize font", ref tgt.localizeFont, ref changed);

            if (changed)
            {
                tgt.transform.SetChanged();
                settings.SetChanged();
            }
        }
    }
}
#endif