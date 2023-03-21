#if ECS_PLAYER_MONEY
using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Time = FriendsGamesTools.GameTime;

namespace FriendsGamesTools.ECSGame.Player.Money
{
    [UpdateAfter(typeof(PlayerController))]
    public abstract class PlayerMoneyController : Controller
    {
        protected virtual float HistoryLength { get; } = 30;
        protected virtual float IncomeUpdateInterval { get; } = 10;
        private struct RegularIncomeHistoryItem
        {
            public float time;
            public double amount;
        }
        static List<RegularIncomeHistoryItem> currSessionHistory = new List<RegularIncomeHistoryItem>();
        public override void InitDefault()
        {
            PlayerController.entity.AddComponent(new PlayerMoney());
        }
        protected void SetStartMoney(double amount) 
            => PlayerController.entity.ModifyComponent((ref PlayerMoney money) => money.amount = amount);
        public override void OnInited()
        {
            instance = this;
            currSessionHistory.Clear();
            // Create curr session history for current income.
            double money = amount;
            int count = Mathf.Max(1, Mathf.RoundToInt(HistoryLength / IncomeUpdateInterval));
            double oneItemMoney = money / count;
            for (int i = 0; i < count; i++)
                currSessionHistory.Add(new RegularIncomeHistoryItem { amount = oneItemMoney, time = GameTime.time - (i / (float)count) * HistoryLength });
        }
        public static PlayerMoneyController instance { get; private set; }
        public double amount => GetData().amount;
        public double income => GetData().income;
        public static PlayerMoney GetData() => PlayerController.entity.GetComponentData<PlayerMoney>();
        /// <param name="regular">Only regular income counts in 'income per minute'</param>
        public virtual void AddMoney(double income, bool regular = true)
        {
            income *= incomeMultiplier;
            Debug.Assert(income >= 0);
            PlayerController.entity.ModifyComponent((ref PlayerMoney money) => money.amount += income);
            if (regular)
            {
                currSessionHistory.Add(new RegularIncomeHistoryItem { amount = income, time = GameTime.time });
                AddExpForMoney(income);
            }
        }
#if ECS_GAME_TIME
        public virtual Entity AddMultiplier(double multiplier, float duration)
            => ECSUtils.CreateEntity(new IncomeMultiplier { multiplier = multiplier }, DurableProcess.Create(duration, 0, true));
#endif
        public double GetDurableIncomeMultiplier()
        {
            var (multiplier, _) = GetIncomeMultiplierDuration();
            return multiplier;
        }
        public (double multiplier, float minRemainingTime) GetIncomeMultiplierDuration()
        {
            var multiplier = 1d;
            var minTime = float.MaxValue;
            Entities.ForEach((Entity e, ref IncomeMultiplier m, ref DurableProcess p) => {
                if (p.finished)
                {
                    e.RemoveEntity();
                    return;
                }
                multiplier *= m.multiplier;
                minTime = Mathf.Min(minTime, p.remaining);
            });
            return (multiplier, minTime);
        }
        public double incomeMultiplier => incomeAdditionalMultiplier* GetDurableIncomeMultiplier();
        public virtual double incomeAdditionalMultiplier => 1;
        public delegate void OnMoneySpent(double pay, double currAmount, string reason);
        public event OnMoneySpent onMoneySpent; // pay, curr money
        public virtual void PayMoney(double pay, string reason = null)
        {
            Debug.Assert(pay >= 0);
            PlayerController.entity.ModifyComponent((ref PlayerMoney money) => {
                onMoneySpent?.Invoke(pay, money.amount, reason);
                money.amount -= pay;
            });
        }
        public void DebugMultiply(double multiplier)
        {
            PlayerController.entity.ModifyComponent((ref PlayerMoney money) => {
                if (money.amount > 0)
                    money.amount *= multiplier;
                else
                    money.amount = 1;
            });
        }
        protected override void OnUpdate()
        {
            UpdateIncome();
            UpdateSoaking();
        }
        void UpdateIncome()
        {
            Entities.ForEach((ref PlayerMoney money) => {
                while (currSessionHistory.Count > 0 && currSessionHistory[0].time + HistoryLength < GameTime.time)
                    currSessionHistory.RemoveAt(0);
                money.remainingToUpdate -= deltaTime;
                if (money.remainingToUpdate <= 0)
                {
                    money.remainingToUpdate = IncomeUpdateInterval;
                    var sumMass = HistoryLength / 2;
                    double sum = 0;
                    foreach (var item in currSessionHistory)
                    {
                        float currMass = HistoryLength - (int)(GameTime.time - item.time);
                        sum += currMass * item.amount;
                    }
                    if (sumMass == 0)
                        money.income = 0;
                    else
                        money.income = 60d * sum / sumMass / HistoryLength;
                }
            });
        }


#if ECS_PLAYER_LEVEL
        public virtual double moneyToExpCoef => -1;
        void AddExpForMoney(double money)
        {
            if (moneyToExpCoef < 0) return;
            var exp = money * moneyToExpCoef;
            GameRoot.instance.Get<Level.PlayerLevelController>().AddExp(exp);
        }
#else
        void AddExpForMoney(double money) { }
#endif

