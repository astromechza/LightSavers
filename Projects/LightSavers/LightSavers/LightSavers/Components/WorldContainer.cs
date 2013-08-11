using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components
{
    public class WorldContainer
    {

        


        private List<WorldSection> sections;

        public WorldContainer()
        {
            sections = new List<WorldSection>();
        }

        public void Update()
        {
            foreach (WorldSection s in sections) s.Update();
        }

        public void Draw(float fromx, float tox)
        {
            int fx = (int)(fromx / WorldSection.SECTION_WIDTH);
            int tx = (int)(tox / WorldSection.SECTION_WIDTH);

            for (int x = fx; x < tx; x++) sections[x].Draw();
        }

        


    }


    public class WorldSection
    {
        public const int SECTION_WIDTH = 64;
        public const int SECTION_LENGTH = 64;

        float sectionx;

        HashSet<GameObject> contained;

        List<GameObject>[,] grid;

        public WorldSection()
        {
            contained = new HashSet<GameObject>();
            grid = new List<GameObject>[SECTION_LENGTH,SECTION_WIDTH];
        }

        public void Draw()
        {
            foreach (GameObject go in contained) go.Draw();
        }

        public void Update()
        {
            foreach (GameObject go in contained) go.Update();
        }

        public void Remove(GameObject go)
        {
            contained.Remove(go);

            RectangleF r = go.getBoundRect();
            r.setX(r.x - sectionx);

            for (int y = (int)r.y; y < r.y2; y++)
            {
                for (int x = (int)r.x; x < r.x2; x++)
                {
                    grid[y, x].Remove(go);
                }
            }
        }

        public void Add(GameObject go)
        {
            contained.Add(go);

            RectangleF r = go.getBoundRect();
            r.setX(r.x - sectionx);

            for (int y = (int)r.y; y < r.y2; y++)
            {
                for (int x = (int)r.x; x < r.x2; x++)
                {
                    grid[y, x].Add(go);
                }
            }
        }



    }

}
