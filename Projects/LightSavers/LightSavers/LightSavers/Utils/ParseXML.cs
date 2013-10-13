using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace LightSavers.Utils
{
    class ParseXML
    {
        public String filename;
        public int[] settings; 
        public int[] main;

        public ParseXML(String filename, int mainToggles, int settingToggles)
        {
            this.filename = filename;
            settings = new int[settingToggles];
            settings = new int[mainToggles];
            this.parse();
        }

        public void parse()
        {
            using (XmlReader reader = XmlReader.Create("perls.xml"))
	        {
	            while (reader.Read())
	            {
		        // Only detect start elements.
		            if (reader.IsStartElement())
		            {
		                // Get element name and switch on it.
		                switch (reader.Name)
		                {
			            case "toggle":
			                // Detect this element.
			                Console.WriteLine("Start <toggle> element.");
			                break;
			            case "main":
			                // Detect this article element.
			                Console.WriteLine("Start <main> element.");
			                // Search for the attribute name on this current node.
			                if (reader.Read())
			                {
				                
			                }
			                break;
		                }
		            }
	            }
	        }
        }

        public void WriteXML()
        {
            
        }
    }
}