        #region Soaking effect
        MoneySoakEffect soakView => MoneySoakEffect.instance;
        public void AddMoneySoaked(double income, double additionalMultiplier = 1, CurrencyType currency = CurrencyType.Soft)
        {
            var multiplier = incomeMultiplier * additionalMultiplier;
#if ECS_LOCATIONS
            multiplier /= root.Get<Locations.LocationsController>().multiplier;
#endif

            var bonus = multiplier > 1.001f;

            //AddMoney(income, false);
            //Debug.Log($"{UnityEngine.Time.realtimeSinceStartup} MODEL SOAKING START ");
            ECSUtils.CreateEntity(new MoneySoaking {
                bonus = bonus, state = MoneySoaking.State.Explosion, currency = currency, totalIncome = income },
                DurableProcess.Create(soakView.explosionDuration)
                );
        }
        void UpdateSoaking()
        {
            Entities.ForEach((Entity e, ref MoneySoaking s, ref DurableProcess p) => {
                if (!p.finished) return;
                switch (s.state)
                {
                    case MoneySoaking.State.Explosion:
                        //Debug.Log($"{UnityEngine.Time.realtimeSinceStartup} MODEL PARTICLES START FLYING");
                        s.state = MoneySoaking.State.FirstAddingDelay;
                        p = DurableProcess.Create(soakView.particleFlyDuration - soakView.timeBetweenParticlesLaunch);
                        break;
                    case MoneySoaking.State.FirstAddingDelay:
                        //Debug.Log($"{UnityEngine.Time.realtimeSinceStartup} MODEL PARTICLES START REGULAR FLYING");
                        s.state = MoneySoaking.State.AddingMoney;
                        p = DurableProcess.Create(soakView.timeBetweenParticlesLaunch);
                        break;
                    case MoneySoaking.State.AddingMoney:
                        //Debug.Log($"{UnityEngine.Time.realtimeSinceStartup} MODEL PARTICLE ind={s.moneyChunksCompleted} ARRIVED");
                        double GetMoneyAddedBefore(int moneyChunks, double totalIncome)
                            => Math.Round(totalIncome * moneyChunks / soakView.particlesCount);
                        var alreadyAddedMoney = GetMoneyAddedBefore(s.moneyChunksCompleted, s.totalIncome);
                        s.moneyChunksCompleted++;
                        var currAddedMoney = GetMoneyAddedBefore(s.moneyChunksCompleted, s.totalIncome);
                        var currIncome = currAddedMoney - alreadyAddedMoney;
                        if (s.currency == CurrencyType.Soft)
                            AddMoney(currIncome, false);
                        else
                        {
#if ECS_HARD_CURRENCY
                            GameRoot.instance.Get<HardCurrencyController>().Add(Mathf.RoundToInt((float)currIncome));
#endif
                        }
                        if (s.moneyChunksCompleted >= soakView.particlesCount)
                            e.RemoveEntity();
                        else
                            p.elapsed = 0;
                        break;
                }
            });
        }
        public bool shouldSoak => ECSUtils.GetSingleEntity<MoneySoaking>(true) != Entity.Null;
        #endregion
    }
    public struct MoneySoaking : IComponentData {
        public enum State { Explosion, FirstAddingDelay, AddingMoney }
        public State state;
        public CurrencyType currency;
        public double totalIncome;
        public int moneyChunksCompleted;
        public bool bonus;
    }
}
#endif