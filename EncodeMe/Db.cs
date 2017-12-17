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

        public static SQLiteAsyncConnection Connection()
        {
            var personalFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            return new SQLiteAsyncConnection(Path.Combine(personalFolder, "encodeMe.db3"));
        }

        public static async void CreateDatabases()
        {
            await Connection().CreateTableAsync<Student>();
        }
    }
}