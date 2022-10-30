using System;
using System.Collections.Generic;

using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.FlashPro
{
    public class FlashProDatalogFrame : IOBDIIDatalogFrame<FlashProFaultCode, FlashProReadinessTests, FlashProReadinessCode>
    {
        public TimeSpan Offset => throw new NotImplementedException();
        public int RPM => throw new NotImplementedException();
        public double VSS => throw new NotImplementedException();
        public double INJ => throw new NotImplementedException();
        public double IGN => throw new NotImplementedException();
        public double IAT => throw new NotImplementedException();
        public double ECT => throw new NotImplementedException();
        public bool VTP => throw new NotImplementedException();
        public double Lambda => throw new NotImplementedException();
        public double STRIM => throw new NotImplementedException();
        public double LTRIM => throw new NotImplementedException();
        public double KLevel => throw new NotImplementedException();
        public double PA => throw new NotImplementedException();
        public double BAT => throw new NotImplementedException();
        public int Gear => throw new NotImplementedException();
        public byte FuelStatus => throw new NotImplementedException();

        public IReadOnlyDictionary<FlashProReadinessTests, FlashProReadinessCode> ReadinessCodes => throw new NotImplementedException();
        public IReadOnlyCollection<FlashProFaultCode> FaultCodes => throw new NotImplementedException();
        IReadOnlyCollection<IFaultCode> IDatalogFrame.FaultCodes => throw new NotImplementedException();
    }
}
