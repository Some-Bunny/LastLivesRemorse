using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace LastLivesRemorse
{
    public class Particles
    {
        public static void StartParticles()
        {
            fireParticles = BundleStarter.Bundle.LoadAsset<GameObject>("Quick Fire System").GetComponent<ParticleSystem>();
        }
        public static ParticleSystem fireParticles;
    }
}
