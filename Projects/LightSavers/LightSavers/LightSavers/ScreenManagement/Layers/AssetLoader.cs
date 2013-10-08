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
using LightPrePassRenderer;
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
        static string[] characterAnimationsList = new string[] {"idle_assault", "walk_assault", "run_assault", "walk_assault_shoot","run_assault_shoot", "idle_assault_shoot",
                                                                "idle_snipshot", "walk_snipshot", "run_snipshot", "walk_snipshot_shoot", "run_snipshot_shoot", "idle_snipshot_shoot",
                                                                "idle_pistol", "walk_pistol", "run_pistol", "walk_pistol_shoot", "run_pistol_shoot", "idle_pistol_shoot",
                                                                "idle_sword", "walk_sword", "run_sword", "walk_sword_shoot", "run_sword_shoot", "idle_sword_shoot",
                                                                "death_1"};
        public static Dictionary<string, AnimationClip> ani_character;

        public static Model mdl_alien1;
        static string[] alien01AnimationsList = new string[] { "idle", "death", "moving", "attacking" };
        public static Dictionary<string, AnimationClip> ani_alien1;

        public static Model mdl_alien2;
        public string[] alien02AnimationsList = new string[] { "idle" , "moving", "attacking", "death" };
        public static Dictionary<string, AnimationClip> ani_alien2;

        public static Model mdl_alien3;
        public string[] alien03AnimationsList = new string[] { "idle", "attacking_melee", "attacking_range", "death", "moving" };
        public static Dictionary<string, AnimationClip> ani_alien3;

        public static Model mdl_alien4;
        public string[] alien04AnimationsList = new string[] { "attacking", "charging", "death", "idle", "impact", "moving" };
        public static Dictionary<string, AnimationClip> ani_alien4;

        public static Model mdl_sphere;
        public static Model mdl_ceilinglight;
        public static Model mdl_filingcabinet;
        public static Model mdl_menuscene;
        public static Model mdl_bullet;
        public static Model mdl_doorPanel;
        public static Model mdl_doorBase;
        public static Model mdl_desk;
        public static Model mdl_pipe;
        public static Model mdl_assault_rifle;
        public static Model mdl_pistol;
        public static Model mdl_shotgun;
        public static Model mdl_sniper_rifle;
        public static Model mdl_sword;

        public static Texture2D title2;
        public static Texture2D tex_black;
        public static Texture2D tex_white;
        public static SpriteFont fnt_assetloadscreen;

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
        }
        
        public bool Start()
        {
            Thread t = new Thread(new ThreadStart(LoadAssets));
            t.Start();
            return true;
        }

        public Dictionary<string, AnimationClip> generateAnimations(string prefix, string[] animations)
        {
            Dictionary<string, AnimationClip> newAnimations = new Dictionary<string, AnimationClip>();

            foreach (string ani in animations)
            {
                Model tempModel = loadModel(String.Format("{0}-{1}",prefix, ani));
                newAnimations.Add(ani, ((MeshMetadata)tempModel.Tag).SkinningData.AnimationClips["Take 001"]);

            }
            return newAnimations;
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
            num_assets = 24;
            num_assets += CountSections();
            num_assets += characterAnimationsList.Length;
            num_assets += alien01AnimationsList.Length;
            num_assets += alien02AnimationsList.Length;
            num_assets += alien03AnimationsList.Length;
            num_assets += alien04AnimationsList.Length;
            LoadSections();
            // assets
            mdl_menuscene = loadModel("models/menuscene/MenuScene");

            //Load Character and animations
            mdl_character = loadModel("animatedmodels/player/spacemanAnimated");
            ani_character = generateAnimations("animatedmodels/player/spacemanAnimated", characterAnimationsList);

            mdl_alien1 = loadModel("animatedmodels/alien01/alien01_2");
            ani_alien1 = generateAnimations("animatedmodels/alien01/alien01_2", alien01AnimationsList);

            mdl_alien2 = loadModel("animatedmodels/alien02/alien02_2");
            ani_alien2 = generateAnimations("animatedmodels/alien02/alien02_2", alien02AnimationsList);

            mdl_alien3 = loadModel("animatedmodels/alien03/alien03_2");
            ani_alien3 = generateAnimations("animatedmodels/alien03/alien03_2", alien03AnimationsList);

            mdl_alien4 = loadModel("animatedmodels/alien04/alien04_2");
            ani_alien4 = generateAnimations("animatedmodels/alien04/alien04_2", alien04AnimationsList);
            
            mdl_sphere = loadModel("models/sphere");
            mdl_ceilinglight = loadModel("models/ceilinglight/ceilinglight_model");
            mdl_filingcabinet = loadModel("models/filing/Filing");
            mdl_bullet = loadModel("projectiles/StandardBullet");

            mdl_doorPanel = loadModel("models/door/doorPanel");
            mdl_doorBase = loadModel("models/door/doorBase");

            mdl_desk = loadModel("models/desk/Desk");

            mdl_pipe = loadModel("models/pipe/pipe");

            mdl_assault_rifle = loadModel("models/weapons/assaultrifle/AssaultRifle");
            mdl_sniper_rifle = loadModel("models/weapons/sniperrifle/SniperRifle");
            mdl_pistol = loadModel("models/weapons/pistol/Pistol");
            mdl_shotgun = loadModel("models/weapons/shotgun/Shottie");
            mdl_sword = loadModel("models/weapons/sword/Sword");

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
