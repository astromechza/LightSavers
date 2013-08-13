using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.WorldBuilding
{
    public class TextureCornersFactory
    {
        List<Tuple<TextureCorners, float>> tcsets;
        Random random;

        public TextureCornersFactory()
        {
            random = new Random();
            tcsets = new List<Tuple<TextureCorners, float>>();
        }

        public void Add(Tuple<TextureCorners, float> tc)
        {
            tcsets.Add(tc);
        }

        public TextureCorners Get()
        {
            float target = (float)random.NextDouble();

            float current = 0;
            foreach (Tuple<TextureCorners, float> tcset in tcsets)
            {
                if ((current + tcset.Item2) > target)
                {
                    return tcset.Item1;
                }
                else
                {
                    current += tcset.Item2;
                }
            }
            return tcsets[tcsets.Count - 1].Item1;
        }


        
    }
}
