using System;
using System.Linq;

using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.KPro
{
    public sealed class KProFaultCode : IOBDIIFaultCode
    {
        internal KProFaultCode(DTC dtc, string? celCode, string description)
        {
            DTC = dtc;
            CELCode = celCode;
            Description = description ?? throw new ArgumentNullException(nameof(description));

            var celCodeParts = celCode?.Split("-", StringSplitOptions.RemoveEmptyEntries);
            CELMainCode = celCodeParts?.FirstOrDefault();
            CELSubCode = celCodeParts?.Skip(1).FirstOrDefault();
        }

        public string Description { get; }
        public DTC DTC { get; }
        public string? CELCode { get; }
        public string? CELMainCode { get; }
        public string? CELSubCode { get; }

        public override string ToString()
        {
            return $"{nameof(KProFaultCode)}({DTC}, {(CELCode == null ? "null" : $"\"{CELCode}\"")}, \"{Description}\")";
        }
    }
}
