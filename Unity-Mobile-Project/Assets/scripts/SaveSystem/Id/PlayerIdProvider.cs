using UnityEngine;
namespace Dreamshade.SaveSystem.Id
{
    public static class PlayerIdProvider
    {
        private const string Key = "PLAYER_ID";
        public static string GetOrCreatePlayerId()
        {
            if (!PlayerPrefs.HasKey(Key))
            {
                string newId = System.Guid.NewGuid().ToString();
                PlayerPrefs.SetString(Key, newId);
                PlayerPrefs.Save();
            }
            return PlayerPrefs.GetString(Key);
        }
    }
}
