namespace Infrastructure.Authorization;

public class AuthorizationService : IAuthorizationService
{
    private object _user;

    public void SetUser(object user)
    {
        _user = user;
    }

    public object GetUser()
    {
        return _user;
    }
}