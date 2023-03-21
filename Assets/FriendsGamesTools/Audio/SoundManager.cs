#if AUDIO
using System;
using System.Collections.Generic;
using UnityEngine;

namespace FriendsGamesTools.Audio
{
    public class SoundManager : MonoBehaviour
    {
        Camera cam;
        AudioSource source;
        public static SoundManager instance { get; private set; }
        const string PrefsKey = "SoundOn";
        public static bool on
        {
            get => PlayerPrefsUtils.GetBool(PrefsKey, true);
            set => PlayerPrefsUtils.SetBool(PrefsKey, value);
        }
        protected virtual void Awake()
        {
            instance = this;
            cam = Camera.main;
        }
        public void Play(AudioClip clip)
        {
            if (!on)
                return;
            UpdatePlayingTooOften(clip, out var cantPlay);
            if (cantPlay)
                return;
            if (source == null)
                source = gameObject.AddComponent<AudioSource>();
            source.PlayOneShot(clip);

        }
        public void Play(AudioClip clip, Vector3 pos)
        {
            if (!IsInsideScreen(pos))
                return;
            Play(clip);
        }
        public bool IsInsideScreen(Vector3 pos) => Utils.IsInsideScreen(pos, cam);

        #region Some sounds cant be played too often
        [Serializable] class DontPlayTooOften
        {
            public AudioClip clip;
            public float minInterval = 0.5f;
        }
        [SerializeField] List<DontPlayTooOften> dontPlayTooOften = new List<DontPlayTooOften>();
        class DontPlayTooOftenState { public float minInterval, lastTime; }
        Dictionary<AudioClip, DontPlayTooOftenState> lastPlayTimes;
        void UpdatePlayingTooOften(AudioClip clip, out bool cantPlay)
        {
            if (lastPlayTimes == null) {
                lastPlayTimes = new Dictionary<AudioClip, DontPlayTooOftenState>();
                dontPlayTooOften.ForEach(d => lastPlayTimes.Add(d.clip, new DontPlayTooOftenState { minInterval = d.minInterval }));
            }
            if (!lastPlayTimes.TryGetValue(clip, out var state))
            {
                cantPlay = false;
                return;
            }
            cantPlay = state.lastTime + state.minInterval > Time.time;
            if (!cantPlay)
                state.lastTime = Time.time;
        }
        #endregion
    }
    public class SoundManager<T> : SoundManager
        where T : SoundManager<T>
    {
        public static new T instance { get; private set; }
        protected override void Awake()
        {
            base.Awake();
            instance = (T)this;
        }
    }
}
#endif