#if !UNITY_EDITOR && ECSGame
namespace FriendsGamesTools.ECSGame.DataMigration
{
    public class ECSMethodsForAOT
    {
        public void CallECSMethods()
        {
            var manager = Unity.Entities.World.Active.EntityManager;
            var e = new Unity.Entities.Entity();
            manager.SetComponentData(e, manager.GetComponentData<Superball.RateUsData>(e));
            manager.AddComponent<Superball.RateUsData>(e);
            manager.AddComponentData<Superball.RateUsData>(e, manager.GetComponentData<Superball.RateUsData>(e));
            manager.HasComponent<Superball.RateUsData>(e);
            manager.SetComponentData(e, manager.GetComponentData<Superball.Score>(e));
            manager.AddComponent<Superball.Score>(e);
            manager.AddComponentData<Superball.Score>(e, manager.GetComponentData<Superball.Score>(e));
            manager.HasComponent<Superball.Score>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.DurableProcess>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.DurableProcess>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.DurableProcess>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.DurableProcess>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.DurableProcess>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Id>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.Id>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.Id>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Id>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.Id>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Ids>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.Ids>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.Ids>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Ids>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.Ids>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.GameTimeData>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.GameTimeData>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.GameTimeData>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.GameTimeData>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.GameTimeData>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Level>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.Level>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.Level>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Level>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.Level>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.MoneySkin>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.MoneySkin>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.MoneySkin>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.MoneySkin>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.MoneySkin>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.ProgressSkin>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.ProgressSkin>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.ProgressSkin>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.ProgressSkin>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.ProgressSkin>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.SkinsData>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.SkinsData>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.SkinsData>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.SkinsData>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.SkinsData>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Player.IsPlayer>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.Player.IsPlayer>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.Player.IsPlayer>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Player.IsPlayer>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.Player.IsPlayer>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Player.Money.IncomeMultiplier>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.Player.Money.IncomeMultiplier>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.Player.Money.IncomeMultiplier>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Player.Money.IncomeMultiplier>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.Player.Money.IncomeMultiplier>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Player.Money.PlayerMoney>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.Player.Money.PlayerMoney>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.Player.Money.PlayerMoney>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Player.Money.PlayerMoney>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.Player.Money.PlayerMoney>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Player.Money.MoneySoaking>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.Player.Money.MoneySoaking>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.Player.Money.MoneySoaking>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Player.Money.MoneySoaking>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.Player.Money.MoneySoaking>(e);
            manager.SetComponentData(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Locations.LocationsData>(e));
            manager.AddComponent<FriendsGamesTools.ECSGame.Locations.LocationsData>(e);
            manager.AddComponentData<FriendsGamesTools.ECSGame.Locations.LocationsData>(e, manager.GetComponentData<FriendsGamesTools.ECSGame.Locations.LocationsData>(e));
            manager.HasComponent<FriendsGamesTools.ECSGame.Locations.LocationsData>(e);
            manager.GetBuffer<FriendsGamesTools.ECSGame.StringChar>(e);
            manager.AddBuffer<FriendsGamesTools.ECSGame.StringChar>(e);
            manager.HasComponent(e, Unity.Entities.ComponentType.ReadWrite<FriendsGamesTools.ECSGame.StringChar>());
            manager.GetBuffer<FriendsGamesTools.ECSGame.Skin>(e);
            manager.AddBuffer<FriendsGamesTools.ECSGame.Skin>(e);
            manager.HasComponent(e, Unity.Entities.ComponentType.ReadWrite<FriendsGamesTools.ECSGame.Skin>());
            
        }
    }
}
#endif
