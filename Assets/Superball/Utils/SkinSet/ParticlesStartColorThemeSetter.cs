using UnityEngine;
namespace HcUtils
{
    public class ParticlesStartColorThemeSetter : ThemeSetter
    {
        private new Renderer renderer;
        private ParticleSystem particles;
        public Color[] colors;
        protected override void Init()
        {
            renderer = GetComponent<Renderer>();
            if (renderer) particles = renderer.GetComponent<ParticleSystem>();
            base.Init();
        }
      
        protected override void SetTheme(int ind)
        {
            if (!renderer) return;

            if (colors.Length==0) return;

            ind = Mathf.Clamp(ind, 0, colors.Length-1);

            var main = particles.main;
            main.startColor = colors[ind];

            ParticleSystem.Particle[] p= new ParticleSystem.Particle[particles.particleCount];
            particles.GetParticles(p);

            for (int i = 0; i < p.Length; i++)
            {
                p[i].startColor = colors[ind];
            }
            particles.SetParticles(p);

        }
    }
}