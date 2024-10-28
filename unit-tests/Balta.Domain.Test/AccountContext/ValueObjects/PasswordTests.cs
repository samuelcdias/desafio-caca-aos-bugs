using Balta.Domain.AccountContext.ValueObjects;
using Balta.Domain.AccountContext.ValueObjects.Exceptions;
using Balta.Domain.SharedContext.Abstractions;
using Balta.Domain.SharedContext.Extensions;

namespace Balta.Domain.Test.AccountContext.ValueObjects;

public class PasswordTests
{
    [Fact]
    public void ShouldFailIfPasswordIsNull(){
        Assert.Throws<InvalidPasswordException>(()=> {
            Password.ShouldCreate(null);
        });
    }
    
    [Fact]
    public void ShouldFailIfPasswordIsEmpty(){
        Assert.Throws<InvalidPasswordException>(()=> {
            Password.ShouldCreate(string.Empty);
        });

    }
    
    [Fact]
    public void ShouldFailIfPasswordIsWhiteSpace(){
        Assert.Throws<InvalidPasswordException>(()=> {
            Password.ShouldCreate("     ");    
        });
    }
    
    [Fact]
    public void ShouldFailIfPasswordLenIsLessThanMinimumChars(){
        string password = "senha";
        Assert.Throws<InvalidPasswordException>(()=> {
            Password.ShouldCreate(password);
        });
    }
    
    [Fact]
    public void ShouldFailIfPasswordLenIsGreaterThanMaxChars() {
        string password = "aBcD1fGhIjKl2MnOpQrSt3UvWxYz4AbCdEfGhIjKl5MnOpQrS";
        Assert.Throws<InvalidPasswordException>(()=>{
            Password.ShouldCreate(password);
        });
    }

    [Fact]
    public void ShouldHashPassword(){

        var plainTextPassword = "Password123!";
        var passwords = Password.ShouldCreate(plainTextPassword);
        Assert.True(Password.ShouldMatch(passwords.Hash, plainTextPassword));

    }
    
    [Fact]
    public void ShouldVerifyPasswordHash(){
    
        string passwordText = "passwordDeTeste";
        Password password = Password.ShouldCreate(passwordText);

        bool isMatch = Password.ShouldMatch(password.Hash, passwordText);

        Assert.True(isMatch, "senhas diferentes");

    }
    
    [Fact]
    public void ShouldGenerateStrongPassword()  => Assert.Fail();
    
    [Fact]
    public void ShouldImplicitConvertToString() => Assert.Fail();

    [Fact]
    public void ShouldReturnHashAsStringWhenCallToStringMethod() => Assert.Fail();

    [Fact]
    public void ShouldMarkPasswordAsExpired() => Assert.Fail();

    [Fact]
    public void ShouldFailIfPasswordIsExpired() => Assert.Fail();

    [Fact]
    public void ShouldMarkPasswordAsMustChange() => Assert.Fail();

    [Fact]
    public void ShouldFailIfPasswordIsMarkedAsMustChange() => Assert.Fail();
}