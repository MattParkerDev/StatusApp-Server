using MudBlazor;
using StatusApp.WebUI.Components;

namespace StatusApp.WebUI.Services;

public class StatusAppDialogService
{
    private readonly IDialogService _dialogService;

    public StatusAppDialogService(IDialogService dialogService)
    {
        _dialogService = dialogService;
    }

    public async Task<bool> LoginDialog()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = false,
            CloseButton = false,
            DisableBackdropClick = true,
            NoHeader = true
        };
        var dialog = await _dialogService.ShowAsync<LoginDialog>(string.Empty, options);
        var result = await dialog.Result;
        var success = dialog.Result.IsCompletedSuccessfully;
        return success;
    }

    public async Task ProfileDialog()
    {
        var options = new DialogOptions
        {
            CloseOnEscapeKey = true,
            CloseButton = false,
            DisableBackdropClick = false,
            NoHeader = true,
            MaxWidth = MaxWidth.ExtraExtraLarge
        };
        var dialog = await _dialogService.ShowAsync<ProfileDialog>(string.Empty, options);
    }
}
