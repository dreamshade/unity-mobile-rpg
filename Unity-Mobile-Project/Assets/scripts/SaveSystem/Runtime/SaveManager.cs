using System;
using System.Threading.Tasks;
using UnityEngine;
using Dreamshade.SaveSystem.Data;
using Dreamshade.SaveSystem.Id;
using Dreamshade.SaveSystem.Services;
using Dreamshade.SaveSystem.Defaults;
namespace Dreamshade.SaveSystem.Runtime
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }
        public SaveData CurrentSave { get; private set; }
        public bool IsLoaded => CurrentSave != null;
        public event Action<SaveData> OnSaveLoaded;
        public event Action<SaveData> OnSaveWritten;
        private ISaveService _service;
        private string _playerId;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
                Instance = this;
                DontDestroyOnLoad(gameObject);
        }

        /// Call once at startup. You can pass a different ISaveService later (e.g., RemoteSaveService).

        public void Initialize(ISaveService service = null, string playerId = null)
        {
            _service = service ?? new LocalSaveService();
            _playerId = string.IsNullOrEmpty(playerId) ?
            PlayerIdProvider.GetOrCreatePlayerId() : playerId;
        }
        /// Loads save if present; otherwise creates and saves a default save.
        public async Task LoadOrCreateAsync()
        {
            if (_service == null)
            Initialize(); // be forgiving: auto‑init local if not set
            var loaded = await _service.LoadAsync(_playerId);
            if (loaded == null)
            {
                loaded = DefaultSaveFactory.Create(_playerId);
                await _service.SaveAsync(loaded);
            }
            CurrentSave = loaded;
            OnSaveLoaded?.Invoke(CurrentSave);
        }

        /// Saves the current in‑memory data.

        public async Task SaveAsync()
        {
            if (!IsLoaded) return;
            await _service.SaveAsync(CurrentSave);
            OnSaveWritten?.Invoke(CurrentSave);
        }
    }
}
