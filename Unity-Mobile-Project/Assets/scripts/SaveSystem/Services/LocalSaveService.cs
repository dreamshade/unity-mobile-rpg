using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Dreamshade.SaveSystem.Data;
namespace Dreamshade.SaveSystem.Services
{
    public class LocalSaveService : ISaveService
    {
        private const string FilePrefix = "save_";
        private const string FileExt = ".json";
        private string GetPath(string playerId)
        {
            string fileName = FilePrefix + playerId + FileExt;
            return Path.Combine(Application.persistentDataPath, fileName);
        }
        public async Task<SaveData> LoadAsync(string playerId)
        {
            string path = GetPath(playerId);
            if (!File.Exists(path))
            return null; // signal: no save yet
            try
            {
                // File IO off main thread for good habits
                string json = await Task.Run(() => File.ReadAllText(path));
                // JsonUtility requires [Serializable] classes with fields
                SaveData data = JsonUtility.FromJson<SaveData>(json);
                return data;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LocalSaveService] Failed to load save at {path}: {ex.Message}\n{ex.StackTrace}");
                return null; // treat as missing/corrupt -> caller can create default
            }
        }
        public async Task SaveAsync(SaveData data)
        {
            if (data == null || string.IsNullOrEmpty(data.playerId))
            {
                Debug.LogError("[LocalSaveService] Refusing to save: data or playerId is null.");
                return;
            }
            string path = GetPath(data.playerId);
            try
            {
                string json = JsonUtility.ToJson(data, prettyPrint: true);
                // Ensure directory exists (paranoia; persistentDataPath exists by default)
                Directory.CreateDirectory(Path.GetDirectoryName(path));
                await Task.Run(() => File.WriteAllText(path, json));
                #if UNITY_EDITOR
                Debug.Log($"[LocalSaveService] Saved to {path}\n{json}");
                #endif
            }
            catch (Exception ex)
            {
                Debug.LogError($"[LocalSaveService] Failed to save at {path} - {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
