using System;
using System.Collections.Generic;
using System.Text;

namespace VoltFlow.Service.Core.Exceptions
{
    public class ConflictException : BusinessException { public ConflictException(string msg) : base(msg, 409) { } }
}
