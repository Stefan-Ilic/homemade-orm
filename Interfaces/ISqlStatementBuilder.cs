using System;
using System.Collections.Generic;
using System.Text;

namespace Interfaces
{
    public interface ISqlStatementBuilder
    {
        string Statement { get; }
        SqlStatementType StatementType { get; set; }
        string TableName { get; set; }
        IDictionary<string, Type> Columns { get; set; }
    }

    public enum SqlStatementType
    {
        Create,
        Alter,
        Drop,
        Delete,
        Insert,
        Select,
        Update,
        TableExists
    }
}
