using TelegramPoster.Domain.Entity;

namespace TelegramPoster.Application.Interfaces.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> CheckUserAsync(string userName);
    Task<User?> GetAsync(Guid id);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByPhoneAsync(string phone);
    Task<User?> GetByUserNameAsync(string userName);
    Task UpdateAsync(User user);
    Task UpdateRefreshAsync(Guid id, string refreshToken, DateTime refreshTokenExpiryTime);
}