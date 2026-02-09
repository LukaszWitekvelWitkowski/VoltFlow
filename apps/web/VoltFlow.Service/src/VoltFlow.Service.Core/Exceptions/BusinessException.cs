using System;
using System.Collections.Generic;
using System.Text;

namespace VoltFlow.Service.Core.Exceptions
{
    public abstract class BusinessException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; } // Dodatkowy kod dla frontendu, np. "TASK_001"

        protected BusinessException(string message, int statusCode, string errorCode = "GENERIC_ERROR")
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
