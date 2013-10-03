using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.MenuObjects
{
    public class DelegateItem : BaseMenuItem
    {
        public delegate bool MenuItemDelegate();

        private String label;
        private MenuItemDelegate function;

        public DelegateItem(String label, MenuItemDelegate function)
        {
            this.label = label;
            this.function = function;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch canvas, int x, int y, bool selected)
        {
            canvas.DrawString(AssetLoader.fnt_assetloadscreen, label, new Vector2(x, y), (selected) ? Color.White : Color.Gray);
        }
    }
}
