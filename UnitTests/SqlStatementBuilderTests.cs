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
        
        [Fact]
        public void Create_CorrectInput_CorrectStatement()
        {
            var builder = new SqlStatementBuilder(SqlStatementType.Create)
            {
                TableName = "people",
                Columns = new Dictionary<string, Type>
                {
                    {"id", typeof(int)},
                    {"firstname", typeof(string)},
                    {"LastName", typeof(string)},
                    {"lastname", typeof(int)}
                }
            };

            const string expected = @"CREATE TABLE people (id INT PRIMARY KEY,firstname TEXT,lastname TEXT,age INT)";
            builder.Statement.ToLower().ShouldBe(expected.ToLower());
        }
    }
}