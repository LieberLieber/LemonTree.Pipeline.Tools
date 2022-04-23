using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemovePrerenderedDiagrams
{
    class Program
    {
		private static OleDbConnectionStringBuilder _builder = new OleDbConnectionStringBuilder();

		public static int RunSQLnonQuery(string sql)
		{
			int RecordCount = -1;
			using (var cn = new OleDbConnection { ConnectionString = _builder.ConnectionString })
			{
				using (var cmd = new OleDbCommand { CommandText = sql, Connection = cn })
				{
					cn.Open();
					RecordCount = cmd.ExecuteNonQuery();
				}
			}

			return RecordCount;
		}
		static int Main(string[] args)
        {
			Console.WriteLine("RemovePrerenderedDiagrams is starting");
			try
			{
				if (args.Length != 1)
				{
					Console.WriteLine($"Wrong number of commandline parameters! (1) e.g.:  PWC.eapx");
					return -1;
				}

				string filename = args[0];
				if (!(File.Exists(filename)))
				{
					Console.WriteLine($"{filename} doesn't exist!");
					return -2;
				}

				Console.WriteLine($"RemovePrerenderedDiagrams from {filename}");
				_builder.Provider = "Microsoft.Jet.OLEDB.4.0";
				_builder.DataSource = filename;

				int retVal = RunSQLnonQuery("Delete from t_document where t_document.DocName = 'DIAGRAMIMAGEMAP' ");
				Console.WriteLine($"Removed {retVal} Prerendered Diagrams from {filename}");
				Console.WriteLine("RemovePrerenderedDiagrams is finished");
				return 0;
			}
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occured: {ex.Message}");
                return -9;
            }
}
    }
}
