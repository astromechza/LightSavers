#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
#endregion

namespace LightSavers.Components
{
    class SEffectInstanceManager
    {
        SoundEffect sound;
        SoundEffectInstance[] instances;
        bool paused = false;
        int numInstances, currentInstance=0;
        public SEffectInstanceManager(SoundEffect sound, int instances)
        {
            this.sound = sound;
            this.instances = new SoundEffectInstance[instances];
            this.numInstances = instances;
        }

        //Play the sound associated with the manager
        public void playSound(float volume)
        {
            int newInstance = currentInstance % numInstances;
            instances[newInstance] = sound.CreateInstance();
            instances[newInstance].Volume = volume;
            instances[newInstance].Play();
        }

        /// <summary>
        /// Pause all of the instances - may ignore this
        /// </summary>
        public void TogglePause()
        {       
            for (int i = 0; i < numInstances; ++i)
            {
                if (paused)
                    instances[i].Resume();
                else
                    instances[i].Pause();
            }
            paused = (paused) ? false : true;
        }
    }
}
