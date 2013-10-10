using LightSavers.Utils;
using LightSavers.Utils.Geometry;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects.Aliens
{
    public class AlienSpawner<AlienType> where AlienType : BaseAlien, new()
    {
        private const int MIN_RANGE_FROM_PLAYERS = 10;

        private int maxPopulation;
        private int majorInterval;
        private int intervalVariance;

        private DateTime nextSpawnAttempt;

        private RealGame game;

        private List<AlienType> population;

        public AlienSpawner(RealGame game, int maxPopulation, int majorInterval, int intervalVariance)
        {
            this.game = game;
            this.maxPopulation = maxPopulation;
            this.majorInterval = majorInterval;
            this.intervalVariance = intervalVariance;

            this.population = new List<AlienType>();

            GenNextSpawn();
        }

        public void GenNextSpawn()
        {
            int rms = majorInterval + Globals.random.Next(intervalVariance * 2) - intervalVariance;
            nextSpawnAttempt = DateTime.Now + new TimeSpan(0, 0, 0, 0, majorInterval);
        }

        public void UpdateAliens(float ms)
        {
            foreach (AlienType a in this.population) a.Update(ms);
        }

        public void Update(float sm)
        {
            if (DateTime.Now > nextSpawnAttempt)
            {
                if (this.population.Count < this.maxPopulation)
                {
                    Spawn();
                    GenNextSpawn();
                }
            }
        }

        public void Spawn()
        {
            float minx = Int32.MaxValue;
            float maxx = Int32.MinValue;

            //loop through active players
            for (int i = 0; i < game.players.Length; i++)
            {
                PlayerObject p = game.players[i];
                if (p.Position.X > maxx) maxx = p.Position.X;
                if (p.Position.X < minx) minx = p.Position.X;
            }

            //minx -= 4;
            maxx += 32;
            float range = maxx - minx;

            int attempts = 0;
            while ((attempts++)<200)
            {


                Vector3 r = new Vector3(
                    (float)Globals.random.NextDouble() * range + minx,
                    0,
                    (float)Globals.random.NextDouble() * 30 + 1                    
                    );

                // create box
                RectangleF rr = new RectangleF(r.X - 0.5f, r.Z - 0.5f, 1.0f, 1.0f);
                
                // check collision
                if (game.cellCollider.RectangleCollides(rr)) continue;

                // check range
                PlayerObject p = game.GetClosestPlayer(r);
                float d = (p.Position - r).LengthSquared();
                if (d < MIN_RANGE_FROM_PLAYERS*MIN_RANGE_FROM_PLAYERS) continue;

                AlienType a = new AlienType();
                a.Construct(this.game, r, (float)Math.Pow(Globals.random.NextDouble(),3) * MathHelper.TwoPi);
                population.Add(a);

                // yay!
                break;
            }
        }

    }
}
