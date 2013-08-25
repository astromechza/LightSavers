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

    public delegate bool NoArgCallback();

    public abstract class ScreenLayer
    {
        public ScreenState state = ScreenState.TransitioningOn;

        public TimeSpan transitionOnTime = TimeSpan.Zero;
        public TimeSpan transitionOffTime = TimeSpan.Zero;

        public float transitionPercent = 0;

        public bool isTransparent = false;

        public bool mustExit = false;

        // Callback functions for events
        // - As soon as the layer has completed its fade in
        public NoArgCallback fadeInCompleteCallback;
        // - Just before it fades out
        public NoArgCallback fadeOutBeginCallback;
        // - Once the fade out is complete
        public NoArgCallback fadeOutCompleteCallback;

        public ScreenLayer()
        {
            // do nothing
        }

        private bool UpdateTransition(float ms, TimeSpan totalTime, int direction)
        {
            float delta;

            if (totalTime == TimeSpan.Zero)
                delta = 1;
            else
                delta = (float)(ms / totalTime.TotalMilliseconds);

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
        public virtual void Update(float ms)
        {
            switch (state)
            {
                // TransitioningOn : update transition progress, if complete, switch to active
                case ScreenState.TransitioningOn:
                    if (UpdateTransition(ms, transitionOnTime, 1))
                    {
                        state = ScreenState.TransitioningOn;                        
                    }
                    else
                    {                        
                        state = ScreenState.Active;
                        if (fadeInCompleteCallback != null) fadeInCompleteCallback();
                    }
                    break;
                case ScreenState.Active:
                    break;
                case ScreenState.TransitioningOff:
                    if (UpdateTransition(ms, transitionOffTime, -1))
                    {
                        state = ScreenState.TransitioningOff;
                    }
                    else
                    {
                        state = ScreenState.Hidden;
                        if (fadeOutCompleteCallback != null) fadeOutCompleteCallback();
                        mustExit = true;
                    }
                    break;
            }
        }

        public void StartTransitionOff()
        {
            if (state == ScreenState.TransitioningOff) return;
            if (fadeOutBeginCallback != null) fadeOutBeginCallback();
            state = ScreenState.TransitioningOff;
        }

        public abstract void Draw();




    }
}
