using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.MenuObjects
{
    public class Submenu
    {

        List<BaseMenuItem> items;

        public Submenu()
        {
            items = new List<BaseMenuItem>();
        }

        public void AddItem(BaseMenuItem i)
        {
            items.Add(i);
        }

    }
}
