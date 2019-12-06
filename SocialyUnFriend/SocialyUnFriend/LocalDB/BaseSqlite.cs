using Polly;
using SocialyUnFriend.Common;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SocialyUnFriend.LocalDB
{

    internal abstract class BaseSqlite
    {
        /// <summary>
        /// Lazy initialization of an object means that its creation is deferred until it is first used.
        /// Because File IO operations and creating a database can be expensive 
        /// (i.e., it requires many CPU cycles and can take longer than expected) 
        /// we don't want to initialize our database until we need it. 
        /// This avoids creating our database when the app launches, keeping our app's launch time to a minimum.
        /// </summary>

        protected static readonly Lazy<SQLiteAsyncConnection> _databaseConnectionHolder =
            new Lazy<SQLiteAsyncConnection>
            (
                () => new SQLiteAsyncConnection(Constants.dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache)
            );

        protected static SQLiteAsyncConnection DatabaseConnection => _databaseConnectionHolder.Value;

        protected static async ValueTask<SQLiteAsyncConnection> GetDatabaseConnection<T>()
        {
            if (!DatabaseConnection.TableMappings.Any(x => x.MappedType.Name == typeof(T).Name))
            {
                await DatabaseConnection
                    .EnableWriteAheadLoggingAsync() //faster/more-efficient database writes
                    .ConfigureAwait(false); // avoid unnecessarily returning to the Main Thread

                await DatabaseConnection
                    .CreateTablesAsync(CreateFlags.None, typeof(T)).ConfigureAwait(false);
            }

            return DatabaseConnection;
        }

        protected static Task<T> AttemptAndRetry<T>(Func<Task<T>> action, int numRetries = 3)
        {
            // Polly Policy, Used to attempt database actions and automatically retry if unsuccessful
            return Policy
                .Handle<SQLiteException>()
                .WaitAndRetryAsync(numRetries, PollyRetryAttempt)
                .ExecuteAsync(action);

            //call back function,

            TimeSpan PollyRetryAttempt(int attemptNumber) => TimeSpan.FromSeconds(Math.Pow(2, attemptNumber));
        }
    }
}


