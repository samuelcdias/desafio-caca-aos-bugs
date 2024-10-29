using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Balta.Domain.SharedContext.Extensions;
using Moq;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class VerificationCodeTest
{
    private readonly DateTime _now;
    private readonly Mock<IDateTimeProvider> _dateTimeProvider;
    public VerificationCodeTest()
    {
        _now = DateTime.UtcNow;
        _dateTimeProvider = new Mock<IDateTimeProvider>();
        _dateTimeProvider.Setup((e) => e.UtcNow).Returns(_now.AddMinutes(-5));
    }

    [Fact]
    public void ShouldGenerateVerificationCode()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);

        Assert.IsType<string>(validCode.Code);
        Assert.IsType<DateTime>(validCode.ExpiresAtUtc);
        Assert.True(validCode.Code.Length == 6);
        Assert.True(validCode.Code.Equals(validCode.Code, StringComparison.CurrentCultureIgnoreCase));
        Assert.NotNull(validCode.ExpiresAtUtc);
        Assert.Equal((validCode.ExpiresAtUtc ?? new DateTime()).ToLongDateString(), _now.ToLongDateString());

    }

    [Fact]
    public void ShouldGenerateExpiresAtInFuture()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        Assert.True(validCode.ExpiresAtUtc > _now);
    }
        
    [Fact]
    public void ShouldGenerateVerifiedAtAsNull()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        Assert.Null(validCode.VerifiedAtUtc);
    }

    [Fact]
    public void ShouldBeInactiveWhenCreated()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        Assert.False(validCode.IsActive);
    }

    [Fact]
    public void ShouldFailIfExpired()
    {
        var expiredCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        Assert.Throws<InvalidVerificationCodeException>(()=> expiredCode.ShouldVerify(expiredCode.Code));
    }

    [Fact]
    public void ShouldFailIfCodeIsInvalid()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify(null));
        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify(""));
        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify(" "));

    }

    [Fact]
    public void ShouldFailIfCodeIsLessThanSixChars()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        var invalidCode = validCode.Code.Substring(0, 4);
        Assert.Throws<InvalidVerificationCodeException>(()=> validCode.ShouldVerify(invalidCode));

    }

    [Fact]
    public void ShouldFailIfCodeIsGreaterThanSixChars()
    {
        var valideCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        var invalidCode = valideCode.Code + "rs";

        Assert.Throws<InvalidVerificationCodeException>(() => valideCode.ShouldVerify(invalidCode));
    }

    [Fact]
    public void ShouldFailIfIsNotActive()
    {

        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        validCode.ShouldVerify(validCode.Code);
          
        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify(validCode.Code));
    }
    [Fact]
    public void ShouldFailIfIsAlreadyVerified()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        validCode.ShouldVerify(validCode.Code);

        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify(validCode.Code));
    }

    [Fact]
    public void ShouldFailIfIsVerificationCodeDoesNotMatch()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        var incorrectCode = "INVALID";

        Assert.Throws<InvalidVerificationCodeException>(() => validCode.ShouldVerify(incorrectCode));
    }

    [Fact]
    public void ShouldVerify()
    {
        var validCode = VerificationCode.ShouldCreate(_dateTimeProvider.Object);
        validCode.ShouldVerify(validCode.Code);

        Assert.NotNull(validCode.ExpiresAtUtc);
        Assert.NotNull(validCode.VerifiedAtUtc);
      
    }
}