using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NORSU.EncodeMe.Network;
using SQLite;

namespace NORSU.EncodeMe
{
    internal static class Db
    {
        public static async Task<List<Student>> GetStudentsAsync()
        {
            var con = Connection();
            await con.CreateTableAsync<Student>();
            return await con.Table<Student>().ToListAsync();

        }

        private static SQLiteAsyncConnection Connection(string db = "encodeMe.db3")
        {
            return new SQLiteAsyncConnection(Path.Combine(Location, db));
        }
        
        public static string Location { get; } = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

        public static async void CreateDatabases()
        {
            await Connection().CreateTableAsync<Student>();
        }

        public static async Task<List<T>> GetAll<T>(string db = "encodeMe.db3") where T : new ()
        {
            await Connection(db).CreateTableAsync<T>();
            return await Connection(db).Table<T>().ToListAsync();
        }

        public static Task Save<T>(T model, string db="encodeMe.db3") where T : new()
        {
            return Connection(db).CreateTableAsync<T>()
                .ContinueWith(t=>Connection(db).InsertOrReplaceAsync(model));
        }

        public static Task InsertAllAsync<T>(List<T> items,string db="encodeMe.db3") where T:new()
        {
            return Connection(db).CreateTableAsync<T>()
                .ContinueWith(t => Connection(db).InsertAllAsync(items));
        }

        public static Task DropTable<T>(string db="encodeMe.db3") where T :new()
        {
            return Connection(db).DropTableAsync<T>();
        }

        //public static async Task<T> Get<T>(Func<T,bool> predicate, string db = "encodeMe.db3") where T:new()
        //{
        //    await Connection(db).CreateTableAsync<T>();
        //    return await Connection(db)
        //}
    }
}