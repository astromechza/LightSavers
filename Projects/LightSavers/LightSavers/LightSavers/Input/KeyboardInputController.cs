using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LightSavers.Components
{
    public class KeyboardInputController : InputController
    {
        public KeyboardState CurrentKeyboardState;
        public KeyboardState LastKeyboardState;

        public KeyboardInputController()
        {
            CurrentKeyboardState = new KeyboardState();
            LastKeyboardState = new KeyboardState();
        }

        public override void Update()
        {
            LastKeyboardState = CurrentKeyboardState;
            CurrentKeyboardState = Keyboard.GetState();
        }

        public override bool isButtonPressed(Buttons b, PlayerIndex? pi)
        {
            if (pi.HasValue)
            {
                Keys k = getKeyForButton(b, pi);
                return LastKeyboardState.IsKeyUp(k) && CurrentKeyboardState.IsKeyDown(k);
            }
            else
            {
                return isButtonPressed(b, PlayerIndex.One) || isButtonPressed(b, PlayerIndex.Two) || isButtonPressed(b, PlayerIndex.Three) || isButtonPressed(b, PlayerIndex.Four);
            }
        }

        public override bool isButtonDown(Buttons b, PlayerIndex? pi)
        {
            if (pi.HasValue)
            {
                Keys k = getKeyForButton(b, pi);
                return LastKeyboardState.IsKeyDown(k) && CurrentKeyboardState.IsKeyDown(k);
            }
            else
            {
                return isButtonDown(b, PlayerIndex.One) || isButtonDown(b, PlayerIndex.Two) || isButtonDown(b, PlayerIndex.Three) || isButtonDown(b, PlayerIndex.Four);
            }
        }

        public override bool isButtonReleased(Buttons b, PlayerIndex? pi)
        {
            if (pi.HasValue)
            {
                Keys k = getKeyForButton(b, pi);
                return LastKeyboardState.IsKeyDown(k) && CurrentKeyboardState.IsKeyUp(k);
            }
            else
            {
                return isButtonReleased(b, PlayerIndex.One) || isButtonReleased(b, PlayerIndex.Two) || isButtonReleased(b, PlayerIndex.Three) || isButtonReleased(b, PlayerIndex.Four);
            }
        }

        public override bool isTriggerPressed(Triggers t, PlayerIndex? pi)
        {
            if (pi.HasValue)
            {
                Keys k = getKeyForTrigger(t, pi);
                return LastKeyboardState.IsKeyUp(k) && CurrentKeyboardState.IsKeyDown(k);
            }
            else
            {
                return isTriggerPressed(t, PlayerIndex.One) || isTriggerPressed(t, PlayerIndex.Two) || isTriggerPressed(t, PlayerIndex.Three) || isTriggerPressed(t, PlayerIndex.Four);
            }
        }

        public override bool isTriggerDown(Triggers t, PlayerIndex? pi)
        {
            if (pi.HasValue)
            {
                Keys k = getKeyForTrigger(t, pi);
                return LastKeyboardState.IsKeyDown(k) && CurrentKeyboardState.IsKeyDown(k);
            }
            else
            {
                return isTriggerPressed(t, PlayerIndex.One) || isTriggerPressed(t, PlayerIndex.Two) || isTriggerPressed(t, PlayerIndex.Three) || isTriggerPressed(t, PlayerIndex.Four);
            }
        }

        public override bool isTriggerReleased(Triggers t, PlayerIndex? pi)
        {
            if (pi.HasValue)
            {
                Keys k = getKeyForTrigger(t, pi);
                return LastKeyboardState.IsKeyDown(k) && CurrentKeyboardState.IsKeyUp(k);
            }
            else
            {
                return isTriggerPressed(t, PlayerIndex.One) || isTriggerPressed(t, PlayerIndex.Two) || isTriggerPressed(t, PlayerIndex.Three) || isTriggerPressed(t, PlayerIndex.Four);
            }
        }

        public override Vector2 getAnalogVector(AnalogStick a, PlayerIndex? pi)
        {
            if (a == AnalogStick.Left)
            {
                Vector2 v = Vector2.Zero;

                if (CurrentKeyboardState.IsKeyDown(Keys.A)) v += new Vector2(-1.0f, 0.0f);
                if (CurrentKeyboardState.IsKeyDown(Keys.D)) v += new Vector2(1.0f, 0.0f);
                if (CurrentKeyboardState.IsKeyDown(Keys.S)) v += new Vector2(0.0f, -1.0f);
                if (CurrentKeyboardState.IsKeyDown(Keys.W)) v += new Vector2(0.0f, 1.0f);

                return v;
            }
            else
            {
                Vector2 v = Vector2.Zero;
                if (CurrentKeyboardState.IsKeyDown(Keys.Left)) v += new Vector2(-1.0f, 0.0f);
                if (CurrentKeyboardState.IsKeyDown(Keys.Right)) v += new Vector2(1.0f, 0.0f);
                if (CurrentKeyboardState.IsKeyDown(Keys.Down)) v += new Vector2(0.0f, -1.0f);
                if (CurrentKeyboardState.IsKeyDown(Keys.Up)) v += new Vector2(0.0f, 1.0f);

                return v;
            }
        }

        private Keys getKeyForButton(Buttons b, PlayerIndex? pi)
        {
            switch (b)
            {
                case Buttons.A:
                    return Keys.Enter;

                case Buttons.B:
                    return Keys.Back;

                case Buttons.Back:
                    return Keys.Escape;

                case Buttons.DPadDown:
                    return Keys.S;

                case Buttons.DPadLeft:
                    return Keys.A;

                case Buttons.DPadRight:
                    return Keys.D;

                case Buttons.DPadUp:
                    return Keys.W;

                case Buttons.LeftShoulder:
                    return Keys.Q;

                case Buttons.RightShoulder:
                    return Keys.D3;

                case Buttons.X:
                    return Keys.E;

                //Temporary fix for disabling switching to any weapon
                //case Buttons.Y:
                //    return Keys.F;

                case Buttons.Start:
                    return Keys.Escape;

                case Buttons.LeftThumbstickLeft:
                    return Keys.A;

                case Buttons.LeftThumbstickRight:
                    return Keys.D;

                case Buttons.LeftThumbstickDown:
                    return Keys.S;

                case Buttons.LeftThumbstickUp:
                    return Keys.W;

                case Buttons.RightThumbstickLeft:
                    return Keys.Left;

                case Buttons.RightThumbstickRight:
                    return Keys.Right;

                case Buttons.RightThumbstickDown:
                    return Keys.Down;

                case Buttons.RightThumbstickUp:
                    return Keys.Up;

                case Buttons.LeftTrigger:
                    return Keys.LeftShift;

                case Buttons.RightTrigger:
                    return Keys.Space;

                default:
                    return Keys.F1;
            }
        }

        private Keys getKeyForTrigger(Triggers t, PlayerIndex? pi)
        {
            switch (t)
            {
                case Triggers.Right:
                    return Keys.Space;
                case Triggers.Left:
                    return Keys.LeftShift;
                default:
                    return Keys.F1;
            }
        }

        public override bool isDBGKeyboardKeyPressed(Keys k)
        {
            return LastKeyboardState.IsKeyUp(k) && CurrentKeyboardState.IsKeyDown(k);
        }
        public override bool isDBGKeyboardKeyDown(Keys k)
        {
            return LastKeyboardState.IsKeyDown(k) && CurrentKeyboardState.IsKeyDown(k);
        }
        public override bool isDBGKeyboardKeyReleased(Keys k)
        {
            return LastKeyboardState.IsKeyDown(k) && CurrentKeyboardState.IsKeyUp(k);
        }




    }
}
