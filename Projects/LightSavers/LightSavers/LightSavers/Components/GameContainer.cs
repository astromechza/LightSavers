using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightSavers.ScreenManagement.Layers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LightSavers.Components
{
    public class GameContainer
    {
        private GameLayer gameLayer;

        public GameContainer(GameLayer layer)
        {
            gameLayer = layer;
        }

        public void DrawWorld()
        {
            //throw new NotImplementedException();
        }

        public void DrawHud(SpriteBatch canvas, Viewport viewport)
        {
            //throw new NotImplementedException();
        }

        public void Update()
        {
            if (Globals.inputController.isButtonReleased(Buttons.Back, null))
            {
                gameLayer.StartTransitionOff();
            }
        }

    }
}
