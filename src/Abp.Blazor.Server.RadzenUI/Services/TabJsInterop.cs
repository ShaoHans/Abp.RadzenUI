using Microsoft.JSInterop;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Abp.Blazor.Server.RadzenUI.Services
{
    public class TabJsInterop : ITransientDependency
    {
        private readonly IJSRuntime _jsRuntime;

        public TabJsInterop(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
        }

        public async ValueTask SaveTabsAsync(string tabsJson)
        {
            await _jsRuntime.InvokeVoidAsync("tabInterop.saveTabs", tabsJson);
        }

        public async ValueTask<string> LoadTabsAsync()
        {
            return await _jsRuntime.InvokeAsync<string>("tabInterop.loadTabs");
        }

        public async ValueTask RemoveTabsAsync()
        {
            await _jsRuntime.InvokeVoidAsync("tabInterop.removeTabs");
        }
    }
}