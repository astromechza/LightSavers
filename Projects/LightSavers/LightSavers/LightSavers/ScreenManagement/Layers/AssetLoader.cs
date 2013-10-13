using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using LightSavers.ScreenManagement;
using System.Threading;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using LightSavers.ScreenManagement.Layers;
using LightSavers.Utils;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using LightPrePassRenderer;
using LightSavers.Components.GameObjects;
using SkinnedModel;
using System.IO;

namespace LightSavers
{
    /** AssetLoader layer ***
     * loads larger assets at the beginning of the game with nice fade in / out and progress bar.
     * To add a new asset, add a static variable for it and add it to the LoadAssets method. 
     * 
     * Naming convention:
     *   Model -> mdl_relevantname
     *   Texture -> tex_relevantname
     *   Sound -> snd_relevantname
     *   Font -> fnt_relevantname
     * 
     ************************/ 
    public class AssetLoader : ScreenLayer
    {
        /************* ASSETS **************/
        public static Model mdl_character;
        public static DurationBasedAnimator.AnimationPackage ani_character;

        public static Model mdl_alien1;
        public static DurationBasedAnimator.AnimationPackage ani_alien1;

        public static Model mdl_alien2;
        public static DurationBasedAnimator.AnimationPackage ani_alien2;

        public static Model mdl_alien3;
        public static DurationBasedAnimator.AnimationPackage ani_alien3;

        public static Model mdl_alien4;
        public static DurationBasedAnimator.AnimationPackage ani_alien4;

        public static Model mdl_sphere;
        public static Model mdl_ceilinglight;
        public static Model mdl_filingcabinet;
        public static Model mdl_menuscene;
        public static Model mdl_pistolBullet;
        public static Model mdl_shotgunBullet;
        public static Model mdl_assaultBullet;
        public static Model mdl_sniper_bullet;
        public static Model mdl_doorPanel;
        public static Model mdl_doorBase;
        public static Model mdl_desk;
        public static Model mdl_pipe;
        public static Model mdl_assault_rifle;
        public static Model mdl_pistol;
        public static Model mdl_shotgun;
        public static Model mdl_sniper_rifle;
        public static Model mdl_sword;
        public static Model mdl_dropfragment;

        public static SoundEffect snd_game_music;
        public static SoundEffect snd_game_start;

        public static SoundEffect snd_pistol;
        public static SoundEffect snd_shottie;
        public static SoundEffect snd_sniper;
        public static SoundEffect snd_sword;
        public static SoundEffect snd_assault;
        public static SoundEffect snd_bullet_impact1;

        public static SoundEffect snd_alien_death1;

        public static Texture2D title2;
        public static Texture2D tex_black;
        public static Texture2D tex_white;
        public static SpriteFont fnt_assetloadscreen;

        public static SpriteFont fnt_healthgamescreen;
        public static SpriteFont fnt_paragraph;

        //HUDS
        public static Texture2D ammo;
        public static Texture2D pistol_blue;
        public static Texture2D pistol_green;

        public static Texture2D rifle_green;
        public static Texture2D rifle_blue;

        public static Texture2D sniper_green;
        public static Texture2D sniper_blue;

        public static Texture2D shot_green;
        public static Texture2D shot_blue;

        public static Texture2D sword_green;
        public static Texture2D sword_blue;

        public static Texture2D diamond;

        public static Texture2D keyboard;
        public static Texture2D controller;
        public static Texture2D group;

        //subheadings
        public static Texture2D controls;
        public static Texture2D about;
        public static Texture2D settings;
        public static Texture2D paused;

        // -- Sections
        public static Model[] mdl_section;
        public static Texture2D[] tex_section_ent;

        /***********************************/

        private String loading_msg = "Loading Assets";
        private int loading_bar_length = 0;
        private int loaded_assets = 0;
        private int num_assets = -1;

        private Rectangle titleRect;
        private Viewport viewport;
        private SpriteBatch spriteBatch;
        
        private int drawingY;

