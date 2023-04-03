using MudBlazor;
using StatusApp.WebUI.Components;

namespace StatusApp.WebUI.Services;

public class LoginDialogService
{
    private readonly IDialogService _dialogService;

    public LoginDialogService(IDialogService dialogService)
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
}
