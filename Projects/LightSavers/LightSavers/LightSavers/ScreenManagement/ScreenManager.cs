using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using LightSavers.ScreenManagement;

namespace LightSavers
{
    public class ScreenManager
    {
        private List<ScreenLayer> layers;
        private int lowestVisibleLayer = -1;
        private Boolean gameHasStarted = false;

        public ScreenManager() 
        {
            layers = new List<ScreenLayer>();
        }

        public void Update(float ms)
        {
            if (layers.Count > 0)
            {
                layers[layers.Count - 1].Update(ms);
                if (layers[layers.Count - 1].mustExit)
                {
                    Pop();
                }
            }
            else if (gameHasStarted)
            {
                Globals.mainGame.Exit();
            }
        }

        // *** Draw ***
        // Draw the layers from the lowest non-transparent layer upwards 
        public void Draw()
        {
            if (lowestVisibleLayer == -1) return;

            for (int i = lowestVisibleLayer; i < layers.Count; i++)
            {
                layers[i].Draw();
            }
        }

        // *** Push ***
        // Push a new layer onto the list
        public void Push(ScreenLayer layer)
        {
            gameHasStarted = true;
            layers.Add(layer);

            // If the added layer is not transparent, it must be the first layer drawn
            if (lowestVisibleLayer == -1 || !layer.isTransparent)
            {
                lowestVisibleLayer = layers.Count - 1;
            }
        }

        // *** Pop ***
        // Remove and return the topmost layer
        public ScreenLayer Pop()
        {
            ScreenLayer sl = layers[layers.Count - 1];
            layers.RemoveAt(layers.Count - 1);

            // Recalculate lowestVisibleLayer
            lowestVisibleLayer = -1;
            for (int i = layers.Count-1; i >=0; i--)
            {
                if (!layers[i].isTransparent)
                {
                    lowestVisibleLayer = i;
                    break;
                }
            }

            return sl;
        }

        public ScreenLayer GetTop()
        {
            return layers[layers.Count - 1];
        }

        public int Count()
        {
            return layers.Count;
        }





        public bool IsEmpty()
        {
            return layers.Count == 0;
        }
    }
}