        public AssetLoader() : base()
        {
            spriteBatch = new SpriteBatch(Globals.graphics.GraphicsDevice);
            viewport = Globals.graphics.GraphicsDevice.Viewport;

            // Assets required to view the loading screen
            fnt_assetloadscreen = Globals.content.Load<SpriteFont>("fonts/LoadingFont");
            fnt_healthgamescreen = Globals.content.Load<SpriteFont>("fonts/HealthFont");
            fnt_paragraph = Globals.content.Load<SpriteFont>("fonts/ParagraphFont");

            tex_black = new Texture2D(Globals.graphics.GraphicsDevice, 1, 1);
            tex_black.SetData(new Color[] { Color.Black });
            tex_white = new Texture2D(Globals.graphics.GraphicsDevice, 1, 1);
            tex_white.SetData(new Color[] { Color.White });

            title2 = Globals.content.Load<Texture2D>("textures/title2");


            // Fade times
            transitionOnTime = TimeSpan.FromSeconds(0.6);
            transitionOffTime = TimeSpan.FromSeconds(0.5);      
     
            // vertical position
            drawingY = (int)(viewport.Height * 0.75f);

            fadeInCompleteCallback = Start;
            fadeOutCompleteCallback = DisplayMainMenu;

            int tx = (viewport.Width - 800) / 2;
            titleRect = new Rectangle(tx, 100, 800, 230);

            //Load Music and sounds
            Globals.audioManager.LoadMenuMusic("soundz/menu/menu_music");
            Globals.audioManager.PlayMenuMusic();
            //Globals.audioManager.LoadMenuSound("soundz/menu/menu_select", "menu_select");
        }
        
        public bool Start()
        {
            Thread t = new Thread(new ThreadStart(LoadAssets));
            t.Start();
            return true;
        }

        /// <summary>
        /// Extracts the Animations from Take 001
        /// </summary>
        /// <param name="array containing the names of the animations"></param>
        /// <param name="array containing key ranges, eg 0-48,49-75 as [0,48,49,75]"></param>
        /// <param name="Model to be animated"></param>
        /// <param name="How many keyframes are baked into the model (generally if the last key is 11, this would be 10)"></param>
        /// <returns></returns>
        public DurationBasedAnimator.AnimationPackage generateAnimationPackage(int[] names, int[] keyRanges, Model m)
        {
            DurationBasedAnimator.AnimationPackage newPackage = new DurationBasedAnimator.AnimationPackage(((MeshMetadata)m.Tag).SkinningData, (float)keyRanges[keyRanges.Length-1]-1);

            for ( int i=0; i<names.Length;++i)
            {
                newPackage.AddDurationClipEasy(names[i], keyRanges[i*2], keyRanges[i*2+1]);
            }
            

            return newPackage;
        }


        public bool DisplayMainMenu()
        {
            // remove the loading layer since its not needed
            Globals.screenManager.Pop();
            // add main menu screen
            Globals.screenManager.Push(new MainMenuLayer());
            return true;
        }

