using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductInventory.BLL.Common
{
    public enum ResultStatus
    {
        Success,
        ValidationFailed,
        NotFound
    }

    public record ValidationError(string Field, string Message);
    public class ServiceResult
    {
        public ResultStatus Status { get; private init; }

        public IReadOnlyList<ValidationError> Errors { get; private init; } = Array.Empty<ValidationError>();

        public bool Succeeded => Status == ResultStatus.Success;

        public static ServiceResult Ok() => new() { Status = ResultStatus.Success };

        public static ServiceResult NotFound() => new() { Status = ResultStatus.NotFound };

        public static ServiceResult Invalid(string field, string message) => new()
        {
            Status = ResultStatus.ValidationFailed,
            Errors = new[] { new ValidationError(field, message) }
        };
    }
}
