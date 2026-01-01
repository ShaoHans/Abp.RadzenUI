using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;

namespace Abp.RadzenUI.Application.Contracts.Organizations;

public interface IOrganizationUnitAppService
    : ICrudAppService<
        OrganizationUnitDto,
        Guid,
        OrganizationUnitGetListInput,
        OrganizationUnitCreateDto,
        OrganizationUnitUpdateDto
    >
{
    Task<ListResultDto<OrganizationUnitDto>> GetAllAsync();

    Task<OrganizationUnitDto> GetLastChildOrNullAsync(Guid? parentId);

    Task MoveAsync(Guid id, OrganizationUnitMoveDto input);

    Task<ListResultDto<OrganizationUnitDto>> GetRootAsync(bool recursive = false);

    Task<ListResultDto<OrganizationUnitDto>> FindChildrenAsync(
        OrganizationUnitGetChildrenDto input
    );

    Task<ListResultDto<string>> GetRoleNamesAsync(Guid id);

    Task<PagedResultDto<IdentityRoleDto>> GetUnaddedRolesAsync(
        Guid id,
        OrganizationUnitGetUnaddedRoleListInput input
    );

    Task<PagedResultDto<IdentityRoleDto>> GetRolesAsync(
        Guid id,
        PagedAndSortedResultRequestDto input
    );

    Task AddRolesAsync(Guid id, OrganizationUnitAddRoleDto input);

    Task<PagedResultDto<IdentityUserDto>> GetUnaddedUsersAsync(
        Guid id,
        OrganizationUnitGetUnaddedUserListInput input
    );

    Task<PagedResultDto<IdentityUserDto>> GetUsersAsync(Guid id, GetIdentityUsersInput input);

    Task AddUsersAsync(Guid id, OrganizationUnitAddUserDto input);

    Task DeleteRoleAsync(Guid organizationUnitId, Guid roleId);

    Task DeleteUserAsync(Guid organizationUnitId, Guid userId);
}
