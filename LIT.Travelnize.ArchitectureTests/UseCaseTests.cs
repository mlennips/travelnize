using LIT.Travelnize.Domain.Auth;
using LIT.Travelnize.Domain.Base;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetArchTest.Rules;
using System.Reflection;

namespace LIT.Travelnize.ArchitectureTests
{
    [TestClass]
    public class UseCaseTests
    {
        private static readonly Assembly DomainAssembly = typeof(Domain.Common.Address).Assembly;
        private static readonly Assembly UseCasesAssembly = typeof(UseCases.DependencyInjection).Assembly;
        private static readonly Assembly SharedAssembly = typeof(LoginDto).Assembly;

        [TestMethod]
        public void Handlers_ShouldHaveDependenciesToDomainAssembly()
        {
            var result = Types.InAssembly(UseCasesAssembly)
                              .That()
                              .HaveNameEndingWith("Handler")
                              .And()
                              .DoNotHaveNameEndingWith("ReportHandler")
                              .Should()
                              .HaveDependencyOn(DomainAssembly.FullName)
                              .GetResult();

            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public void Handlers_ShouldEndWithHandler()
        {
            var result = Types.InAssembly(UseCasesAssembly)
                              .That()
                              .HaveDependencyOn(typeof(IQueryHandler<,>).FullName)
                              .Or()
                              .HaveDependencyOn(typeof(ICommandHandler<>).FullName)
                              .Should()
                              .HaveNameEndingWith("Handler")
                              .GetResult();

            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public void Commands_ShouldEndWithCommand()
        {
            var result = Types.InAssembly(UseCasesAssembly)
                              .That()
                              .ImplementInterface(typeof(ICommand))
                              .Or()
                              .ImplementInterface(typeof(ICommand<>))
                              .Should()
                              .HaveNameEndingWith("Command")
                              .GetResult();

            Assert.IsTrue(result.IsSuccessful);
        }

        [TestMethod]
        public void Queries_ShouldEndWithQuery()
        {
            var result = Types.InAssembly(UseCasesAssembly)
                              .That()
                              .ImplementInterface(typeof(IQuery<>))
                              .Should()
                              .HaveNameEndingWith("Query")
                              .GetResult();

            Assert.IsTrue(result.IsSuccessful);
        }

        //[TestMethod]
        //public void DTOs_ShouldEndWithDtoOrResponse()
        //{
        //    var result = Types.InAssembly(UseCasesAssembly)
        //                      .That()
        //                      .ImplementInterface(typeof(IDTO))
        //                      .Should()
        //                      .HaveNameEndingWith("DTO")
        //                      .Or()
        //                      .HaveNameEndingWith("Response")
        //                      .GetResult();

        //    Assert.IsTrue(result.IsSuccessful);
        //}
    }
}
