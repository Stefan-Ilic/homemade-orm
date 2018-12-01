using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseDriver.Interfaces
{
    public interface IDatabaseDriver
    {
        int RunInsertStatement(string statement);
        void RunUpdateStatement(string statement);
        void RunDeleteStatement(string statement);
        void RunStatement(string statement);
        IList<IDictionary<string, object>> RunSelectStatement(string statement);
    }
}
