#if UI
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.UI
{
    public class AnimatedEffect : MonoBehaviour
    {
        public Image pic, shadow;
        public SpriteRenderer picSprite, shadowSprite;
        public List<Sprite> sprites, shadows;
        public async Task PlayInstance(Vector3 worldPos, Transform parent)
        {
            var instance = Instantiate(this, parent);
            await instance.Play(worldPos);
        }
        async Task Play(Vector3 worldPos, float FPS = 30)
        {
            transform.position = worldPos;
            await Play(FPS);
        }
        async Task Play(float FPS = 30)
        {
            float elapsed = 0;
            float duration = sprites.Count / FPS;
            bool last = false;
            do
            {
                last = elapsed >= duration && !looped;
                int ind = Mathf.Clamp((int)(elapsed / duration * sprites.Count), 0, sprites.Count - 1);
                ShowInd(ind);
                elapsed += Time.deltaTime;
                elapsed %= duration;
                await Awaiters.EndOfFrame;
                if (this == null)
                    return;
            } while (!last);

            if (this != null && gameObject != null)
                Destroy(gameObject);
        }
        public void ShowInd(int ind)
        {
            if (pic != null)
                pic.sprite = sprites[ind];
            if (shadow != null)
                shadow.sprite = shadows[ind];
            if (picSprite != null)
                picSprite.sprite = sprites[ind];
            if (shadowSprite != null)
                shadowSprite.sprite = shadows[ind];
        }
        [SerializeField] bool autoplay = false;
        [SerializeField] bool looped = false;
        private void Awake()
        {
            if (autoplay)
                Play().WrapErrors();
        }
    }
}
#endif