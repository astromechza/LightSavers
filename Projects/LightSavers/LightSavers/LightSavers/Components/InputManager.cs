using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LightSavers.Components
{
    /* InputController
     *  
     * This needs to become an awesome crossplatform input thing capable of:
     * - support all 4 controllers
     * - support keyboard for debugging 2 players
     * - isKeyPressed / isKeyReleased / isKeyJustPressed / isKeyJustReleased
     * - get list of connected players.
     * - disconnected / reconnected event..
     * 
     */
    public class InputManager
    {

        InputController controller;

        public InputManager()
        {
            OperatingSystem os = Environment.OSVersion;
            PlatformID pid = os.Platform;
            if (pid == PlatformID.Win32NT)
            {
                controller = new KeyboardInputController();
            }
            else
            {
                controller = new XBoxInputController();
            }
        }

        // Called on each game update step
        public void Update()
        {
            controller.Update();
        }

        public bool isKeyPressed(PlayerIndex? player, GameKey k)
        {
            return controller.IsKeyPressed(player, k);
        }

        public bool isKeyDown(PlayerIndex? player, GameKey k)
        {
            return controller.IsKeyDown(player, k);
        }

        public bool isKeyUp(PlayerIndex? player, GameKey k)
        {
            return controller.IsKeyUp(player, k);
        }

    }

    // Possible Keys to query for. These are all the keys used in the game.
    public enum GameKey { UP, DOWN, LEFT, RIGHT, SELECT, BACK, FIRE };


    // Skeleton InputController for Xbox and Keyboard to subclass from
    public abstract class InputController
    {
        public abstract void Update();
        public abstract bool IsKeyPressed(PlayerIndex? player, GameKey k);
        public abstract bool IsKeyDown(PlayerIndex? player, GameKey k);
        public abstract bool IsKeyUp(PlayerIndex? player, GameKey k);      
    }



}
