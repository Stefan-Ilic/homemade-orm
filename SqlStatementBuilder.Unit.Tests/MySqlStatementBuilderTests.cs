using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Shouldly;
using SqlStatementBuilder.Interfaces;
using Xunit;

namespace SqlStatementBuilder.Unit.Tests
{
    public class MySqlStatementBuilderTests
    {
        [Fact]
        public void InsertStatement_CorrectInput_CorrectStatement()
        {
            ISqlStatementBuilder builder = new MySqlStatementBuilder
            {
                TableName = "people",
                Columns = new Dictionary<string, (Type, object)>
                {
                    {"Id", (typeof(int), 1)},
                    {"FirstName", (typeof(string), "Mike")},
                    {"LastName", (typeof(string), "Rosoft")},
                    {"Age", (typeof(int), 1337)}
                }
            };
            const string expected = "INSERT INTO people (FirstName,LastName,Age) VALUES ('Mike','Rosoft',1337)";
            builder.InsertStatement.ShouldBe(expected);
        }

        [Fact]
        public void UpdateStatement_CorrectInput_CorrectStatement()
        {
            ISqlStatementBuilder builder = new MySqlStatementBuilder
            {
                TableName = "people",
                Columns = new Dictionary<string, (Type, object)>
                {
                    {"Id", (typeof(int), 1)},
                    {"FirstName", (typeof(string), "Mike")},
                    {"LastName", (typeof(string), "Rosoft")},
                    {"Age", (typeof(int), 1337)}
                }
            };
            const string expected = "UPDATE people SET FirstName='Mike',LastName='Rosoft',Age=1337 WHERE Id=1";
            builder.UpdateStatement.ShouldBe(expected);
        }

        [Fact]
        public void DeleteStatement_CorrectInput_CorrectStatement()
        {
            ISqlStatementBuilder builder = new MySqlStatementBuilder
            {
                TableName = "people",
                Columns = new Dictionary<string, (Type, object)>
                {
                    {"Id", (typeof(int), 1)},
                    {"FirstName", (typeof(string), "Mike")},
                    {"LastName", (typeof(string), "Rosoft")},
                    {"Age", (typeof(int), 1337)}
                }
            };
            const string expected = "DELETE FROM people WHERE Id=1 AND FirstName='Mike' AND LastName='Rosoft' AND Age=1337";
            builder.DeleteStatement.ShouldBe(expected);
        }

        [Fact]
        public void SelectStatement_CorrectInput_CorrectStatement()
        {
            ISqlStatementBuilder builder = new MySqlStatementBuilder
            {
                TableName = "people",
                Columns = new Dictionary<string, (Type, object)>
                {
                    {"Id", (typeof(int), 1)},
                    {"FirstName", (typeof(string), "Mike")},
                    {"LastName", (typeof(string), "Rosoft")},
                    {"Age", (typeof(int), 1337)}
                }
            };
            const string expected = "SELECT Id,FirstName,LastName,Age FROM people";
            builder.SelectStatement.ShouldBe(expected);
        }

        [Fact]
        public void SelectStatement_CorrectWhereClause_CorrectStatement()
        {
            ISqlStatementBuilder builder = new MySqlStatementBuilder
            {
                TableName = "people",
                Columns = new Dictionary<string, (Type, object)>
                {
                    {"Id", (typeof(int), 1)},
                    {"FirstName", (typeof(string), "Mike")},
                    {"LastName", (typeof(string), "Rosoft")},
                    {"Age", (typeof(int), 1337)}
                }
            };

            builder.StartCondition();
            builder.AddColumnToCondition("Id");
            builder.AddBinaryOperatorToCondition(ExpressionType.Equal);
            builder.AddIntToCondition(1);
            builder.EndCondition();
            builder.StartCondition();
            builder.AddColumnToCondition("FirstName");
            builder.AddBinaryOperatorToCondition(ExpressionType.Equal);
            builder.AddStringToCondition("Mike");
            builder.EndCondition();
            builder.StartCondition();
            builder.AddColumnToCondition("LastName");
            builder.AddBinaryOperatorToCondition(ExpressionType.Equal);
            builder.AddStringToCondition("Rosoft");
            builder.EndCondition();
            builder.StartCondition();
            builder.AddColumnToCondition("Age");
            builder.AddBinaryOperatorToCondition(ExpressionType.Equal);
            builder.AddIntToCondition(1337);
            builder.EndCondition();

            const string expected = "SELECT Id,FirstName,LastName,Age FROM people WHERE (Id = 1) AND (FirstName = 'Mike') AND (LastName = 'Rosoft') AND (Age = 1337)";
            builder.SelectStatement.ShouldBe(expected);
        }
    }
}
