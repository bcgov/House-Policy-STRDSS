using System.Linq.Expressions;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using StrDss.Common;
using StrDss.Data;
using StrDss.Data.Entities;
using StrDss.Data.Repositories;
using StrDss.Model;
using StrDss.Service;
using Xunit;

namespace StrDss.Test
{
    public class OrganizationServiceShould
    {
        [Theory]
        [InlineData(null)]
        [InlineData("All")]
        [InlineData(OrganizationTypes.Platform)]
        [InlineData(OrganizationTypes.LGSub)]
        [InlineData("missing_type")]
        public async Task GetOrganizationsDropdownAsync_UsesRepositoryProjection(string? type)
        {
            var organizations = new[]
            {
                new DssOrganization { OrganizationId = 42, OrganizationNm = "Alpha", LocalGovernmentType = "City" },
                new DssOrganization { OrganizationId = 99, OrganizationNm = "Beta", LocalGovernmentType = null }
            };
            var repository = new Mock<IOrganizationRepository>(MockBehavior.Strict);
            repository.Setup(x => x.GetOrganizationsAsync(type,
                    It.IsAny<Expression<Func<DssOrganization, DropdownNumDto>>>()))
                .Returns((string? _, Expression<Func<DssOrganization, DropdownNumDto>> projection) =>
                    Task.FromResult(organizations.AsQueryable().Select(projection).ToList()));
            var service = new OrganizationService(Mock.Of<ICurrentUser>(), Mock.Of<IFieldValidatorService>(),
                Mock.Of<IUnitOfWork>(), Mock.Of<IMapper>(), Mock.Of<IHttpContextAccessor>(),
                NullLogger<StrDssLogger>.Instance, repository.Object, Mock.Of<ICodeSetRepository>());

            var actual = await service.GetOrganizationsDropdownAsync(type);

            Assert.Collection(actual,
                item =>
                {
                    Assert.Equal(42, item.Id);
                    Assert.Equal("Alpha", item.Description);
                    Assert.Equal("City", item.LocalGovernmentType);
                },
                item =>
                {
                    Assert.Equal(99, item.Id);
                    Assert.Equal("Beta", item.Description);
                    Assert.Null(item.LocalGovernmentType);
                });
            repository.Verify(x => x.GetOrganizationsAsync(type,
                It.IsAny<Expression<Func<DssOrganization, DropdownNumDto>>>()), Times.Once);
            repository.Verify(x => x.GetOrganizationsAsync(It.IsAny<string?>()), Times.Never);
        }
    }
}
