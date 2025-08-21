using System;
using UnityEngine;

namespace GMS
{
    /// <summary>
    /// Plays particle systems on Do(), and stops the emmiters on particles systems in Undo()
    /// </summary>
    public class IAction_PlayParticles : MonoBehaviour, IAction
    {
        [Header("Assign Particle Systems to Play"), SerializeField]
        private ParticleSystem[] _particleSystems = Array.Empty<ParticleSystem>();

        public void Do()
        {
            foreach (ParticleSystem particle in _particleSystems)
            {
                particle.Play();
            }
        }

        public void Undo()
        {
            foreach (ParticleSystem particle in _particleSystems)
            {
                ParticleSystem.EmissionModule emission = particle.emission;
                emission.enabled = false;
            }
        }
    }
}