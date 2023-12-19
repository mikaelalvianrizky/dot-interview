namespace ECommerceApi.Models.DTO.Response.ResponseBuilders;

public class ResponseFormat
{
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public dynamic Data { get; set; }
    public dynamic Errors { get; set; }

}

