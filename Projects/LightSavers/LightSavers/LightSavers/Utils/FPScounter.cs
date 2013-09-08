using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.Utils
{
    class FPScounter
    {
        private int frameRate = 0;
        private int frameCounter = 0;
        private TimeSpan elapsedTime = TimeSpan.Zero;
        private TimeSpan outPrint = TimeSpan.Zero;
        private long fstc = 0;
        private long lastFrameDrawTicks = 0;
        

        public void frameStart()
        {
            fstc = DateTime.Now.Ticks;
            frameCounter++;
        }

        public void frameEnd()
        {
            lastFrameDrawTicks = DateTime.Now.Ticks - fstc;
        }

        public void updateTick(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;
            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }

            outPrint += gameTime.ElapsedGameTime;
            if (outPrint > TimeSpan.FromSeconds(3))
            {
                System.Diagnostics.Debug.WriteLine("fps: " + frameRate);
                System.Diagnostics.Debug.WriteLine("dticks: " + lastFrameDrawTicks);
                outPrint -= TimeSpan.FromSeconds(3);
            }
        }

        public int getFrameRate()
        {
            return frameRate;
        }

    }
}
