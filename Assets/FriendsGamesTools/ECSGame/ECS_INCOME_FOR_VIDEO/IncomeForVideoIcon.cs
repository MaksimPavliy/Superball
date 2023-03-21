#if ECS_INCOME_FOR_VIDEO
using UnityEngine;
using UnityEngine.UI;
namespace FriendsGamesTools.ECSGame.IncomeForVideo
{
    public abstract class IncomeForVideoIcon<TWindow> : MonoBehaviour
        where TWindow : IncomeForVideoWindow<TWindow>
    {
        [SerializeField] protected GameObject availableParent;
        [SerializeField] Button button;
        private void Awake()
        {
            if (button != null)
                button.onClick.AddListener(OnPressed);
        }
        protected virtual void Update()
        {
            if (availableParent != null)
                availableParent.SetActive(controller.available);
        }
        protected IncomeForVideoController controller => GameRoot.instance.Get<IncomeForVideoController>();
        protected virtual void OnPressed() => IncomeForVideoWindow<TWindow>.Show();
    }
}
#endif