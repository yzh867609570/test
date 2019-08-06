using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using Microsoft.Win32.SafeHandles;

namespace WindowsFormsApp3
{
    public class _VCI_CAN_OBJ
    {
        public uint ID { get; set; }
        public uint TimeStamp { get; set; }
        public byte TimeFlag { get; set; }
        /// <summary>
        /// 发送格式：0:正常发送 1:单次正常发送 2:自发自收 3.单次自发自收
        /// </summary>
        public byte SendType { get; set; }
        /// <summary>
        /// 帧格式：0：数据帧 1：远程帧
        /// </summary>
        public byte RemoteFlag { get; set; }
        /// <summary>
        /// 帧类型：0：标准帧 1为扩展帧，29位ID
        /// </summary>
        public byte ExternFlag { get; set; }
        public byte DataLen { get; set; }
        public byte[] Data { get; set; }
        public byte[] Reserved { get; set; }
    }

    public class _VCI_INIT_CONFIG
    {
        public uint AccCode { get; set; }
        public uint AccMask { get; set; }
        public byte Reserved { get; set; }
        public byte Filter { get; set; }
        public byte Timing0 { get; set; }
        public byte Timing1 { get; set; }
        public byte Mode { get; set; }
    }
    public class ControlCAN
    {
        #region 接口函数定义
        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct VCI_CAN_OBJ
        {
            public uint ID;
            public uint TimeStamp;
            public byte TimeFlag;
            public byte SendType;//发送格式：0:正常发送 1:单次正常发送 2:自发自收 3.单次自发自收
            public byte RemoteFlag;//帧格式：0：数据帧 1：远程帧
            public byte ExternFlag;//帧类型：0：标准帧 1为扩展帧，29位ID
            public byte DataLen;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] Data;
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3)]
            public byte[] Reserved;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct VCI_INIT_CONFIG
        {
            public uint AccCode;
            public uint AccMask;
            public uint Reserved;
            public byte Filter;
            public byte Timing0;
            public byte Timing1;
            public byte Mode;
        }

        [StructLayoutAttribute(LayoutKind.Sequential)]
        struct VCI_FILTER_RECORD
        {
            public uint ExtFrame;
            public uint Start;
            public uint End;
        }

        [DllImport("kernel32.dll")]
        static extern uint GetLastError();

        [DllImport("ControlCAN.dll")]
        static extern uint VCI_OpenDevice(uint DevType, uint DevIndex, uint Reserved);//Reserved系统保留字段

        [DllImport("ControlCAN.dll")]
        static extern uint VCI_CloseDevice(uint DevType, uint DevIndex);

        [DllImport("ControlCAN.dll")]
        static extern uint VCI_StartCAN(uint DevType, uint DevIndex, uint CANIndex);

        [DllImport("ControlCAN.dll")]
        static extern uint VCI_ResetCAN(uint DevType, uint DevIndex, uint CANIndex);

        [DllImport("ControlCAN.dll")]
        static extern uint VCI_SetReference(uint DevType, uint DevIndex, uint CANIndex, uint RefType, IntPtr pData);

        //[DllImport("ControlCAN.dll")]
        //unsafe static extern UInt32 VCI_SetReference(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, UInt32 RefType, byte* pData);

        [DllImport("ControlCAN.dll")]
        static extern uint VCI_Receive(uint DevType, uint DevIndex, uint CANIndex, [Out] VCI_CAN_OBJ[] pReceive, uint Len, int WaitTime);
        //参照官方
        [DllImport("ControlCAN.dll", CharSet = CharSet.Ansi)]
        static extern UInt32 VCI_Receive(UInt32 DeviceType, UInt32 DeviceInd, UInt32 CANInd, IntPtr pReceive, UInt32 Len, Int32 WaitTime);

        [DllImport("ControlCAN.dll")]
        static extern uint VCI_Transmit(uint DevType, uint DevIndex, uint CANIndex, [In] VCI_CAN_OBJ[] pSend, uint Len);

        [DllImport("ControlCAN.dll")]
        static extern uint VCI_InitCAN(uint DevType, uint DevIndex, uint CANIndex, ref VCI_INIT_CONFIG pInitConfig);

        [DllImport("ControlCAN.dll")]
        static extern uint VCI_GetReceiveNum(uint DevType, uint DevIndex, uint CANIndex);

        [DllImport("ControlCAN.dll")]
        static extern uint VCI_ClearBuffer(uint DevType, uint DevIndex, uint CANIndex);

        [DllImport("RC500USB.dll")]
        static extern byte RC500USB_init();

        [DllImport("RC500USB.dll")]
        static extern void RC500USB_exit();

        [DllImport("RC500USB.dll")]
        static extern byte RC500USB_request(byte mode, ref ushort tagtype);

        [DllImport("RC500USB.dll")]
        static extern byte RC500USB_anticoll(byte bcnt, ref uint snr);

        [DllImport("RC500USB.dll")]
        static extern byte RC500USB_select(uint snr, ref byte size);

        [DllImport("RC500USB.dll")]
        internal static extern byte RC500USB_authkey(byte mode, [In] byte[] key, byte secnr);

        [DllImport("RC500USB.dll")]
        internal static extern byte RC500USB_read(byte addr, [Out] byte[] data);

        [DllImport("RC500USB.dll")]
        internal static extern byte RC500USB_buzzer(byte contrl, byte opentm, byte closetm, byte repcnt);

        // serialport
        //Win32 io errors
        public const int ERROR_BROKEN_PIPE = 109;
        public const int ERROR_NO_DATA = 232;
        public const int ERROR_HANDLE_EOF = 38;
        public const int ERROR_IO_INCOMPLETE = 996;
        public const int ERROR_IO_PENDING = 997;
        public const int ERROR_FILE_EXISTS = 0x50;
        public const int ERROR_FILENAME_EXCED_RANGE = 0xCE;  // filename too long.
        public const int ERROR_MORE_DATA = 234;
        public const int ERROR_CANCELLED = 1223;
        public const int ERROR_FILE_NOT_FOUND = 2;
        public const int ERROR_PATH_NOT_FOUND = 3;
        public const int ERROR_ACCESS_DENIED = 5;
        public const int ERROR_INVALID_HANDLE = 6;
        public const int ERROR_NOT_ENOUGH_MEMORY = 8;
        public const int ERROR_BAD_COMMAND = 22;
        public const int ERROR_SHARING_VIOLATION = 32;
        public const int ERROR_OPERATION_ABORTED = 995;
        public const int ERROR_NO_ASSOCIATION = 1155;
        public const int ERROR_DLL_NOT_FOUND = 1157;
        public const int ERROR_DDE_FAIL = 1156;
        public const int ERROR_INVALID_PARAMETER = 87;
        public const int ERROR_PARTIAL_COPY = 299;

        // Since C# does not provide access to bitfields and the native DCB structure contains
        // a very necessary one, these are the positional offsets (from the right) of areas
        // of the 32-bit integer used in SerialStream's SetDcbFlag() and GetDcbFlag() methods.
        internal const int FBINARY = 0;
        internal const int FPARITY = 1;
        internal const int FOUTXCTSFLOW = 2;
        internal const int FOUTXDSRFLOW = 3;
        internal const int FDTRCONTROL = 4;
        internal const int FDSRSENSITIVITY = 6;
        internal const int FTXCONTINUEONXOFF = 7;
        internal const int FOUTX = 8;
        internal const int FINX = 9;
        internal const int FERRORCHAR = 10;
        internal const int FNULL = 11;
        internal const int FRTSCONTROL = 12;
        internal const int FABORTONOERROR = 14;
        internal const int FDUMMY2 = 15;

        // The following are unique to the SerialPort/SerialStream classes
        internal const byte ONESTOPBIT = 0;
        internal const byte ONE5STOPBITS = 1;
        internal const byte TWOSTOPBITS = 2;

        public const int FILE_READ_DATA = (0x0001),
        FILE_LIST_DIRECTORY = (0x0001),
        FILE_WRITE_DATA = (0x0002),
        FILE_ADD_FILE = (0x0002),
        FILE_APPEND_DATA = (0x0004),
        FILE_ADD_SUBDIRECTORY = (0x0004),
        FILE_CREATE_PIPE_INSTANCE = (0x0004),
        FILE_READ_EA = (0x0008),
        FILE_WRITE_EA = (0x0010),
        FILE_EXECUTE = (0x0020),
        FILE_TRAVERSE = (0x0020),
        FILE_DELETE_CHILD = (0x0040),
        FILE_READ_ATTRIBUTES = (0x0080),
        FILE_WRITE_ATTRIBUTES = (0x0100),
        FILE_SHARE_READ = 0x00000001,
        FILE_SHARE_WRITE = 0x00000002,
        FILE_SHARE_DELETE = 0x00000004,
        FILE_ATTRIBUTE_READONLY = 0x00000001,
        FILE_ATTRIBUTE_HIDDEN = 0x00000002,
        FILE_ATTRIBUTE_SYSTEM = 0x00000004,
        FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
        FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
        FILE_ATTRIBUTE_NORMAL = 0x00000080,
        FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
        FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
        FILE_ATTRIBUTE_OFFLINE = 0x00001000,
        FILE_NOTIFY_CHANGE_FILE_NAME = 0x00000001,
        FILE_NOTIFY_CHANGE_DIR_NAME = 0x00000002,
        FILE_NOTIFY_CHANGE_ATTRIBUTES = 0x00000004,
        FILE_NOTIFY_CHANGE_SIZE = 0x00000008,
        FILE_NOTIFY_CHANGE_LAST_WRITE = 0x00000010,
        FILE_NOTIFY_CHANGE_LAST_ACCESS = 0x00000020,
        FILE_NOTIFY_CHANGE_CREATION = 0x00000040,
        FILE_NOTIFY_CHANGE_SECURITY = 0x00000100,
        FILE_ACTION_ADDED = 0x00000001,
        FILE_ACTION_REMOVED = 0x00000002,
        FILE_ACTION_MODIFIED = 0x00000003,
        FILE_ACTION_RENAMED_OLD_NAME = 0x00000004,
        FILE_ACTION_RENAMED_NEW_NAME = 0x00000005,
        FILE_CASE_SENSITIVE_SEARCH = 0x00000001,
        FILE_CASE_PRESERVED_NAMES = 0x00000002,
        FILE_UNICODE_ON_DISK = 0x00000004,
        FILE_PERSISTENT_ACLS = 0x00000008,
        FILE_FILE_COMPRESSION = 0x00000010,
        OPEN_EXISTING = 3,
        OPEN_ALWAYS = 4,
        FILE_FLAG_WRITE_THROUGH = unchecked((int)0x80000000),
        FILE_FLAG_OVERLAPPED = 0x40000000,
        FILE_FLAG_NO_BUFFERING = 0x20000000,
        FILE_FLAG_RANDOM_ACCESS = 0x10000000,
        FILE_FLAG_SEQUENTIAL_SCAN = 0x08000000,
        FILE_FLAG_DELETE_ON_CLOSE = 0x04000000,
        FILE_FLAG_BACKUP_SEMANTICS = 0x02000000,
        FILE_FLAG_POSIX_SEMANTICS = 0x01000000,
        FILE_TYPE_UNKNOWN = 0x0000,
        FILE_TYPE_DISK = 0x0001,
        FILE_TYPE_CHAR = 0x0002,
        FILE_TYPE_PIPE = 0x0003,
        FILE_TYPE_REMOTE = unchecked((int)0x8000),
        FILE_VOLUME_IS_COMPRESSED = 0x00008000;

        // The following are unique to the SerialPort/SerialStream classes
        internal const int DTR_CONTROL_DISABLE = 0x00;
        internal const int DTR_CONTROL_ENABLE = 0x01;
        internal const int DTR_CONTROL_HANDSHAKE = 0x02;

        internal const int RTS_CONTROL_DISABLE = 0x00;
        internal const int RTS_CONTROL_ENABLE = 0x01;
        internal const int RTS_CONTROL_HANDSHAKE = 0x02;
        internal const int RTS_CONTROL_TOGGLE = 0x03;

        internal const int MS_CTS_ON = 0x10;
        internal const int MS_DSR_ON = 0x20;

        internal const byte DEFAULTXONCHAR = (byte)17;
        internal const byte DEFAULTXOFFCHAR = (byte)19;

        internal const byte EOFCHAR = (byte)26;

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct COMMTIMEOUTS
        {
            public int ReadIntervalTimeout;
            public int ReadTotalTimeoutMultiplier;
            public int ReadTotalTimeoutConstant;
            public int WriteTotalTimeoutMultiplier;
            public int WriteTotalTimeoutConstant;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct DCB
        {
            public uint DCBlength;
            public uint BaudRate;
            public uint Flags;
            public ushort wReserved;
            public ushort XonLim;
            public ushort XoffLim;
            public byte ByteSize;
            public byte Parity;
            public byte StopBits;
            public byte XonChar;
            public byte XoffChar;
            public byte ErrorChar;
            public byte EofChar;
            public byte EvtChar;
            public ushort wReserved1;
        }

        public const int GENERIC_READ = unchecked(((int)0x80000000));
        public const int GENERIC_WRITE = (0x40000000);

        internal const int PURGE_TXABORT = 0x0001;  // Kill the pending/current writes to the comm port.
        internal const int PURGE_RXABORT = 0x0002;  // Kill the pending/current reads to the comm port.
        internal const int PURGE_TXCLEAR = 0x0004;  // Kill the transmit queue if there.
        internal const int PURGE_RXCLEAR = 0x0008;  // Kill the typeahead buffer if there.
        #endregion

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public uint MaxFrames { get; set; } = 16;
        public int ReceiveTimeout { get; set; } = 2000;

        private int disposed = 0;
        private CANDevice objCANDevice;

        protected virtual void Dispose(bool disposing)
        {
            if (Interlocked.Exchange(ref disposed, 1) == 0)
            {
                if (Interlocked.Exchange(ref disposed, 1) == 0)
                {
                    //释放非托管资源
                    if (ControlCAN.VCI_CloseDevice(objCANDevice.DevType, objCANDevice.DevID) == 0)
                    {
                        log.Error("Failed to close CAN device");
                    }
                }
            }
        }

        public void Dispose()
        {
            ///<summary>
            /// 实现IDisposable中的Dispose方法
            ///</summary>
            Dispose(true);//必须为true
            //通知垃圾回收机制不再调用终结器（析构器）
            GC.SuppressFinalize(this);//禁止终结操作
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="devType"></param>
        /// <param name="devID"></param>
        /// <param name="baudRate"></param>
        /// <param name="sendTimeout"></param>
        /// <param name="canID">默认为0</param>
        public ControlCAN(CANDevice canDevice)
        {
            objCANDevice = canDevice;
        }

        /// <summary>
        /// 打开CAN设备
        /// </summary>
        /// <returns></returns>
        public bool OpenCAN()
        {
            if (VCI_OpenDevice(objCANDevice.DevType, objCANDevice.DevID, 0) == 0)
            {
                //log.Error("Failed to open CAN device");
                //uint error = GetLastError();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 启动某一路CAN
        /// </summary>
        public bool StartCan(uint canId = 0)
        {
            if (Thread.VolatileRead(ref disposed) != 0)
            {
                throw new ObjectDisposedException("CanDevice already disposed");
            }

            if (canId != 0)
            {
                objCANDevice.CANID = canId;
            }

            if (!SetBaudRate(objCANDevice.BoudRate))
            {
                throw new Exception("波特率设置失败!");
            }
            if (!SetWorkingMode())
            {
                throw new Exception("工作模式设置失败!");
            }

            if (VCI_StartCAN(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID) == 0)
            {

                //log.Error("Failed to start CAN device");
                return false;
            }
            if (!SetSendTimeout())
            {
                throw new Exception("发送超时设置失败!");
            }
            ClearBuffer();

            return true;
        }

        /// <summary>
        /// 设置波特率
        /// </summary>
        /// <param name="baudRate">uint类型</param>
        /// <returns></returns>
        public bool SetBaudRate(uint baudRate)
        {
            if (Thread.VolatileRead(ref disposed) != 0)
            {
                throw new ObjectDisposedException("CanDevice already disposed");
            }

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(baudRate));

            try
            {
                Marshal.WriteInt32(ptr, (int)baudRate);
                if (VCI_SetReference(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID, 0, ptr) == 0)
                {
                    log.Error("Failed to set CAN baud rate");
                    return false;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return true;
        }

        /// <summary>
        /// 设置工作模式，需提供工作模式Id
        /// 必须先设置波特率再设置工模式
        /// </summary>
        /// <param name="modeId"> =0 表示正常模式（相当于正常节点）， =1 表示只听模式（只接收，不影响总线）</param>
        /// <returns></returns>
        public bool SetWorkingMode(byte modeId = 0)
        {
            if (Thread.VolatileRead(ref disposed) != 0)
            {
                throw new ObjectDisposedException("CanDevice already disposed");
            }

            VCI_INIT_CONFIG initConfig = new VCI_INIT_CONFIG();
            initConfig.Mode = modeId;//正常模式

            if (VCI_InitCAN(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID, ref initConfig) == 0)
            {
                log.Error("Failed to set CAN working mode");
                return false;
            }

            return true;
        }

        /// <summary>
        ///设置发送超时时间
        /// </summary>
        public bool SetSendTimeout(uint timeout = 2000)
        {
            if (Thread.VolatileRead(ref disposed) != 0)
            {
                throw new ObjectDisposedException("CanDevice already disposed");
            }

            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(timeout));

            try
            {
                Marshal.WriteInt32(ptr, (int)timeout);
                if (VCI_SetReference(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID, 4, ptr) == 0)
                {
                    log.Error("Failed to set CAN send timeout");
                    return false;
                }
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }

            return true;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="frames">帧结构体数组</param>
        /// 举例：VCI_CAN_OBJ[] frames=new VCI_CAN_OBJ[2];//将发送两帧数据
        /// frames[0].ID=0x00000001;//第一帧ID
        /// frames[0].SendType=0;//正常发送
        /// frames[0].RemoteFlag=0;//数据帧
        /// frames[0].ExternFlag=0;//标准帧
        /// frames[0].DataLen=1;//数据长度
        /// frames[0].Data[0]=0x56;//数据
        /// frames[1]~
        /// <returns></returns>
        public bool Transmit(_VCI_CAN_OBJ[] frames)
        {
            if (Thread.VolatileRead(ref disposed) != 0)
            {
                throw new ObjectDisposedException("CanDevice already disposed");
            }
            int length = frames.Length;
            VCI_CAN_OBJ[] obj = new VCI_CAN_OBJ[length];
            for (int i = 0; i < length; i++)
            {
                obj[i] = new VCI_CAN_OBJ
                {
                    ID = frames[i].ID,
                    TimeFlag = frames[i].TimeFlag,
                    TimeStamp = frames[i].TimeStamp,
                    SendType = frames[i].SendType,
                    RemoteFlag = frames[i].RemoteFlag,
                    ExternFlag = frames[i].ExternFlag,
                    Data = frames[i].Data,
                    DataLen = frames[i].DataLen,
                    Reserved = frames[i].Reserved
                };
            }

            return VCI_Transmit(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID, obj, (uint)length) != 0;
        }

        public bool Transmit(_VCI_CAN_OBJ frame)
        {
            if (Thread.VolatileRead(ref disposed) != 0)
            {
                throw new ObjectDisposedException("CanDevice already disposed");
            }

            VCI_CAN_OBJ[] frames = new VCI_CAN_OBJ[1];
            frames[0] = new VCI_CAN_OBJ
            {
                ID = frame.ID,
                TimeFlag = frame.TimeFlag,
                TimeStamp = frame.TimeStamp,
                SendType = frame.SendType,
                RemoteFlag = frame.RemoteFlag,
                ExternFlag = frame.ExternFlag,
                Data = frame.Data,
                DataLen = frame.DataLen,
                Reserved = frame.Reserved
            };

            return VCI_Transmit(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID, frames, (uint)frames.Length) != 0;
        }

        /// <summary>
        /// 设置CAN相关参数
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public bool InitCAN(_VCI_INIT_CONFIG config)
        {
            if (Thread.VolatileRead(ref disposed) != 0)
            {
                throw new ObjectDisposedException("CanDevice already disposed");
            }

            VCI_INIT_CONFIG obj = new VCI_INIT_CONFIG
            {
                AccCode = config.AccCode,
                AccMask = config.AccMask,
                Mode = config.Mode,
                Filter = config.Filter,
                Timing0 = config.Timing0,
                Timing1 = config.Timing1,
                Reserved = config.Reserved
            };

            if (VCI_InitCAN(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID, ref obj) == 0)
            {
                return false;
            }
            return true;
        }

        #region 参考ZLG官方示例
        /// <summary>
        /// 接收数据
        /// </summary>
        /// <returns></returns>
        public List<_VCI_CAN_OBJ> Receive()
        {
            //获取CAN通道缓冲区中已接收但未读取的帧数量
            uint receiveNum = VCI_GetReceiveNum(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID);
            if (receiveNum == 0) return null;

            //如果缓冲区未读取的帧数量大于规定的最大读取量，只能读取规定的最大量的帧数据，其它数据丢失
            uint needReceiveNum = receiveNum > MaxFrames ? MaxFrames : receiveNum;

            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)receiveNum);
            //返回值为实际读取的帧数量，数据填充至buf
            receiveNum = VCI_Receive(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID, pt, receiveNum, 100);

            List<_VCI_CAN_OBJ> result = new List<_VCI_CAN_OBJ>();
            //string str = "";
            for (int i = 0; i < receiveNum; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));

               
                //foreach (var item in obj.Data)
                //{
                //    str += " " + System.Convert.ToString(item, 16);
                //}

                result.Add(new _VCI_CAN_OBJ
                {
                    ID = obj.ID,
                    TimeFlag = obj.TimeFlag,
                    TimeStamp = obj.TimeStamp,
                    SendType = obj.SendType,
                    RemoteFlag = obj.RemoteFlag,
                    ExternFlag = obj.ExternFlag,
                    Data = obj.Data,
                    DataLen = obj.DataLen,
                    Reserved = obj.Reserved
                });
            }
            return result;
        }

        /// <summary>
        /// 直接获取接收数据的string形式
        /// </summary>
        /// <returns></returns>
        public string OnlyReceiveData()
        {
            //获取CAN通道缓冲区中已接收但未读取的帧数量
            uint receiveNum = VCI_GetReceiveNum(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID);
            if (receiveNum == 0) return null;

            //如果缓冲区未读取的帧数量大于规定的最大读取量，只能读取规定的最大量的帧数据，其它数据丢失
            uint needReceiveNum = receiveNum > MaxFrames ? MaxFrames : receiveNum;

            IntPtr pt = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(VCI_CAN_OBJ)) * (Int32)receiveNum);
            //返回值为实际读取的帧数量，数据填充至buf
            receiveNum = VCI_Receive(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID, pt, receiveNum, 100);
           
            string str = "";
            for (int i = 0; i < receiveNum; i++)
            {
                VCI_CAN_OBJ obj = (VCI_CAN_OBJ)Marshal.PtrToStructure((IntPtr)((UInt32)pt + i * Marshal.SizeOf(typeof(VCI_CAN_OBJ))), typeof(VCI_CAN_OBJ));

                foreach (var item in obj.Data)
                {
                    str += " " + System.Convert.ToString(item, 16);
                }
            }
            return str;
        }
        #endregion

        #region 修改接收方法：调用的VCI_Receive参数不同
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<_VCI_CAN_OBJ> _Receive()
        {
            if (Thread.VolatileRead(ref disposed) != 0)
            {
                throw new ObjectDisposedException("CanDevice already disposed");
            }

            VCI_CAN_OBJ[] buf = new VCI_CAN_OBJ[MaxFrames];//MaxFrames为规定的最大接收数

            uint num = VCI_Receive(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID, buf, (uint)buf.Length,2000);
           
           VCI_CAN_OBJ[] frames = new VCI_CAN_OBJ[num];
            Array.Copy(buf, frames, num);

            List<_VCI_CAN_OBJ> result = new List<_VCI_CAN_OBJ>();
            foreach (var obj in frames)
            {
                result.Add(new _VCI_CAN_OBJ
                {
                    ID = obj.ID,
                    TimeFlag = obj.TimeFlag,
                    TimeStamp = obj.TimeStamp,
                    SendType = obj.SendType,
                    RemoteFlag = obj.RemoteFlag,
                    ExternFlag = obj.ExternFlag,
                    Data = obj.Data,
                    DataLen = obj.DataLen,
                    Reserved = obj.Reserved
                });
            }

            return result;
        }
        #endregion
        /// <summary>
        /// 查看缓存区是否存在未读取数据
        /// </summary>
        /// <returns></returns>
        public bool HasReceive()
        {
            return VCI_GetReceiveNum(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID) != 0;
        }

        /// <summary>
        /// 清除接收缓冲区数据
        /// </summary>
        /// <returns></returns>
        public bool ClearBuffer()
        {
            if (Thread.VolatileRead(ref disposed) != 0)
            {
                throw new ObjectDisposedException("CanDevice already disposed");
            }

            if (ControlCAN.VCI_ClearBuffer(objCANDevice.DevType, objCANDevice.DevID, objCANDevice.CANID) == 0)
            {
                log.Error("Failed to clear can.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 关闭CAN
        /// </summary>
        /// <returns></returns>
        public bool CloseCAN()
        {
            if (VCI_CloseDevice(objCANDevice.DevType, objCANDevice.DevID) == 0)
            {
                //log.Error("Failed to close CAN device");
                return false;
            }
            return true;
        }
    }
}
