using Abp.RadzenUI.Application.Contracts.AuditLogs;
using Abp.RadzenUI.Application.Contracts.DataDictionaries;
using Abp.RadzenUI.Application.Contracts.IdentitySecurityLogs;
using Abp.RadzenUI.Application.Contracts.Messages;
using Abp.RadzenUI.Application.Contracts.Organizations;
using Abp.RadzenUI.DataDictionaries;
using Abp.RadzenUI.Messages;
using Riok.Mapperly.Abstractions;
using Volo.Abp.AuditLogging;
using Volo.Abp.Identity;
using Volo.Abp.Mapperly;

namespace Abp.RadzenUI;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AuditLogToAuditLogDtoMapper : MapperBase<AuditLog, AuditLogDto>
{
	public override partial AuditLogDto Map(AuditLog source);

	public override partial void Map(AuditLog source, AuditLogDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class AuditLogToAuditLogDetailDtoMapper
	: MapperBase<AuditLog, AuditLogDetailDto>
{
	public override partial AuditLogDetailDto Map(AuditLog source);

	public override partial void Map(AuditLog source, AuditLogDetailDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class IdentitySecurityLogToIdentitySecurityLogDtoMapper
	: MapperBase<IdentitySecurityLog, IdentitySecurityLogDto>
{
	public override partial IdentitySecurityLogDto Map(IdentitySecurityLog source);

	public override partial void Map(
		IdentitySecurityLog source,
		IdentitySecurityLogDto destination
	);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class OrganizationUnitToOrganizationUnitDtoMapper
	: MapperBase<OrganizationUnit, OrganizationUnitDto>
{
	public override partial OrganizationUnitDto Map(OrganizationUnit source);

	public override partial void Map(OrganizationUnit source, OrganizationUnitDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class DataDictionaryTypeToDataDictionaryTypeDtoMapper
	: MapperBase<DataDictionaryType, DataDictionaryTypeDto>
{
	public override partial DataDictionaryTypeDto Map(DataDictionaryType source);

	public override partial void Map(DataDictionaryType source, DataDictionaryTypeDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateDataDictionaryTypeDtoToDataDictionaryTypeMapper
	: MapperBase<CreateDataDictionaryTypeDto, DataDictionaryType>
{
	[MapperIgnoreTarget(nameof(DataDictionaryType.TenantId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.CreatorId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.CreationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.LastModifierId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.LastModificationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.IsDeleted))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.DeleterId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.DeletionTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.ConcurrencyStamp))]
	public override partial DataDictionaryType Map(CreateDataDictionaryTypeDto source);

	[MapperIgnoreTarget(nameof(DataDictionaryType.TenantId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.CreatorId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.CreationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.LastModifierId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.LastModificationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.IsDeleted))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.DeleterId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.DeletionTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.ConcurrencyStamp))]
	public override partial void Map(CreateDataDictionaryTypeDto source, DataDictionaryType destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UpdateDataDictionaryTypeDtoToDataDictionaryTypeMapper
	: MapperBase<UpdateDataDictionaryTypeDto, DataDictionaryType>
{
	[MapperIgnoreTarget(nameof(DataDictionaryType.Code))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.TenantId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.CreatorId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.CreationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.LastModifierId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.LastModificationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.IsDeleted))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.DeleterId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.DeletionTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.ConcurrencyStamp))]
	public override partial DataDictionaryType Map(UpdateDataDictionaryTypeDto source);

	[MapperIgnoreTarget(nameof(DataDictionaryType.Code))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.TenantId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.CreatorId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.CreationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.LastModifierId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.LastModificationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.IsDeleted))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.DeleterId))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.DeletionTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryType.ConcurrencyStamp))]
	public override partial void Map(UpdateDataDictionaryTypeDto source, DataDictionaryType destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class DataDictionaryItemToDataDictionaryItemDtoMapper
	: MapperBase<DataDictionaryItem, DataDictionaryItemDto>
{
	public override partial DataDictionaryItemDto Map(DataDictionaryItem source);

	public override partial void Map(DataDictionaryItem source, DataDictionaryItemDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class CreateDataDictionaryItemDtoToDataDictionaryItemMapper
	: MapperBase<CreateDataDictionaryItemDto, DataDictionaryItem>
{
	[MapperIgnoreTarget(nameof(DataDictionaryItem.TenantId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.IsActive))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.CreatorId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.CreationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.LastModifierId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.LastModificationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.IsDeleted))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.DeleterId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.DeletionTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.ConcurrencyStamp))]
	public override partial DataDictionaryItem Map(CreateDataDictionaryItemDto source);

	[MapperIgnoreTarget(nameof(DataDictionaryItem.TenantId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.IsActive))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.CreatorId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.CreationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.LastModifierId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.LastModificationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.IsDeleted))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.DeleterId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.DeletionTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.ConcurrencyStamp))]
	public override partial void Map(CreateDataDictionaryItemDto source, DataDictionaryItem destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UpdateDataDictionaryItemDtoToDataDictionaryItemMapper
	: MapperBase<UpdateDataDictionaryItemDto, DataDictionaryItem>
{
	[MapperIgnoreTarget(nameof(DataDictionaryItem.Code))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.TenantId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.DataDictionaryTypeId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.CreatorId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.CreationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.LastModifierId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.LastModificationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.IsDeleted))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.DeleterId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.DeletionTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.ConcurrencyStamp))]
	public override partial DataDictionaryItem Map(UpdateDataDictionaryItemDto source);

	[MapperIgnoreTarget(nameof(DataDictionaryItem.Code))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.TenantId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.DataDictionaryTypeId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.CreatorId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.CreationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.LastModifierId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.LastModificationTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.IsDeleted))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.DeleterId))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.DeletionTime))]
	[MapperIgnoreTarget(nameof(DataDictionaryItem.ConcurrencyStamp))]
	public override partial void Map(UpdateDataDictionaryItemDto source, DataDictionaryItem destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class UserMessageToUserMessageDtoMapper : MapperBase<UserMessage, UserMessageDto>
{
	public override partial UserMessageDto Map(UserMessage source);

	public override partial void Map(UserMessage source, UserMessageDto destination);
}

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class SaveUserMessageDtoToUserMessageMapper : MapperBase<SaveUserMessageDto, UserMessage>
{
	[MapperIgnoreTarget(nameof(UserMessage.TenantId))]
	[MapperIgnoreTarget(nameof(UserMessage.IsRead))]
	[MapperIgnoreTarget(nameof(UserMessage.ReadTime))]
	[MapperIgnoreTarget(nameof(UserMessage.CreatorId))]
	[MapperIgnoreTarget(nameof(UserMessage.CreationTime))]
	[MapperIgnoreTarget(nameof(UserMessage.LastModifierId))]
	[MapperIgnoreTarget(nameof(UserMessage.LastModificationTime))]
	[MapperIgnoreTarget(nameof(UserMessage.IsDeleted))]
	[MapperIgnoreTarget(nameof(UserMessage.DeleterId))]
	[MapperIgnoreTarget(nameof(UserMessage.DeletionTime))]
	[MapperIgnoreTarget(nameof(UserMessage.ConcurrencyStamp))]
	public override partial UserMessage Map(SaveUserMessageDto source);

	[MapperIgnoreTarget(nameof(UserMessage.TenantId))]
	[MapperIgnoreTarget(nameof(UserMessage.IsRead))]
	[MapperIgnoreTarget(nameof(UserMessage.ReadTime))]
	[MapperIgnoreTarget(nameof(UserMessage.CreatorId))]
	[MapperIgnoreTarget(nameof(UserMessage.CreationTime))]
	[MapperIgnoreTarget(nameof(UserMessage.LastModifierId))]
	[MapperIgnoreTarget(nameof(UserMessage.LastModificationTime))]
	[MapperIgnoreTarget(nameof(UserMessage.IsDeleted))]
	[MapperIgnoreTarget(nameof(UserMessage.DeleterId))]
	[MapperIgnoreTarget(nameof(UserMessage.DeletionTime))]
	[MapperIgnoreTarget(nameof(UserMessage.ConcurrencyStamp))]
	public override partial void Map(SaveUserMessageDto source, UserMessage destination);
}