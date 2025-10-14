namespace OF.ProductData.Common.Helpers;

public static class Utils
{
    public static string GetStatus(PostStatus status)
    {
        return status switch
        {
            PostStatus.ERROR => "ERR",
            PostStatus.HOST_ERROR => "HRR",
            PostStatus.PENDING => "PND",
            PostStatus.SUCCESS => "SUC",
            PostStatus.TIMEOUT => "TIM",
            _ => "PND"
        };
    }
    
}

public enum PostStatus { PENDING, SUCCESS, ERROR, HOST_ERROR, TIMEOUT };
