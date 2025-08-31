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
        private static readonly Assembly ApiAssembly = typeof(API.Utils.ApiServiceExtensions).Assembly;
        private static readonly Assembly SharedAssembly = typeof(Shared.Auth.LoginDto).Assembly;
        private static readonly Assembly UIAssembly = typeof(Travelnize._Imports).Assembly;

        [TestMethod]
        public void DomainLayer_ShouldNotHaveDependencyOn()
        {
            // Arrange  
            var notIn = new[] { UseCasesAssembly, InfrastructureAssembly, ApiAssembly, UIAssembly };

            // Act  
            var result = Types.InAssembly(DomainAssembly)
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
            var notIn = new[] { ApiAssembly, UseCasesAssembly, UIAssembly };

            // Act  
            var result = Types.InAssembly(InfrastructureAssembly)
                .ShouldNot()
                .HaveDependencyOnAll(notIn.Select(x => x.FullName).ToArray())
                .GetResult();

            // Assert  
            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public void UseCasesLayer_ShouldNotHaveDependencyOn()
        {
            // Arrange
            var notIn = new[] { InfrastructureAssembly, ApiAssembly, UIAssembly };

            // Act
            var result = Types.InAssembly(UseCasesAssembly)
                .ShouldNot()
                .HaveDependencyOnAll(notIn.Select(x => x.FullName).ToArray())
                .GetResult();

            // Assert
            Assert.IsTrue(result.IsSuccessful);
        }

        //[TestMethod]
        //public void ApiLayer_ShouldNotHaveDependencyOn()
        //{
        //    // Arrange
        //    var notIn = new[] { UIAssembly };

        //    // Act
        //    var result = Types.InAssembly(ApiAssembly)
        //        .ShouldNot()
        //        .HaveDependencyOnAll(notIn.Select(x => x.FullName).ToArray())
        //        .GetResult();

        //    // Assert
        //    Assert.IsTrue(result.IsSuccessful);
        //}

        [TestMethod]
        public void SharedLayer_ShouldNotHaveDependencyOn()
        {
            // Arrange
            var notIn = new[] { DomainAssembly, UseCasesAssembly, InfrastructureAssembly, ApiAssembly, UIAssembly };
            // Act
            var result = Types.InAssembly(SharedAssembly)
                .ShouldNot()
                .HaveDependencyOnAll(notIn.Select(x => x.FullName).ToArray())
                .GetResult();
            // Assert
            Assert.IsTrue(result.IsSuccessful);
        }
    }
}