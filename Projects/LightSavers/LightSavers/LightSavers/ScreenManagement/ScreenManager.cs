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

        public ScreenManager() 
        {
            layers = new List<ScreenLayer>();
        }

        public void Update(GameTime gameTime)
        {
            if (layers.Count > 0)
            {
                layers[layers.Count - 1].Update(gameTime);
                if (layers[layers.Count - 1].mustExit)
                {
                    pop();
                }
            }
        }

        // *** Draw ***
        // Draw the layers from the lowest non-transparent layer upwards 
        public void Draw(GameTime gameTime)
        {
            if (lowestVisibleLayer == -1) return;

            for (int i = lowestVisibleLayer; i < layers.Count; i++)
            {
                layers[i].Draw(gameTime);
            }
        }

        // *** Push ***
        // Push a new layer onto the list
        public void push(ScreenLayer layer)
        {
            layers.Add(layer);

            // If the added layer is not transparent, it must be the first layer drawn
            if (!layer.isTransparent || lowestVisibleLayer == -1)
            {
                lowestVisibleLayer = layers.Count - 1;
            }
        }

        // *** Pop ***
        // Remove and return the topmost layer
        public ScreenLayer pop()
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

        public ScreenLayer top()
        {
            return layers[layers.Count - 1];
        }



        public bool empty()
        {
            return layers.Count == 0;
        }
    }
}
