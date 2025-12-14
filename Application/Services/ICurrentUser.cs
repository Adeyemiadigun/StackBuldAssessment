namespace Application.Services;

public interface ICurrentUser
{
    Task<Guid> GetUserAsync();
    bool IsAuthenticated();
}