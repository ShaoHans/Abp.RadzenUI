using Volo.Abp;

namespace Abp.RadzenUI.Models;

public class UpdateTimezoneSettingsVm
{
    public string Timezone { get; set; } = default!;

    public List<NameValue> TimeZoneItems { get; set; } = [];
}
