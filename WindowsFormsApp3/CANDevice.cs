using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Runtime.InteropServices;

namespace WindowsFormsApp3
{
    [Serializable]
    public class CardType
    {
        public string TyPeName { get; set; }
        public uint TypeId { get; set; }
    }
    [Serializable]
    public class BoudRoute
    {
        public string RateName { get; set; }
        public UInt32 RateValue { get; set; }
    }

    [Serializable]
    public class CANDevice
    {        
        //private readonly uint devType;
        //private readonly uint devID;
        //private readonly uint canID;
        //private readonly uint baudRate;
        //private readonly uint sendTimeout;

        public uint DevType { get; set; }
        public uint DevID { get; set; }
        public uint CANID { get; set; }
        public uint BoudRate { get; set; }
        public uint SendTimeout { get; set; }
        //CAN参数
        public uint AccCode { get; set; }
        public uint AccMask { get; set; }
        public uint Reserved { get; set; }
        public byte Filter { get; set; }
        public byte Timing0 { get; set; }
        public byte Timing1 { get; set; }
        public byte Mode { get; set; }
        public List<CardType> CardTypeList { get; set; }
        public List<BoudRoute> BoudRateList { get; set; }

        public CANDevice(uint devType, uint devID, uint boudRate, uint sendTimeout, uint canID = 0)
        {
            this.DevType = devType;
            this.DevID = devID;
            this.CANID = canID;
            this.BoudRate = boudRate;
            this.SendTimeout = sendTimeout;

        }

        public CANDevice()
        {
            this.CardTypeList = new List<CardType>();           
            this.CardTypeList.Add(new CardType { TyPeName = "VCI_USBCAN_E_U", TypeId = 20 });
            this.CardTypeList.Add(new CardType { TyPeName = "VCI_USBCAN_2E_U", TypeId = 21 });

            this.BoudRateList = new List<BoudRoute>();
            this.BoudRateList.Add(new BoudRoute { RateName = "1000Kbps", RateValue = 0x060003 });
            this.BoudRateList.Add(new BoudRoute { RateName = "800Kbps", RateValue = 0x060004 });
            this.BoudRateList.Add(new BoudRoute { RateName = "500Kbps", RateValue = 0x060007 });
            this.BoudRateList.Add(new BoudRoute { RateName = "250Kbps", RateValue = 0x1C0008 });
            this.BoudRateList.Add(new BoudRoute { RateName = "125Kbps", RateValue = 0x1C0011 });
            this.BoudRateList.Add(new BoudRoute { RateName = "100Kbps", RateValue = 0x160023 });
            this.BoudRateList.Add(new BoudRoute { RateName = "50Kbps", RateValue = 0x1C002C });
            this.BoudRateList.Add(new BoudRoute { RateName = "20Kbps", RateValue = 0x1600B3 });
            this.BoudRateList.Add(new BoudRoute { RateName = "10Kbps", RateValue = 0x1C00E0 });
            this.BoudRateList.Add(new BoudRoute { RateName = "5Kbps", RateValue = 0x1C01C1 });           
        }   
    }
}
