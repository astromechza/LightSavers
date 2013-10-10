using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    /// <summary>
    /// Animations and their ranges are specified here: to be updated
    /// </summary>
    struct Animation_States
    {
        static int idle = 0, run =1, shoot = 2, assault = 4, snipshot =8, pistol = 16, sword = 32, walk = 64, death = -1;

        static const int idle_assault = idle + assault;
        static const int walk_assault = walk+assault;
        static const int run_assault = run+assault;
        static const int walk_assault_shoot = walk+assault+shoot;
        static const int run_assault_shoot = run + assault + shoot;
        static const int idle_assault_shoot = idle + assault+shoot;
        static const int idle_snipshot = idle+ snipshot;
        static const int walk_snipshot = walk+snipshot;
        static const int run_snipshot = run+snipshot;
        static const int walk_snipshot_shoot = walk+snipshot+shoot;
        static const int run_snipshot_shoot = run+snipshot+shoot;
        static const int idle_snipshot_shoot = idle+snipshot + shoot;
        static const int idle_pistol = idle+pistol;
        static const int walk_pistol = walk + pistol;
        static const int run_pistol = run + pistol;
        static const int walk_pistol_shoot = walk+pistol+shoot;
        static const int run_pistol_shoot = run + pistol+shoot;
        static const int idle_pistol_shoot = idle+pistol+shoot;
        static const int idle_sword = idle+sword;
        static const int walk_sword = walk + sword;
        static const int run_sword = run + sword;
        static const int walk_sword_shoot = walk + sword+shoot;
        static const int run_sword_shoot = run + sword + shoot;
        static const int idle_sword_shoot = idle + sword + shoot;

        public static string[] characterAnimationsList = new string[] {"idle_assault", "walk_assault", "run_assault", "walk_assault_shoot","run_assault_shoot", "idle_assault_shoot",
                                                                "idle_snipshot", "walk_snipshot", "run_snipshot", "walk_snipshot_shoot", "run_snipshot_shoot", "idle_snipshot_shoot",
                                                                "idle_pistol", "walk_pistol", "run_pistol", "walk_pistol_shoot", "run_pistol_shoot", "idle_pistol_shoot",
                                                                "idle_sword", "walk_sword", "run_sword", "walk_sword_shoot", "run_sword_shoot", "idle_sword_shoot",
                                                                "death_1"};
        public static int[] characterAnimationKeys = new int[] { 0, 48, 49, 76, 77, 97, 98, 125, 126, 146, 147, 173, 174, 221, 222, 249, 250, 270, 271, 298, 299, 319, 320, 367, 368, 415, 146, 443, 444, 464, 465, 492, 493, 513, 514, 561, 562, 609, 610, 637, 638, 658, 659, 686, 689, 707, 708, 748, 749, 769 };

        public static string[] alien01AnimationsList = new string[] { "idle", "moving", "attacking", "death" };
        public static int[] alien01AnimationKeys = new int[] { 0, 125, 126, 150, 151, 200, 201, 225 };

        public static string[] alien02AnimationsList = new string[] { "idle", "moving", "attacking", "death" };
        public static int[] alien02AnimationKeys = new int[] { 0, 125, 126, 150, 151, 200, 201, 225 };

        public static string[] alien03AnimationsList = new string[] { "idle", "moving", "attacking_melee", "attacking_range", "death" };
        public static int[] alien03AnimationKeys = new int[] { 0, 125, 126, 150, 151, 175, 176, 200, 201, 225 };

        public static string[] alien04AnimationsList = new string[] { "idle", "moving", "charging", "impact", "attacking", "death" };
        public static int[] alien04AnimationKeys = new int[] { 0, 125, 126, 150, 151, 175, 176, 200, 201, 225, 226, 300 };
    }
}
