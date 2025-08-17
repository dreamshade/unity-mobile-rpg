using System.Threading.Tasks;
using Dreamshade.SaveSystem.Data;
namespace Dreamshade.SaveSystem.Services
{
    public interface ISaveService
    {
        Task<SaveData> LoadAsync(string playerId);
        Task SaveAsync(SaveData data);
    }
}