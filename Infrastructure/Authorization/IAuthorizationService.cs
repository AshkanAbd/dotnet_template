namespace Infrastructure.Authorization;

public interface IAuthorizationService
{
    public void SetUser(object user);
    public object GetUser();
}