using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.MenuObjects
{
    public class TransitionItem : BaseMenuItem
    {

        private String label;
        private int destination;

        public TransitionItem(String label, int submenu)
        {
            this.label = label;
            this.destination = submenu;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch canvas, int x, int y, bool selected)
        {
            canvas.DrawString(AssetLoader.fnt_assetloadscreen, label, new Vector2(x, y), (selected)?Color.White:Color.Gray);
        }

    }
}
