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

        public void frameTick()
        {
            frameCounter++;
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
                System.Diagnostics.Debug.WriteLine(frameRate);
                System.Diagnostics.Debug.WriteLine(Globals.screenManager.Count());
                outPrint -= TimeSpan.FromSeconds(3);
            }
        }

        public int getFrameRate()
        {
            return frameRate;
        }

    }
}
