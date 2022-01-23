namespace DocumentsStore.Core;


public static class SimpleResult
{
    public static IResult Ok(string? message = "Ok")
    {
        return Result<object>.Ok(message);
    }


    public static IResult Error(string? message = "Error")
    {
        return Result<object>.Error(message);
    }
}
