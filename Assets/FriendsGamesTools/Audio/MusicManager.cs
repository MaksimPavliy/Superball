#if AUDIO
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FriendsGamesTools.Audio
{
    public class MusicManager : MonoBehaviourHasInstance<MusicManager>
    {
        const string PrefsKey = "MusicOn";
        public static bool on
        {
            get => PlayerPrefsUtils.GetBool(PrefsKey, true);
            set => PlayerPrefsUtils.SetBool(PrefsKey, value);
        }
        AudioSource source;
        protected override void Awake()
        {
            base.Awake();
            source = gameObject.GetComponent<AudioSource>();
            if (source == null)
                source = gameObject.AddComponent<AudioSource>();
        }
        public List<AudioClip> musics;
        int ind;
        private void Start()
        {
            ind = Utils.Random(0, musics.Count - 1);
        }
        private void Update()
        {
            if (!on)
            {
                if (source.isPlaying)
                    source.Stop();
            }
            else
            {
                if (!source.isPlaying)
                {
                    Playing(musics[ind], -1, 2).WrapErrors();
                    ind = (ind + 1) % musics.Count;
                }
            }
        }
        public async Task Playing(AudioClip clip, float endTime = -1, float fadeInOutTime = -1)
        {
            source.clip = clip;
            source.Play();
            while (source.isPlaying && (endTime < 0 || source.time < endTime))
            {
                await Awaiters.EndOfFrame;
                if (this == null || source == null)
                    return;
                if (fadeInOutTime > 0)
                    source.volume = Mathf.Clamp01(Mathf.Min(source.time, clip.length - source.time) / fadeInOutTime);
            }
        }
    }
}
#endif