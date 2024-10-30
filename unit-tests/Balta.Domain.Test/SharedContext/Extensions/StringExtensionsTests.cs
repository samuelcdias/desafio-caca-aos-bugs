using System.Text;
using Balta.Domain.SharedContext.Extensions;

namespace Balta.Domain.Test.SharedContext.Extensions;

public class StringExtensionsTests
{
    [Fact]
    public void ShouldGenerateBase64FromString()
    {   
        var text = "não";
        var correct =  Convert.ToBase64String(Encoding.ASCII.GetBytes(text));
        var wrong =  Convert.ToBase64String(Encoding.UTF8.GetBytes(text));
        var result = StringExtensions.ToBase64(text);
        
        Assert.NotEqual(wrong, result);
        Assert.Equal(correct,result);
    }
}