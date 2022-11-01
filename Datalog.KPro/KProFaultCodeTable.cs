using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;

using HondataDotNet.Datalog.OBDII;

namespace HondataDotNet.Datalog.KPro
{
    public sealed class KProFaultCodeTable : IReadOnlyDictionary<BigInteger, KProFaultCode>
    {
        private static readonly Lazy<Dictionary<BigInteger, Lazy<KProFaultCode>>> LazyTable = new(() =>
        {
            return new()
            {
                [BigInteger.Parse("0000000000000000000000000000000000000001", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 0")),
                [BigInteger.Parse("0000000000000000000000000000000000000002", NumberStyles.HexNumber)] = new(() => new(DTC.P1607, "0", "ECU Internal Circuit Malfunction (check calibration type)")),
                [BigInteger.Parse("0000000000000000000000000000000000000004", NumberStyles.HexNumber)] = new(() => new(DTC.P0131, "1-1", "02 Sensor Voltage Low")),
                [BigInteger.Parse("0000000000000000000000000000000000000008", NumberStyles.HexNumber)] = new(() => new(DTC.P0132, "1-1", "02 Sensor Voltage High")),
                [BigInteger.Parse("0000000000000000000000000000000000000010", NumberStyles.HexNumber)] = new(() => new(DTC.P0107, "3-1", "MAP Sensor Low Voltage")),
                [BigInteger.Parse("0000000000000000000000000000000000000020", NumberStyles.HexNumber)] = new(() => new(DTC.P0108, "3-2", "MAP Sensor High Voltage")),
                [BigInteger.Parse("0000000000000000000000000000000000000040", NumberStyles.HexNumber)] = new(() => new(DTC.P0335, "41", "CRK Sensor No Signal")),
                [BigInteger.Parse("0000000000000000000000000000000000000080", NumberStyles.HexNumber)] = new(() => new(DTC.P0336, "42", "CRK Sensor Intermittent Interruption")),
                [BigInteger.Parse("0000000000000000000000000000000000000100", NumberStyles.HexNumber)] = new(() => new(DTC.P1128, "5-1", "MAP Sensor Signal Lower Than Expected")),
                [BigInteger.Parse("0000000000000000000000000000000000000200", NumberStyles.HexNumber)] = new(() => new(DTC.P1129, "5-2", "MAP Sensor Signal Higher Than Expected")),
                [BigInteger.Parse("0000000000000000000000000000000000000400", NumberStyles.HexNumber)] = new(() => new(DTC.P0117, "6-1", "ECT Sensor Low Voltage")),
                [BigInteger.Parse("0000000000000000000000000000000000000800", NumberStyles.HexNumber)] = new(() => new(DTC.P0118, "6-2", "ECT Sensor High Voltage")),
                [BigInteger.Parse("0000000000000000000000000000000000001000", NumberStyles.HexNumber)] = new(() => new(DTC.P0122, "7-1", "TPS Low Voltage")),
                [BigInteger.Parse("0000000000000000000000000000000000002000", NumberStyles.HexNumber)] = new(() => new(DTC.P0123, "7-2", "TPS High Voltage")),
                [BigInteger.Parse("0000000000000000000000000000000000004000", NumberStyles.HexNumber)] = new(() => new(DTC.P1121, "7-3", "TPS Signal Lower Than Expected")),
                [BigInteger.Parse("0000000000000000000000000000000000008000", NumberStyles.HexNumber)] = new(() => new(DTC.P1122, "7-4", "TPS Signal Higher Than Expected")),
                [BigInteger.Parse("0000000000000000000000000000000000010000", NumberStyles.HexNumber)] = new(() => new(DTC.P0176, "1514", "Ethanol sensor input error")),
                [BigInteger.Parse("0000000000000000000000000000000000020000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "1523", "No description available")),
                [BigInteger.Parse("0000000000000000000000000000000000040000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "1514", "No description available")),
                [BigInteger.Parse("0000000000000000000000000000000000080000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "1523", "No description available")),
                [BigInteger.Parse("0000000000000000000000000000000000100000", NumberStyles.HexNumber)] = new(() => new(DTC.P1362, "8-2", "TDC Sensor No Signal")),
                [BigInteger.Parse("0000000000000000000000000000000000200000", NumberStyles.HexNumber)] = new(() => new(DTC.P1361, "8-1", "TDC Sensor Intermittent Signal Interruption")),
                [BigInteger.Parse("0000000000000000000000000000000000400000", NumberStyles.HexNumber)] = new(() => new(DTC.P0112, "10", "IAT Sensor Low Voltage")),
                [BigInteger.Parse("0000000000000000000000000000000000800000", NumberStyles.HexNumber)] = new(() => new(DTC.P0113, "10", "IAT Sensor High Voltage")),
                [BigInteger.Parse("0000000000000000000000000000000001000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1213, "11", "IMA sensor voltage low")),
                [BigInteger.Parse("0000000000000000000000000000000002000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1214, "11", "IMA sensor voltage high")),
                [BigInteger.Parse("0000000000000000000000000000000004000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "12", "EGR lift sensor voltage error")),
                [BigInteger.Parse("0000000000000000000000000000000008000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "12", "EGR F/B error")),
                [BigInteger.Parse("0000000000000000000000000000000010000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1107, "13-1", "Barometric Pressure Low Voltage")),
                [BigInteger.Parse("0000000000000000000000000000000020000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1108, "13-2", "Barometric Pressure High Voltage")),
                [BigInteger.Parse("0000000000000000000000000000000040000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1106, "13-3", "Barometric Pressure Sensor Range/Performance")),
                [BigInteger.Parse("0000000000000000000000000000000080000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 37")),
                [BigInteger.Parse("0000000000000000000000000000000100000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0505, "14-2", "Idle Control System Malfunction")),
                [BigInteger.Parse("0000000000000000000000000000000200000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1519, "14-3", "Idle Air Control Valve Circuit Malfunction")),
                [BigInteger.Parse("0000000000000000000000000000000400000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0500, "17-1", "VSS Malfunction")),
                [BigInteger.Parse("0000000000000000000000000000000800000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "17-2", "VSP")),
                [BigInteger.Parse("0000000000000000000000000000001000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1297, "20-1", "ELD Circuit Low Voltage")),
                [BigInteger.Parse("0000000000000000000000000000002000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1298, "20-2", "ELD Circuit High Voltage")),
                [BigInteger.Parse("0000000000000000000000000000004000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1253, "21", "VTEC solenoid drive time road abnormality")),
                [BigInteger.Parse("0000000000000000000000000000008000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 47")),
                [BigInteger.Parse("0000000000000000000000000000010000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1259, "22-4", "VTEC System Malfunction (Oil Pressure Switch)")),
                [BigInteger.Parse("0000000000000000000000000000020000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 51")),
                [BigInteger.Parse("0000000000000000000000000000040000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0325, "23-1", "Knock Sensor Circuit Malfunction")),
                [BigInteger.Parse("0000000000000000000000000000080000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1324, "23-2", "Knock Sensor Circuit Failure")),
                [BigInteger.Parse("0000000000000000000000000000100000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0563, "34-2", "VBU Voltage High")),
                [BigInteger.Parse("0000000000000000000000000000200000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 55")),
                [BigInteger.Parse("0000000000000000000000000000400000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 56")),
                [BigInteger.Parse("0000000000000000000000000000800000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 57")),
                [BigInteger.Parse("0000000000000000000000000001000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1162, "48", "Air Fuel Ratio Sensor Circuit Malfunction")),
                [BigInteger.Parse("0000000000000000000000000002000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "48", "LAF Sensor AFV 1 short")),
                [BigInteger.Parse("0000000000000000000000000004000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "48", "LAF Sensor AFC 1 short")),
                [BigInteger.Parse("0000000000000000000000000008000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 63")),
                [BigInteger.Parse("0000000000000000000000000010000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Daughter board communication error (battery voltage low when cranking)")),
                [BigInteger.Parse("0000000000000000000000000020000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 65")),
                [BigInteger.Parse("0000000000000000000000000040000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0600, "39-1", "Multiplexer not present (for engine swaps disable in misc parameters)")),
                [BigInteger.Parse("0000000000000000000000000080000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 67")),
                [BigInteger.Parse("0000000000000000000000000100000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1505, "109", "PCV negative pressure")),
                [BigInteger.Parse("0000000000000000000000000200000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 71")),
                [BigInteger.Parse("0000000000000000000000000400000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 72")),
                [BigInteger.Parse("0000000000000000000000000800000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Idle valve")),
                [BigInteger.Parse("0000000000000000000000001000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0135, "41", "Primary Heated Oxygen Sensor (Primary HO2S) (Sensor 1) Heater Circuit Malfunction")),
                [BigInteger.Parse("0000000000000000000000002000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1166, "41-1", "Heated Oxygen Sensor Sensor 1 (Primary HO2S) Heater Circuit Malfunction")),
                [BigInteger.Parse("0000000000000000000000004000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1167, "41", "Heated Oxygen Sensor Sensor 1 (Primary LAF HO2S) Heater System Malfunction")),
                [BigInteger.Parse("0000000000000000000000008000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0134, null, "Air/Fuel Ratio (A/F) Sensor (Sensor 1) No Activity Detected")),
                [BigInteger.Parse("0000000000000000000000010000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0172, "45", "Fuel System Too Rich")),
                [BigInteger.Parse("0000000000000000000000020000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0171, "45", "Fuel System Too Lean")),
                [BigInteger.Parse("0000000000000000000000040000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1162, null, "Air/Fuel Ratio (A/F) Sensor (Sensor 1) Slow Response")),
                [BigInteger.Parse("0000000000000000000000080000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 83")),
                [BigInteger.Parse("0000000000000000000000100000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0010, "56", "VTC Oil Control Solenoid Valve Malfunction")),
                [BigInteger.Parse("0000000000000000000000200000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0011, "56", "VTC System Malfunction")),
                [BigInteger.Parse("0000000000000000000000400000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0340, "57-1", "CMP Sensor No Signal")),
                [BigInteger.Parse("0000000000000000000000800000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0344, "57-2", "CMP Sensor Intermittent Interruption")),
                [BigInteger.Parse("0000000000000000000001000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0341, "57-3", "Variable Valve Timing (VTC) Phase Gap")),
                [BigInteger.Parse("0000000000000000000002000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 91")),
                [BigInteger.Parse("0000000000000000000008000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1163, "61", "Air Fuel Ratio Sensor Slow Response")),
                [BigInteger.Parse("0000000000000000000010000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1164, "61", "Air Fuel Ratio Sensor Range/Performance Problem")),
                [BigInteger.Parse("0000000000000000000040000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0137, "63", "Secondary 02 Circuit Low Voltage")),
                [BigInteger.Parse("0000000000000000000080000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0138, "63", "Secondary 02 Circuit High Voltage")),
                [BigInteger.Parse("0000000000000000000100000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0139, "63", "Secondary 02 Circuit Slow Response")),
                [BigInteger.Parse("0000000000000000000200000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 101")),
                [BigInteger.Parse("0000000000000000000400000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0141, "65-2", "Secondary 02 Circuit Heater Circuit Malfunction")),
                [BigInteger.Parse("0000000000000000000800000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 103")),
                [BigInteger.Parse("0000000000000000001000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0420, "67-1", "Catalyst System Efficiency Below Threshold")),
                [BigInteger.Parse("0000000000000000002000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "67-2", "Catalyst / Secondary o2 Sensor")),
                [BigInteger.Parse("0000000000000000004000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 106")),
                [BigInteger.Parse("0000000000000000008000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 107")),
                [BigInteger.Parse("0000000000000000010000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0301, "71", "Cylinder #1 Misfire")),
                [BigInteger.Parse("0000000000000000020000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 111")),
                [BigInteger.Parse("0000000000000000040000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0302, "72", "Cylinder #2 Misfire")),
                [BigInteger.Parse("0000000000000000100000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0303, "73", "Cylinder #3 Misfire")),
                [BigInteger.Parse("0000000000000000200000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 115")),
                [BigInteger.Parse("0000000000000000400000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0304, "74", "Cylinder #4 Misfire")),
                [BigInteger.Parse("0000000000000000800000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 117")),
                [BigInteger.Parse("0000000000000001000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0300, null, "Random Misfire")),
                [BigInteger.Parse("0000000000000002000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 121")),
                [BigInteger.Parse("0000000000000004000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "80", "EGR flow")),
                [BigInteger.Parse("0000000000000008000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 123")),
                [BigInteger.Parse("0000000000000010000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0116, "86", "ECT Sensor Range/Performance Problem")),
                [BigInteger.Parse("0000000000000020000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 125")),
                [BigInteger.Parse("0000000000000040000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0128, "87", "Cooling System Malfunction (Thermostat)")),
                [BigInteger.Parse("0000000000000080000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "VTC solenoid")),
                [BigInteger.Parse("0000000000000100000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1456, "90-1", "EVAP Control System Leakage (Fuel Tank)")),
                [BigInteger.Parse("0000000000000200000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1457, "90-2", "EVAP Control System Leakage (Canister System)")),
                [BigInteger.Parse("0000000000000400000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0452, "91-1", "Fuel Tank Pressure Sensor Low Voltage")),
                [BigInteger.Parse("0000000000000800000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0453, "91-2", "Fuel Tank Pressure Sensor High Voltage")),
                [BigInteger.Parse("0000000000001000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0451, "91-3", "Fuel Tank Pressure Sensor Sensor Range/Performance")),
                [BigInteger.Parse("0000000000004000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0443, "92", "EVAP purge valve circuit")),
                [BigInteger.Parse("0000000000008000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 137")),
                [BigInteger.Parse("0000000000010000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "95", "PF2 low")),
                [BigInteger.Parse("0000000000020000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "95", "PF2 high")),
                [BigInteger.Parse("0000000000040000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "95", "PF2 monitor")),
                [BigInteger.Parse("0000000000100000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "96", "TF2 low")),
                [BigInteger.Parse("0000000000200000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "96", "TF2 high")),
                [BigInteger.Parse("0000000000400000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "97", "PFO low")),
                [BigInteger.Parse("0000000000800000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "97", "PFO high")),
                [BigInteger.Parse("0000000001000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "98", "TFO low")),
                [BigInteger.Parse("0000000002000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "98", "TFO high")),
                [BigInteger.Parse("0000000004000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "103", "3rd 02 sensor voltage low")),
                [BigInteger.Parse("0000000008000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "103", "3rd 02 sensor voltage high")),
                [BigInteger.Parse("0000000010000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "103", "3rd 02 sensor response")),
                [BigInteger.Parse("0000000020000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 155")),
                [BigInteger.Parse("0000000040000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "104", "3rd 02 sensor heater current abnormality")),
                [BigInteger.Parse("0000000080000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 157")),
                [BigInteger.Parse("0000000100000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "105", "NOx ability monitor")),
                [BigInteger.Parse("0000000200000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 161")),
                [BigInteger.Parse("0000000400000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1077, "106-1", "Intake Manifold Runner Control System Malfunction (low rpm)")),
                [BigInteger.Parse("0000000800000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P1078, "106-2", "Intake Manifold Runner Control System Malfunction (high rpm)")),
                [BigInteger.Parse("0000001000000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0661, "107-1", "Intake Manifold Runner Control Low Voltage")),
                [BigInteger.Parse("0000002000000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0662, "107-2", "Intake Manifold Runner Control High Voltage")),
                [BigInteger.Parse("0000004000000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, "108", "P1 switch abnormality")),
                [BigInteger.Parse("0000008000000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.Unknown, null, "Unknown error 167")),
                [BigInteger.Parse("0000010000000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0219, "119", "Engine over-rev")),
                [BigInteger.Parse("0000020000000000000000000000000000000000", NumberStyles.HexNumber)] = new(() => new(DTC.P0217, null, "Engine over-heating"))
            };
        });

        private static readonly Lazy<ILookup<DTC, BigInteger>> LazyDTCIndex = new(() =>
        {
            return LazyTable.Value.ToLookup(k => k.Value.Value.DTC, v => v.Key);
        });

        private static readonly Lazy<IDictionary<string, ILookup<string, BigInteger>>> LazyCELCodeIndex = new(() =>
        {
            return (
                from row in LazyTable.Value
                where row.Value.Value.CELCode != null
                group row by row.Value.Value.CELMainCode into grouping
                select new { CELMainCode = grouping.Key, Lookup = grouping.ToLookup(k => k.Value.Value.CELSubCode, v => v.Key) })
                .ToDictionary(k => k.CELMainCode, v => v.Lookup);
        });

        private static readonly Lazy<KProFaultCodeTable> LazyInstance = new(() => new());

        private KProFaultCodeTable()
        {
        }

        public static KProFaultCodeTable Instance => LazyInstance.Value;

        public KProFaultCode this[BigInteger flag]
        {
            get
            {
                return LazyTable.Value[flag].Value;
            }
        }

        public int Count => LazyTable.Value.Count;

        public IEnumerable<BigInteger> Keys => LazyTable.Value.Keys;

        public IEnumerable<KProFaultCode> Values => LazyTable.Value.Values.Select(lazyFaultCode => lazyFaultCode.Value);

        public bool ContainsKey(BigInteger key) => LazyTable.Value.ContainsKey(key);

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        public bool TryGetValue(BigInteger key, [MaybeNullWhen(false)] out KProFaultCode value)
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn't match implicitly implemented member (possibly because of nullability attributes).
        {
            if (LazyTable.Value.TryGetValue(key, out var lazyValue))
            {
                value = lazyValue.Value;
                return true;
            }
            value = null;
            return false;
        }

        public IEnumerator<KeyValuePair<BigInteger, KProFaultCode>> GetEnumerator()
        {
            foreach (var (key, lazyValue) in LazyTable.Value)
            {
                yield return new(key, lazyValue.Value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public static IEnumerable<KProFaultCode> LookupDTC(DTC dtc)
        {
            return LookupDTCIndex(dtc).Select(faultCodeFlag => LazyTable.Value[faultCodeFlag].Value);
        }

        public static IEnumerable<KProFaultCode> LookupCELCode(string celCode)
        {
            return LookupCELCodeIndex(celCode).Select(faultCodeFlag => LazyTable.Value[faultCodeFlag].Value);
        }

        internal static IEnumerable<BigInteger> LookupDTCIndex(DTC dtc)
        {
            return LazyDTCIndex.Value[dtc];
        }

        internal static IEnumerable<BigInteger> LookupCELCodeIndex(string celCode)
        {
            if (celCode == null)
                throw new ArgumentNullException(nameof(celCode));

            string[] splitCode = celCode.Split("-", StringSplitOptions.RemoveEmptyEntries);

            if (splitCode?.Length == 2)
            {
                return LookupCELCodeIndex(splitCode[0], splitCode[1]);
            }
            return LazyCELCodeIndex.Value[celCode].SelectMany(lookup => lookup);
        }

        internal static IEnumerable<BigInteger> LookupCELCodeIndex(string celMainCode, string celSubCode)
        {
            if (celMainCode == null)
                throw new ArgumentNullException(nameof(celMainCode));
            if (celSubCode == null)
                throw new ArgumentNullException(nameof(celSubCode));

            return LazyCELCodeIndex.Value[celMainCode][celSubCode];
        }
    }
}
