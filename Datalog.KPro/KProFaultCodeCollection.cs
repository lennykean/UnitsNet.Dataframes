using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.KPro
{
    public sealed class KProFaultCodeCollection : IReadOnlyCollection<KProFaultCode>
    {
        private readonly BigInteger _faultCodeFlags;

        internal KProFaultCodeCollection(byte[] faultCodeFlags)
        {
            _faultCodeFlags = new(faultCodeFlags);
        }

        public int Count
        {
            get
            {
                return KProFaultCodeTable.Instance.Keys.Count(faultCodeFlag => HasFaultFlag(_faultCodeFlags, faultCodeFlag));
            }
        }

        public IEnumerator<KProFaultCode> GetEnumerator()
        {
            foreach (var faultCodeFlag in KProFaultCodeTable.Instance.Keys)
            {
                if (HasFaultFlag(_faultCodeFlags, faultCodeFlag))
                {
                    yield return KProFaultCodeTable.Instance[faultCodeFlag];
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool HasDTC(DTC dtc)
        {
            return KProFaultCodeTable.LookupDTCIndex(dtc).Any(faultCodeFlag => HasFaultFlag(_faultCodeFlags, faultCodeFlag));
        }

        public IEnumerable<KProFaultCode> GetByDTC(DTC dtc)
        {
            return KProFaultCodeTable.LookupDTCIndex(dtc).Select(faultCodeFlag => KProFaultCodeTable.Instance[faultCodeFlag]);
        }

        public bool HasCELCode(string celCode)
        {
            return KProFaultCodeTable.LookupCELCodeIndex(celCode).Any(faultCodeFlag => HasFaultFlag(_faultCodeFlags, faultCodeFlag));
        }

        public bool HasCELCode(string celMainCode, string celSubCode)
        {
            return KProFaultCodeTable.LookupCELCodeIndex(celMainCode, celSubCode).Any(faultCodeFlag => HasFaultFlag(_faultCodeFlags, faultCodeFlag));
        }

        private static bool HasFaultFlag(BigInteger faultCodeFlags, BigInteger faultCodeFlag)
        {
            return (faultCodeFlags | faultCodeFlag) == faultCodeFlags;
        }
    }
}
