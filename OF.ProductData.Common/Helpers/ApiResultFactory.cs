using OF.ProductData.Model.CoreBank;

namespace OF.ProductData.Common.Helpers;

public static class ApiResultFactory
{
    public static ApiResult<T> Success<T>(T data, string message = "Success", string code = "200")
    {
        return new ApiResult<T>
        {
            Success = true,
            Data = data,
            Message = message,
            Code = code
        };
    }

    public static ApiResult<T> Success<T>(string message = "Success", string code = "200")
    {
        return new ApiResult<T>
        {
            Success = true,
            Data = default,
            Message = message,
            Code = code
        };
    }

    public static ApiResult<T> Failure<T>(string message, string code = "500")
    {
        return new ApiResult<T>
        {
            Success = false,
            Data = default,
            Message = message,
            Code = code
        };
    }
}
