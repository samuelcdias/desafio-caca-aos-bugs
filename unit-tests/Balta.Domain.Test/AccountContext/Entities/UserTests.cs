using Balta.Domain.AccountContext.Entities;
using Balta.Domain.AccountContext.Errors;
using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Moq;

namespace Balta.Domain.Test.AccountContext.Entities;

public class UserTests
{
    private const string EmailAddress = "test@test.com";
    private const string PlainTextPassword = "senhabemlouca123";
    private readonly Mock<IDateTimeProvider> _dateTimeProvider = new();

    [Fact]
    public void ShouldCreateUserIfEmailAndPasswordIsValid()
    {
        var email = Email.ShouldCreate(EmailAddress, _dateTimeProvider.Object);
        var password = Password.ShouldCreate(PlainTextPassword);
        var user = User.ShouldCreate(email, password);

        Assert.NotNull(user);
        Assert.Equal(email, user.Email);
        Assert.Equal(password, user.Password);
        Assert.Null(user.LockOut);
        Assert.Equal(0, user.AccessFailedCount);
    }

    [Fact]
    public void ShouldIncrementAccessFailedCountIfAuthFails()
    {

        var email = Email.ShouldCreate(EmailAddress, _dateTimeProvider.Object);
        var password = Password.ShouldCreate(PlainTextPassword);
        var user = User.ShouldCreate(email, password);

        user.ShouldAuthenticate();

        Assert.Equal(1, user.AccessFailedCount);
    }

    [Theory]
    [InlineData(4)] 
    public void ShouldBlockUserIfMultipleFailAttempts(int attempts)
    {
        var email = Email.ShouldCreate(EmailAddress, _dateTimeProvider.Object);
        var password = Password.ShouldCreate(PlainTextPassword);
        var user = User.ShouldCreate(email, password);

        for (int i = 0; i < attempts; i++)
        {
            user.ShouldAuthenticate();
        }

        Assert.NotNull(user.LockOut);
    }

    [Theory]
    [InlineData(4)]
    public void ShouldSendBlockingMessageIfMultipleFailAttempts(int attempts)
    {
        var email = Email.ShouldCreate(EmailAddress, _dateTimeProvider.Object);
        var password = Password.ShouldCreate(PlainTextPassword);
        var user = User.ShouldCreate(email, password);

        for (int i = 0; i < attempts; i++)
        {
            user.ShouldAuthenticate();
        }

        Assert.Equal(UserErrors.LockedOutByLoginAttemptsMessage, user.LockOut?.LockOutReason);
    }

    [Theory]
    [InlineData(4)]
    public void ShouldNotIncrementAccessFailedCountAfterBlock(int attempts)
    {
        var email = Email.ShouldCreate(EmailAddress, _dateTimeProvider.Object);
        var password = Password.ShouldCreate(PlainTextPassword);
        var user = User.ShouldCreate(email, password);

        for (int i = 0; i < attempts; i++)
        {
            user.ShouldAuthenticate();
        }

        Assert.True(user.AccessFailedCount <= 3);
    }

}