using System;
using System.Text;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Runtime.ConstrainedExecution;

namespace OaktreeLab.USBDevice {
    public class HIDSimple : IDisposable {

        /// <summary>
        /// パケットサイズ（IN/OUT共通）
        /// </summary>
        public uint PacketSize {
            get;
            private set;
        }

        /// <summary>
        /// HIDDeviceオブジェクトを作成する
        /// </summary>
        public HIDSimple() {
            PacketSize = 64;
        }

        private IntPtr hDev = IntPtr.Zero;

        /// <summary>
        /// USB HIDデバイスをオープンする
        /// </summary>
        /// <param name="vid">デバイスのVID</param>
        /// <param name="pid">デバイスのPID</param>
        /// <returns>成功した場合true、存在しなかった場合false</returns>
        /// <exception cref="InvalidOperationException">デバイスをオープンできなかった場合</exception>
        public bool Open( uint vid, uint pid ) {
            string path = string.Format( @"\\?\hid#vid_{0,0:x4}&pid_{1,0:x4}", vid, pid );

            Guid guid = new Guid();
            Native.HidD_GetHidGuid( ref guid );

            IntPtr hDeviceInfoSet = Native.SetupDiGetClassDevs( ref guid, 0, IntPtr.Zero, Native.DIGCF_PRESENT | Native.DIGCF_DEVICEINTERFACE );

            Native.SP_DEVICE_INTERFACE_DATA spid = new Native.SP_DEVICE_INTERFACE_DATA();
            spid.cbSize = Marshal.SizeOf( spid );
            int i = 0;
            while ( Native.SetupDiEnumDeviceInterfaces( hDeviceInfoSet, null, ref guid, i, spid ) ) {
                i++;

                Native.SP_DEVINFO_DATA devData = new Native.SP_DEVINFO_DATA();
                devData.cbSize = Marshal.SizeOf( devData );
                int size = 0;
                Native.SetupDiGetDeviceInterfaceDetail( hDeviceInfoSet, spid, IntPtr.Zero, 0, ref size, devData );

                IntPtr buffer = Marshal.AllocHGlobal( size );
                Native.SP_DEVICE_INTERFACE_DETAIL_DATA detailData = new Native.SP_DEVICE_INTERFACE_DETAIL_DATA();
                detailData.cbSize = Marshal.SizeOf( typeof( Native.SP_DEVICE_INTERFACE_DETAIL_DATA ) );
                Marshal.StructureToPtr( detailData, buffer, false );
                Native.SetupDiGetDeviceInterfaceDetail( hDeviceInfoSet, spid, buffer, size, ref size, devData );
                IntPtr pDevicePath = (IntPtr)( (int)buffer + Marshal.SizeOf( typeof( int ) ) );
                string devicePath = Marshal.PtrToStringAuto( pDevicePath );
                Marshal.FreeHGlobal( buffer );

                if ( devicePath.IndexOf( path ) == 0 ) {
                    hDev = Native.CreateFile(
                        devicePath,
                        Native.GENERIC_READ | Native.GENERIC_WRITE,
                        0,
                        IntPtr.Zero,
                        Native.OPEN_EXISTING,
                        0,
                        IntPtr.Zero
                    );
                    break;
                }
            }

            Native.SetupDiDestroyDeviceInfoList( hDeviceInfoSet );

            if ( hDev.ToInt32() <= -1 ) {
                hDev = IntPtr.Zero;
                throw new InvalidOperationException( GetErrorMessage() );
            } else if ( DeviceReady ) {
                return true;
            }
            return false;
        }

        /// <summary>
        /// アンマネージドリソースを開放する
        /// </summary>
        public void Dispose() {
            Close();
        }

        /// <summary>
        /// デバイスをクローズする
        /// </summary>
        public void Close() {
            if ( DeviceReady ) {
                Native.CloseHandle( hDev );
                hDev = IntPtr.Zero;
            }
        }

        ~HIDSimple() {
            Close();
        }

        /// <summary>
        /// デバイスが利用可能かどうかを示す
        /// </summary>
        public bool DeviceReady {
            get {
                if ( hDev.ToInt32() <= -1 ) {
                    return false;
                } else if ( hDev.Equals( IntPtr.Zero ) ) {
                    return false;
                }
                return true;
            }
        }

        private string GetErrorMessage() {
            int errCode = Marshal.GetLastWin32Error();
            StringBuilder message = new StringBuilder( 255 );
            Native.FormatMessage(
                0x00001000,
                IntPtr.Zero,
                (uint)errCode,
                0,
                message,
                message.Capacity,
                IntPtr.Zero
            );
            return message.ToString();
        }

