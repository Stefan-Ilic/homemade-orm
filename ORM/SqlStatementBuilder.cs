using System;
using System.Collections.Generic;
using System.Text;
using Interfaces;

namespace ORM
{
    public class SqlStatementBuilder : ISqlStatementBuilder
    {
        public string Statement
        {
            get
            {
                switch (StatementType)
                {
                    case SqlStatementType.Select:
                        return $"SELECT {string.Join(",", ColumnNames)} FROM {TableName}";
                    default: throw new Exception();
                }
            }
        }

        public SqlStatementType StatementType { get; set; }
        public string TableName { get; set; }
        public IList<string> ColumnNames { get; set; }
    }
}
