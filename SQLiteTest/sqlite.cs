using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;
using System.Data;

namespace SQLiteTest
{
    class Sqlite
    {
        private SQLiteConnectionStringBuilder sqlConnectionSb;

        public Sqlite(string dbname)
        {
            sqlConnectionSb = new SQLiteConnectionStringBuilder { DataSource = dbname };
        }

        public int ExecSQL(string sql)
        {
            int result;
            try
            {
                using (var cn = new SQLiteConnection(sqlConnectionSb.ToString()))
                {
                    cn.Open();
                    using (var cmd = new SQLiteCommand(cn))
                    {
                        //テーブル作成
                        cmd.CommandText = sql;
                        result = cmd.ExecuteNonQuery();
                    }
                }
            }
            catch
            {
                return -1000;
            }
            return result;
        }

        public DataTable PullTable(string sql)
        {
            DataTable reslut=new DataTable();

            using (SQLiteConnection con = new SQLiteConnection(sqlConnectionSb.ToString()))
            using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(sql, con))
            {
                adapter.Fill(reslut);
            }

            return reslut;
        }
    }
}