        // Asset loading thread
        // Each asset updates the message and increments the load count
        private void LoadAssets()
        {
            // number of assets to be loaded. (used to compute progress bar size)
            num_assets = 37;
            num_assets += CountSections();
            LoadSections();
            // assets
            mdl_menuscene = loadModel("models/menuscene/MenuScene");

            snd_game_music = Globals.content.Load<SoundEffect>("soundz/game/music/desolate");
            Globals.audioManager.LoadGameSound(snd_game_music, "music", 1, 1.0f, true);

            snd_pistol = Globals.content.Load<SoundEffect>("soundz/game/weapons/pistol");
            Globals.audioManager.LoadGameSound(snd_pistol, "pistol", 10, Globals.audioManager.pistol, false);           

            snd_shottie = Globals.content.Load<SoundEffect>("soundz/game/weapons/shottie");
            Globals.audioManager.LoadGameSound(snd_shottie, "shottie", 5, Globals.audioManager.shottie, false);           

            snd_sniper = Globals.content.Load<SoundEffect>("soundz/game/weapons/sniper");
            Globals.audioManager.LoadGameSound(snd_sniper, "sniper", 5, Globals.audioManager.sniper, false);            

            snd_sword = Globals.content.Load<SoundEffect>("soundz/game/weapons/sword");
            Globals.audioManager.LoadGameSound(snd_sword, "sword", 5, Globals.audioManager.sword, false);           

            snd_assault = Globals.content.Load<SoundEffect>("soundz/game/weapons/assault");
            Globals.audioManager.LoadGameSound(snd_assault, "assault", 15, Globals.audioManager.assault, false);
            
            snd_alien_death1 = Globals.content.Load<SoundEffect>("soundz/game/creatures/deathalien1");
            Globals.audioManager.LoadGameSound(snd_alien_death1, "aliendeath1", 6, Globals.audioManager.alienDeath1, false);            

            snd_game_start = Globals.content.Load<SoundEffect>("soundz/game/ambience/begin");
            Globals.audioManager.LoadGameSound(snd_game_start, "start_game", 1, 1.0f, false);

            snd_bullet_impact1 = Globals.content.Load<SoundEffect>("soundz/game/weapons/bulletimpact1");
            Globals.audioManager.LoadGameSound(snd_bullet_impact1, "impact1", 4, 0.1f, false);           

            //Load Character and animations
            mdl_character = loadModel("animatedmodels/player/spacemanAnimated");
            ani_character = generateAnimationPackage(Animation_States.characterAnimationsList, Animation_States.characterAnimationKeys, mdl_character);

            mdl_alien1 = loadModel("animatedmodels/alien01/alien01_2");
            ani_alien1 = generateAnimationPackage(Animation_States.alien01AnimationsList, Animation_States.alien01AnimationKeys, mdl_alien1);

            mdl_alien2 = loadModel("animatedmodels/alien02/alien02_2");
            ani_alien2 = generateAnimationPackage(Animation_States.alien02AnimationsList, Animation_States.alien02AnimationKeys, mdl_alien2);

            mdl_alien3 = loadModel("animatedmodels/alien03/alien03_2");
            ani_alien3 = generateAnimationPackage(Animation_States.alien03AnimationsList, Animation_States.alien03AnimationKeys, mdl_alien3);

            mdl_alien4 = loadModel("animatedmodels/alien04/alien04_2");
            ani_alien4 = generateAnimationPackage(Animation_States.alien04AnimationsList, Animation_States.alien04AnimationKeys, mdl_alien4);

            Animation_States.upperCharacterBones = new Dictionary<int, int>();

            for (int i = 1; i < 48; i++)
            {
                Animation_States.upperCharacterBones.Add(i, 1);
            }

            Animation_States.lowerCharacterBonesandRoot = new Dictionary<int, int>();
            Animation_States.lowerCharacterBonesandRoot.Add(0, 1);

            for (int i = 48; i < 70; ++i)
            {
                Animation_States.lowerCharacterBonesandRoot.Add(i, 1);
            }

            mdl_sphere = loadModel("models/sphere");
            mdl_ceilinglight = loadModel("models/ceilinglight/ceilinglight_model");
            mdl_filingcabinet = loadModel("models/filing/Filing");
            mdl_pistolBullet = loadModel("projectiles/PistolBullet");

            mdl_shotgunBullet = loadModel("projectiles/ShotgunBullet");

            mdl_assaultBullet = loadModel("projectiles/AssaultBullet");
            mdl_sniper_bullet = loadModel("projectiles/SniperBullet");


            mdl_dropfragment = loadModel("projectiles/DropFragment");

            mdl_doorPanel = loadModel("models/door/doorPanel");
            mdl_doorBase = loadModel("models/door/doorBase");

            mdl_desk = loadModel("models/desk/Desk");

            mdl_pipe = loadModel("models/pipe/pipe");

            mdl_assault_rifle = loadModel("models/weapons/assaultrifle/AssaultRifle");
            mdl_sniper_rifle = loadModel("models/weapons/sniperrifle/SniperRifle");
            mdl_pistol = loadModel("models/weapons/pistol/Pistol");
            mdl_shotgun = loadModel("models/weapons/shotgun/Shottie");
            mdl_sword = loadModel("models/weapons/sword/Sword");

            diamond = loadTexture("textures/diamond");

            //HUDs
            ammo = loadTexture("textures/HUDs/ammo");
            pistol_blue = loadTexture("textures/HUDs/pistol_blue");
            pistol_green = loadTexture("textures/HUDs/pistol_green");
            shot_blue = loadTexture("textures/HUDs/shot_blue");
            shot_green = loadTexture("textures/HUDs/shot_green");
            rifle_blue = loadTexture("textures/HUDs/rifle_blue");
            rifle_green = loadTexture("textures/HUDs/rifle_green");
            sniper_blue = loadTexture("textures/HUDs/sniper_blue");
            sniper_green = loadTexture("textures/HUDs/sniper_green");
            sword_blue = loadTexture("textures/HUDs/sword_blue");
            sword_green = loadTexture("textures/HUDs/sword_green");


            //adding diagrams
            controller = Globals.content.Load<Texture2D>("textures/xbox_controller_diagram");
            keyboard  = Globals.content.Load<Texture2D>("textures/keyboard_controller_diagram");
            group = Globals.content.Load<Texture2D>("textures/group_small");
            
            //subheadings
            controls = Globals.content.Load<Texture2D>("textures/Headings/controls");
            about = Globals.content.Load<Texture2D>("textures/Headings/about");
            paused = Globals.content.Load<Texture2D>("textures/Headings/gamepaused");
            settings = Globals.content.Load<Texture2D>("textures/Headings/settings");

            // once its loaded, fade out
            StartTransitionOff();
        }

