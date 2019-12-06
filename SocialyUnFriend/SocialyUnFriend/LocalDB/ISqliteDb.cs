using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialyUnFriend.LocalDB
{
    public interface ISqliteDb
    {
        Task<int> SaveAsync<T>(T model);


        Task<int> SaveAllDataAsync<T>(List<T> listOfModel);


        Task<T> GetAsync<T>(object pk) where T : new();  


        Task<List<T>> GetAllDataAsync<T>() where T : new();

        Task<int> DeleteAsync<T>(T objectToDelete);

        Task<int> DeleteAllDataAsync<T>();
    }
}
