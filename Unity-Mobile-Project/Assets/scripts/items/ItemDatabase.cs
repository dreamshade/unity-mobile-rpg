// Assets/scripts/items/ItemDatabase.cs
using System.Collections.Generic;
using UnityEngine;

namespace Dreamshade.Items
{
    [CreateAssetMenu(menuName = "Dreamshade/Items/ItemDatabase", fileName = "ItemDatabase")]
    public class ItemDatabase : ScriptableObject
    {
        [SerializeField] private List<ItemDef> items = new();

        private Dictionary<string, ItemDef> _byId;

        public IReadOnlyList<ItemDef> Items => items;

        public void BuildIndex()
        {
            _byId = new Dictionary<string, ItemDef>(items.Count);
            foreach (var it in items)
            {
                if (!it) continue;
                var key = it.Id;
                if (_byId.ContainsKey(key))
                    Debug.LogWarning($"Duplicate Item Id '{key}' ({_byId[key].name} vs {it.name}).");
                else
                    _byId.Add(key, it);
            }
        }

        public bool TryGet(string id, out ItemDef def)
        {
            if (_byId == null) BuildIndex();
            return _byId.TryGetValue(id, out def);
        }

        public bool TryGet<T>(string id, out T def) where T : ItemDef
        {
            def = null;
            if (!TryGet(id, out var any)) return false;
            def = any as T;
            return def != null;
        }
    }
}
