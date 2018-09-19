using Shouldly;
using Xunit;

namespace UnitTests
{
  public class UnitTest1
  {
    [Fact]
    public void Empty_Must_Succeed()
    {
      (true).ShouldBeTrue();
    }
  }
}
