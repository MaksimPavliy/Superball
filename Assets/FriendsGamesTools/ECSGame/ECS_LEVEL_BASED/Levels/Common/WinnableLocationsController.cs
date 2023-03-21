#if ECS_LEVEL_BASED
using FriendsGamesTools.ECSGame.Locations;
using FriendsGamesTools.ECSGame.Player.Money;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FriendsGamesTools.ECSGame
{
    public abstract class WinnableLocationsController : LocationsController, ILocationSet
    {
        protected LocationsView view => LocationsView.instance;
        public override int locationsCount => view.locationsCount;
        protected override bool canChangePrivate => false;
        protected Level data => e.GetComponentData<Level>();
        public bool isPlaying => data.state == Level.State.playing;
        public Level.State state => data.state;
        public abstract int winStarsCount { get; }
        public int levelsPlayed => data.levelsPlayed;
        public override void InitDefault()
        {
            base.InitDefault();
            e.AddComponent(new Level { });
        }
        public virtual void OnLocationSet(int newLocationInd)
        {
            //e.AddOrModifyComponent((ref Level l) => l.state = Level.State.playing);
        }
        public virtual void GoToMenu()
        {
            var left = false;
            e.ModifyComponent((ref Level l) =>
            {
                if (l.state == Level.State.playing || l.state == Level.State.lose || l.state == Level.State.win)
                    left = true;
                l.state = Level.State.inMenu;
            });
            if (left)
                OnMatchLeft();
        }

        protected virtual void OnMatchLeft() {
            CallLocationLeave();
        }

        protected abstract (bool win, bool lose) CheckWinLose();
        void UpdateWinLose()
        {
            if (state != Level.State.playing) return;
            var (win, lose) = CheckWinLose();
            if (!lose && win)
                DoWin();
            else if (lose)
                DoLose();
        }
        protected override void OnUpdate()
        {
            base.OnUpdate();
            UpdateWinLose();
        }
        public virtual void DoWin()
        {
            if (state != Level.State.playing) return;
            e.ModifyComponent((ref Level l) =>
            {
                l.state = Level.State.win;
                l.levelsPlayed++;
            });
            OnWin();
        }
        protected virtual void OnWin() { }
        public virtual void DoLose()
        {
            if (state != Level.State.playing) return;
            e.ModifyComponent((ref Level l) =>
            {
                l.state = Level.State.lose;
                l.levelsPlayed++;
            });
            OnLose();
        }
        protected virtual void OnLose() { }
        public void DebugWin() => DoWin();
        public void DebugLose() => DoLose();
        public abstract int levelWinMoney { get; }
#if ADS
        public abstract float levelWinX3Chance { get; }
#else
        public float levelWinX3Chance => 0;
#endif
        public void GiveWinMoney(int multiplier)
        {
            var money = levelWinMoney * multiplier;
            root.Get<PlayerMoneyController>().AddMoneySoaked(money);
        }
        public virtual void Play()
        {
            Debug.Assert(state == Level.State.inMenu, $"play is only available from menu");
            if (state != Level.State.inMenu) return;
            e.ModifyComponent((ref Level l) => l.state = Level.State.playing);
            CallLocationPlay();
        }
        public virtual void ContinueAfterLose()
        {
            Debug.Assert(state == Level.State.lose, $"play is only available from menu");
            if (state != Level.State.lose) return;
            e.ModifyComponent((ref Level l) => l.state = Level.State.playing);
        }

        void CallLocationPlay() {
            root.controllers.ForEach(c => {
                if (c is ILocationPlay l)
                    l.OnLocationPlay();
            });
            root.views.ForEach(c => {
                if (c is ILocationPlay l)
                    l.OnLocationPlay();
            });
            Utils.IterateInterfacesInScene<ILocationPlay>(i => i.OnLocationPlay());
        }
        void CallLocationLeave() {
            root.controllers.ForEach(c => {
                if (c is ILocationLeave l)
                    l.OnLocationLeave();
            });
            root.views.ForEach(c => {
                if (c is ILocationLeave l)
                    l.OnLocationLeave();
            });
            Utils.IterateInterfacesInScene<ILocationLeave>(i => i.OnLocationLeave());
        }
    }
}
#endif