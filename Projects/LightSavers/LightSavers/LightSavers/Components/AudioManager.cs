#region File Description
//-----------------------------------------------------------------------------
// AudioManager.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
#endregion

namespace LightSavers.Components
{
    /// <summary>
    /// Component that manages audio playback for all sounds.
    /// </summary>
    public class AudioManager : GameComponent
    {
        #region Fields

        readonly string soundAssetLocation = "Sounds/";

        // Audio Data        
        Dictionary<string, SoundEffectInstance> soundBank;
        Dictionary<string, Song> musicBank;


        #endregion

        #region Initialization
        public AudioManager(Game game)
            : base(game)
        {
            soundBank = new Dictionary<string, SoundEffectInstance>();
            musicBank = new Dictionary<string, Song>();

            game.Components.Add(this);
        }

        #endregion

        #region Loading Methodes


        /// <summary>
        /// Loads a single sound into the sound manager, giving it a specified alias.
        /// </summary>
        /// <param name="contentName">The content name of the sound file. Assumes all sounds are located under
        /// the "Sounds" folder in the content project.</param>
        /// <param name="alias">Alias to give the sound. This will be used to identify the sound uniquely.</param>
        /// <remarks>Loading a sound with an alias that is already used will have no effect.</remarks>
        public void LoadSound(string contentName, string alias)
        {
            SoundEffect soundEffect = Game.Content.Load<SoundEffect>(soundAssetLocation + contentName);
            SoundEffectInstance soundEffectInstance = soundEffect.CreateInstance();

            if (!soundBank.ContainsKey(alias))
            {
                soundBank.Add(alias, soundEffectInstance);
            }
        }

        /// <summary>
        /// Loads a single song into the sound manager, giving it a specified alias.
        /// </summary>
        /// <param name="contentName">The content name of the sound file containing the song. Assumes all sounds are 
        /// located under the "Sounds" folder in the content project.</param>
        /// <param name="alias">Alias to give the song. This will be used to identify the song uniquely.</param>
        /// /// <remarks>Loading a song with an alias that is already used will have no effect.</remarks>
        public void LoadSong(string path, string alias)
        {
            Song song = Game.Content.Load<Song>(path);

            if (!musicBank.ContainsKey(alias))
            {
                musicBank.Add(alias, song);
            }
        }

        /// <summary>
        /// Loads and organizes the sounds used by the game.
        /// </summary>


        #endregion

        #region Sound Methods


        /// <summary>
        /// Indexer. Return a sound instance by name.
        /// </summary>
        public SoundEffectInstance this[string soundName]
        {
            get
            {
                if (soundBank.ContainsKey(soundName))
                {
                    return soundBank[soundName];
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Plays a sound by name.
        /// </summary>
        /// <param name="soundName">The name of the sound to play.</param>
        public void PlaySound(string soundName)
        {
            // If the sound exists, start it
            if (soundBank.ContainsKey(soundName))
            {
                soundBank[soundName].Play();
            }
        }

        /// <summary>
        /// Plays a sound by name.
        /// </summary>
        /// <param name="soundName">The name of the sound to play.</param>
        /// <param name="isLooped">Indicates if the sound should loop.</param>
        public void PlaySound(string soundName, bool isLooped)
        {
            // If the sound exists, start it
            if (soundBank.ContainsKey(soundName))
            {
                if (soundBank[soundName].IsLooped != isLooped)
                {
                    soundBank[soundName].IsLooped = isLooped;
                }

                soundBank[soundName].Play();
            }
        }


        /// <summary>
        /// Plays a sound by name.
        /// </summary>
        /// <param name="soundName">The name of the sound to play.</param>
        /// <param name="isLooped">Indicates if the sound should loop.</param>
        /// <param name="volume">Indicates if the volume</param>
        public void PlaySound(string soundName, bool isLooped, float volume)
        {
            // If the sound exists, start it
            if (soundBank.ContainsKey(soundName))
            {
                if (soundBank[soundName].IsLooped != isLooped)
                {
                    soundBank[soundName].IsLooped = isLooped;
                }

                soundBank[soundName].Volume = volume;
                soundBank[soundName].Play();
            }
        }

        /// <summary>
        /// Stops a sound mid-play. If the sound is not playing, this
        /// method does nothing.
        /// </summary>
        /// <param name="soundName">The name of the sound to stop.</param>
        public void StopSound(string soundName)
        {
            // If the sound exists, stop it
            if (soundBank.ContainsKey(soundName))
            {
                soundBank[soundName].Stop();
            }
        }

        /// <summary>
        /// Stops all currently playing sounds.
        /// </summary>
        public void StopSounds()
        {
            foreach (SoundEffectInstance sound in soundBank.Values)
            {
                if (sound.State != SoundState.Stopped)
                {
                    sound.Stop();
                }
            }
        }

        /// <summary>
        /// Pause or resume all sounds.
        /// </summary>
        /// <param name="resumeSounds">True to resume all paused sounds or false
        /// to pause all playing sounds.</param>
        public void PauseResumeSounds(bool resumeSounds)
        {
            SoundState state = resumeSounds ? SoundState.Paused : SoundState.Playing;

            foreach (SoundEffectInstance sound in soundBank.Values)
            {
                if (sound.State == state)
                {
                    if (resumeSounds)
                    {
                        sound.Resume();
                    }
                    else
                    {
                        sound.Pause();
                    }
                }
            }
        }
        /// <summary>
        /// Play music by name. This stops the currently playing music first. Music will loop until stopped.
        /// </summary>
        /// <param name="musicSoundName">The name of the music sound.</param>
        /// <remarks>If the desired music is not in the music bank, nothing will happen.</remarks>
        public void PlayMusic(string musicSoundName)
        {
            // If the music sound exists
            if (musicBank.ContainsKey(musicSoundName))
            {
                // Stop the old music sound
                if (MediaPlayer.State != MediaState.Stopped)
                {
                    MediaPlayer.Stop();
                }

                MediaPlayer.IsRepeating = true;

                MediaPlayer.Play(musicBank[musicSoundName]);
            }
        }

        /// <summary>
        /// Stops the currently playing music.
        /// </summary>
        public void StopMusic()
        {
            if (MediaPlayer.State != MediaState.Stopped)
            {
                MediaPlayer.Stop();
            }
        }


        #endregion

        #region Instance Disposal Methods


        /// <summary>
        /// Clean up the component when it is disposing.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    foreach (var item in soundBank)
                    {
                        item.Value.Dispose();
                    }
                    soundBank.Clear();
                    soundBank = null;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }


        #endregion
    }
}
