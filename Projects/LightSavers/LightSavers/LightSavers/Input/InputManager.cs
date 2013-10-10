using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LightSavers.Components
{

    // Possible Keys to query for. These are all the keys used in the game.
    public enum Triggers
    {
        Left,
        Right
    }

    public enum AnalogStick
    {
        Left,
        Right
    }

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
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                controller = new KeyboardInputController();
            else
                controller = new XBoxInputController();
        }

        public void Update()
        {
            controller.Update();
        }

        public bool isButtonPressed(Buttons b, PlayerIndex? pi)
        {
            return controller.isButtonPressed(b, pi);
        }

        public bool isButtonDown(Buttons b, PlayerIndex? pi)
        {
            return controller.isButtonDown(b, pi);
        }

        public bool isButtonReleased(Buttons b, PlayerIndex? pi)
        {
            return controller.isButtonReleased(b, pi);
        }

        public bool isTriggerPressed(Triggers t, PlayerIndex? pi)
        {
            return controller.isTriggerPressed(t, pi);
        }

        public bool isTriggerDown(Triggers t, PlayerIndex? pi)
        {
            return controller.isTriggerDown(t, pi);
        }

        public bool isTriggerReleased(Triggers t, PlayerIndex? pi)
        {
            return controller.isTriggerReleased(t, pi);
        }

        public Vector2 getAnalogVector(AnalogStick a, PlayerIndex? pi)
        {
            return controller.getAnalogVector(a, pi);
        }


        public bool isDBGKeyboardKeyPressed(Keys k)
        {
            return controller.isDBGKeyboardKeyPressed(k);
        }

        public bool isDBGKeyboardKeyDown(Keys k)
        {
            return controller.isDBGKeyboardKeyDown(k);
        }

        public bool isDBGKeyboardKeyReleased(Keys k)
        {
            return controller.isDBGKeyboardKeyReleased(k);
        }

    }


    // Skeleton InputController for Xbox and Keyboard to subclass from
    public abstract class InputController
    {
        public abstract void Update();
        public abstract bool isButtonPressed(Buttons b, PlayerIndex? pi);
        public abstract bool isButtonDown(Buttons b, PlayerIndex? pi);
        public abstract bool isButtonReleased(Buttons b, PlayerIndex? pi);
        public abstract bool isTriggerPressed(Triggers t, PlayerIndex? pi);
        public abstract bool isTriggerDown(Triggers t, PlayerIndex? pi);
        public abstract bool isTriggerReleased(Triggers t, PlayerIndex? pi);
        public abstract Vector2 getAnalogVector(AnalogStick a, PlayerIndex? pi);
        public abstract bool isDBGKeyboardKeyPressed(Keys k);
        public abstract bool isDBGKeyboardKeyDown(Keys k);
        public abstract bool isDBGKeyboardKeyReleased(Keys k);
        
    }



}
