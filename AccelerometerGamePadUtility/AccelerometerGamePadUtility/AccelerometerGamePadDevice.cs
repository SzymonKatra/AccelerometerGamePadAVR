using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AccelerometerGamePadUtility
{
    public class AccelerometerGamePadDevice : IDisposable
    {
        private IntPtr m_ioHandle;

        private bool m_disposed = false;

        public const int MyVid = 0x16C0;
        public const int MyPid = 0x27DC;

        public static bool IsDeviceConnected()
        {
            IntPtr device = FindDevice();
            if (device == NativeMethods.InvalidHandleValue) return false;

            NativeMethods.CloseHandle(device);
            return true;
        }
        public static AccelerometerGamePadDevice OpenDevice()
        {
            AccelerometerGamePadDevice result = null;

            //HidDevice device = HidDevices.Enumerate(MyVid, MyPid).FirstOrDefault();
            //if (device == null) throw new DeviceNotFoundException(string.Format("AccelerometerGamePad with VID=0x{0} PID=0x{1} not found", MyVid.ToString("X"), MyPid.ToString("X")));

            //result = new AccelerometerGamePadDevice { m_device = device };
            //result.m_ioHandle = OpenDeviceIo(result.m_device.DevicePath);
            IntPtr device = FindDevice();
            if (device == NativeMethods.InvalidHandleValue) throw new DeviceNotFoundException("Error occurred while opening IO for AccelerometerGamePad");
            result = new AccelerometerGamePadDevice() { m_ioHandle = device };

            return result;
        }
        private unsafe static IntPtr FindDevice()
        {
            IntPtr currentHandle = NativeMethods.InvalidHandleValue;
            Guid guid = new Guid();
            NativeMethods.HidD_GetHidGuid(ref guid);
            NativeMethods.SP_DEVICE_INTERFACE_DATA deviceInfo = new NativeMethods.SP_DEVICE_INTERFACE_DATA();
            deviceInfo.cbSize = sizeof(NativeMethods.SP_DEVICE_INTERFACE_DATA);
            NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA deviceDetails = new NativeMethods.SP_DEVICE_INTERFACE_DETAIL_DATA();        
            
            IntPtr deviceInfoList = NativeMethods.SetupDiGetClassDevs(ref guid, null, 0, NativeMethods.DIGCF_PRESENT | NativeMethods.DIGCF_DEVICEINTERFACE);

            for (int i = 0; ; i++)
            {
                if (currentHandle != NativeMethods.InvalidHandleValue)
                {
                    NativeMethods.CloseHandle(currentHandle);
                    currentHandle = NativeMethods.InvalidHandleValue;
                }

                if (!NativeMethods.SetupDiEnumDeviceInterfaces(deviceInfoList, IntPtr.Zero, ref guid, i, ref deviceInfo)) break; // no more entries

                int size = 0;
                deviceDetails.Size = IntPtr.Size == 4 ? 4 + Marshal.SystemDefaultCharSize : 8;
                NativeMethods.SetupDiGetDeviceInterfaceDetailBuffer(deviceInfoList, ref deviceInfo, IntPtr.Zero, 0, ref size, IntPtr.Zero);
                NativeMethods.SetupDiGetDeviceInterfaceDetail(deviceInfoList, ref deviceInfo, ref deviceDetails, size, ref size, IntPtr.Zero);

                currentHandle = NativeMethods.CreateFile(deviceDetails.DevicePath, NativeMethods.GENERIC_READ | NativeMethods.GENERIC_WRITE, NativeMethods.FILE_SHARE_READ | NativeMethods.FILE_SHARE_WRITE, IntPtr.Zero, NativeMethods.OPEN_EXISTING, 0, 0);
                if(currentHandle!= NativeMethods.InvalidHandleValue)
                {
                    NativeMethods.HIDD_ATTRIBUTES deviceAttributes = new NativeMethods.HIDD_ATTRIBUTES();
                    deviceAttributes.Size = sizeof(NativeMethods.HIDD_ATTRIBUTES);
                    NativeMethods.HidD_GetAttributes(currentHandle, ref deviceAttributes);
                    if (deviceAttributes.VendorID == MyVid && deviceAttributes.ProductID == MyPid) break; // device found
                }
            }

            NativeMethods.SetupDiDestroyDeviceInfoList(deviceInfoList);

            return currentHandle;
        }
        public void CloseDevice()
        {
            Dispose();
        }

        public void SelectAxis(AxisType axis)
        {
            SelectAxis(new SelectedAxisReport() { Axis = axis });
        }
        public unsafe void SelectAxis(SelectedAxisReport report)
        {
            BuildAndSend(report, sizeof(SelectedAxisReport), ReportType.ReportSelectAxis);
        }
        public unsafe SelectedAxisReport GetSelectedAxis()
        {
            return ReceiveAndBuild<SelectedAxisReport>(ReportType.ReportGetSelectedAxis, sizeof(SelectedAxisReport));
        }

        public void SelectProfile(byte profile)
        {
            SelectProfile(new SelectedProfileReport() { Profile = profile });
        }
        public unsafe void SelectProfile(SelectedProfileReport report)
        {
            BuildAndSend(report, sizeof(SelectedProfileReport), ReportType.ReportSelectProfile);
        }
        public unsafe SelectedProfileReport GetSelectedProfile()
        {
            return ReceiveAndBuild<SelectedProfileReport>(ReportType.ReportGetSelectedProfile, sizeof(SelectedProfileReport));
        }

        public unsafe void SaveSettings()
        {
            BuildAndSend(new SaveProfileReport(), sizeof(SaveProfileReport), ReportType.ReportSaveProfile);
        }
        public unsafe void SetSettings(SettingsReport report)
        {
            BuildAndSend(report, sizeof(SettingsReport), ReportType.ReportSetSettings);
        }
        public unsafe SettingsReport GetSettings()
        {
            return ReceiveAndBuild<SettingsReport>(ReportType.ReportGetSettings, sizeof(SettingsReport));
        }

        public unsafe void SaveCallibration()
        {
            BuildAndSend(new SaveCallibrationReport(), sizeof(SaveCallibrationReport), ReportType.ReportSaveCallibration);
        }
        public unsafe void SetCallibration(CallibrationReport report)
        {
            BuildAndSend(report, sizeof(CallibrationReport), ReportType.ReportSetCallibration);
        }
        public unsafe CallibrationReport GetCallibration()
        {
            return ReceiveAndBuild<CallibrationReport>(ReportType.ReportGetCallibration, sizeof(CallibrationReport));
        }

        public unsafe RawAxisReport GetRawAxis()
        {
            return ReceiveAndBuild<RawAxisReport>(ReportType.ReportGetRawAxis, sizeof(RawAxisReport));
        }

        private bool SetOutputReport(byte[] data)
        {
            return NativeMethods.HidD_SetOutputReport(m_ioHandle, data, data.Length);
        }
        private bool GetInputReport(byte[] data)
        {
            return NativeMethods.HidD_GetInputReport(m_ioHandle, data, data.Length);
        }

        private void BuildAndSend(object structure, int size, ReportType report)
        {
            byte[] array = new byte[size + 1];
            array[0] = (byte)report;
            MarshalExt.StructureToByteArray(structure, array, 1, size);
            if (!SetOutputReport(array)) throw new Exception("Error accessing device");
        }
        private T ReceiveAndBuild<T>(ReportType report, int size)
        {
            byte[] array = new byte[size + 1];
            array[0] = (byte)report;
            if (!GetInputReport(array)) throw new Exception("Error accessing device");
            return MarshalExt.ByteArrayToStructure<T>(array, 1, size);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!m_disposed)
            {
                if (disposing)
                {
                    //managed
                }

                if (m_ioHandle != NativeMethods.InvalidHandleValue) NativeMethods.CloseHandle(m_ioHandle);

                //unmanaged
                m_disposed = true;
            }
        }
        ~AccelerometerGamePadDevice()
        {
            Dispose(false);
        }
    }
}
