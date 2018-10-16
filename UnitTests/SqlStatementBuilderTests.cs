using System;
using System.Collections.Generic;
using System.Text;
using Interfaces;
using ORM;
using Shouldly;
using Xunit;

namespace UnitTests
{
    public class SqlStatementBuilderTests
    {
        [Theory]
        [InlineData(SqlStatementType.Select, "SELECT")]
        [InlineData(SqlStatementType.Alter, "ALTER")]
        [InlineData(SqlStatementType.Drop, "DROP")]
        [InlineData(SqlStatementType.Delete, "DELETE")]
        [InlineData(SqlStatementType.Insert, "INSERT")]
        [InlineData(SqlStatementType.Create, "CREATE")]
        [InlineData(SqlStatementType.Update, "UPDATE")]

        public void StatementType_Equals_FirstWordInStatement(SqlStatementType type, string typeAsString)
        {
            var builder = new SqlStatementBuilder(type);
            builder.Statement.Split(' ')[0].ShouldBe(typeAsString.ToUpper());
        }
    }
}