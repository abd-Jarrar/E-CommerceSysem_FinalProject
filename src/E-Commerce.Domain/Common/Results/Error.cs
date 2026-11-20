namespace E_Commerce.Domain.Common.Results
{
    public readonly record struct Error
    {
        private Error(string code, string description, ErrorKind kind)
        {
            Code = code;
            Description = description;
            Kind = kind;
        }

        public string Code { get; }
        public string Description { get; }
        public ErrorKind Kind { get; }

        public static Error Failure(string code=nameof(Failure),string description="General Failure.")
            => new Error(code, description, ErrorKind.Failure);
        public static Error Unexpected(string code = nameof(Unexpected), string description = "Unexpected Failure.")
            => new Error(code, description, ErrorKind.Unexpected);
        public static Error Validation(string code = nameof(Validation), string description = "Validation Failure.")
            => new Error(code, description, ErrorKind.Validation);
        public static Error Conflict(string code = nameof(Conflict), string description = "Conflict Failure.")
            => new Error(code, description, ErrorKind.Conflict);
        public static Error NotFound(string code = nameof(NotFound), string description = "NotFound Failure.")
            => new Error(code, description, ErrorKind.NotFound);

        public static Error Unauthorized(string code = nameof(Unauthorized), string description = "Unauthorized Failure.")
            => new Error(code, description, ErrorKind.Unauthorized);

        public static Error Forbidden(string code = nameof(Forbidden), string description = "Forbidden Failure.")
            => new Error(code, description, ErrorKind.Forbidden);

    }
}
