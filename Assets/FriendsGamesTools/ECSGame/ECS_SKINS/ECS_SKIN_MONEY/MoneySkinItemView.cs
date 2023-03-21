namespace FriendsGamesTools.ECSGame
{
#if ECS_SKIN_MONEY || ECS_SKINS
    public class MoneySkinItemView : SkinItemView<MoneySkin> { }
#elif ECS_SKINS
    public class MoneySkinItemView : SkinItemView {
        protected override SkinsController controller => throw new System.NotImplementedException();
    }
#endif
}
