using AutoFixture.Xunit2;
using Moq;
using StrDss.Data.Repositories;
using StrDss.Data;
using StrDss.Model.UserDtos;
using StrDss.Service;
using Xunit;
using StrDss.Common;

namespace StrDss.Test
{
    public class RoleServiceShould
    {
        [Theory]
        [AutoDomainData]
        public async Task GetRolesAsync_ReturnsRoleList(
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            RoleService sut,
            List<RoleDto> expectedRoles)
        {
            // Arrange
            roleRepoMock.Setup(x => x.GetRolesAync()).ReturnsAsync(expectedRoles);

            // Act
            var result = await sut.GetRolesAync();

            // Assert
            Assert.Equal(expectedRoles, result);
        }

        [Theory]
        [AutoDomainData]
        public async Task GetRoleAsync_ExistingRole_ReturnsRoleDto(
            string roleCd,
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            RoleService sut,
            RoleDto expectedRole)
        {
            // Arrange
            roleRepoMock.Setup(x => x.GetRoleAync(roleCd)).ReturnsAsync(expectedRole);

            // Act
            var result = await sut.GetRoleAync(roleCd);

            // Assert
            Assert.Equal(expectedRole, result);
        }

        [Theory]
        [AutoDomainData]
        public async Task GetRoleAsync_NonExistingRole_ReturnsNull(
            string roleCd,
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            RoleService sut)
        {
            // Arrange
            roleRepoMock.Setup(x => x.GetRoleAync(roleCd)).ReturnsAsync((RoleDto)null);

            // Act
            var result = await sut.GetRoleAync(roleCd);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [AutoDomainData]
        public async Task GetPermissionsAsync_ReturnsPermissionList(
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            RoleService sut,
            List<PermissionDto> expectedPermissions)
        {
            // Arrange
            roleRepoMock.Setup(x => x.GetPermissionsAync()).ReturnsAsync(expectedPermissions);

            // Act
            var result = await sut.GetPermissionsAync();

            // Assert
            Assert.Equal(expectedPermissions, result);
        }

        [Theory]
        [AutoDomainData]
        public async Task CreateRoleAsync_ValidDto_ReturnsNoErrors(
            RoleUpdateDto dto,
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
            [Frozen] Mock<IFieldValidatorService> validatorMock,
            RoleService sut)
        {
            // Arrange
            validatorMock.Setup(x => x.Validate(Entities.Role, dto, It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<int>(), It.IsAny<string[]>()))
                .Returns(new Dictionary<string, List<string>>());
            roleRepoMock.Setup(x => x.DoesRoleCdExist(dto.UserRoleCd)).ReturnsAsync(false);
            roleRepoMock.Setup(x => x.CountActivePermissionIdsAsnyc(dto.Permissions)).ReturnsAsync(dto.Permissions.Count);

            // Act
            var result = await sut.CreateRoleAsync(dto);

            // Assert
            Assert.Empty(result);
            roleRepoMock.Verify(x => x.CreateRoleAsync(dto), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Theory]
        [AutoDomainData]
        public async Task CreateRoleAsync_RoleCdExists_ReturnsRoleCdError(
            RoleUpdateDto dto,
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            RoleService sut)
        {
            // Arrange
            roleRepoMock.Setup(x => x.DoesRoleCdExist(dto.UserRoleCd)).ReturnsAsync(true);

            // Act
            var result = await sut.CreateRoleAsync(dto);

            // Assert
            Assert.Contains("userRoleCd", result.Keys);
            Assert.Single(result["userRoleCd"]);
            Assert.Equal($"Role Code [{dto.UserRoleCd}] already exists.", result["userRoleCd"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task CreateRoleAsync_InvalidPermissions_ReturnsPermissionError(
            RoleUpdateDto dto,
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            [Frozen] Mock<IFieldValidatorService> validatorMock,
            RoleService sut)
        {
            // Arrange
            validatorMock.Setup(x => x.Validate(Entities.Role, dto, It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<int>(), It.IsAny<string[]>()))
                .Returns(new Dictionary<string, List<string>>());
            roleRepoMock.Setup(x => x.DoesRoleCdExist(dto.UserRoleCd)).ReturnsAsync(false);
            roleRepoMock.Setup(x => x.CountActivePermissionIdsAsnyc(dto.Permissions)).ReturnsAsync(0);

            // Act
            var result = await sut.CreateRoleAsync(dto);

            // Assert
            Assert.Contains("permission", result.Keys);
            Assert.Single(result["permission"]);
            Assert.Equal("Some of the permissions are invalid.", result["permission"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task UpdateRoleAsync_ValidDto_ReturnsNoErrors(
            RoleUpdateDto dto,
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
            [Frozen] Mock<IFieldValidatorService> validatorMock,
            RoleService sut)
        {
            // Arrange
            validatorMock.Setup(x => x.Validate(Entities.Role, dto, It.IsAny<Dictionary<string, List<string>>>(), It.IsAny<int>(), It.IsAny<string[]>()))
                .Returns(new Dictionary<string, List<string>>());
            roleRepoMock.Setup(x => x.DoesRoleCdExist(dto.UserRoleCd)).ReturnsAsync(true);
            roleRepoMock.Setup(x => x.CountActivePermissionIdsAsnyc(dto.Permissions)).ReturnsAsync(dto.Permissions.Count);

            // Act
            var result = await sut.UpdateRoleAsync(dto);

            // Assert
            Assert.Empty(result);
            roleRepoMock.Verify(x => x.UpdateRoleAsync(dto), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Theory]
        [AutoDomainData]
        public async Task UpdateRoleAsync_RoleCdNotFound_ReturnsRoleCdError(
            RoleUpdateDto dto,
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            RoleService sut)
        {
            // Arrange
            roleRepoMock.Setup(x => x.DoesRoleCdExist(dto.UserRoleCd)).ReturnsAsync(false);

            // Act
            var result = await sut.UpdateRoleAsync(dto);

            // Assert
            Assert.Contains("userRoleCd", result.Keys);
            Assert.Single(result["userRoleCd"]);
            Assert.Equal($"Role Code [{dto.UserRoleCd}] not found.", result["userRoleCd"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task DeleteRoleAsync_ValidRoleCd_ReturnsNoErrors(
            string roleCd,
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            [Frozen] Mock<IUnitOfWork> unitOfWorkMock,
            RoleService sut,
            RoleDto role)
        {
            // Arrange
            roleRepoMock.Setup(x => x.GetRoleAync(roleCd)).ReturnsAsync(role);
            role.IsReferenced = false;

            // Act
            var result = await sut.DeleteRoleAsync(roleCd);

            // Assert
            Assert.Empty(result);
            roleRepoMock.Verify(x => x.DeleteRoleAsync(roleCd), Times.Once);
            unitOfWorkMock.Verify(x => x.Commit(), Times.Once);
        }

        [Theory]
        [AutoDomainData]
        public async Task DeleteRoleAsync_RoleCdNotFound_ReturnsRoleCdError(
            string roleCd,
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            RoleService sut)
        {
            // Arrange
            roleRepoMock.Setup(x => x.GetRoleAync(roleCd)).ReturnsAsync((RoleDto)null);

            // Act
            var result = await sut.DeleteRoleAsync(roleCd);

            // Assert
            Assert.Contains("userRoleCd", result.Keys);
            Assert.Single(result["userRoleCd"]);
            Assert.Equal($"Role Code [{roleCd}] not found.", result["userRoleCd"].First());
        }

        [Theory]
        [AutoDomainData]
        public async Task DeleteRoleAsync_RoleCdReferenced_ReturnsRoleCdError(
            string roleCd,
            [Frozen] Mock<IRoleRepository> roleRepoMock,
            RoleService sut)
        {
            // Arrange
            var role = new RoleDto { UserRoleCd = roleCd, IsReferenced = true };
            roleRepoMock.Setup(x => x.GetRoleAync(roleCd)).ReturnsAsync(role);

            // Act
            var result = await sut.DeleteRoleAsync(roleCd);

            // Assert
            Assert.Contains("userRoleCd", result.Keys);
            Assert.Single(result["userRoleCd"]);
            Assert.Equal($"Role Code [{roleCd}] is assigned to users and cannot be deleted.", result["userRoleCd"].First());
        }
    }

}
