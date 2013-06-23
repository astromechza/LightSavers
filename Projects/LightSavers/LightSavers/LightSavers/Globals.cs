using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers
{
    /**** GLOBALS class ****
     * Use this for static singletons that need to be accessible from everywhere.
     * These objects should only exist once, and only in here where they can be 
     * accessible everywhere. 
     */
    public class Globals
    {
        public static Game1 mainGame;
        public static ScreenManager screenManager;
        public static GraphicsDeviceManager graphics;

    }
}
