using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Reflection;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Reflection.Emit;

namespace XXY_VisitorMJAsst
{
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    public struct T_HEX_RECORD
    {
        public byte RecDataLen;
        public UInt64 Address;
        public UInt64 MaxAddress;
        public UInt64 MinAddress;
        public byte RecType;
        public byte CheckSum;
        public UInt64 ExtSegAddress;
        public UInt64 ExtLinAddress;
    }

    public struct HEXRECORD
    {
        public int RecDataLen;
        public byte[] Data;
        public string line;
    }

    public struct OnePageRecord
    {
        public UInt16 Index;
        public byte[] Data;
        public string line;
        public int num;
        public UInt16 crc;
    }

    public class UpdateFimware
    {
        const UInt64 BOOT_SECTOR_BEGIN = 0x9FC00000;
        const UInt64 APPLICATION_START = 0x9D000000;
        const byte DATA_RECORD = 0;
        const byte END_OF_FILE_RECORD = 1;
        const byte EXT_SEG_ADRS_RECORD = 2;
        const byte EXT_LIN_ADRS_RECORD = 4;
        UInt16[] crc_table = new UInt16[16] { 0x0000, 0x1021, 0x2042, 0x3063, 0x4084, 0x50A5, 0x60C6, 0x70E7, 0x8108, 0x9129, 0xA14A, 0xB16B, 0xC18C, 0xD1AD, 0xE1CE, 0xF1EF };

        ArrayList DataSet;
        ArrayList PageRecord;
        byte[] VirtualFlash = new byte[5 * 1024 * 1024]; // 5 MB flash
        UInt16 DataCRC = 0;

        public UpdateFimware()
        {
            DataSet = new ArrayList();
            PageRecord = new ArrayList();
        }

