using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace SqlStatementBuilder.Interfaces
{
    public interface ISqlStatementBuilder
    {
        string SelectStatement { get; }
        string InsertStatement { get; }
        string DeleteStatement { get; }
        string CreateTableStatement { get; }
        string TableName { get; set; }
        Type TableType { get; set; }
        IDictionary<string, (Type, object)> Columns { get; set; }

        void StartCondition();
        void EndCondition();
        void AddBinaryOperatorToCondition(ExpressionType expressionType);
        void AddStringToCondition(string stringToBeAdded);
        void AddIntToCondition(int intToBeAdded);
        void AddColumnToCondition(string columnToBeAdded);

    }
}
