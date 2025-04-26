using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductCore.Models;

namespace ProductCore.Helper;

public class ResponseHelper
{
    public static OkObjectResult OK_Result(dynamic? data,string? message)
    {
        return new(new DefaultResponseModel
        {
            Success = true,
            Code = StatusCodes.Status200OK,
            Meta = new { message },
            Data = data,
            Error = null
        });
    }

    public static CreatedResult Created_Result(string endpoint, dynamic? data)
    {
        return new CreatedResult(endpoint, new DefaultResponseModel
        {
            Success = true,
            Code = StatusCodes.Status201Created,
            Meta =null,
            Data = data,
            Error = null
        });
    }

    public static BadRequestObjectResult Bad_Request(dynamic? data, string? message)
    {
        return new(new DefaultResponseModel
        {
            Success = false,
            Code = StatusCodes.Status400BadRequest,
            Meta = null,
            Data = data,
            Error = new { message }
        });
    }

    public static NotFoundObjectResult NotFound_Request(dynamic? data, string? message)
    {
        return new(new DefaultResponseModel
        {
            Success = false,
            Code = StatusCodes.Status404NotFound,
            Meta = null,
            Data = data,
            Error = new { message }
        });
    }

    public static BadRequestObjectResult InternalServerError_Request(dynamic? data, string? message)
    {
        return new(new DefaultResponseModel
        {
            Success = false,
            Code = StatusCodes.Status500InternalServerError,
            Meta = null,
            Data = data,
            Error = new { message}
        });
    }

    public static UnauthorizedObjectResult Unauthorized_Request(dynamic? data, string? message)
    {
        return new(new DefaultResponseModel
        {
            Success = false,
            Code = StatusCodes.Status401Unauthorized,
            Meta = null,
            Data = data,
            Error = new { message }
        });
    }
}
