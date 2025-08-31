using NetArchTest.Rules;
using System.Reflection;
using LIT.Travelnize.Domain.Base;

namespace LIT.Travelnize.ArchitectureTests
{
    [TestClass]
    public class DomainTests
    {
        private static readonly Assembly DomainAssembly = typeof(Domain.DependencyInjection).Assembly;

        [TestMethod]
        public void IdTypes_ShouldHaveDependencyToEntityId()
        {
            var result = Types.InAssembly(DomainAssembly)
                .That()
                .HaveNameEndingWith("Id")
                .Should()
                .HaveDependencyOn(typeof(Guid).FullName)
                .GetResult();

            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public void Specifications_ShouldEndWithSpec()
        {
            var result = Types.InAssembly(DomainAssembly)
                              .That()
                              .AreNotInterfaces()
                              .And()
                              .HaveDependencyOn(typeof(ISpecification<>).FullName)
                              .Should()
                              .HaveNameEndingWith("Spec")
                              .Or()
                              .HaveNameMatching("Spec`")                              
                              .GetResult();

            Assert.IsTrue(result.IsSuccessful);
        }
    }
}
