using Abp.RadzenUI.Application.Contracts.Messages;
using Abp.RadzenUI.Localization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Radzen;
using Radzen.Blazor;
using Volo.Abp.AspNetCore.Components.Notifications;

namespace Abp.RadzenUI.Components.Pages.Messages;

public partial class List
{
    [Inject]
    protected IMessageAppService MessageAppService { get; set; } = default!;

    [Inject]
    protected IMessageTypeLookupAppService MessageTypeLookupAppService { get; set; } = default!;

    [Inject]
    protected ISideDialogCoordinatorFactory SideDialogCoordinatorFactory { get; set; } = default!;

    [Inject]
    protected MessageCenterState MessageCenterState { get; set; } = default!;

    [Inject]
    protected new IUiNotificationService Notify { get; set; } = default!;

    [Inject]
    public IStringLocalizer<AbpRadzenUIResource> UL { get; set; } = default!;

    private readonly int _defaultPageSize = 20;
    private readonly IEnumerable<int> _pageSizeOptions = [10, 20, 50];
    private IReadOnlyList<ReadStatusOption> _readStatusOptions = [];

    private RadzenDataGrid<UserMessageDto> _grid = default!;
    private SideDialogCoordinator<UserMessageDto> _sideDialogCoordinator = default!;
    private IReadOnlyList<UserMessageDto> _messages = [];
    private IList<UserMessageDto> _selectedMessages = [];
    private List<MessageTypeLookupDto> _messageTypeOptions = [];
    private int _totalCount;
    private bool _isLoading;
    private bool _initialized;
    private string? _keyword;
    private string? _selectedMessageType;
    private MessageReadStatus? _readStatus;

    public List()
    {
        LocalizationResource = typeof(AbpRadzenUIResource);
    }

    private bool? SelectAllValue => _messages.Count == 0
        ? false
        : _selectedMessages.Count == 0
            ? false
            : _selectedMessages.Count == _messages.Count
                ? true
                : null;

    protected override async Task OnInitializedAsync()
    {
        _sideDialogCoordinator = SideDialogCoordinatorFactory.Create<UserMessageDto>();
        MessageCenterState.Changed += HandleMessageCenterChangedAsync;
        _readStatusOptions =
        [
            new ReadStatusOption { Text = L["Message:Read"], Value = MessageReadStatus.Read },
            new ReadStatusOption { Text = L["Message:Unread"], Value = MessageReadStatus.Unread },
        ];
        await LoadMessageTypesAsync();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        await base.OnAfterRenderAsync(firstRender);

        if (!firstRender || _initialized || _grid is null)
        {
            return;
        }

        _initialized = true;
        await _grid.FirstPage(true);
    }

    private async Task LoadDataAsync(LoadDataArgs args)
    {
        _isLoading = true;

        try
        {
            var result = await MessageAppService.GetListAsync(
                new GetUserMessagesInput
                {
                    Filter = _keyword,
                    MessageType = _selectedMessageType,
                    ReadStatus = _readStatus,
                    Sorting = args.OrderBy,
                    SkipCount = args.Skip ?? 0,
                    MaxResultCount = args.Top ?? _defaultPageSize,
                }
            );

            _messages = result.Items.ToList();
            _totalCount = (int)result.TotalCount;
            _selectedMessages = _selectedMessages.Where(x => _messages.Any(y => y.Id == x.Id)).ToList();
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task SearchAsync()
    {
        if (_grid is not null)
        {
            await _grid.FirstPage(true);
        }
    }

    private async Task OnReadStatusChangedAsync(object? value)
    {
        _readStatus = value as MessageReadStatus?;
        await SearchAsync();
    }

    private async Task OnMessageTypeChangedAsync(object? value)
    {
        _selectedMessageType = value?.ToString();
        await SearchAsync();
    }

    private void OnSelectAllChanged(bool? value)
    {
        _selectedMessages = value == true ? _messages.ToList() : [];
    }

    private async Task OpenDetailAsync(UserMessageDto message)
    {
        await _sideDialogCoordinator.OpenDetailAsync<UserMessageDto, Detail>(
            message,
            L["Message:OpenDetail"],
            "SelectedMessage",
            "720px"
        );
    }

    private async Task BatchMarkAsReadAsync()
    {
        if (_selectedMessages.Count == 0)
        {
            return;
        }

        try
        {
            await MessageAppService.BatchMarkAsReadAsync(
                new MarkUserMessagesAsReadInput { Ids = _selectedMessages.Select(x => x.Id).ToList() }
            );
            _selectedMessages.Clear();
            await MessageCenterState.NotifyChangedAsync();
            await Notify.Success(UL["SavedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task MarkAllAsReadAsync()
    {
        try
        {
            await MessageAppService.MarkAllAsReadAsync();
            _selectedMessages.Clear();
            await MessageCenterState.NotifyChangedAsync();
            await Notify.Success(UL["SavedSuccessfully"]);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(ex);
        }
    }

    private async Task LoadMessageTypesAsync()
    {
        var result = await MessageTypeLookupAppService.GetListAsync(new GetMessageTypeLookupInput());
        _messageTypeOptions = result.Items.ToList();
    }

    private async Task HandleMessageCenterChangedAsync()
    {
        await InvokeAsync(async () =>
        {
            await LoadMessageTypesAsync();

            if (_grid is not null)
            {
                await _grid.Reload();
            }

            StateHasChanged();
        });
    }

    public void Dispose()
    {
        MessageCenterState.Changed -= HandleMessageCenterChangedAsync;
        _sideDialogCoordinator.Dispose();
    }

    private sealed class ReadStatusOption
    {
        public string Text { get; set; } = default!;

        public MessageReadStatus? Value { get; set; }
    }
}