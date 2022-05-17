using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LemonTree.Pipeline.Tools.Database
{
    public interface IEADatabase
    {
        void SetModel(string model);

        bool Compact(string source, string destination);

        int RunSQLnonQuery(string sql);

        long RunSQLQueryScalar(string sql);
        string GetExtension();
        string GetWildcard();
    }
}
