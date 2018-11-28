using System;
using System.Collections.Generic;
using System.Text;

namespace DatabaseDriver.Interfaces
{
    public interface IDatabaseDriver
    {
        void RunStatementOnDatabase(string statement);
    }
}
