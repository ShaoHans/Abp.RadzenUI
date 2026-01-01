using Abp.RadzenUI.Application.Contracts.Organizations;
using Abp.RadzenUI.Localization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Identity;
using Volo.Abp.ObjectExtending;

namespace Abp.RadzenUI.Application;

public class OrganizationUnitAppService : ApplicationService, IOrganizationUnitAppService
{
    protected OrganizationUnitManager OrganizationUnitManager { get; }
    protected IOrganizationUnitRepository OrganizationUnitRepository { get; }
    protected IdentityUserManager UserManager { get; }
    protected IIdentityRoleRepository RoleRepository { get; }
    protected IIdentityUserRepository UserRepository { get; }

    public OrganizationUnitAppService(
        IdentityUserManager userManager,
        IIdentityRoleRepository roleRepository,
        IIdentityUserRepository userRepository,
        OrganizationUnitManager organizationUnitManager,
        IOrganizationUnitRepository organizationUnitRepository
    )
    {
        UserManager = userManager;
        RoleRepository = roleRepository;
        UserRepository = userRepository;
        OrganizationUnitManager = organizationUnitManager;
        OrganizationUnitRepository = organizationUnitRepository;
        LocalizationResource = typeof(AbpRadzenUIResource);
    }

    public virtual async Task<OrganizationUnitDto> CreateAsync(OrganizationUnitCreateDto input)
    {
        var organizationUnit = new OrganizationUnit(
            GuidGenerator.Create(),
            input.DisplayName,
            input.ParentId,
            CurrentTenant.Id
        );
        input.MapExtraPropertiesTo(organizationUnit);

        await OrganizationUnitManager.CreateAsync(organizationUnit);

        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(organizationUnit);
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var organizationUnit = await OrganizationUnitRepository.FindAsync(id);
        if (organizationUnit == null)
        {
            return;
        }
        await OrganizationUnitManager.DeleteAsync(id);
    }

    public virtual async Task<ListResultDto<OrganizationUnitDto>> GetRootAsync(
        bool recursive = false
    )
    {
        var rootOrganizationUnits = await OrganizationUnitManager.FindChildrenAsync(
            null,
            recursive
        );

        return new ListResultDto<OrganizationUnitDto>(
            ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(
                rootOrganizationUnits
            )
        );
    }

    public virtual async Task<ListResultDto<OrganizationUnitUsersDto>> GetOrgsUsersAsync(
        bool recursive = false
    )
    {
        var rootOrganizationUnits = await OrganizationUnitManager.FindChildrenAsync(
            null,
            recursive
        );
        var result = new List<OrganizationUnitUsersDto>();
        foreach (var org in rootOrganizationUnits)
        {
            var tmpOrg = ObjectMapper.Map<OrganizationUnit, OrganizationUnitUsersDto>(org);
            var organizationUnitUsers = await OrganizationUnitRepository.GetMembersAsync(
                org,
                null,
                int.MaxValue,
                0,
                null
            );
            tmpOrg.Users = ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(
                organizationUnitUsers
            );
            result.Add(tmpOrg);
        }
        return new ListResultDto<OrganizationUnitUsersDto>(result);
    }

    public virtual async Task<ListResultDto<OrganizationUnitDto>> FindChildrenAsync(
        OrganizationUnitGetChildrenDto input
    )
    {
        var organizationUnitChildren = await OrganizationUnitManager.FindChildrenAsync(
            input.Id,
            input.Recursive
        );

        return new ListResultDto<OrganizationUnitDto>(
            ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(
                organizationUnitChildren
            )
        );
    }

    public virtual async Task<OrganizationUnitDto> GetAsync(Guid id)
    {
        var organizationUnit = await OrganizationUnitRepository.FindAsync(id);

        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(organizationUnit);
    }

    public virtual async Task<OrganizationUnitDto> GetLastChildOrNullAsync(Guid? parentId)
    {
        var organizationUnitLastChildren = await OrganizationUnitManager.GetLastChildOrNullAsync(
            parentId
        );

        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(
            organizationUnitLastChildren
        );
    }

    public virtual async Task<ListResultDto<OrganizationUnitDto>> GetAllAsync()
    {
        var organizationUnits = (await OrganizationUnitRepository.GetListAsync(false))
            .OrderBy(x => x.CreationTime)
            .ToList();

        return new ListResultDto<OrganizationUnitDto>(
            ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(organizationUnits)
        );
    }

    public virtual async Task<PagedResultDto<OrganizationUnitDto>> GetListAsync(
        OrganizationUnitGetListInput input
    )
    {
        var organizationUnitCount = await OrganizationUnitRepository.GetCountAsync();
        var organizationUnits = await OrganizationUnitRepository.GetListAsync(
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            false
        );

        return new PagedResultDto<OrganizationUnitDto>(
            organizationUnitCount,
            ObjectMapper.Map<List<OrganizationUnit>, List<OrganizationUnitDto>>(organizationUnits)
        );
    }

    public virtual async Task<ListResultDto<string>> GetRoleNamesAsync(Guid id)
    {
        var inOrganizationUnitRoleNames = await UserRepository.GetRoleNamesInOrganizationUnitAsync(
            id
        );
        return new ListResultDto<string>(inOrganizationUnitRoleNames);
    }

