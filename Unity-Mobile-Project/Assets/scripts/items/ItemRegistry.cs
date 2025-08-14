// Assets/scripts/items/ItemRegistry.cs
using UnityEngine;

namespace Dreamshade.Items
{
    public class ItemRegistry : MonoBehaviour
    {
        [SerializeField] private ItemDatabase database;

        public static ItemDatabase DB { get; private set; }

        private void Awake()
        {
            if (!database)
            {
                Debug.LogError("ItemRegistry has no ItemDatabase assigned.");
                return;
            }
            database.BuildIndex();
            DB = database;
        }
    }
}
