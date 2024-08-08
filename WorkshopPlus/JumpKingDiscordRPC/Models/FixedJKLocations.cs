using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JumpKingDiscordRPC.Models;
using JumpKingDiscordRPC;

namespace JumpKingDiscordRPC.Models
{
    internal class FixedJKLocations
    {
        internal static Location[] Locations = new Location[]
        {
            // mb
            new Location(1, 5, "LOCATION_REDCROWN_WOODS", "redcrown_woods"),
            new Location(6, 10, "LOCATION_COLOSSAL_DRAIN", "colossal_drain"),
            new Location(11, 14, "LOCATION_FALSE_KINGS_KEEP", "falsekeep"),
            new Location(15, 19, "LOCATION_BARGAINBURG", "bargainburg"),
            new Location(20, 25, "LOCATION_GREAT_FRONTIER", "new_frontier"),
            new Location(26, 26, "LOCATION_WINDSWEPT_BLUFF", "windswept_bluff"),
            new Location(27, 32, "LOCATION_STORMWALL_PASS", "stormwall_pass"),
            new Location(33, 36, "LOCATION_CHAPEL_PERILOUS", "chapel"),
            new Location(37, 39, "LOCATION_BLUE_RUIN", "blue_ruin"),
            new Location(40, 42, "LOCATION_THE_TOWER", "maintower"),
            new Location(43, 43, "Main Babe Screen", "mainbabe"),

            // nbp
            new Location(45, 46, "Room of the Imp", "improom"),
            new Location(47, 52, "LOCATION_BRIGHTCROWN_WOODS", "brightcrown"),
            new Location(53, 58, "LOCATION_COLOSSAL_DUNGEON", "colossal_dungeon"),
            new Location(58, 62, "LOCATION_FALSE_KINGS_CASTLE", "falsecastle"), 
            new Location(63, 70, "LOCATION_UNDERBURG", "underburg"),
            new Location(71, 77, "LOCATION_LOST_FRONTIER", "lost_frontier"),
            new Location(78, 83, "LOCATION_HIDDEN_KINGDOM", "hiddenkingdom"),
            new Location(84, 89, "LOCATION_BLACK_SANCTUM", "black_sanctum"),
            new Location(90, 95, "LOCATION_DEEP_RUIN", "deep_ruin"),
            new Location(96, 99, "LOCATION_THE_DARK_TOWER", "dark_tower"),
            new Location(100, 100, "New Babe+ Screen", "newbabe"),

            // gotb
            new Location(156, 160, "LOCATION_PHILOSOPHERS_FOREST", "philosopher"),
            new Location(161, 164, "Hole", "hole"),
            new Location(102, 108, "LOCATION_BOG", "bog"),
            new Location(109, 116, "LOCATION_MOULDING_MANOR", "manor"),
            new Location(117, 123, "LOCATION_BUGSTALK", "bugstalk"),
            new Location(124, 130, "LOCATION_HOUSE_OF_NINE_LIVES", "tower_of_nine_lives"),
            new Location(131, 139, "LOCATION_THE_PHANTOM_TOWER", "phantom_tower"),
            new Location(140, 147, "LOCATION_HALTED_RUIN", "halted_ruin"),
            new Location(148, 153, "LOCATION_THE_TOWER_OF_ANTUMBRA", "antumbra"),
            new Location(154, 154, "Ghost of The Babe Screen", "ghostbabe")
        };
    }

    public struct Location
    {
        public int Start;
        public int End;
        public string Name;
        public string ImageKey;

        public Location(int start, int end, string resourceName, string imageKey)
        {
            Start = start;
            End = end;
            Name = resourceName;
            ImageKey = imageKey;
        }

        public static Location Empty => new Location { };
    }
}
