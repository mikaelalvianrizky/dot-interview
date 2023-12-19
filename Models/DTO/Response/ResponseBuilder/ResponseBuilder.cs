namespace ECommerceApi.Models.DTO.Response.ResponseBuilders;

public class ResponseBuilder
{
    public static ResponseFormat SuccessResponse(string message, dynamic data = null, dynamic totalData = null)
    {
        return new ResponseFormat
        {
            StatusCode = 200,
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ResponseFormat ErrorResponseWithData(int statusCode, string message, dynamic data, dynamic errors = null)
    {
        return new ResponseFormat
        {
            StatusCode = statusCode,
            Success = false,
            Message = message,
            Data = data,
            Errors = errors
        };
    }

    public static ResponseFormat ErrorResponse(int statusCode, string message, dynamic errors = null)
    {
        return new ResponseFormat
        {
            StatusCode = statusCode,
            Success = false,
            Message = message,
            Errors = errors
        };
    }

    public static ResponseFormat ErrorResponseObject(int statusCode, string message)
    {
        var errorList = new Dictionary<string, dynamic>();
        errorList.Add("notif", message);

        var errorMessage = new Dictionary<string, object>();
        errorMessage.Add("code", statusCode);
        errorMessage.Add("message", errorList);

        return new ResponseFormat
        {
            StatusCode = statusCode,
            Success = false,
            Message = "Validation Failure",
            Errors = errorMessage
        };
    }

    public static string ToUnderscoreCase(string str)
    {
        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
    }
}

