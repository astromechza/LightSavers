using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LightSavers.Components.GameObjects
{
    /// <summary>
    /// Animations and their ranges are specified here: to be updated
    /// </summary>
    public static class Animation_States
    {
        public static Dictionary<int, int> upperCharacterBones;
        public static Dictionary<int, int> lowerCharacterBonesandRoot;

        public const int idle = 0, run = 1, shoot = 2, assault = 4, snipshot = 8, pistol = 16, sword = 32, walk = 64, death = -1;

        public const int idle_assault = idle + assault;
        public const int walk_assault = walk + assault;
        public const int run_assault = run + assault;
        public const int walk_assault_shoot = walk + assault + shoot;
        public const int run_assault_shoot = run + assault + shoot;
        public const int idle_assault_shoot = idle + assault + shoot;
        public const int idle_snipshot = idle + snipshot;
        public const int walk_snipshot = walk + snipshot;
        public const int run_snipshot = run + snipshot;
        public const int walk_snipshot_shoot = walk + snipshot + shoot;
        public const int run_snipshot_shoot = run + snipshot + shoot;
        public const int idle_snipshot_shoot = idle + snipshot + shoot;
        public const int idle_pistol = idle + pistol;
        public const int walk_pistol = walk + pistol;
        public const int run_pistol = run + pistol;
        public const int walk_pistol_shoot = walk + pistol + shoot;
        public const int run_pistol_shoot = run + pistol + shoot;
        public const int idle_pistol_shoot = idle + pistol + shoot;
        public const int idle_sword = idle + sword;
        public const int walk_sword = walk + sword;
        public const int run_sword = run + sword;
        public const int walk_sword_shoot = walk + sword + shoot;
        public const int run_sword_shoot = run + sword + shoot;
        public const int idle_sword_shoot = idle + sword + shoot;

        public const int moving = 2;
        public const int attacking = 4;

        public const int attacking_melee = 4;
        public const int attacking_range = 8;

        public const int charging = 8;
        public const int impact = 16;

        public static int[] characterAnimationsList = new int[] {idle_assault, walk_assault, run_assault, walk_assault_shoot,run_assault_shoot, idle_assault_shoot,
                                                                idle_snipshot, walk_snipshot, run_snipshot, walk_snipshot_shoot, run_snipshot_shoot, idle_snipshot_shoot,
                                                                idle_pistol, walk_pistol, run_pistol, walk_pistol_shoot, run_pistol_shoot, idle_pistol_shoot,
                                                                idle_sword, walk_sword, run_sword, walk_sword_shoot, run_sword_shoot, idle_sword_shoot,
                                                                death};
        public static int[] characterAnimationKeys = new int[] { 
            0, 48,          //idle_assault
            49, 76,         //walk_assault
            77, 97,         //run_assault
            98, 125,        //walk_assault_shoot
            126, 146,       //run_assault_shoot
            147, 173,       //idle_assault_shoot
            174, 221,       //idle_snipshot
            222, 249,       //walk_snipshot
            250, 270,       //run_snipshot
            271, 298,       //walk_snipshot_shoot
            308, 319,       //run_snipshot_shoot
            328, 367,       //idle_snipshot_shoot
            368, 415,       //idle_sword
            146, 443,       //walk_pistol
            444, 464,       //run_pistol
            465, 492,       //walk_pistol_shoot
            497, 513,       //run_pistol_shoot
            515, 525,       //idle_pistol_shoot
            562, 609,       //idle_sword
            610, 637,       //walk_sword
            638, 658,       //run_sword
            659, 686,       //walk_sword_shoot
            689, 714,       //run_sword_shoot
            718, 748,       //idle_sword_shoot
            749, 769        //death
        };

        public static int[] alien01AnimationsList = new int[] { idle, moving, attacking, death };
        public static int[] alien01AnimationKeys = new int[] { 0, 125, 126, 150, 151, 200, 201, 225 };

        public static int[] alien02AnimationsList = new int[] { idle, moving, attacking, death };
        public static int[] alien02AnimationKeys = new int[] { 0, 125, 126, 150, 151, 200, 201, 225 };

        public static int[] alien03AnimationsList = new int[] { idle, moving, attacking_melee, attacking_range, death };
        public static int[] alien03AnimationKeys = new int[] { 0, 125, 126, 150, 151, 175, 176, 200, 201, 225 };

        public static int[] alien04AnimationsList = new int[] { idle, moving, charging, impact, attacking, death };
        public static int[] alien04AnimationKeys = new int[] { 0, 125, 126, 150, 151, 175, 176, 200, 201, 225, 226, 300 };
    }
}
