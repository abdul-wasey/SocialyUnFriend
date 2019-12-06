using Polly;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SocialyUnFriend.LocalDB
{
    
    internal class SqliteDb : BaseSqlite, ISqliteDb
    {
        public async Task<int> SaveAsync<T>(T modelObj)
        {
            var databaseConnection = await GetDatabaseConnection<T>().ConfigureAwait(false);

            return await AttemptAndRetry(() => databaseConnection.InsertAsync(modelObj)).ConfigureAwait(false);
        }

        public async Task<int> SaveAllDataAsync<T>(List<T> listOfModel)
        {
            try
            {
                var databaseConnection = await GetDatabaseConnection<T>().ConfigureAwait(false);

                return await AttemptAndRetry(() => databaseConnection.InsertAllAsync(listOfModel)).ConfigureAwait(false);
            }
            catch (SQLiteException e) when (e.Result is SQLite3.Result.Constraint)
            {
                int count = 0;

                foreach (var model in listOfModel)
                {
                    await SaveAsync(model).ConfigureAwait(false);
                    count++;
                }

                return count;
            }
        }

        public async Task<T> GetAsync<T>(object pk) where T : new ()
        {
            var databaseConnection = await GetDatabaseConnection<T>().ConfigureAwait(false);

            var model = await AttemptAndRetry(() => databaseConnection.GetAsync<T>(pk)).ConfigureAwait(false);

            return model;
        }

        public async Task<List<T>> GetAllDataAsync<T>() where T : new ()
        {
            var databaseConnection = await GetDatabaseConnection<T>().ConfigureAwait(false);

            var listOfData = await AttemptAndRetry(() => databaseConnection.Table<T>().ToListAsync()).ConfigureAwait(false);

            return listOfData;
        }

        public async Task<int> DeleteAsync<T>(T objectToDelete)
        {
            var databaseConnection = await GetDatabaseConnection<T>().ConfigureAwait(false);

            return await AttemptAndRetry(() => databaseConnection.DeleteAsync<T>(objectToDelete)).ConfigureAwait(false);
        }
        public async Task<int> DeleteAllDataAsync<T>()
        {
            var databaseConnection = await GetDatabaseConnection<T>().ConfigureAwait(false);

            return await AttemptAndRetry(() => databaseConnection.DeleteAllAsync<T>()).ConfigureAwait(false);
        }


    }
}
