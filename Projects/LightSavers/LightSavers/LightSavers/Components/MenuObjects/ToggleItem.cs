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
    }
}
