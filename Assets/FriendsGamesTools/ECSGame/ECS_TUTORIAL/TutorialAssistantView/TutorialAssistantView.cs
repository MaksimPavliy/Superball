using FriendsGamesTools;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame.Tutorial
{
    public class TutorialAssistantView : MonoBehaviourHasInstance<TutorialAssistantView>
    {
        [SerializeField] TextMeshProUGUI textLabel;
        [SerializeField] GameObject parent;
        [SerializeField] Animator anim;
        [SerializeField] GameObject canCloseParent;
        [SerializeField] float letterDelay = 0.1f;
        public TutorialButton tutorialButton;

#if TUTORIAL || QUESTS
        [SerializeField] bool skipTextAnimationOnTap = true;
        private TouchesManager touchesManager => TouchesManager.instance;
        private bool isCutTextAnimation;
        private bool canClose;

        public static bool shown => instance?.parent.activeInHierarchy ?? false;
        public static bool okShown => shown && instance.canCloseParent.activeInHierarchy;
        public static bool isBlocking => shown && !okShown;
        protected override void Awake()
        {
            base.Awake();
            parent.SetActive(false);

            if(skipTextAnimationOnTap)
                instance.touchesManager.onInputDown.AddListener(PerformTextAnimationSkipping);
        }

        private void OnDestroy()
        {
            if (skipTextAnimationOnTap)
                instance.touchesManager.onInputDown.RemoveListener(PerformTextAnimationSkipping);
        }

        static HashSet<int> textIndsHidden = new HashSet<int>();
        
        public static async void Show(string text)
        {
            if (text.IsNullOrEmpty()) return;

            instance.isCutTextAnimation = false;
            instance.canClose = false;

            instance.isHiding = false;
            instance.textLabel.text = "";
            instance.parent.SetActive(true);
            instance.SetTrigger("show");
            instance.canCloseParent.SetActive(false);
            textIndsHidden.Clear();
            var isHidden = false;
            for (int i = 0; i < text.Length; i++)
            {
                if (!isHidden && text[i] == '<')
                {
                    isHidden = true;
                    textIndsHidden.Add(i);
                }
                else if (isHidden && text[i] == '>')
                {
                    isHidden = false;
                    textIndsHidden.Add(i);
                }
                else if (isHidden)
                    textIndsHidden.Add(i);
            }

            for (int i = 0; i < text.Length && !instance.isCutTextAnimation; i++)
            {
                if (!textIndsHidden.Contains(i))
                    await Awaiters.SecondsRealtime(instance.letterDelay);

                instance.textLabel.text = text.Substring(0, i + 1);
            }

            if (instance.isCutTextAnimation)
            {
                instance.textLabel.text = text;

                await Awaiters.While(() => !instance.canClose);
            }
            
            instance.canCloseParent.SetActive(true);
        }
        public static void Hide()
        {
            instance.SetTrigger("hide");
            instance.isHiding = true;
            instance.UpdateHiding();
        }
        public static async Task Hiding()
        {
            Hide();
            while (instance.isHiding)
                await Awaiters.EndOfFrame;
        }
        public static async Task Showing(string text, float duration)
        {
            Show(text);
            await Awaiters.SecondsRealtime(duration);
            await Hiding();
        }

        private void Update()
        {
            UpdateHiding();
            if (shown && okShown && Input.GetMouseButtonUp(0))
                Hide();
        }

        private void PerformTextAnimationSkipping()
        {
            if (!isCutTextAnimation)
                isCutTextAnimation = true;
            else
                canClose = true;
        }

        void SetTrigger(string name)
        {
            if (!anim)
                return;
            anim.SetTrigger(name);
            anim.Update(0);
        }
        bool isHiding;
        public const string HiddenEventName = "TutorialAssistantHidden";
        void UpdateHiding()
        {
            if (!isHiding)
                return;
            if (anim == null || !anim.IsPlaying())
            {
                parent.SetActive(false);
                isHiding = false;
#if ANALYTICS
                Analytics.AnalyticsManager.Send(HiddenEventName, ("text", textLabel.text));
#endif
            }
        }
        //public void OnOkPressed() => Hide();
#endif
    }
}