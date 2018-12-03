using System;
using ORM.Attributes;
using Shouldly;
using Xunit;

namespace ORM.Utilities.Unit.Tests
{
    internal class PersonWithId
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    internal class PersonWithPrimaryKey
    {
        [PrimaryKey]
        public int MyPrimaryKey { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
    internal class PersonWithPrimaryKeyAsString
    {
        [PrimaryKey]
        public string MyPrimaryKey { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }

    internal class PersonWithMultiplePrimaryKeys
    {
        [PrimaryKey]
        public string MyPrimaryKey { get; set; }
        public string Name { get; set; }
        [PrimaryKey]
        public int Age { get; set; }
    }
    internal class PersonWithoutIdOrPrimaryKey
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }
    [Table("people")]
    internal class PersonWithTableAndColumnAttribute
    {
        public int Id { get; set; }
        [Column("nomen")]
        public string Name { get; set; }
    }
    public class OrmUtilitiesTests
    {
        [Fact]
        public void GetPrimaryKeyAttribute_Id_ReturnsCorrectProperty()
        {
            var property = OrmUtilities.GetPrimaryKeyProperty(typeof(PersonWithId));
            property.ShouldNotBeNull();
            property.Name.ShouldBe("Id");
            property.PropertyType.ShouldBe(typeof(int));
        }

        [Fact]
        public void GetPrimaryKeyAttribute_PrimaryKey_ReturnsCorrectProperty()
        {
            var property = OrmUtilities.GetPrimaryKeyProperty(typeof(PersonWithPrimaryKey));
            property.ShouldNotBeNull();
            property.Name.ShouldBe("MyPrimaryKey");
            property.PropertyType.ShouldBe(typeof(int));
        }

        [Fact]
        public void GetPrimaryKeyAttribute_PrimaryKeyAsString_ThrowsException()
        {
            Should.Throw<Exception>(
                () => OrmUtilities.GetPrimaryKeyProperty(typeof(PersonWithPrimaryKeyAsString))
            );
        }

        [Fact]
        public void GetPrimaryKeyAttribute_MultiplePrimaryKeys_ThrowsException()
        {
            Should.Throw<Exception>(
                () => OrmUtilities.GetPrimaryKeyProperty(typeof(PersonWithMultiplePrimaryKeys))
            );
        }

        [Fact]
        public void GetTableName_NoTableAttribute_ClassName()
        {
            var tableName = OrmUtilities.GetTableName(typeof(PersonWithId));
            tableName.ShouldBe("PersonWithId");
        }

        [Fact]
        public void GetTableName_TableAttribute_AttributeNameProperty()
        {
            var tableName = OrmUtilities.GetTableName(typeof(PersonWithTableAndColumnAttribute));
            tableName.ShouldBe("people");
        }

        [Fact]
        public void GetColumns_Object_ColumnsNamedCorrectly()
        {
            var person = new PersonWithTableAndColumnAttribute();

            var columns = OrmUtilities.GetColumns(person);

            columns.ContainsKey("Id").ShouldBe(true);
            columns.ContainsKey("nomen").ShouldBe(true);
        }

        [Fact]
        public void GetColumns_Object_ValuesAndTypeCorrect()
        {
            var person = new PersonWithTableAndColumnAttribute
            {
                Id = 1,
                Name = "Stefan"
            };

            var columns = OrmUtilities.GetColumns(person);

            columns["Id"].Item1.ShouldBe(typeof(int));
            columns["Id"].Item2.Equals(1).ShouldBe(true);
            columns["nomen"].Item1.ShouldBe(typeof(string));
            columns["nomen"].Item2.Equals("Stefan").ShouldBe(true);
        }

        [Fact]
        public void GetColumns_Type_ColumnsNamedCorrectly()
        {
            var columns = OrmUtilities.GetColumns(typeof(PersonWithTableAndColumnAttribute));

            columns.ContainsKey("Id").ShouldBe(true);
            columns.ContainsKey("nomen").ShouldBe(true);
        }

        [Fact]
        public void GetColumns_Type_ValuesAndTypeCorrect()
        {

            var columns = OrmUtilities.GetColumns(typeof(PersonWithTableAndColumnAttribute));

            columns["Id"].Item1.ShouldBe(typeof(int));
            columns["Id"].Item2.ShouldBeNull();
            columns["nomen"].Item1.ShouldBe(typeof(string));
            columns["nomen"].Item2.ShouldBeNull();
        }
    }
}
