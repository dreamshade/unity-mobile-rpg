using Dreamshade.SaveSystem.Data;
namespace Dreamshade.SaveSystem.Defaults
{
    public static class DefaultSaveFactory
    {
        public static SaveData Create(string playerId)
        {
            var save = new SaveData
            {
                playerId = playerId,
                version = 1,
            };
            save.roster.Add(new RecruitData{});

            return save;
        }
    }
}
