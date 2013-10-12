using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.MenuObjects
{
    public class Submenu
    {

        public List<BaseMenuItem> items;
        public int selected = 0;

        public Submenu()
        {
            items = new List<BaseMenuItem>();
            selected = 0;
        }

        public void AddItem(BaseMenuItem i)
        {
            items.Add(i);
        }

        public void Draw(SpriteBatch canvas, int x, int y)
        {
            for (int i = 0; i < items.Count; i++)
            {
                items[i].Draw(canvas, x, y + i * 40, i == selected);
            }
        }

    }
}
