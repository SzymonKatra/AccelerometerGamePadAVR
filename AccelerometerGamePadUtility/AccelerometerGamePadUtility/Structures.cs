using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace AccelerometerGamePadUtility
{
    public static class MarshalExt
    {
        public static byte[] StructureToByteArray(object structure)
        {
            int len = Marshal.SizeOf(structure);
            byte[] result = new byte[len];
            StructureToByteArray(structure, result, 0, len);
            return result;
        }
        public static void StructureToByteArray(object structure, byte[] target, int startIndex, int length)
        {
            IntPtr ptr = Marshal.AllocHGlobal(length);
            Marshal.StructureToPtr(structure, ptr, true);
            Marshal.Copy(ptr, target, startIndex, length);
            Marshal.FreeHGlobal(ptr);
        }
        public static T ByteArrayToStructure<T>(byte[] array, int startIndex, int length)
        {
            IntPtr ptr = Marshal.AllocHGlobal(length);
            Marshal.Copy(array, startIndex, ptr, length);
            T result = (T) Marshal.PtrToStructure(ptr, typeof (T));
            Marshal.FreeHGlobal(ptr);
            return result;
        }
    }

    [Flags]
    public enum AxisSettingsFlags : byte
    {
        Truncate = 1 << 0,
        Invert = 1 << 1,
    }

    public enum AxisType : byte
    {
        AccelerometerHorizontal = 1,
        AccelerometerVertical = 0,
        LeftAnalogHorizontal = 2,
        LeftAnalogVertical = 3,
        RightAnalogHorizontal = 4,
        RightAnalogVertical = 5,
    }
    public enum ReportType : byte
    {
        ReportGetState = 1,             // device  to  host
        ReportSelectAxis = 10,          // host    to  device
        ReportGetSelectedAxis = 11,     // device  to  host
        ReportSelectProfile = 12,       // host    to  device
        ReportGetSelectedProfile = 13,  // device  to  host
        ReportSaveCallibration = 18,    // host    to  device
        ReportSaveProfile = 19,         // host    to  device
        ReportSetSettings = 20,         // host    to  device
        ReportGetSettings = 21,         // device  to  host
        ReportSetCallibration = 22,     // host    to  device
        ReportGetCallibration = 23,     // device  to  host
        ReportGetRawAxis = 50,          // device  to  host
    }
    public enum AxisInterpretation : byte
    {
        InterpretationNull = 0,
        InterpretationXAxis = 1,
        InterpretationYAxis = 2,
        InterpretationZAxis = 3,
        InterpretationXRotation = 4,
        InterpretationYRotation = 5,
        InterpretationZRotation = 6,
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AxisSettings
    {
        public byte ZeroDeadZoneRaw;
        public byte OneDeadZoneRaw;
        public byte Flags;
        public AxisInterpretation Interpretation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AxisCallibration
    {
        public ushort ZeroValue;
        public ushort PlusOneVariation;
        public ushort MinusOneVariation;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Report
    {
        public ushort ButtonsAndDPad;
        public sbyte X;
        public sbyte Y;
        public sbyte Z;
        public sbyte Rx;
        public sbyte Ry;
        public sbyte Rz;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SettingsReport
    {
        public AxisSettings Settings;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SelectedAxisReport
    {
        public AxisType Axis;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SelectedProfileReport
    {
        public byte Profile;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SaveProfileReport
    {
        public byte Dummy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CallibrationReport
    {
        public AxisCallibration Callibration;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SaveCallibrationReport
    {
        public byte Dummy;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct RawAxisReport
    {
        public ushort Value;
    }
}
