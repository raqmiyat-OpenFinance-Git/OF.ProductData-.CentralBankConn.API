using OF.ProductData.Model.EFModel.Audit;

namespace OF.ProductData.Common.Helpers;

public static class AuditLogFactory
{
    public static AuditLog CreateAuditLog(
        Guid correlationId,
        string sourceSystem,
        string targetSystem,
        string endpoint,
        string requestPayload,
        string? responsePayload,
        string statusCode,
        string requestType,
        int executionTimeMs,
        string? errorMessage = null)
    {
        return new AuditLog
        {
            CorrelationId = correlationId,
            SourceSystem = sourceSystem,
            TargetSystem = targetSystem,
            Endpoint = endpoint,
            RequestPayload = requestPayload,
            ResponsePayload = responsePayload,
            StatusCode = statusCode,
            ErrorMessage = errorMessage,
            RequestType = requestType,
            ExecutionTimeMs = executionTimeMs
        };
    }
}
