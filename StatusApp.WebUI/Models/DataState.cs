namespace StatusApp.WebUI.Models;

public class DataState
{
    public Profile? UserProfile { get; set; }
    public bool Authorized { get; set; }
    public IReadOnlyList<Profile>? FriendList { get; set; }
    public event Action OnChange;

    public void SetAuthorized(bool authorized)
    {
        Authorized = authorized;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}
