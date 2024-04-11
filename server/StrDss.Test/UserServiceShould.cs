using Moq;
using StrDss.Data.Repositories;
using StrDss.Data;
using StrDss.Model.UserDtos;
using StrDss.Model;
using AutoFixture.Xunit2;
using StrDss.Service;
using Xunit;
using StrDss.Model.OrganizationDtos;
using StrDss.Common;
using Microsoft.EntityFrameworkCore;
using StrDss.Data.Entities;

namespace StrDss.Test
{
    public class UserServiceShould
    {
        //[Theory]
        //[AutoDomainData]
        //public async Task CreateAccessRequestAsync_ValidDto_NoErrors(
        //    AccessRequestCreateDto dto,
        //    UserDto userDto,
        //    OrganizationDto organizationDto,
        //    [Frozen] Mock<IUserRepository> userRepoMock,
        //    [Frozen] Mock<ICurrentUser> currentUserMock,
        //    [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
        //    [Frozen] Mock<IOrganizationRepository> organizationRepoMock,
        //    [Frozen] Mock<IEmailMessageService> emailServiceMock,
        //    [Frozen] Mock<DssDbContext> dbContextMock,
        //    UserService sut)
        //{
        //    // Arrange
        //    SetupCurrentUser(currentUserMock);
        //    SetupUserRepository(userRepoMock, (UserDto)null);
        //    SetupUnitOfWork(unitOfWorkMock, dbContextMock);
        //    SetupOrganizationRepository(organizationRepoMock, organizationDto);

        //    // Act
        //    var result = await sut.CreateAccessRequestAsync(dto);

        //    // Assert
        //    Assert.Empty(result);
        //    unitOfWorkMock.Verify(x => x.Commit(), Times.AtLeastOnce);
        //}

