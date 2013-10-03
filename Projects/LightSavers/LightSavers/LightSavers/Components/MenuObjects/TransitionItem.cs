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

    }
}
