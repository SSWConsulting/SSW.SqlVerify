using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;
using System.Data.SqlServerCe;
using System.IO;
using SSW.SqlVerify.Core.SqlCe;

namespace SSW.SqlVerify.Core.Test.Integration
{
    [TestClass]
    public class IntegrationTests
    {

        const string TEMP_FILENAME = "\\data\\nw40_temp.sdf";
        const string SDF_FILE = "\\data\\nw40.sdf";

        private static string AppPath
        {
            get { return System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location); }
        }



        [ClassInitialize]
        public static void ClassInitialize(TestContext ctx)
        {
             // make a temporary copy of the data file
             if (File.Exists(AppPath+TEMP_FILENAME)) File.Delete(AppPath+TEMP_FILENAME);
             File.Copy(AppPath+SDF_FILE, AppPath+TEMP_FILENAME);
        }



        
        [ClassCleanup]
        public static void ClassCleanup()
        {
            // delete temporary copy
            if (File.Exists(AppPath+TEMP_FILENAME)) File.Delete(AppPath+TEMP_FILENAME);
        }



        [TestMethod]
        public void Test01SqlConnection()
        {
            
            var connString = "Data Source=" + AppPath + TEMP_FILENAME +"; Persist Security Info=False;";
            var config = new SqlVerifyConfiguration(new SqlCeConnectionFactory(connString));

            using (var conn = config.ConnectionFactory.GetConnection())
            {
                conn.Open();
                Assert.IsTrue(conn.State == ConnectionState.Open, "Sql Connection failed to open");
            }

        }



        [TestMethod]
        public void Test02NoChangeSameHash()
        {
            var connString = "Data Source=" + AppPath + TEMP_FILENAME +"; Persist Security Info=False;";
            var SqlVerify = new SqlVerify(new SqlVerifyConfiguration(new SqlCeConnectionFactory(connString)));

            var hash = SqlVerify.buildSchemaHash();
            var hash2 = SqlVerify.buildSchemaHash();

            Assert.AreEqual(hash, hash2, "differing hash with no SQL change");

        }




        [TestMethod]
        public void Test03ChangeSchemaChangeHash()
        {
            var connString = "Data Source=" + AppPath + TEMP_FILENAME + "; Persist Security Info=False;";
            var SqlVerify = new SqlVerify(new SqlVerifyConfiguration(new SqlCeConnectionFactory(connString)));

            var hash = SqlVerify.buildSchemaHash();

            // make a change to the schema
            using (var conn = SqlVerify.Configuration.ConnectionFactory.GetConnection())
            {
                conn.Open();
                var command  = conn.CreateCommand();
                command.CommandText = "ALTER TABLE [Customers] ADD COLUMN [Mobile] NVARCHAR NULL;";
                command.ExecuteNonQuery();
            }

            var hash2 = SqlVerify.buildSchemaHash();

            Assert.AreNotEqual(hash, hash2, "hash values equal after schema change");
        }

        


       

    }
}
