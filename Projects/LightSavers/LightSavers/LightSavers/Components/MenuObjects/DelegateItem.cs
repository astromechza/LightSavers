using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LightSavers.ScreenManagement;

namespace LightSavers.Components.MenuObjects
{
    public class DelegateItem : BaseMenuItem
    {
        public delegate bool MenuItemDelegate();

        public String label;
        public NoArgCallback function;
        private Color selectedCol;
        private Color notSelectedCol;

        public DelegateItem(String label, NoArgCallback function, Color selected, Color notSelected)
        {
            this.label = label;
            this.function = function;
            this.selectedCol = selected;
            this.notSelectedCol = notSelected;
        }

        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch canvas, int x, int y, bool selected)
        {
            if (selected)
            {
                canvas.Draw(AssetLoader.diamond, new Rectangle(x - 50, y + 6, 40, 15), Color.White);
            }
            canvas.DrawString(AssetLoader.fnt_assetloadscreen, label, new Vector2(x, y), (selected) ? selectedCol : notSelectedCol);
        }
    }
}
