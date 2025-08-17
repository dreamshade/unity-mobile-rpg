// Assets/scripts/items/RecruitClass.cs
using System;

namespace Dreamshade.Characters
{
    /// Flags lets a recruit belong to multiple classes (e.g., Warrior|Rogue).
    [Flags]
    public enum RecruitClass
    {
        None    = 0,
        Warrior = 1 << 0,
        Thief   = 1 << 1,
        Mage    = 1 << 2,
        Cleric  = 1 << 3,
        Ranger  = 1 << 4,
        Paladin = 1 << 5,
        // add more as needed
        Any     = ~0 // helper if you ever need “everyone”
    }
}
