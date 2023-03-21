using System.Collections.Generic;
using System.Threading.Tasks;
using FriendsGamesTools;
using FriendsGamesTools.ECSGame;
using FriendsGamesTools.UI;
#if ECS_PLAYER_MONEY
using Unity.Entities;
#endif
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace FriendsGamesTools.ECSGame.Player.Money
{
    public class MoneySoakEffect : MonoBehaviourHasInstance<MoneySoakEffect>
    {
        [SerializeField] GameObject parent;
        [SerializeField] new ParticleSystem particleSystem;
        [SerializeField] Mesh softMesh;
        [SerializeField] Material softMat;
        [SerializeField] Mesh hardMesh;
        [SerializeField] Material hardMat;
        public float explosionDuration = 2f;
        public float countingDuration = 2f;
        public float particleFlyDuration = 0.3f;
        public int particlesCount { get; private set; }
        public float timeBetweenParticlesLaunch { get; private set; }
        [SerializeField] GameObject bonusParent;

#if ECS_PLAYER_MONEY
        PlayerCurrencyView GetView(CurrencyType type)
        {
#if ECS_HARD_CURRENCY
            if (type == CurrencyType.Hard)
                return HardCurrencyView.instance;
#endif
            return PlayerMoneyView.instance;
        }

        public bool isPlaying => parent.activeInHierarchy;
        LimitVelocityOverLifetimeModule limitVelocityOverLifetime;
        Particle[] particles;
        protected override void Awake()
        {
            base.Awake();
            particleSystem.Stop();
            parent.SetActive(false);
            particles = new Particle[particleSystem.main.maxParticles];
            limitVelocityOverLifetime = particleSystem.limitVelocityOverLifetime;
            var main = particleSystem.main;
            main.duration = explosionDuration + countingDuration + 2;
            main.startLifetime = main.duration;
            particlesCount = particleSystem.emission.GetBurst(0).maxCount;//particleSystem.GetParticles(particles);
            timeBetweenParticlesLaunch = (countingDuration - particleFlyDuration) / particlesCount;
            Debug.Assert(countingDuration<particlesCount, "effect requires countingDuration<particlesCount");
            // (countingDuration - particleFlyDuration) / particlesCount < particleFlyDuration
            // countingDuration - particleFlyDuration < particleFlyDuration*particlesCount
            // countingDuration < particlesCount*(particleFlyDuration+1)
            // 2 < 5*1.3
        }
        public async Task Soaking(MoneySoaking data)
        {
            if (isPlaying) return;
//            Debug.Log($"{UnityEngine.Time.realtimeSinceStartup} VIEW STARTED");
            parent.SetActive(true);
            if (bonusParent != null)
                bonusParent.SetActive(data.bonus);
            var moneyView = GetView(data.currency);
            var startMoney = moneyView.shownValue;
            var endMoney = moneyView.realValue;
            limitVelocityOverLifetime.enabled = true;
            var renderer = particleSystem.GetComponent<ParticleSystemRenderer>();
            if (data.currency == CurrencyType.Soft) {
                renderer.mesh = softMesh;
                renderer.material = softMat;
            } else {
                renderer.mesh = hardMesh;
                renderer.material = hardMat;
            }
            particleSystem.Play();
            await Awaiters.SecondsRealtime(explosionDuration);
            await Ads();
            //Debug.Log($"{UnityEngine.Time.realtimeSinceStartup} VIEW PARTICLES START FLYING");
            limitVelocityOverLifetime.enabled = false;
            var tgtPosLocal = particleSystem.transform.InverseTransformPoint(moneyView.moneyIcoTween.transform.position);
            for (int i = 0; i < particlesCount; i++) {
                //Debug.Log($"{UnityEngine.Time.realtimeSinceStartup} VIEW PARTICLE ind={i} START FLYING");
                var startPos = Vector3.zero;
                ChangeParticle(i, (ref Particle p) => startPos = p.position);
                FlyParticle(startPos, tgtPosLocal, i, moneyView);
                await Awaiters.SecondsRealtime(timeBetweenParticlesLaunch);
                await Ads();
            }
            while (countFlying > 0)
                await Awaiters.EndOfFrame;
            parent.SetActive(false);
        }
#if ADS
        async Task Ads() => await FriendsGamesTools.Ads.AdsManager.instance.WhileAdsShowing();
#else
        Task Ads() => Task.CompletedTask;
#endif
        int countFlying;
        async void FlyParticle(Vector3 startPos, Vector3 tgtPosLocal, int i, PlayerCurrencyView moneyView)
        {
            countFlying++;
            await AsyncUtils.SecondsWithProgress(particleFlyDuration, progress => ChangeParticle(i, (ref Particle p)
                    => p.position = Vector3.Lerp(startPos, tgtPosLocal, Mathf.Pow(progress, 2))), true);
            countFlying--;
            //ChangeParticle(i, (ref Particle p) => p.startSize = 0);
            ChangeParticle(i, (ref Particle p) => p.startSize = 0);
            moneyView.Bump();
            //Debug.Log($"{UnityEngine.Time.realtimeSinceStartup} VIEW PARTICLE ind={i} ARRIVED");
        }
        void ChangeParticle(int ind, RefAction<Particle> change)
        {
            particleSystem.GetParticles(particles);
            var p = particles[ind];
            change(ref p);
            particles[ind] = p;
            particleSystem.SetParticles(particles);
        }
        List<Entity> add = new List<Entity>();
        protected virtual void Update()
        {
            if (isPlaying) return;
            ECSUtils.GetAllEntitiesWith<MoneySoaking>(add);
            var data = new MoneySoaking();
            var found = false;
            add.ForEach(e =>
            {
                if (found) return;
                data = e.GetComponentData<MoneySoaking>();
                if (data.state == MoneySoaking.State.Explosion)
                    found = true;
            });
            if (found)
                Soaking(data).StartAsync();
        }
#endif
    }
}