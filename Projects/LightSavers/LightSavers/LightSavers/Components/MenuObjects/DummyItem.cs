using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.MenuObjects
{
    public class DummyItem : BaseMenuItem
    {
        private String label;

        public DummyItem(String label)
        {
            this.label = label;
        }

    }
}
