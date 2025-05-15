using Volo.Abp;

namespace Abp.RadzenUI.Models;

public class UpdateTimezoneSettingsVM
{
    public string Timezone { get; set; } = default!;

    public List<NameValue> TimeZoneItems { get; set; } = [];
}
