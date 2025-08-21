using System.Collections.Generic;
using UnityEngine;

namespace GMS
{
    public class VFXManager: ISubManager
    {
        private VFXData _data;
        private const float DORMANT_PARTICLES_CHECK_RATE = 2.5f;

        private ViewContent<ParticleSystem> _particles;

        private List<string> _poolCategories;
        private List<ParticleSystem> _poolParticlesEmittionCheck;

       // working variable
        private ParticleSystem particleToPlay;
        private ParticleSystem particleToCheck;
        private string _keycache;
        private float _dormantParticlesCheckCooldown;

        public VFXManager(VFXData data)
        {
            _data = data;
        }

        public bool Initialize(GameManager gameManager)
        {
            _poolCategories = new List<string>();
            _particles = new ViewContent<ParticleSystem>(_data.ParticlesPoolGameObjectName, GameInstance.S.transform);
            return true;
        }

        public void UnInitialize()
        {
            // Cleanup, ie. unload instances, send cleanup messages
        }

        public void OnUpdate()
        {
            UpdateParticlesToDormant();
        }
        public bool Equals(ISubManager other)
        {
            // Check if other is null
            if (other == null)
            {
                return false;
            }

            // Compare the runtime types of the current instance and the other instance
            return GetType() == other.GetType();
        }

        public override bool Equals(object obj)
        {
            if (obj is ISubManager other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            // Generate a hash code based on the fields that contribute to equality
            return GetType().GetHashCode();
        }

        private void UpdateParticlesToDormant()
        {
            if (!Cooldown.IsCooled(_dormantParticlesCheckCooldown, DORMANT_PARTICLES_CHECK_RATE))
            {
                return;
            }

            _dormantParticlesCheckCooldown = Cooldown.Cool();

            for (int i = _poolCategories.Count -1; i >=0 ; i--)
            {
                if (!_particles.GetPool(_poolCategories[i], out _poolParticlesEmittionCheck))
                {
                    continue;
                }

                for (int j = _poolParticlesEmittionCheck.Count-1; i >=0 ; i--)
                {
                    particleToCheck = _poolParticlesEmittionCheck[i];

                    _particles.ReturnToPool(particleToCheck);
                }
            }
        }

        /// <summary>
        /// Plays a particle system, uses ViewContent as pooling system.
        /// </summary>
        /// <param name="particlePrefab"></param>
        /// <param name="position">World position</param>
        /// <param name="direction">euler direction passed in, Converted to quaternion with LookRotation using UpVector default World up as Vector3.up</param>
        /// <returns></returns>
        public ParticleSystem PlayParticle(ParticleSystem particlePrefab, Vector3 position, Vector3 direction)
        {
            if (particlePrefab == null)
            {
                return null;
            }
            
            particleToPlay = _particles.GetOrCreate(particlePrefab, particlePrefab.name, particlePrefab.name);

            direction.Normalize();
            Quaternion rotation = Quaternion.identity;
            if (direction != Vector3.zero)
            {
                rotation = Quaternion.LookRotation(direction, Vector3.up);
            }

            particleToPlay.transform.position = position;
            particleToPlay.transform.rotation = rotation;

            particleToPlay.Play(true);
            return particleToPlay;
        }

    }
}
