using System;
using DatabaseDriver.Interfaces;
using Shouldly;
using Xunit;

namespace DatabaseDriver.Unit.Tests
{
    public class MySqlDriverTests
    {
        [Fact]
        public void RunInsertStatement_IsNotInsert_ThrowsException()
        {
            IDatabaseDriver driver = new MySqlDriver("");
            Should.Throw<Exception>(() => driver.RunInsertStatement(""));
        }

        [Fact]
        public void RunUpdateStatement_IsNotUpdate_ThrowsException()
        {
            IDatabaseDriver driver = new MySqlDriver("");
            Should.Throw<Exception>(() => driver.RunUpdateStatement(""));
        }

        [Fact]
        public void RunDeleteStatement_IsNotDelete_ThrowsException()
        {
            IDatabaseDriver driver = new MySqlDriver("");
            Should.Throw<Exception>(() => driver.RunDeleteStatement(""));
        }
    }
}
