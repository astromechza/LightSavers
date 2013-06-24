using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.ScreenManagement
{
    // The state of the screen in the ScreenManager
    public enum ScreenState
    {
        TransitioningOn,        // fade in
        Active,                 // active
        TransitioningOff,       // fade out
        Hidden
    }

    public abstract class ScreenLayer
    {
        ScreenState state = ScreenState.TransitioningOn;

        public TimeSpan transitionOnTime = TimeSpan.Zero;
        public TimeSpan transitionOffTime = TimeSpan.Zero;

        public float transitionPercent = 0;

        public bool isTransparent = false;

        public bool mustExit = false;

        private bool updateTransition(GameTime gameTime, TimeSpan totalTime, int direction)
        {
            float delta;

            if (totalTime == TimeSpan.Zero)
                delta = 1;
            else
                delta = (float)(gameTime.ElapsedGameTime.TotalMilliseconds / totalTime.TotalMilliseconds);

            transitionPercent += delta * direction;

            if ( (direction < 0) && (transitionPercent <= 0) || (direction > 0) && (transitionPercent >= 1) )
            {
                transitionPercent = MathHelper.Clamp(transitionPercent, 0, 1);
                return false;
            }

            return true;
        }

        /* Layer Update - control transition percentage
         * call via base.Update 
         */
        public void Update(GameTime gameTime)
        {
            switch (state)
            {
                // TransitioningOn : update transition progress, if complete, switch to active
                case ScreenState.TransitioningOn:
                    if (updateTransition(gameTime, transitionOnTime, 1))
                    {
                        state = ScreenState.TransitioningOn;                        
                    }
                    else
                    {                        
                        state = ScreenState.Active;
                    }
                    break;
                case ScreenState.Active:
                    break;
                case ScreenState.TransitioningOff:
                    if (updateTransition(gameTime, transitionOffTime, -1))
                    {
                        state = ScreenState.TransitioningOff;
                    }
                    else
                    {
                        state = ScreenState.Hidden;
                        mustExit = true;
                    }
                    break;
            }
        }

        public void StartTransitionOff()
        {
            state = ScreenState.TransitioningOff;
        }

        public abstract void Draw(GameTime gameTime);




    }
}