        /// <summary>
        /// デバイスにデータを送信する
        /// </summary>
        /// <param name="data">送信するデータ。PacketSize以下でなければならない。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public void Send( params byte[] data ) {
            if ( !DeviceReady ) {
                throw new InvalidOperationException( "デバイスがオープンされていません" );
            }
            if ( data.Length > PacketSize ) {
                throw new ArgumentOutOfRangeException( "パケットサイズに対してデータが大きすぎます" );
            }

            byte[] buff = new byte[PacketSize + 1];
            Array.Clear( buff, 0, buff.Length );
            Array.Copy( data, 0, buff, 1, data.Length );

            uint written = 0;
            bool result = Native.WriteFile( hDev, buff, (uint)buff.Length, ref written, IntPtr.Zero );
            if ( !result ) {
                throw new InvalidOperationException( GetErrorMessage() );
            }
        }

        /// <summary>
        /// デバイスからデータを受信する
        /// </summary>
        /// <returns>受信したデータ</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public byte[] Receive() {
            if ( !DeviceReady ) {
                throw new InvalidOperationException( "デバイスがオープンされていません" );
            }

            byte[] buff = new byte[PacketSize + 1];
            Array.Clear( buff, 0, buff.Length );
            uint received = 0;
            bool result = Native.ReadFile( hDev, buff, (uint)buff.Length, ref received, IntPtr.Zero );
            if ( !result ) {
                throw new InvalidOperationException( GetErrorMessage() );
            }
            byte[] buff2 = new byte[PacketSize];
            Array.Copy( buff, 1, buff2, 0, buff2.Length );
            return buff2;
        }
    }

    [SecurityPermission( SecurityAction.InheritanceDemand, UnmanagedCode = true )]
    [SecurityPermission( SecurityAction.Demand, UnmanagedCode = true )]
    [SuppressUnmanagedCodeSecurity()]
    static class Native {
        [DllImport( "kernel32.dll" )]
        internal static extern uint FormatMessage(
            uint dwFlags, IntPtr lpSource,
            uint dwMessageId, uint dwLanguageId,
            StringBuilder lpBuffer, int nSize,
            IntPtr Arguments
        );

        [DllImport( "hid.dll", SetLastError = true )]
        internal static extern void HidD_GetHidGuid(
            ref Guid lpHidGuid
        );

        [DllImport( "hid.dll", SetLastError = true )]
        internal static extern bool HidD_GetAttributes(
            IntPtr hDevice,
            out HIDD_ATTRIBUTES Attributes
        );

        [DllImport( "hid.dll", SetLastError = true )]
        internal static extern bool HidD_GetPreparsedData(
            IntPtr hDevice,
            out IntPtr hData
        );

        [DllImport( "hid.dll", SetLastError = true )]
        internal static extern bool HidD_FreePreparsedData(
            IntPtr hData
        );

        [DllImport( "hid.dll", SetLastError = true )]
        internal static extern bool HidP_GetCaps(
            IntPtr hData,
            out HIDP_CAPS capabilities
        );

        [DllImport( "hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall )]
        internal static extern bool HidD_GetFeature(
            IntPtr hDevice,
            IntPtr hReportBuffer,
            uint ReportBufferLength
        );

        [DllImport( "hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall )]
        internal static extern bool HidD_SetFeature(
            IntPtr hDevice,
            IntPtr ReportBuffer,
            uint ReportBufferLength
        );

        [DllImport( "hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall )]
        internal static extern bool HidD_GetProductString(
            IntPtr hDevice,
            IntPtr Buffer,
            uint BufferLength
        );

        [DllImport( "hid.dll", SetLastError = true, CallingConvention = CallingConvention.StdCall )]
        internal static extern bool HidD_GetSerialNumberString(
            IntPtr hDevice,
            IntPtr Buffer,
            uint BufferLength
        );

        [DllImport( "setupapi.dll", SetLastError = true )]
        internal static extern IntPtr SetupDiGetClassDevs(
            ref Guid classGuid,
            int enumerator,
            IntPtr hwndParent,
            int flags
        );

        [DllImport( "setupapi.dll", SetLastError = true )]
        internal static extern bool SetupDiDestroyDeviceInfoList(
            IntPtr deviceInfoSet
        );

        [DllImport( "setupapi.dll", SetLastError = true, CharSet = CharSet.Auto )]
        internal static extern bool SetupDiEnumDeviceInterfaces(
            IntPtr deviceInfoSet,
            SP_DEVINFO_DATA deviceInfoData,
            ref Guid interfaceClassGuid,
            int memberIndex,
            SP_DEVICE_INTERFACE_DATA deviceInterfaceData
        );

        [DllImport( "setupapi.dll" )]
        internal static extern bool SetupDiOpenDeviceInfo(
            IntPtr deviceInfoSet,
            string deviceInstanceId,
            IntPtr hwndParent,
            int openFlags,
            SP_DEVINFO_DATA deviceInfoData
         );

        [DllImport( "setupapi.dll", SetLastError = true, CharSet = CharSet.Auto )]
        internal static extern bool SetupDiGetDeviceInterfaceDetail(
            IntPtr deviceInfoSet,
            SP_DEVICE_INTERFACE_DATA deviceInterfaceData,
            IntPtr deviceInterfaceDetailData,
            int deviceInterfaceDetailDataSize,
            ref int requiredSize,
            SP_DEVINFO_DATA deviceInfoData
        );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
        [DllImport( "kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true )]
        internal static extern IntPtr CreateFile(
            string lpFileName,
            uint dwDesiredAccess,
            uint dwShareMode,
            IntPtr SecurityAttributes,
            uint dwCreationDisposition,
            uint dwFlagsAndAttributes,
            IntPtr hTemplateFile
        );

        [ReliabilityContract( Consistency.WillNotCorruptState, Cer.MayFail )]
        [DllImport( "kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall, SetLastError = true )]
        internal static extern bool CloseHandle(
            IntPtr hHandle
        );

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern bool ReadFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToRead,
            ref uint lpNumberOfBytesRead, 
            IntPtr lpOverlapped
        );

        [DllImport( "kernel32.dll", SetLastError = true )]
        internal static extern bool WriteFile(
            IntPtr hFile,
            byte[] lpBuffer,
            uint nNumberOfBytesToWrite,
            ref uint lpNumberOfBytesWritten,
            IntPtr lpOverlapped
        );

        [StructLayout( LayoutKind.Sequential )]
        internal class SP_DEVICE_INTERFACE_DATA {
            internal int cbSize = Marshal.SizeOf( typeof( SP_DEVICE_INTERFACE_DATA ) );
            internal Guid interfaceClassGuid = Guid.Empty;
            internal int flags = 0;
            internal int reserved = 0;
        }

        [StructLayout( LayoutKind.Sequential )]
        internal class SP_DEVINFO_DATA {
            internal int cbSize = Marshal.SizeOf( typeof( SP_DEVINFO_DATA ) );
            internal Guid classGuid = Guid.Empty;
            internal int devInst = 0;
            internal int reserved = 0;
        }

        [StructLayout( LayoutKind.Sequential, Pack = 2 )]
        internal struct SP_DEVICE_INTERFACE_DETAIL_DATA {
            internal int cbSize;
            internal short devicePath;
        }

        [StructLayout( LayoutKind.Sequential )]
        internal struct HIDD_ATTRIBUTES {
            public int Size; // = sizeof (struct _HIDD_ATTRIBUTES) = 10 
            public UInt16 VendorID;
            public UInt16 ProductID;
            public UInt16 VersionNumber;
        }

        [StructLayout( LayoutKind.Sequential )]
        internal struct HIDP_CAPS {
            public UInt16 Usage;
            public UInt16 UsagePage;
            public UInt16 InputReportByteLength;
            public UInt16 OutputReportByteLength;
            public UInt16 FeatureReportByteLength;
            [MarshalAs( UnmanagedType.ByValArray, SizeConst = 17 )]
            public UInt16[] Reserved;
            public UInt16 NumberLinkCollectionNodes;
            public UInt16 NumberInputButtonCaps;
            public UInt16 NumberInputValueCaps;
            public UInt16 NumberInputDataIndices;
            public UInt16 NumberOutputButtonCaps;
            public UInt16 NumberOutputValueCaps;
            public UInt16 NumberOutputDataIndices;
            public UInt16 NumberFeatureButtonCaps;
            public UInt16 NumberFeatureValueCaps;
            public UInt16 NumberFeatureDataIndices;
        }

        internal const int DIGCF_PRESENT = 0x00000002;
        internal const int DIGCF_DEVICEINTERFACE = 0x00000010;
        internal const uint GENERIC_READ = 0x80000000;
        internal const uint GENERIC_WRITE = 0x40000000;
        internal const uint FILE_SHARE_READ = 0x00000001;
        internal const uint FILE_SHARE_WRITE = 0x00000002;
        internal const int OPEN_EXISTING = 3;
        internal const int FILE_FLAG_OVERLAPPED = 0x40000000;
        internal const uint MAX_USB_DEVICES = 16;

    }
}