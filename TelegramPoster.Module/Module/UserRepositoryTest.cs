using Dapper;
using FluentAssertions;
using Moq;
using Moq.Dapper;
using System.Data;
using TelegramPoster.Application.Interfaces.Repositories;
using TelegramPoster.Domain.Entity;
using TelegramPoster.Persistence;
using TelegramPoster.Persistence.Repositories;

namespace TelegramPosterTest.Module;

public class UserRepositoryTests
{
    private readonly Mock<ISqlConnectionFactory> mockConnectionFactory;
    private readonly IUserRepository userRepository;

    public UserRepositoryTests()
    {
        mockConnectionFactory = new Mock<ISqlConnectionFactory>();
        userRepository = new UserRepository(mockConnectionFactory.Object);
    }

    [Fact]
    public async Task GetByEmailAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var email = "test@example.com";
        var expectedUser = new User { Id = Guid.NewGuid(), PasswordHash = "hash", Email = email };
        var mockConnection = new Mock<IDbConnection>();

        mockConnection.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<User>(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null, null, null)).ReturnsAsync(expectedUser);

        mockConnectionFactory.Setup(f => f.Create()).Returns(mockConnection.Object);

        // Act
        var result = await userRepository.GetByEmailAsync(email);

        // Assert
        result.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task GetByUserNameAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userName = "testuser";
        var expectedUser = new User { Id = Guid.NewGuid(), UserName = userName, PasswordHash = "" };
        var mockConnection = new Mock<IDbConnection>();

        mockConnection.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<User>(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null, null, null)).ReturnsAsync(expectedUser);

        mockConnectionFactory.Setup(f => f.Create()).Returns(mockConnection.Object);

        // Act
        var result = await userRepository.GetByUserNameAsync(userName);

        // Assert
        result.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task CheckUserAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var userName = "testuser";
        var expectedUser = new User { Id = Guid.NewGuid(), UserName = userName, PasswordHash = "" };
        var mockConnection = new Mock<IDbConnection>();

        mockConnection.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<User>(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null, null, null)).ReturnsAsync(expectedUser);

        mockConnectionFactory.Setup(f => f.Create()).Returns(mockConnection.Object);

        // Act
        var result = await userRepository.CheckUserAsync(userName);

        // Assert
        result.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task GetByPhoneAsync_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var phone = "1234567890";
        var expectedUser = new User { Id = Guid.NewGuid(), PhoneNumber = phone, PasswordHash = "" };
        var mockConnection = new Mock<IDbConnection>();

        mockConnection.SetupDapperAsync(c => c.QueryFirstOrDefaultAsync<User>(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null, null, null)).ReturnsAsync(expectedUser);

        mockConnectionFactory.Setup(f => f.Create()).Returns(mockConnection.Object);

        // Act
        var result = await userRepository.GetByPhoneAsync(phone);

        // Assert
        result.Should().BeEquivalentTo(expectedUser);
    }

    [Fact]
    public async Task AddAsync_ShouldInsertUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            PasswordHash = "hash",
            Email = "test@example.com",
            TelegramUserName = "testtelegram",
            PhoneNumber = "1234567890"
        };
        var mockConnection = new Mock<IDbConnection>();

        mockConnection.SetupDapperAsync(c => c.ExecuteAsync(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null, null, null)).ReturnsAsync(1);

        mockConnectionFactory.Setup(f => f.Create()).Returns(mockConnection.Object);

        // Act
        await userRepository.AddAsync(user);

        // Assert
        mockConnection.Verify(conn => conn.ExecuteAsync(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null, null, null), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateUser()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "testuser",
            TelegramUserName = "testtelegram",
            PhoneNumber = "1234567890",
            PasswordHash = ""
        };
        var mockConnection = new Mock<IDbConnection>();

        mockConnection.SetupDapperAsync(c => c.ExecuteAsync(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null, null, null)).ReturnsAsync(1);

        mockConnectionFactory.Setup(f => f.Create()).Returns(mockConnection.Object);

        // Act
        await userRepository.UpdateAsync(user);

        // Assert
        mockConnection.Verify(conn => conn.ExecuteAsync(
            It.IsAny<string>(),
            It.IsAny<object>(),
            null, null, null), Times.Once);
    }
}
