using System.Collections.Generic;
using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.ECSGame.Player.Money;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FriendsGamesTools.ECSGame
{
#if ECS_SKIN_MONEY || ECS_SKINS
    public class MoneySkinTabView: SkinsTabView<MoneySkin> {
        new MoneySkinController controller => (MoneySkinController)base.controller;
#elif ECS_SKINS
    public class MoneySkinTabView : SkinsTabView {
        protected override SkinsController controller => throw new System.NotImplementedException();
#else
    public class MoneySkinTabView : MonoBehaviour { 
#endif
        public Button unlockButton;
        [SerializeField] TextMeshProUGUI unlockPriceText;
#if ECS_SKIN_MONEY || ECS_SKINS
        public override string TabName => "Skins";
        public override string TabHint => "Unlock random skin.";
        public bool enoughMoney => GameRoot.instance.Get<PlayerMoneyController>().amount >= controller.nextSkinPrice;
        protected override void Awake()
        {
            base.Awake();
            unlockButton.Safe(() => unlockButton.onClick.AddListener(OnUnlockRandomSkinPressed));
        }
        public override void UpdateView()
        {
            base.UpdateView();
            unlockButton.gameObject.SetActive(isActiveTab && controller.anySkinLocked);
            unlockPriceText.text = ((double)controller.nextSkinPrice).ToStringWithSuffixes();
        }
        public void OnUnlockRandomSkinPressed()
        {
            controller.BuyRandomSkin();
            UpdateView();
        }
        protected override void Update() => unlockButton.interactable = controller.buySkinAvailable;
#endif
    }
}