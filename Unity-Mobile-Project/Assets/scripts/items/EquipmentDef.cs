// Assets/scripts/items/EquipmentDef.cs
using UnityEngine;
using Dreamshade.Characters;
namespace Dreamshade.Items
{
    /// Child of Item that can be equipped.
    [CreateAssetMenu(menuName = "Dreamshade/Items/Equipment", fileName = "NewEquipment")]
    public class EquipmentDef : ItemDef
    {
        [Header("Equip Rules")]
        public RecruitClass ClassRestrictions = RecruitClass.None; // None = anyone can equip
        public EquipmentSlot EquipSlot = EquipmentSlot.Main;

        [Header("Combat")]
        [Min(1)] public int DelayIncrements = 10; // your "delay" as battle increments
        [Min(0)] public int Damage = 0;           // your "damage" (base power for now)

        /// Returns true if the recruit's classes match this item's allowed classes.
        /// If ClassRestrictions == None, we treat it as "no restriction".
        public bool CanEquip(RecruitClass recruitClasses)
        {
            if (ClassRestrictions == RecruitClass.None) return true;
            return (ClassRestrictions & recruitClasses) != 0;
        }
    }
}
