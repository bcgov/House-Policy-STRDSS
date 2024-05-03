using Configuration;

namespace ConfigurationUnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            //arrange
            AppSettings appSettings = new AppSettings();
            
            //act
            var result = appSettings.GetUser("TestPassword");

            //assert

            Assert.NotNull(result);
        }
    }
}