    public virtual async Task<PagedResultDto<IdentityRoleDto>> GetUnaddedRolesAsync(
        Guid id,
        OrganizationUnitGetUnaddedRoleListInput input
    )
    {
        var organizationUnit = await OrganizationUnitRepository.GetAsync(id);
        var organizationUnitRoleCount = await OrganizationUnitRepository.GetUnaddedRolesCountAsync(
            organizationUnit,
            input.Filter
        );

        var organizationUnitRoles = await OrganizationUnitRepository.GetUnaddedRolesAsync(
            organizationUnit,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.Filter
        );

        return new PagedResultDto<IdentityRoleDto>(
            organizationUnitRoleCount,
            ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(organizationUnitRoles)
        );
    }

    public virtual async Task<PagedResultDto<IdentityRoleDto>> GetRolesAsync(
        Guid id,
        PagedAndSortedResultRequestDto input
    )
    {
        var organizationUnit = await OrganizationUnitRepository.GetAsync(id);

        var organizationUnitRoleCount = await OrganizationUnitRepository.GetRolesCountAsync(
            organizationUnit
        );

        var organizationUnitRoles = await OrganizationUnitRepository.GetRolesAsync(
            organizationUnit,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount
        );

        return new PagedResultDto<IdentityRoleDto>(
            organizationUnitRoleCount,
            ObjectMapper.Map<List<IdentityRole>, List<IdentityRoleDto>>(organizationUnitRoles)
        );
    }

    public virtual async Task<PagedResultDto<IdentityUserDto>> GetUnaddedUsersAsync(
        Guid id,
        OrganizationUnitGetUnaddedUserListInput input
    )
    {
        var organizationUnit = await OrganizationUnitRepository.GetAsync(id);

        var organizationUnitUserCount = await OrganizationUnitRepository.GetUnaddedUsersCountAsync(
            organizationUnit,
            input.Filter
        );
        var organizationUnitUsers = await OrganizationUnitRepository.GetUnaddedUsersAsync(
            organizationUnit,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.Filter
        );

        return new PagedResultDto<IdentityUserDto>(
            organizationUnitUserCount,
            ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(organizationUnitUsers)
        );
    }

    public virtual async Task<PagedResultDto<IdentityUserDto>> GetUsersAsync(
        Guid id,
        GetIdentityUsersInput input
    )
    {
        var organizationUnit = await OrganizationUnitRepository.GetAsync(id);

        var organizationUnitUserCount = await OrganizationUnitRepository.GetMembersCountAsync(
            organizationUnit,
            input.Filter
        );
        var organizationUnitUsers = await OrganizationUnitRepository.GetMembersAsync(
            organizationUnit,
            input.Sorting,
            input.MaxResultCount,
            input.SkipCount,
            input.Filter
        );

        return new PagedResultDto<IdentityUserDto>(
            organizationUnitUserCount,
            ObjectMapper.Map<List<IdentityUser>, List<IdentityUserDto>>(organizationUnitUsers)
        );
    }

    public virtual async Task MoveAsync(Guid id, OrganizationUnitMoveDto input)
    {
        await OrganizationUnitManager.MoveAsync(id, input.ParentId);
    }

    public virtual async Task<OrganizationUnitDto> UpdateAsync(
        Guid id,
        OrganizationUnitUpdateDto input
    )
    {
        var organizationUnit = await OrganizationUnitRepository.GetAsync(id);
        organizationUnit.DisplayName = input.DisplayName;
        input.MapExtraPropertiesTo(organizationUnit);

        await OrganizationUnitManager.UpdateAsync(organizationUnit);
        return ObjectMapper.Map<OrganizationUnit, OrganizationUnitDto>(organizationUnit);
    }

    public virtual async Task AddUsersAsync(Guid id, OrganizationUnitAddUserDto input)
    {
        var organizationUnit = await OrganizationUnitRepository.GetAsync(id);

        foreach (var item in input.UserIds)
        {
            var user = await UserRepository.GetAsync(item);
            await UserManager.AddToOrganizationUnitAsync(user, organizationUnit);
        }
    }

    public virtual async Task AddRolesAsync(Guid id, OrganizationUnitAddRoleDto input)
    {
        var organizationUnit = await OrganizationUnitRepository.GetAsync(id);

        foreach (var item in input.RoleIds)
        {
            var role = await RoleRepository.GetAsync(item);
            await OrganizationUnitManager.AddRoleToOrganizationUnitAsync(role, organizationUnit);
        }
    }

    public virtual async Task DeleteRoleAsync(Guid organizationUnitId, Guid roleId)
    {
        var organizationUnit = await OrganizationUnitRepository.GetAsync(organizationUnitId);
        var role = await RoleRepository.GetAsync(roleId);
        await OrganizationUnitManager.RemoveRoleFromOrganizationUnitAsync(role, organizationUnit);
    }

    public virtual async Task DeleteUserAsync(Guid organizationUnitId, Guid userId)
    {
        await UserManager.RemoveFromOrganizationUnitAsync(userId, organizationUnitId);
    }

    public async Task<ICollection<IdentityUserOrganizationUnit>?> GetOrganizationAsync(Guid userId)
    {
        var user = await UserRepository.GetAsync(userId);
        return user?.OrganizationUnits;
    }
}
