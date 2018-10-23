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
        public void CreateTable_CorrectInput_CorrectStatement()
        {
            var builder = new SqlStatementBuilder(SqlStatementType.Create)
            {
                TableName = "people",
                ColumnNamesAndTypes = new Dictionary<string, Type>
                {
                    {"id", typeof(int)},
                    {"firstname", typeof(string)},
                    {"lastName", typeof(string)},
                    {"age", typeof(int)}
                }
            };

            const string expected = @"CREATE TABLE people (id INT PRIMARY KEY,firstname TEXT,lastname TEXT,age INT)";
            builder.Statement.ToLower().ShouldBe(expected.ToLower());
        }

        [Fact]
        public void Insert_CorrectInput_CorrectStatement()
        {
            var person = new Person
            {
                Id = 1,
                FirstName = "Mike",
                LastName = "Rosoft",
                Age = 1337
            };
            var builder = new SqlStatementBuilder(SqlStatementType.Insert)
            {
                TableName = "people",
                ColumnNamesAndValues = new Dictionary<string, object>
                {
                    {"Id", 1},
                    {"FirstName", "Mike"},
                    {"LastName", "Rosoft"},
                    {"Age", 1337}
                }
            };
            const string expected = "INSERT INTO people (Id,FirstName,LastName,Age) VALUES (1,'Mike','Rosoft',1337)";
            builder.Statement.ShouldBe(expected);

        }
    }
}