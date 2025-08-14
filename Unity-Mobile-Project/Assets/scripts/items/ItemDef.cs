// Assets/scripts/items/ItemDef.cs
using UnityEngine;

namespace Dreamshade.Items
{
    /// Base definition for any item (consumable, material, equipment, etc.).
    /// ScriptableObject so you can author assets in the editor.
    [CreateAssetMenu(menuName = "Dreamshade/Items/Item", fileName = "NewItem")]
    public class ItemDef : ScriptableObject
    {
        [Header("Identity")]
        [SerializeField] private string id;
        public string Id => string.IsNullOrEmpty(id) ? name : id;
        public string Description = "Placeholder Description";
        public string DisplayName = "New Item";
        [Header("Trading")]
        public bool Sellable = true;   // your "sellable"
        public bool Stackable = false; // your "stackable"
        [Min(0)] public int SellPrice = 0; // your "sell_price" (int for now)

        // (Later: icon, rarity, description, max stack, tags, etc.)
    }
}
