using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetArchTest.Rules;
using System.Reflection;

namespace LIT.Travelnize.ArchitectureTests
{
    [TestClass]
    public class LayerTests
    {
        private static readonly Assembly DomainAssembly = typeof(Domain.DependencyInjection).Assembly;
        private static readonly Assembly UseCasesAssembly = typeof(UseCases.DependencyInjection).Assembly;
        private static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.DependencyInjection).Assembly;
        private static readonly Assembly ApiAssembly = typeof(API.Endpoints.Auth).Assembly;

        [TestMethod]
        public void DomainLayer_ShouldNotHaveDependencyOn()
        {
            // Arrange  
            var notIn = new[] { UseCasesAssembly, InfrastructureAssembly, ApiAssembly };

            // Act  
            var result = Types.InAssembly(DomainAssembly)
                .ShouldNot()
                .HaveDependencyOnAll(notIn.Select(x => x.FullName).ToArray())
                .GetResult();

            // Assert  
            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public void ApplicationLayer_ShouldNotHaveDependencyOn()
        {
            // Arrange  
            var notIn = new[] { InfrastructureAssembly, ApiAssembly };

            // Act  
            var result = Types.InAssembly(InfrastructureAssembly)
                .ShouldNot()
                .HaveDependencyOnAll(notIn.Select(x => x.FullName).ToArray())
                .GetResult();

            // Assert  
            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public void InfrastructureLayer_ShouldNotHaveDependencyOn()
        {
            // Arrange  
            var notIn = new[] { ApiAssembly };

            // Act  
            var result = Types.InAssembly(InfrastructureAssembly)
                .ShouldNot()
                .HaveDependencyOnAll(notIn.Select(x => x.FullName).ToArray())
                .GetResult();

            // Assert  
            Assert.IsTrue(result.IsSuccessful);
        }
    }
}