        private Model loadModel(String modelfile)
        {
            loading_msg = String.Format("Loading model '{0}'", modelfile);
            Stopwatch s = new Stopwatch(); s.Start();
            Model m = Globals.content.Load<Model>(modelfile);
            System.Diagnostics.Debug.WriteLine(String.Format("Loading model: '{0}' took {1}ms", modelfile, s.ElapsedMilliseconds));
            loaded_assets += 1;
            return m;
        }

        private Texture2D loadTexture(String texfile)
        {
            loading_msg = String.Format("Loading texture '{0}'", texfile);
            Stopwatch s = new Stopwatch(); s.Start();
            Texture2D t = Globals.content.Load<Texture2D>(texfile);
            System.Diagnostics.Debug.WriteLine(String.Format("Loading texture: '{0}' took {1}ms", texfile, s.ElapsedMilliseconds));
            loaded_assets += 1;
            return t;
        }

        // Draw the layer
        // A black background + text + progress bar
        public override void Draw()
        {
            Color talpha = new Color(transitionPercent, transitionPercent, transitionPercent, transitionPercent);

            spriteBatch.Begin();

            spriteBatch.Draw(tex_black, viewport.Bounds, talpha);

            spriteBatch.Draw(title2, titleRect, talpha);

            spriteBatch.DrawString(fnt_assetloadscreen, loading_msg, new Vector2((viewport.Width - fnt_assetloadscreen.MeasureString(loading_msg).X) / 2, drawingY), talpha);

            spriteBatch.Draw(tex_white, new Rectangle(20, drawingY + 50, loading_bar_length, 2), talpha); 

            spriteBatch.End();
        }

        // Update the size of the loading bar
        public override void Update(float ms)
        {
            loading_bar_length = (int)((loaded_assets / (float)num_assets) * (viewport.Width-20));
            base.Update(ms);
        }


        private int CountSections()
        {
            DirectoryInfo dir = new DirectoryInfo(Globals.content.RootDirectory + "/levels/geometry");
            FileInfo[] files = dir.GetFiles("*.xnb");
            return files.Length;
        }

        private void LoadSections()
        {
            DirectoryInfo dir = new DirectoryInfo(Globals.content.RootDirectory + "/levels/geometry");
            FileInfo[] files = dir.GetFiles("*.xnb");

            int count = files.Length;

            mdl_section = new Model[count];
            tex_section_ent = new Texture2D[count];

            int index = 0;
            foreach (FileInfo f in files)
            {
                string gfile = "levels/geometry/" + Path.GetFileNameWithoutExtension(f.Name);
                string efile = "levels/entities/" + Path.GetFileNameWithoutExtension(f.Name);

                mdl_section[index] = loadModel(gfile);
                tex_section_ent[index] = loadTexture(efile);

                index++;
            }
        }

        
    }
}
