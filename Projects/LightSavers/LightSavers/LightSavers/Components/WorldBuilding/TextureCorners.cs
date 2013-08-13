using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LightSavers.Components.WorldBuilding
{
    public class TextureCorners
    {
        public Vector2 topleft;
        public Vector2 topright;
        public Vector2 bottomleft;
        public Vector2 bottomright;

        public static TextureCorners Build(Vector2 topleft, Vector2 bottomright)
        {
            TextureCorners tc = new TextureCorners();
            tc.topleft = new Vector2(topleft.X, topleft.Y);
            tc.bottomright = new Vector2(bottomright.X, bottomright.Y);
            tc.topright = new Vector2( bottomright.X, topleft.Y);
            tc.bottomleft = new Vector2(topleft.X, bottomright.Y);
            return tc;
        }

        public static Tuple<TextureCorners, float> BuildProb(Vector2 topleft, Vector2 bottomright, float prob)
        {
            return new Tuple<TextureCorners,float>(Build(topleft, bottomright), prob);
        }

    }
}
