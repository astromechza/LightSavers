using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.MenuObjects
{
    public class ToggleItem : BaseMenuItem
    {

        private String label;
        private String[] values;
        private int current;

        public ToggleItem(String label, String[] values)
        {
            this.label = label;
            this.values = values;
            this.current = 0;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch canvas, int x, int y, bool selected)
        {
            if (selected)
            {
                canvas.Draw(AssetLoader.diamond, new Rectangle(x - 50, y + 6, 40, 15), Color.White);
            }
            canvas.DrawString(AssetLoader.fnt_assetloadscreen, label, new Vector2(x, y), (selected) ? Color.White : Color.Gray);
        }
    }
}
