using System;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    [RequireComponent(typeof(Button)), ExecuteAlways]
    public class FGTButton : MonoBehaviour
    {
        [SerializeField] bool haptic;
        [SerializeField] AudioClip sound;
        [SerializeField, HideInInspector] bool initedInEditMode;
#if UI
        Button button;
        protected virtual void Awake()
        {
            if (!Application.isPlaying && !initedInEditMode)
                InitInEditMode();
            button = GetComponent<Button>();
            button.onClick.AddListener(OnPressed);
        }

        protected virtual void InitInEditMode()
        {
#if UNITY_EDITOR
            initedInEditMode = true;
            sound = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>($"{FriendsGamesManager.MainPluginFolder}/UI/DefaultFGTArt/Sounds/tap_1.mp3");
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        protected virtual void OnPressed()
        {
            if (haptic)
            {
#if HAPTIC
                FriendsGamesTools.Haptic.Vibrate();
#else
                throw new Exception("haptic is not enabled");
#endif
            }
            if (sound != null)
            {
#if AUDIO
                Audio.SoundManager.instance.Play(sound);
#else
                throw new Exception("audio is not enabled");
#endif
            }
        }
#endif
    }
}
