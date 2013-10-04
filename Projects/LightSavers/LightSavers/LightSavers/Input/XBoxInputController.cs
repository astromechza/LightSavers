using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LightSavers.Components
{
    public class XBoxInputController : InputController
    {
        public readonly GamePadState[] CurrentGamepadStates;
        public readonly GamePadState[] LastGamepadStates;

        public XBoxInputController()
        {
            CurrentGamepadStates = new GamePadState[4];
            LastGamepadStates = new GamePadState[4];
        }

        public override void Update()
        {
            for (int i = 0; i < 4; i++)
            {
                LastGamepadStates[i] = CurrentGamepadStates[i];
                CurrentGamepadStates[i] = GamePad.GetState((PlayerIndex)i);
            }
        }

        public override bool isButtonPressed(Buttons b, PlayerIndex? pi)
        {
            if (pi.HasValue)
            {
                return LastGamepadStates[(int)pi.Value].IsButtonUp(b) && CurrentGamepadStates[(int)pi.Value].IsButtonDown(b);
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
                return LastGamepadStates[(int)pi.Value].IsButtonDown(b) && CurrentGamepadStates[(int)pi.Value].IsButtonDown(b);
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
                return LastGamepadStates[(int)pi.Value].IsButtonDown(b) && CurrentGamepadStates[(int)pi.Value].IsButtonUp(b);
            }
            else
            {
                return isButtonReleased(b, PlayerIndex.One) || isButtonReleased(b, PlayerIndex.Two) || isButtonReleased(b, PlayerIndex.Three) || isButtonReleased(b, PlayerIndex.Four);
            }
        }

        public override bool isTriggerPressed(Triggers t, PlayerIndex? pi)
        {
            float threshold = 0.5f;

            if (pi.HasValue)
            {
                if (t == Triggers.Left)
                {
                    return LastGamepadStates[(int)pi.Value].Triggers.Left < threshold && CurrentGamepadStates[(int)pi.Value].Triggers.Left > threshold;
                }
                else
                {
                    return LastGamepadStates[(int)pi.Value].Triggers.Right < threshold && CurrentGamepadStates[(int)pi.Value].Triggers.Right > threshold;
                }
            }
            else
            {
                return isTriggerPressed(t, PlayerIndex.One) || isTriggerPressed(t, PlayerIndex.Two) || isTriggerPressed(t, PlayerIndex.Three) || isTriggerPressed(t, PlayerIndex.Four);
            }
        }

        public override bool isTriggerDown(Triggers t, PlayerIndex? pi)
        {
            float threshold = 0.5f;

            if (pi.HasValue)
            {
                if (t == Triggers.Left)
                {
                    return LastGamepadStates[(int)pi.Value].Triggers.Left > threshold && CurrentGamepadStates[(int)pi.Value].Triggers.Left > threshold;
                }
                else
                {
                    return LastGamepadStates[(int)pi.Value].Triggers.Right > threshold && CurrentGamepadStates[(int)pi.Value].Triggers.Right > threshold;
                }
            }
            else
            {
                return isTriggerDown(t, PlayerIndex.One) || isTriggerDown(t, PlayerIndex.Two) || isTriggerDown(t, PlayerIndex.Three) || isTriggerDown(t, PlayerIndex.Four);
            }
        }

        public override bool isTriggerReleased(Triggers t, PlayerIndex? pi)
        {
            float threshold = 0.5f;

            if (pi.HasValue)
            {
                if (t == Triggers.Left)
                {
                    return LastGamepadStates[(int)pi.Value].Triggers.Left > threshold && CurrentGamepadStates[(int)pi.Value].Triggers.Left < threshold;
                }
                else
                {
                    return LastGamepadStates[(int)pi.Value].Triggers.Right > threshold && CurrentGamepadStates[(int)pi.Value].Triggers.Right < threshold;
                }
            }
            else
            {
                return isTriggerReleased(t, PlayerIndex.One) || isTriggerReleased(t, PlayerIndex.Two) || isTriggerReleased(t, PlayerIndex.Three) || isTriggerReleased(t, PlayerIndex.Four);
            }
        }

        public override Vector2 getAnalogVector(AnalogStick a, PlayerIndex? pi)
        {
            if (pi.HasValue)
            {
                if (a == AnalogStick.Left)
                {
                    return CurrentGamepadStates[(int)pi.Value].ThumbSticks.Left;
                }
                else
                {
                    return CurrentGamepadStates[(int)pi.Value].ThumbSticks.Right;
                }
            }
            else
            {
                return getAnalogVector(a, PlayerIndex.One);
            }
        }

        public override bool isDBGKeyboardKeyPressed(Keys k)
        {
            return false;
        }
        public override bool isDBGKeyboardKeyDown(Keys k)
        {
            return false;
        }
        public override bool isDBGKeyboardKeyReleased(Keys k)
        {
            return false;
        }

    }
}
