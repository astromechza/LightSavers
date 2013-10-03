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
    }
}
