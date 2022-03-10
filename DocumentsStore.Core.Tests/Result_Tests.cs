/* DocumentsStore - (C) 2022 Premysl Fara  */

namespace DocumentsStore.Core.Tests;

using System;
    
using Xunit;


public class Result_Tests
{
    [Fact]
    public void SimpleResult_Ok_IsSuccess()
    {
        Assert.True(SimpleResult.Ok().IsSuccess);
    }
    
    [Fact]
    public void SimpleResult_Ok_MessageIsOk()
    {
        Assert.Equal("Ok", SimpleResult.Ok().Message);
    }
    
    [Fact]
    public void SimpleResult_Ok_DataTypeIsObject()
    {
        Assert.Equal(typeof(object).FullName, SimpleResult.Ok().GetType().BaseType?.FullName);
    }
    
    [Fact]
    public void SimpleResult_Ok_MessageIsSet()
    {
        var msg = "Test message";
        
        Assert.Equal(msg, SimpleResult.Ok(msg).Message);
    }
    
    [Fact]
    public void SimpleResult_Ok_DataIsSet()
    {
        var data = new object();
        
        Assert.Equal(data, ((IResult<object>)SimpleResult.Ok(data)).Data);
    }
    
    
    [Fact]
    public void SimpleResult_Error_IsNotSuccess()
    {
        Assert.False(SimpleResult.Error().IsSuccess);
    }
    
    [Fact]
    public void SimpleResult_Error_MessageIsError()
    {
        Assert.Equal("Error", SimpleResult.Error().Message);
    }
    
    [Fact]
    public void SimpleResultError_MessageIsSet()
    {
        var msg = "Test message";
        
        Assert.Equal(msg, SimpleResult.Error(msg).Message);
    }
    
    [Fact]
    public void SimpleResult_Error_DataTypeIsObject()
    {
        Assert.Equal(typeof(object).FullName, SimpleResult.Error().GetType().BaseType?.FullName);
    }
    
    [Fact]
    public void SimpleResult_Error_MessageIsSet()
    {
        var msg = "Test message";
        
        Assert.Equal(msg, SimpleResult.Error(msg).Message);
    }
    
    [Fact]
    public void SimpleResult_Error_DataIsSet()
    {
        var data = new object();
        
        Assert.Equal(data, ((IResult<object>)SimpleResult.Error(data)).Data);
    }
    
    
    [Fact]
    public void SimpleResult_FromTrue_IsSuccess()
    {
        Assert.True(SimpleResult.FromBoolean(true).IsSuccess);
    }
    
    [Fact]
    public void SimpleResult_FromTrue_IsOk()
    {
        Assert.Equal("Ok", SimpleResult.FromBoolean(true).Message);
    }
    
    [Fact]
    public void SimpleResult_FromFalse_IsNotSuccess()
    {
        Assert.False(SimpleResult.FromBoolean(false).IsSuccess);
    }
    
    [Fact]
    public void SimpleResult_FromFalse_IsError()
    {
        Assert.Equal("Error", SimpleResult.FromBoolean(false).Message);
    }
    
    [Fact]
    public void Result_FromException_ContainsException()
    {
        var exception = new Exception("Something is wrong!");
        
        Assert.Equal(exception, Result<Exception>.Error(exception).Data);
    }
    
    [Fact]
    public void Result_FromException_ContainsMessageFromExceptionIfNoMessageIsSet()
    {
        var exception = new Exception("Something is wrong!");
        
        Assert.Equal(exception.Message, Result<Exception>.Error(exception).Message);
    }
    
    [Fact]
    public void Result_FromException_ContainsMessageIfMessageIsSet()
    {
        var message = "Something ELSE if wrong...";
        var exception = new Exception("Something is wrong!");
        
        Assert.Equal(message, Result<Exception>.Error(exception, message).Message);
    }
}
