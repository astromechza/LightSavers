#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using LightSavers.Utils;
#endregion

namespace LightSavers.Components
{
    class SEffectInstanceManager
    {
        SoundEffect sound;
        SoundEffectInstance[] instances;
        float volume;
        bool paused = false, isMusic;
        int numInstances, currentInstance=0;

        public SEffectInstanceManager(SoundEffect sound, int instances, float volume, bool isMusic)
        {
            this.sound = sound;
            this.volume = volume;
            this.instances = new SoundEffectInstance[instances];
            this.isMusic = isMusic;
            this.numInstances = instances;
        }

        //Play the sound associated with the manager
        //Insure all playing instances are accounted for so that they can be paused
        //only creates instances that are needed, and reuses them if possible
        public void playSound()
        {
            int newInstance;
            if (instances[currentInstance] != null && instances[currentInstance].State == SoundState.Playing)
            {
                int possibleNext = (currentInstance+1) % numInstances;

                if (instances[possibleNext] != null && instances[possibleNext].State == SoundState.Playing)
                    return;
                newInstance = possibleNext;
            }
            else if (instances[currentInstance] == null)
            {
                newInstance = currentInstance;
                instances[newInstance] = sound.CreateInstance();
                instances[newInstance].Volume = (!isMusic) ? volume * Globals.audioManager.masterVolume : volume * Globals.audioManager.masterVolume * Globals.audioManager.musicVolume;
                instances[newInstance].Play();
            }
            else
            {
                newInstance = currentInstance;
                instances[newInstance].Volume = (!isMusic) ? volume * Globals.audioManager.masterVolume : volume * Globals.audioManager.masterVolume * Globals.audioManager.musicVolume;
                instances[newInstance].Play();
            }
            currentInstance = newInstance;
        }

        public void PlayLoopSingle()
        {
            instances[0] = sound.CreateInstance();
            instances[0].Volume = (!isMusic) ? volume * Globals.audioManager.masterVolume : volume * Globals.audioManager.masterVolume * Globals.audioManager.musicVolume;
            instances[0].IsLooped = true;
            instances[0].Play();
        }

        /// <summary>
        /// Pause
        /// </summary>
        public void Pause()
        {
            for (int i = 0; i < numInstances; ++i)
                if (instances[i]!=null)
                {
                    instances[i].Pause();
                    instances[i].Volume = (!isMusic) ? volume * Globals.audioManager.masterVolume : volume * Globals.audioManager.masterVolume*Globals.audioManager.musicVolume;
                }
        }

        /// <summary>
        /// Resume
        /// </summary>
        public void Resume()
        {
            for (int i = 0; i < numInstances; ++i)
                if (instances[i] != null)
                {
                    instances[i].Volume = (!isMusic) ? volume * Globals.audioManager.masterVolume : volume * Globals.audioManager.masterVolume * Globals.audioManager.musicVolume;
                    instances[i].Resume();              
                }
        }
    }
}
