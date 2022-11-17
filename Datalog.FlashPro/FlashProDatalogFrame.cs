using HondataDotNet.Datalog.Core;
using HondataDotNet.Datalog.Core.Utils;
using HondataDotNet.Datalog.OBDII;

using System;
using System.Collections.Generic;
using System.IO;

namespace HondataDotNet.Datalog.FlashPro
{
    public sealed partial class FlashProDatalogFrame : IOBDIIDatalogFrame<FlashProFaultCode, FlashProReadinessTests, FlashProReadinessCode>
    {
        private DatalogFrame _frame;

        public FlashProDatalog? Datalog { get; internal set; }
        public TimeSpan Offset => TimeSpan.FromMilliseconds(_frame.Offset);

        /// <summary>
        /// Engine speed in rpm.
        /// </summary>
        [SensorMetadata("RPM", Description = "Engine speed", QuantityType = "RotationalSpeed", Unit = "rpm")]
        public double RPM => _frame.RPM;
        int IDatalogFrame.RPM => (int)RPM;
        /// <summary>
        /// Vehicle speed in km/h.
        /// </summary>
        [SensorMetadata("VSS", Description = "Vehicle speed", QuantityType = "Speed", Unit = "km/h")]
        public double VSS => _frame.VSS;
        /// <summary>
        /// Gear.
        /// </summary>
        [SensorMetadata("Gear", Description = "Gear", QuantityType = "ScalarUnit", Unit = "")]
        public int Gear => (int)_frame.Gear;
        /// <summary>
        /// Manifold pressure in bar.
        /// </summary>
        [SensorMetadata("MAP", Description = "Manifold pressure", QuantityType = "Pressure", Unit = "bar")]
        public double MAP => _frame.MAP;
        /// <summary>
        /// Throttle pedal in percent.
        /// </summary>
        [SensorMetadata("TPedal", Description = "Throttle pedal", QuantityType = "Ratio", Unit = "%")]
        public double TPedal => _frame.TPedal;
        /// <summary>
        /// Throttle plate in percent
        /// </summary>
        [SensorMetadata("TPlate", Description = "Throttle plate", QuantityType = "Ratio", Unit = "%")]
        public double TPlate => _frame.TPlate;
        /// <summary>
        /// Air flow meter in volts.
        /// </summary>
        [SensorMetadata("AFM.v", Description = "Air flow meter", QuantityType = "ElectricPotential", Unit = "V")]
        public double AFMv => _frame.AFMv;
        /// <summary>
        /// Air flow meter in g/s.
        /// </summary>
        [SensorMetadata("AFM", Description = "Air flow meter", QuantityType = "MassFlow", Unit = "g/s")]
        public double AFM => _frame.AFM;
        /// <summary>
        /// Injector pulse width in ms.
        /// </summary>
        [SensorMetadata("INJ", Description = "Injector pulse width", QuantityType = "Duration", Unit = "ms")]
        public double INJ => _frame.INJ;
        /// <summary>
        /// Ignition advance in degrees.
        /// </summary>
        [SensorMetadata("IGN", Description = "Ignition advance", QuantityType = "Angle", Unit = "°")]
        public double IGN => _frame.IGN;
        /// <summary>
        /// Intake air temperature in °C.
        /// </summary>
        [SensorMetadata("IAT", Description = "Intake air temperature", QuantityType = "Temperature", Unit = "°C")]
        public double IAT => _frame.IAT;
        /// <summary>
        /// Engine coolant temperature in °C.
        /// </summary>
        [SensorMetadata("ECT", Description = "Engine coolant temperature", QuantityType = "Temperature", Unit = "°C")]
        public double ECT => _frame.ECT;
        /// <summary>
        /// Actual VTC cam angle in degrees.
        /// </summary>
        [SensorMetadata("CAM", Description = "Actual VTC cam angle", QuantityType = "Angle", Unit = "°")]
        public double CAM => _frame.CAM;
        /// <summary>
        /// Commanded VTC cam angle in degrees.
        /// </summary>
        [SensorMetadata("CAMCMD", Description = "Commanded VTC cam angle", QuantityType = "Angle", Unit = "°")]
        public double CAMCMD => _frame.CAMCMD;
        /// <summary>
        /// Air / fuel ratio in lambda.
        /// </summary>
        [SensorMetadata("A / F", Description = "Air / fuel ratio", QuantityType = "Ratio", Unit = "%")]
        public double AF => _frame.AF;
        /// <summary>
        /// Target air / fuel ratio in lambda.
        /// </summary>
        [SensorMetadata("A / F.CMD", Description = "Target air / fuel ratio", QuantityType = "Ratio", Unit = "%")]
        public double AFCMD => _frame.AFCMD;
        /// <summary>
        /// Short term fuel trim in lambda.
        /// </summary>
        [SensorMetadata("S.TRIM", Description = "Short term fuel trim", QuantityType = "Ratio", Unit = "%")]
        public double STRIM => _frame.STRIM;
        /// <summary>
        /// Long term fuel trim in lambda.
        /// </summary>
        [SensorMetadata("L.TRIM", Description = "Long term fuel trim", QuantityType = "Ratio", Unit = "%")]
        public double LTRIM => _frame.LTRIM;
        /// <summary>
        /// Fuel system status.
        /// </summary>
        [SensorMetadata("Fuel Status", Description = "Fuel system status", QuantityType = "ScalarUnit", Unit = "")]
        public int FuelStatus => (int)_frame.FuelStatus;
        /// <summary>
        /// Knock level in percent.
        /// </summary>
        [SensorMetadata("Knock Level", Description = "Knock level", QuantityType = "Ratio", Unit = "%")]
        public double KLevel => _frame.KLevel;
        /// <summary>
        /// Knock retard in degrees.
        /// </summary>
        [SensorMetadata("Knock Retard", Description = "Knock retard", QuantityType = "Angle", Unit = "°")]
        public double KRetard => _frame.KRetard;
        /// <summary>
        /// Knock control in percent.
        /// </summary>
        [SensorMetadata("Knock Control", Description = "Knock control", QuantityType = "Ratio", Unit = "%")]
        public double KControl => _frame.KControl;
        /// <summary>
        /// Atmospheric air pressure in bar.
        /// </summary>
        [SensorMetadata("PA", Description = "Atmospheric air pressure", QuantityType = "Pressure", Unit = "bar")]
        public double PA => _frame.PA;
        /// <summary>
        /// Battery voltage in volts.
        /// </summary>
        [SensorMetadata("BAT", Description = "Battery voltage", QuantityType = "ElectricPotential", Unit = "V")]
        public double BAT => _frame.BAT;
        /// <summary>
        /// Air conditioning clutch.
        /// </summary>
        [SensorMetadata("ACCL", Description = "Air conditioning clutch")]
        public double ACCL => _frame.ACCL;
        /// <summary>
        /// VTEC spool.
        /// </summary>
        [SensorMetadata("VTS", Description = "VTEC spool")]
        public bool VTS => _frame.VTS > 0;
        /// <summary>
        /// Fuel Economy in L/100km.
        /// </summary>
        [SensorMetadata("Fuel Eco", Description = "Fuel Economy", QuantityType = "FuelEfficiency", Unit = "L/100km")]
        public double Eco => _frame.Eco;
        /// <summary>
        /// Fuel Used in cm³.
        /// </summary>
        [SensorMetadata("Fuel Used", Description = "Fuel Used", QuantityType = "Volume", Unit = "cm³")]
        public double FuelUsed => _frame.FuelUsed;
        /// <summary>
        /// Wideband input in lambda.
        /// </summary>
        [SensorMetadata("Wideband", Description = "Wideband input", QuantityType = "Ratio", Unit = "%")]
        public double Wide => _frame.Wide;
        /// <summary>
        /// Wideband voltage in volts.
        /// </summary>
        [SensorMetadata("Wideband voltage", Description = "Wideband voltage", QuantityType = "ElectricPotential", Unit = "V")]
        public double WideV => _frame.WideV;
        /// <summary>
        /// Knock count #1 cylinder.
        /// </summary>
        [SensorMetadata("Knock count #1 cylinder", Description = "Knock count #1 cylinder", QuantityType = "ScalarUnit", Unit = "")]
        public double KCount1 => _frame.KCount1;
        /// <summary>
        /// Knock count #2 cylinder.
        /// </summary>
        [SensorMetadata("Knock count #2 cylinder", Description = "Knock count #2 cylinder", QuantityType = "ScalarUnit", Unit = "")]
        public double KCount2 => _frame.KCount2;
        /// <summary>
        /// Knock count #3 cylinder.
        /// </summary>
        [SensorMetadata("Knock count #3 cylinder", Description = "Knock count #3 cylinder", QuantityType = "ScalarUnit", Unit = "")]
        public double KCount3 => _frame.KCount3;
        /// <summary>
        /// Knock count #4 cylinder.
        /// </summary>
        [SensorMetadata("Knock count #4 cylinder", Description = "Knock count #4 cylinder", QuantityType = "ScalarUnit", Unit = "")]
        public double KCount4 => _frame.KCount4;
        /// <summary>
        /// Knock count #5 cylinder.
        /// </summary>
        [SensorMetadata("Knock count #5 cylinder", Description = "Knock count #5 cylinder", QuantityType = "ScalarUnit", Unit = "")]
        public double KCount5 => _frame.KCount5;
        /// <summary>
        /// Knock count #6 cylinder.
        /// </summary>
        [SensorMetadata("Knock count #6 cylinder", Description = "Knock count #6 cylinder", QuantityType = "ScalarUnit", Unit = "")]
        public double KCount6 => _frame.KCount6;
        /// <summary>
        /// Boost Controller Duty Cycle in percent.
        /// </summary>
        [SensorMetadata("Boost Control Duty", Description = "Boost Controller Duty Cycle", QuantityType = "Ratio", Unit = "%")]
        public double BCDuty => _frame.BCDuty;
        /// <summary>
        /// Knock ignition limit in degrees.
        /// </summary>
        [SensorMetadata("Ign.Limit", Description = "Knock ignition limit", QuantityType = "Angle", Unit = "°")]
        public double IgnLimit => _frame.IgnLimit;
        /// <summary>
        /// Engine coolant temperature 2 in °C.
        /// </summary>
        [SensorMetadata("ECT2", Description = "Engine coolant temperature 2", QuantityType = "Temperature", Unit = "°C")]
        public double ECT2 => _frame.ECT2;
        /// <summary>
        /// Traction control voltage(from ECU) in volts.
        /// </summary>
        [SensorMetadata("TC.Volt", Description = "Traction control voltage(from ECU)", QuantityType = "ElectricPotential", Unit = "V")]
        public double TCV => _frame.TCV;
        /// <summary>
        /// Traction control over slip (from ECU) in percent.
        /// </summary>
        [SensorMetadata("TC.ECUSlip", Description = "Traction control over slip (from ECU)", QuantityType = "Ratio", Unit = "%")]
        public double TCECUSlip => _frame.TCECUSlip;
        /// <summary>
        /// Traction control ign retard in degrees.
        /// </summary>
        [SensorMetadata("TC.Retard", Description = "Traction control ign retard", QuantityType = "Angle", Unit = "°")]
        public double TCR => _frame.TCR;
        /// <summary>
        /// Traction control left front wheel speed in km/h.
        /// </summary>
        [SensorMetadata("TC.Left Front", Description = "Traction control left front wheel speed", QuantityType = "Speed", Unit = "km/h")]
        public double TCLF => _frame.TCLF;
        /// <summary>
        /// Traction control right front wheel speed in km/h.
        /// </summary>
        [SensorMetadata("TC.Right Front", Description = "Traction control right front wheel speed", QuantityType = "Speed", Unit = "km/h")]
        public double TCRF => _frame.TCRF;
        /// <summary>
        /// Traction control left rear wheel speed in km/h.
        /// </summary>
        [SensorMetadata("TC.Left Rear", Description = "Traction control left rear wheel speed", QuantityType = "Speed", Unit = "km/h")]
        public double TCLR => _frame.TCLR;
        /// <summary>
        /// Traction control right rear wheel speed in km/h.
        /// </summary>
        [SensorMetadata("TC.Right Rear", Description = "Traction control right rear wheel speed", QuantityType = "Speed", Unit = "km/h")]
        public double TCRR => _frame.TCRR;
        /// <summary>
        /// Traction control slip in percent.
        /// </summary>
        [SensorMetadata("TC.Slip", Description = "Traction control slip", QuantityType = "Ratio", Unit = "%")]
        public double TCSlip => _frame.TCSlip;
        /// <summary>
        /// Traction control turning factor in percent.
        /// </summary>
        [SensorMetadata("TC.Turning", Description = "Traction control turning factor", QuantityType = "Ratio", Unit = "%")]
        public double TCTurn => _frame.TCTurn;
        /// <summary>
        /// Traction control over slip (from TC) in percent.
        /// </summary>
        [SensorMetadata("TC.OverSlip", Description = "Traction control over slip (from TC)", QuantityType = "Ratio", Unit = "%")]
        public double TCOverSlip => _frame.TCOverSlip;
        /// <summary>
        /// Traction control output voltage in volts.
        /// </summary>
        [SensorMetadata("TC.Output", Description = "Traction control output voltage", QuantityType = "ElectricPotential", Unit = "V")]
        public double TCOut => _frame.TCOut;
        /// <summary>
        /// Secondary intake runners.
        /// </summary>
        [SensorMetadata("SVS", Description = "Secondary intake runners")]
        public bool SVS => _frame.SVS > 0;
        /// <summary>
        /// Fuel tank pressure in bar.
        /// </summary>
        [SensorMetadata("PTANK", Description = "Fuel tank pressure", QuantityType = "Pressure", Unit = "bar")]
        public double PTANK => _frame.PTANK;
        /// <summary>
        /// Purge duty cycle in percent.
        /// </summary>
        [SensorMetadata("Purge", Description = "Purge duty cycle", QuantityType = "Ratio", Unit = "%")]
        public double Purge => _frame.Purge;
        /// <summary>
        /// Air / fuel ratio in lambda.
        /// </summary>
        [SensorMetadata("A / F #2", Description = "Air / fuel ratio", QuantityType = "Ratio", Unit = "%")]
        public double AFBank2 => _frame.AFBank2;
        /// <summary>
        /// Target air / fuel ratio in lambda.
        /// </summary>
        [SensorMetadata("A / F.CMD #2", Description = "Target air / fuel ratio", QuantityType = "Ratio", Unit = "%")]
        public double AFCMDBank2 => _frame.AFCMDBank2;
        /// <summary>
        /// Short term fuel trim in lambda.
        /// </summary>
        [SensorMetadata("S.TRIM #2", Description = "Short term fuel trim", QuantityType = "Ratio", Unit = "%")]
        public double STRIMBank2 => _frame.STRIMBank2;
        /// <summary>
        /// Long term fuel trim in lambda.
        /// </summary>
        [SensorMetadata("L.TRIM #2", Description = "Long term fuel trim", QuantityType = "Ratio", Unit = "%")]
        public double LTRIMBank2 => _frame.LTRIMBank2;
        /// <summary>
        /// Fuel system status in .
        /// </summary>
        [SensorMetadata("Fuel Status #2", Description = "Fuel system status", QuantityType = "ScalarUnit", Unit = "")]
        public int FuelStatusBank2 => (int)_frame.FuelStatusBank2;
        /// <summary>
        /// Injector pulse width in ms.
        /// </summary>
        [SensorMetadata("INJ #2", Description = "Injector pulse width", QuantityType = "Duration", Unit = "ms")]
        public double INJBank2 => _frame.INJBank2;
        /// <summary>
        /// Intake air temperature 2 in °C.
        /// </summary>
        [SensorMetadata("IAT2", Description = "Intake air temperature 2", QuantityType = "Temperature", Unit = "°C")]
        public double IAT2 => _frame.IAT2;
        /// <summary>
        /// Boost pressure in bar.
        /// </summary>
        [SensorMetadata("BP", Description = "Boost pressure", QuantityType = "Pressure", Unit = "bar")]
        public double BP => _frame.BP;
        /// <summary>
        /// Actual exhaust cam angle in degrees.
        /// </summary>
        [SensorMetadata("EXCAM", Description = "Actual exhaust cam angle", QuantityType = "Angle", Unit = "°")]
        public double EXCAM => _frame.EXCAM;
        /// <summary>
        /// Commanded exhaust cam angle in degrees.
        /// </summary>
        [SensorMetadata("EXCAMCMD", Description = "Commanded exhaust cam angle", QuantityType = "Angle", Unit = "°")]
        public double EXCAMCMD => _frame.EXCAMCMD;
        /// <summary>
        /// Direct Injection Fuel Pressure in bar.
        /// </summary>
        [SensorMetadata("DIFP", Description = "Direct Injection Fuel Pressure", QuantityType = "Pressure", Unit = "bar")]
        public double DIFP => _frame.DIFP;
        /// <summary>
        /// Waste gate opening in mm.
        /// </summary>
        [SensorMetadata("Waste Gate", Description = "Waste gate opening", QuantityType = "Length", Unit = "mm")]
        public double WG => _frame.WG;
        /// <summary>
        /// Waste gate opening in mm.
        /// </summary>
        [SensorMetadata("WGCMD", Description = "Waste gate opening", QuantityType = "Length", Unit = "mm")]
        public double WGCMD => _frame.WGCMD;
        /// <summary>
        /// Boost pressure CMD in bar.
        /// </summary>
        [SensorMetadata("BP CMD", Description = "Boost pressure CMD", QuantityType = "Pressure", Unit = "bar")]
        public double BPCMD => _frame.BPCMD;
        /// <summary>
        /// Direct Injection Fuel Pressure CMD in bar.
        /// </summary>
        [SensorMetadata("DIFPCMD", Description = "Direct Injection Fuel Pressure CMD", QuantityType = "Pressure", Unit = "bar")]
        public double DIFPCMD => _frame.DIFPCMD;
        /// <summary>
        /// Air Charge in percent.
        /// </summary>
        [SensorMetadata("AIRC", Description = "Air Charge", QuantityType = "Ratio", Unit = "%")]
        public double AIRC => _frame.AIRC;
        /// <summary>
        /// EGR rate in percent.
        /// </summary>
        [SensorMetadata("EGR", Description = "EGR rate", QuantityType = "Ratio", Unit = "%")]
        public double EGR => _frame.EGR;
        /// <summary>
        /// Oil pressure in bar.
        /// </summary>
        [SensorMetadata("Oil Press", Description = "Oil pressure", QuantityType = "Pressure", Unit = "bar")]
        public double OilPress => _frame.OilPress;
        /// <summary>
        /// Catalyst temperature in °C.
        /// </summary>
        [SensorMetadata("Cat.T", Description = "Catalyst temperature", QuantityType = "Temperature", Unit = "°C")]
        public double CatT => _frame.CatT;
        /// <summary>
        /// Knock retard #1 cylinder in degrees.
        /// </summary>
        [SensorMetadata("Knock Retard 1", Description = "Knock retard #1 cylinder", QuantityType = "Angle", Unit = "°")]
        public double KRetard1 => _frame.KRetard1;
        /// <summary>
        /// Knock retard #2 cylinder in degrees.
        /// </summary>
        [SensorMetadata("Knock Retard 2", Description = "Knock retard #2 cylinder", QuantityType = "Angle", Unit = "°")]
        public double KRetard2 => _frame.KRetard2;
        /// <summary>
        /// Knock retard #3 cylinder in degrees.
        /// </summary>
        [SensorMetadata("Knock Retard 3", Description = "Knock retard #3 cylinder", QuantityType = "Angle", Unit = "°")]
        public double KRetard3 => _frame.KRetard3;
        /// <summary>
        /// Knock retard #4 cylinder in degrees.
        /// </summary>
        [SensorMetadata("Knock Retard 4", Description = "Knock retard #4 cylinder", QuantityType = "Angle", Unit = "°")]
        public double KRetard4 => _frame.KRetard4;
        /// <summary>
        /// G Sensor Lateral in g.
        /// </summary>
        [SensorMetadata("G Lat", Description = "G Sensor Lateral", QuantityType = "Acceleration", Unit = "g")]
        public double GLat => _frame.GLat;
        /// <summary>
        /// G Sensor Long in g.
        /// </summary>
        [SensorMetadata("G Long", Description = "G Sensor Long", QuantityType = "Acceleration", Unit = "g")]
        public double GLong => _frame.GLong;
        /// <summary>
        /// Yaw Sensor in °/s.
        /// </summary>
        [SensorMetadata("Yaw", Description = "Yaw Sensor", QuantityType = "RotationalSpeed", Unit = "°/s")]
        public double Yaw => _frame.Yaw;
        /// <summary>
        /// G Sensor Z axis in g.
        /// </summary>
        [SensorMetadata("G Zaxis", Description = "G Sensor Z axis", QuantityType = "Acceleration", Unit = "g")]
        public double GZ => _frame.GZ;
        /// <summary>
        /// Ethanol content in percent.
        /// </summary>
        [SensorMetadata("Ethanol", Description = "Ethanol content", QuantityType = "Ratio", Unit = "%")]
        public double Ethanol => _frame.Ethanol;
        /// <summary>
        /// CVT temperature in °C.
        /// </summary>
        [SensorMetadata("CVT Temp", Description = "CVT temperature", QuantityType = "Temperature", Unit = "°C")]
        public double CVTTemp => _frame.CVTTemp;
        /// <summary>
        /// ABS left front wheel speed in km/h.
        /// </summary>
        [SensorMetadata("ABS.Left Front", Description = "ABS left front wheel speed", QuantityType = "Speed", Unit = "km/h")]
        public double ABSLF => _frame.ABSLF;
        /// <summary>
        /// ABS right front wheel speed in km/h.
        /// </summary>
        [SensorMetadata("ABS.Right Front", Description = "ABS right front wheel speed", QuantityType = "Speed", Unit = "km/h")]
        public double ABSRF => _frame.ABSRF;
        /// <summary>
        /// ABS left rear wheel speed in km/h.
        /// </summary>
        [SensorMetadata("ABS.Left Rear", Description = "ABS left rear wheel speed", QuantityType = "Speed", Unit = "km/h")]
        public double ABSLR => _frame.ABSLR;
        /// <summary>
        /// ABS right rear wheel speed in km/h.
        /// </summary>
        [SensorMetadata("ABS.Right Rear", Description = "ABS right rear wheel speed", QuantityType = "Speed", Unit = "km/h")]
        public double ABSRR => _frame.ABSRR;
        /// <summary>
        /// Clutch pedal position in percent.
        /// </summary>
        [SensorMetadata("Clutch Pos", Description = "Clutch pedal position", QuantityType = "Ratio", Unit = "%")]
        public double ClutchPos => _frame.ClutchPos;
        /// <summary>
        /// Brake pressure in bar.
        /// </summary>
        [SensorMetadata("Brake Press", Description = "Brake pressure", QuantityType = "Pressure", Unit = "bar")]
        public double BrakePress => _frame.BrakePress;
        /// <summary>
        /// Steering wheel angle in degrees.
        /// </summary>
        [SensorMetadata("Steer Ang", Description = "Steering wheel angle", QuantityType = "Angle", Unit = "°")]
        public double SteerAng => _frame.SteerAng;
        /// <summary>
        /// Steering wheel torque in N·m.
        /// </summary>
        [SensorMetadata("Steer Trq", Description = "Steering wheel torque", QuantityType = "Torque", Unit = "N·m")]
        public double SteerTrq => _frame.SteerTrq;
        /// <summary>
        /// Fuel pump duty cycle in percent.
        /// </summary>
        [SensorMetadata("Fuel Pump Duty", Description = "Fuel pump duty cycle", QuantityType = "Ratio", Unit = "%")]
        public double FuelP => _frame.FuelP;
        /// <summary>
        /// Air flow meter frequency in Hz.
        /// </summary>
        [SensorMetadata("AFM Hz", Description = "Air flow meter frequency", QuantityType = "Frequency ", Unit = "Hz")]
        public double AFMHz => _frame.AFMHz;
        /// <summary>
        /// Injector duty cycle in percent.
        /// </summary>
        [SensorMetadata("DUTY", Description = "Injector duty cycle", QuantityType = "Ratio", Unit = "%")]
        public double DUTY => RPM * INJ / 1200;
        /// <summary>
        /// Knock count.
        /// </summary>
        [SensorMetadata("Knock count", Description = "Knock count", QuantityType = "ScalarUnit", Unit = "")]
        public double KCount => KCount1 + KCount2 + KCount3 + KCount4 + KCount5 + KCount6;
        /// <summary>
        /// Total fuel trim in lambda.
        /// </summary>
        [SensorMetadata("Use", Description = "Total fuel trim", QuantityType = "Ratio", Unit = "%")]
        public double Trim => LTRIM + STRIM;

        public IReadOnlyDictionary<FlashProReadinessTests, FlashProReadinessCode> ReadinessCodes => throw new NotImplementedException();
        public IReadOnlyCollection<FlashProFaultCode> FaultCodes => throw new NotImplementedException();

        IReadOnlyCollection<IFaultCode> IDatalogFrame.FaultCodes => throw new NotImplementedException();

        internal static FlashProDatalogFrame ReadFromStream(Stream stream, int frameSize)
        {
            return new()
            {
                _frame = stream.ReadStruct<DatalogFrame>(0, frameSize)
            };
        }

        internal int Save(Stream stream, int frameNumber, int frameSize)
        {
            _frame.FrameNumber = frameNumber;

            return stream.WriteStruct(_frame, offset: 0, length: frameSize);
        }
    }
}
