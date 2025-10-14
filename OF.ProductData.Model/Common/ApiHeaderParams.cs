namespace ConsentModel.Common;
    public class ApiHeaderParams
    {
        public string? X_Channel_Id { get; set; }
        public string? X_Country_Code { get; set; }
        public string? X_SubChannel_Id { get; set; }
        public string? AuthToken { get; set; }
        public bool AuthEnable { get; set; }
        public bool DynamicTokenEnabled { get; set; }
        public string? grant_type { get; set; }
        public string? client_id { get; set; }
        public string? client_secret { get; set; }
    }
