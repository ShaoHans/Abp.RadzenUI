using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace CRM.Operations;

public interface IOperationAppService : IApplicationService
{
    Task<OperationDashboardDto> GetDashboardAsync();

    Task<PagedResultDto<WorkOrderDto>> GetWorkOrdersAsync(GetWorkOrdersInput input);

    Task<WorkOrderDetailDto> GetWorkOrderDetailAsync(Guid id);

    Task ChangeWorkOrderStatusAsync(Guid id, ChangeWorkOrderStatusDto input);

    Task AssignWorkOrdersAsync(AssignWorkOrdersDto input);

    Task<PagedResultDto<OperationAssetDto>> GetAssetsAsync(GetAssetsInput input);

    Task<List<OperationShiftDto>> GetShiftsAsync(DateTime start, DateTime end);
}