        [Theory]
        [AutoDomainData]
        public async Task ValidateAccessRequestCreateDtoAsync_AccessRequestPending_ReturnsError(
            AccessRequestCreateDto dto,
            UserDto userDto,
            [Frozen] Mock<IUserRepository> userRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UserService sut)
        {
            // Arrange
            userDto.AccessRequestStatusCd = AccessRequestStatuses.Requested;
            userRepoMock.Setup(x => x.GetUserByGuid(It.IsAny<Guid>())).ReturnsAsync(userDto);

            // Act
            var errors = await sut.CreateAccessRequestAsync(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("entity"));
            Assert.Equal("Your access request is pending", errors["entity"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAccessRequestCreateDtoAsync_AccessDisabled_ReturnsError(
            AccessRequestCreateDto dto,
            UserDto userDto,
            [Frozen] Mock<IUserRepository> userRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UserService sut)
        {
            // Arrange
            userDto.AccessRequestStatusCd = AccessRequestStatuses.Approved;
            userDto.IsEnabled = false;
            userRepoMock.Setup(x => x.GetUserByGuid(It.IsAny<Guid>())).ReturnsAsync(userDto);

            // Act
            var errors = await sut.CreateAccessRequestAsync(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("entity"));
            Assert.Equal("Your access has been disabled", errors["entity"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAccessRequestCreateDtoAsync_AccessAlreadyApproved_ReturnsError(
            AccessRequestCreateDto dto,
            UserDto userDto,
            [Frozen] Mock<IUserRepository> userRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UserService sut)
        {
            // Arrange
            userDto.AccessRequestStatusCd = AccessRequestStatuses.Approved;
            userRepoMock.Setup(x => x.GetUserByGuid(It.IsAny<Guid>())).ReturnsAsync(userDto);

            // Act
            var errors = await sut.CreateAccessRequestAsync(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("entity"));
            Assert.Equal("Your access has been already approved", errors["entity"].First());
        }


        [Theory]
        [AutoDomainData]
        public async Task ValidateAccessRequestCreateDtoAsync_InvalidOrganizationType_ReturnsError(
            AccessRequestCreateDto dto,
            UserDto userDto,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UserService sut)
        {
            // Arrange
            orgRepoMock.Setup(x => x.GetOrganizationTypesAsnc()).ReturnsAsync(new List<OrganizationTypeDto>());

            // Act
            var errors = await sut.CreateAccessRequestAsync(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("organizationType"));
            Assert.Equal("Organization type is not valid", errors["organizationType"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ValidateAccessRequestCreateDtoAsync_EmptyOrganizationName_ReturnsError(
            AccessRequestCreateDto dto,
            UserDto userDto,
            OrganizationDto organizationDto,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UserService sut)
        {
            // Arrange
            SetupOrganizationRepository(orgRepoMock, organizationDto);
            dto.OrganizationName = "";

            // Act
            var errors = await sut.CreateAccessRequestAsync(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("organizationName"));
            Assert.Equal("Organization name is mandatory", errors["organizationName"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task DenyAccessRequest_UserNotFound_ReturnsError(
            AccessRequestDenyDto dto,
            [Frozen] Mock<IUserRepository> userRepoMock,
            UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync((UserDto)null);

            // Act
            var errors = await sut.DenyAccessRequest(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("entity"));
            Assert.Equal($"Access request ({dto.UserIdentityId}) doesn't exist", errors["entity"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task DenyAccessRequest_RequestNotPending_ReturnsError(
            AccessRequestDenyDto dto,
            UserDto userDto,
            [Frozen] Mock<IUserRepository> userRepoMock,
            UserService sut)
        {
            // Arrange
            userDto.AccessRequestStatusCd = AccessRequestStatuses.Approved;
            userRepoMock.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(userDto);

            // Act
            var errors = await sut.DenyAccessRequest(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("entity"));
            Assert.Equal($"Unable to deny access request. The request is currently in status '{userDto.AccessRequestStatusCd}', which does not allow denial.", errors["entity"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task DenyAccessRequest_UserEmailEmpty_ReturnsError(
            AccessRequestDenyDto dto,
            UserDto userDto,
            [Frozen] Mock<IUserRepository> userRepoMock,
            UserService sut)
        {
            // Arrange
            userDto.EmailAddressDsc = "";
            userRepoMock.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(userDto);

            // Act
            var errors = await sut.DenyAccessRequest(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("entity"));
            Assert.Equal("The user doesn't have email address.", errors["entity"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ApproveAccessRequest_UserNotFound_ReturnsError(
            AccessRequestApproveDto dto,
            [Frozen] Mock<IUserRepository> userRepoMock,
            UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync((UserDto)null);

            // Act
            var errors = await sut.ApproveAccessRequest(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("entity"));
            Assert.Equal($"Access request ({dto.UserIdentityId}) doesn't exist", errors["entity"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ApproveAccessRequest_RequestNotPending_ReturnsError(
            AccessRequestApproveDto dto,
            UserDto userDto,
            [Frozen] Mock<IUserRepository> userRepoMock,
            UserService sut)
        {
            // Arrange
            userDto.AccessRequestStatusCd = AccessRequestStatuses.Approved;
            userRepoMock.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(userDto);

            // Act
            var errors = await sut.ApproveAccessRequest(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("entity"));
            Assert.Equal($"Unable to approve access request. The request is currently in status '{userDto.AccessRequestStatusCd}', which does not allow approval.", errors["entity"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ApproveAccessRequest_OrganizationNotFound_ReturnsError(
            AccessRequestApproveDto dto,
            UserDto userDto,
            [Frozen] Mock<IUserRepository> userRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UserService sut)
        {
            // Arrange
            userRepoMock.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(userDto);
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(It.IsAny<long>())).ReturnsAsync((OrganizationDto)null);

            // Act
            var errors = await sut.ApproveAccessRequest(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("representedByOrganizationId"));
            Assert.Equal($"Organization ({dto.RepresentedByOrganizationId}) doesn't exist", errors["representedByOrganizationId"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task ApproveAccessRequest_NonIdirWithBcGovOrgType_ReturnsError(
            AccessRequestApproveDto dto,
            UserDto userDto,
            OrganizationDto orgDto,
            [Frozen] Mock<IUserRepository> userRepoMock,
            [Frozen] Mock<IOrganizationRepository> orgRepoMock,
            UserService sut)
        {
            // Arrange
            userDto.IdentityProviderNm = "NonIdirProvider";
            userRepoMock.Setup(x => x.GetUserById(It.IsAny<long>())).ReturnsAsync(userDto);
            orgDto.OrganizationType = OrganizationTypes.BCGov;
            orgRepoMock.Setup(x => x.GetOrganizationByIdAsync(It.IsAny<long>())).ReturnsAsync(orgDto);

            // Act
            var errors = await sut.ApproveAccessRequest(dto);

            // Assert
            Assert.Single(errors);
            Assert.True(errors.ContainsKey("representedByOrganizationId"));
            Assert.Equal($"Not IDIR account cannot be associated with {OrganizationTypes.BCGov} type organization", errors["representedByOrganizationId"].First());
        }


        public void SetupCurrentUser(Mock<ICurrentUser> currentUserMock)
        {
            currentUserMock.Setup(x => x.UserGuid).Returns(Guid.NewGuid());
            currentUserMock.Setup(x => x.DisplayName).Returns("Test User");
            currentUserMock.Setup(x => x.IdentityProviderNm).Returns("Test Provider");
            currentUserMock.Setup(x => x.FirstName).Returns("Test");
            currentUserMock.Setup(x => x.LastName).Returns("User");
            currentUserMock.Setup(x => x.EmailAddress).Returns("test@example.com");
            currentUserMock.Setup(x => x.BusinessNm).Returns("Test Business");
        }

        public void SetupUserRepository(Mock<IUserRepository> userRepoMock, UserDto userDto)
        {
            userRepoMock.Setup(x => x.GetUserByGuid(It.IsAny<Guid>())).ReturnsAsync(userDto);
            userRepoMock.Setup(x => x.CreateUserAsync(It.IsAny<UserCreateDto>())).Returns(Task.CompletedTask);
            userRepoMock.Setup(x => x.GetAdminUsers()).ReturnsAsync(new List<UserDto> { userDto });
        }

        public void SetupUnitOfWork(Mock<IUnitOfWork> unitOfWorkMock, Mock<DssDbContext> dbContextMock)
        {
            unitOfWorkMock.Setup(x => x.Commit()).Verifiable();
        }

        public void SetupOrganizationRepository(Mock<IOrganizationRepository> organizationRepoMock, OrganizationDto organizationDto)
        {
            organizationRepoMock.Setup(x => x.GetOrganizationTypesAsnc()).ReturnsAsync(new List<OrganizationTypeDto>
            {
                new OrganizationTypeDto { OrganizationType = "BCGov", OrganizationTypeNm = "BC Government Component" },
                new OrganizationTypeDto { OrganizationType = "Platform", OrganizationTypeNm = "Short Term Rental Platform" },
                new OrganizationTypeDto { OrganizationType = "LG", OrganizationTypeNm = "Local Government" },
            });

            organizationRepoMock.Setup(x => x.GetOrganizationsAsync(It.IsAny<string>())).ReturnsAsync(new List<OrganizationDto>
            {
                organizationDto,
            });

            organizationRepoMock.Setup(x => x.GetOrganizationByIdAsync(It.IsAny<long>())).ReturnsAsync((long id) =>
            {
                return organizationDto;
            });
        }

    }
}