        #region Tool
        private byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        private static object ByteToStruct(byte[] bytes, Type type)
        {
            int size = Marshal.SizeOf(type);
            if (size > bytes.Length)
            {
                return null;
            }
            //分配结构体内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷贝到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }

        private void Addline(string line)
        {
            line = line.Replace(":", "").ToUpper();
            byte[] byts = strToToHexByte(line);
            if (byts.Length > 21) return;

            HEXRECORD b = new HEXRECORD();
            b.RecDataLen = (byts[0] + 5);
            b.Data = new byte[byts.Length];
            b.line = line;
            Array.ConstrainedCopy(byts, 0, b.Data, 0, byts.Length);
            DataSet.Add(b);
        }

        private void ResetData()
        {
            int line = 0;
            int i = 0;
            OnePageRecord R = new OnePageRecord();
            R.Data = new byte[500];
            for (i = 0; i < 500; i++) R.Data[i] = 0xff;
            R.line = "";
            UInt16 index = 0,crc=0;
            int len = 0;

            PageRecord.Clear();
            foreach (HEXRECORD rec in DataSet)
            {
                if ((len + rec.RecDataLen) > 500)
                {
                    R.Index = index;
                    PageRecord.Add(R);

                    R = new OnePageRecord();
                    R.Data = new byte[500];
                    for (i = 0; i < 500; i++) R.Data[i] = 0xff;
                    R.line = "";
                    R.num = 0;
                    len = 0;
                    index++;
                }
                Array.ConstrainedCopy(rec.Data, 0, R.Data, len, rec.RecDataLen);
                R.line = R.line + rec.line;
                len += rec.RecDataLen;
                line++;
                R.num = line;
                crc = CalculateCrc(rec.Data,crc, Convert.ToUInt64(rec.RecDataLen));
                R.crc = crc;
            }

            if (len > 0)
            {
                R.num = line;
                R.Index = index;
                PageRecord.Add(R);
            }
        }

        public void Readfile(string filename)
        {
            PageRecord.Clear();
            DataSet.Clear();
            string[] lines = System.IO.File.ReadAllLines(filename);
            foreach (string line in lines)
            {
                Addline(line);
            }
            ResetData();
            DataCRC = GetDataCrc();
        }
        
        private UInt16 CalculateCrc(byte[] data, UInt64 len)
        {
            UInt64 j;
            int i;
            UInt16 crc;
            byte value;

            crc = 0;
            for (j = 0; j < len; j++)
            {
                value = data[j];
                i = (crc >> 12) ^ (value >> 4);
                crc = Convert.ToUInt16((crc_table[i & 0x0F] ^ ((crc << 4))) & 0xffff);

                i = (crc >> 12) ^ (value >> 0);
                crc = Convert.ToUInt16((crc_table[i & 0x0F] ^ (crc << 4)) & 0xffff);
            }
            return (crc);
        }

        private UInt16 CalculateCrc(byte[] data, UInt16 crc, UInt64 len)
        {
            UInt64 j;
            int i;
            byte value;

            for (j = 0; j < len; j++)
            {
                value = data[j];
                i = (crc >> 12) ^ (value >> 4);
                crc = Convert.ToUInt16((crc_table[i & 0x0F] ^ ((crc << 4))) & 0xffff);

                i = (crc >> 12) ^ (value >> 0);
                crc = Convert.ToUInt16((crc_table[i & 0x0F] ^ (crc << 4)) & 0xffff);
            }
            return (crc);
        }

        private UInt64 PA_TO_KVA0(UInt64 x)
        {
            return (x | 0x80000000);
        }

        private UInt64 PA_TO_VFA(UInt64 x)
        {
            return (x - APPLICATION_START);
        }

        private UInt16 GetDataCrc()
        {
            UInt16 crc = 0;
            foreach (HEXRECORD rec in DataSet)
            {
                crc = CalculateCrc(rec.Data, crc, Convert.ToUInt64( rec.Data[0]+5));
            }
            return crc;
        }
        #endregion

        public byte[] GetRecToUpdate(UInt16 index,out UInt16 crc)
        {
            crc = 0;
            if (index >= PageRecord.Count) return null;
            OnePageRecord R = (OnePageRecord)PageRecord[index];
            crc = R.crc;
            return R.Data;
        }

        public string GetRecNum(UInt16 index)
        {
            if (index >= PageRecord.Count) return null;
            OnePageRecord R = (OnePageRecord)PageRecord[index];
            return R.num.ToString();
        }

        public UInt16 GetCRC()
        {
            return DataCRC;
        }

        public int Count()
        {
            return PageRecord.Count;
        }

        public void Verify(out UInt64 StartAdress, out UInt64 ProgLen, out UInt16 crc)
        {
            UInt64 VirtualFlashAdrs = 0, ProgAddress = 0, j;
            T_HEX_RECORD HexSt = new T_HEX_RECORD();
            int i;
            for (i = 0; i < VirtualFlash.Count(); i++) VirtualFlash[i] = 0xff;
            crc = 0;
            StartAdress = 0;
            ProgLen = 0;

            HexSt.MaxAddress = 0;
            HexSt.MinAddress = 0xFFFFFFFF;

            foreach (HEXRECORD rec in DataSet)
            {
                HexSt.RecDataLen = rec.Data[0];
                HexSt.RecType = rec.Data[3];
                if (HexSt.RecDataLen > 0)
                {
                    switch (HexSt.RecType)
                    {
                        case DATA_RECORD:
                            //    HexSt.Address = StrToInt("0x" + rec.SAdr);
                            HexSt.Address = (((Convert.ToUInt64(rec.Data[1]) << 8) & 0x0000FF00) | (Convert.ToUInt64(rec.Data[2]) & 0x000000FF)) & (0x0000FFFF);
                            HexSt.Address = HexSt.Address + HexSt.ExtLinAddress + HexSt.ExtSegAddress;
                            ProgAddress = PA_TO_KVA0(HexSt.Address);

                            if (ProgAddress < BOOT_SECTOR_BEGIN)  // Make sure we are not writing boot sector.
                            {
                                if (HexSt.MaxAddress < (ProgAddress + HexSt.RecDataLen))
                                {
                                    HexSt.MaxAddress = ProgAddress + HexSt.RecDataLen;
                                }

                                if (HexSt.MinAddress > ProgAddress) HexSt.MinAddress = ProgAddress;

                                VirtualFlashAdrs = PA_TO_VFA(ProgAddress); // Program address to local virtual flash address

                                for (j = 0; j < HexSt.RecDataLen; j++)
                                    VirtualFlash[VirtualFlashAdrs + j] = rec.Data[4 + j];
                            }

                            break;

                        case EXT_SEG_ADRS_RECORD: // Record Type 02, defines 4 to 19 of the data address.
                            HexSt.ExtSegAddress = ((Convert.ToUInt64(rec.Data[4]) << 16) & 0x00FF0000) | ((Convert.ToUInt64(rec.Data[5]) << 8) & 0x0000FF00);
                            HexSt.ExtLinAddress = 0;
                            break;

                        case EXT_LIN_ADRS_RECORD:
                            HexSt.ExtLinAddress = ((Convert.ToUInt64(rec.Data[4]) << 24) & 0xFF000000) | ((Convert.ToUInt64(rec.Data[5]) << 16) & 0x00FF0000);
                            HexSt.ExtSegAddress = 0;
                            break;

                        case END_OF_FILE_RECORD: // Record Type 01          
                            HexSt.ExtSegAddress = 0;
                            HexSt.ExtLinAddress = 0;
                            break;
                        default:
                            {
                                HexSt.ExtSegAddress = 0;
                                HexSt.ExtLinAddress = 0;
                            }
                            break;
                    }
                }
            }
            HexSt.MinAddress = HexSt.MinAddress - (HexSt.MinAddress % 4);
            HexSt.MaxAddress = HexSt.MaxAddress + (HexSt.MaxAddress % 4);

            ProgLen = HexSt.MaxAddress - HexSt.MinAddress;
            StartAdress = HexSt.MinAddress;
            VirtualFlashAdrs = PA_TO_VFA(HexSt.MinAddress);

            byte[] data = new byte[ProgLen];

            Array.Copy(VirtualFlash, Convert.ToInt64(VirtualFlashAdrs), data, 0, Convert.ToInt64(ProgLen));

            crc = CalculateCrc(data, ProgLen);

        }
    }
}
