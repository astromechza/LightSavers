using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightSavers.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LightSavers
{
    /**** GLOBALS class ****
     * Use this for static singletons that need to be accessible from everywhere.
     * These objects should only exist once, and only in here where they can be 
     * accessible everywhere. 
     */
    public class Globals
    {
        public static LightSaverGame mainGame;
        public static ScreenManager screenManager;
        public static GraphicsDeviceManager graphics;
        public static ContentManager content;
        public static InputManager inputController;
        public static Viewport viewport;
        public static Random random;

    }
}
