using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace Corrigo.CorrigoNet.CorrigoNetToGo
{
	/// <summary>
	/// Summary description for Utils.
	/// </summary>
	public class Utils
	{
		private Utils()
		{
		}

		public static bool IsPocketPC()
		{
			Version verPPC = new Version(3, 0);
			Version verCurrent = Environment.OSVersion.Version;
			if ( Environment.OSVersion.Platform == PlatformID.WinCE && verCurrent.Major == verPPC.Major )
			{
				return true;
			}
			return false;
		}

		public static void CenterForm(Form frm)
		{
			Rectangle r = GetVisibleDesktop();
			frm.Location = new Point((r.Width - frm.Width) / 2, (r.Height - frm.Height)/2);
		}

		public static CPU_ARCH GetCPUArchitecture()
		{
			SYSTEM_INFO si = new SYSTEM_INFO();
			GetSystemInfo(ref si);
			switch(si.wProcessorArchitecture)
			{
				case PROCESSOR_ARCHITECTURE_ARM:
					return CPU_ARCH.ARM;
				case PROCESSOR_ARCHITECTURE_SHX:
					return CPU_ARCH.SH3;
				case PROCESSOR_ARCHITECTURE_INTEL:
					return CPU_ARCH.X86;
				case PROCESSOR_ARCHITECTURE_MIPS:
					return CPU_ARCH.MIPS;
				default:
					return CPU_ARCH.Unknown;
			}
		}

		public static string GetPlatformType()
		{
			StringBuilder sb = new StringBuilder(255);
			SystemParametersInfo(SPI_GETPLATFORMTYPE, (uint)sb.Capacity, sb, 0);
			string platType = sb.ToString();
			if ( platType == "PocketPC" )
				return "PPC";
			else if ( platType == "Windows CE" )
				return "WCE";
			else
				return "Unknown";
		}
		
		public static string GetInstructionSet()
		{
            string info = null;
			CpuInstructionSet iset = CpuInstructionSet.X86;
			try
			{
				QueryInstructionSet(0, out iset);
                info = prGetSystemInfo();
            }
			catch (MissingMethodException)
			{
                return prGetSystemInfo();
			}

            return info;// iset.ToString();
		}

        private static string prGetSystemInfo()
        {
            // We are running an older version of the OS
            // so QueryInstructionSet is not available
            SYSTEM_INFO si = new SYSTEM_INFO();
            GetSystemInfo(ref si);
            switch (si.wProcessorArchitecture)
            {
                case PROCESSOR_ARCHITECTURE_ARM:
                    return CpuInstructionSet.ARMV4.ToString();
                case PROCESSOR_ARCHITECTURE_SHX:
                    return CpuInstructionSet.SH3.ToString();
                case PROCESSOR_ARCHITECTURE_INTEL:
                    return CpuInstructionSet.X86.ToString();
                case PROCESSOR_ARCHITECTURE_MIPS:
                    return CpuInstructionSet.MIPSV4.ToString();
                default:
                    return "Unkown";
            }
        }

		public static Rectangle GetVisibleDesktop()
		{
			RECT r = new RECT();
			SystemParametersInfo(SPI_GETWORKAREA, 0, ref r, 0U);
			return new Rectangle(r.Left, r.Top, r.Right - r.Left, r.Bottom - r.Top );
		}

		public struct RECT
		{
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
		}

		public enum CPU_ARCH
		{
			ARM,
			X86,
			SH3,
			MIPS,
			Unknown
		}

		class PROCESSOR_INFO 
		{
			byte [] data;

			public PROCESSOR_INFO()
			{
				data = new byte[574];
				wVersion = 1;
			}

			public byte[] DataBuffer { get { return data; } }

			public ushort wVersion 
			{ 
				get { return (ushort)BitConverter.ToInt16(data, 0); }
				set { BitConverter.GetBytes(value).CopyTo(data, 0); }
			}
			

			public string szProcessorCore
			{ 
				get { byte[] ret = new byte[80]; Buffer.BlockCopy(data, 2, ret, 0, 80);  return Encoding.Unicode.GetString( ret, 0, ret.Length ).TrimEnd('\0'); }
				set { Buffer.BlockCopy(Encoding.Unicode.GetBytes( value ), 0, data, 2, 80); }
			}
			public ushort wCoreRevision
			{ 
				get { return (ushort)BitConverter.ToInt16(data, 82); }
				set { BitConverter.GetBytes(value).CopyTo(data, 82); }
			}

			public string szProcessorName
			{ 
				get { byte[] ret = new byte[80]; Buffer.BlockCopy(data, 84, ret, 0, 80);  return Encoding.Unicode.GetString( ret, 0, ret.Length ).TrimEnd('\0'); }
				set { Buffer.BlockCopy(Encoding.Unicode.GetBytes( value ), 0, data, 84, 80); }
			}

			public ushort wProcessorRevision
			{ 
				get { return (ushort)BitConverter.ToInt16(data, 164); }
				set { BitConverter.GetBytes(value).CopyTo(data, 164); }
			}

			public string szCatalogNumber/*[100]*/
			{ 
				get { byte[] ret = new byte[200]; Buffer.BlockCopy(data, 166, ret, 0, 200);  return Encoding.Unicode.GetString( ret, 0, ret.Length ).TrimEnd('\0'); }
				set { Buffer.BlockCopy(Encoding.Unicode.GetBytes( value ), 0, data, 166, 200); }
			}

			public string szVendor /*[100];*/
			{ 
				get { byte[] ret = new byte[200]; Buffer.BlockCopy(data, 366, ret, 0, 200);  return Encoding.Unicode.GetString(ret, 0, ret.Length).TrimEnd('\0'); }
				set { Buffer.BlockCopy(Encoding.Unicode.GetBytes( value ), 0, data, 366, 200); }
			}

			public uint dwInstructionSet
			{ 
				get { return (uint)BitConverter.ToInt32(data, 566); }
				set { BitConverter.GetBytes(value).CopyTo(data, 566); }
			}

			public uint dwClockSpeed
			{ 
				get { return (uint)BitConverter.ToInt32(data, 570); }
				set { BitConverter.GetBytes(value).CopyTo(data, 570); }
			}
		}

		public struct SYSTEM_INFO 
		{
			public ushort wProcessorArchitecture;
			public ushort wReserved;
			public uint dwPageSize;
			public int lpMinimumApplicationAddress;
			public int lpMaximumApplicationAddress;
			public uint dwActiveProcessorMask;
			public uint dwNumberOfProcessors;
			public uint dwProcessorType;
			public uint dwAllocationGranularity;
			public ushort wProcessorLevel;
			public ushort wProcessorRevision;
		}

		public const int SPI_GETWORKAREA = 48;
		public const uint SPI_GETPLATFORMTYPE = 257;
		public const int SM_CYCAPTION = 4;
		public const int SM_CYMENU = 15;

		#region PROCESSOR_ARCHITECTURE
		public const int PROCESSOR_INTEL_386     =386;
		public const int PROCESSOR_INTEL_486     =486;
		public const int PROCESSOR_INTEL_PENTIUM =586;
		public const int PROCESSOR_INTEL_PENTIUMII =686;
		public const int PROCESSOR_MIPS_R4000    =4000;    // incl R4101 & R3910 for Windows CE
		public const int PROCESSOR_ALPHA_21064   =21064;
		public const int PROCESSOR_PPC_403       =403;
		public const int PROCESSOR_PPC_601       =601;
		public const int PROCESSOR_PPC_603       =603;
		public const int PROCESSOR_PPC_604       =604;
		public const int PROCESSOR_PPC_620       =620;
		public const int PROCESSOR_HITACHI_SH3   =10003;   // Windows CE
		public const int PROCESSOR_HITACHI_SH3E  =10004;   // Windows CE
		public const int PROCESSOR_HITACHI_SH4   =10005;   // Windows CE
		public const int PROCESSOR_MOTOROLA_821  =821;     // Windows CE
		public const int PROCESSOR_SHx_SH3       =103;     // Windows CE
		public const int PROCESSOR_SHx_SH4       =104;     // Windows CE
		public const int PROCESSOR_STRONGARM     =2577;    // Windows CE - 0xA11
		public const int PROCESSOR_ARM720        =1824;    // Windows CE - 0x720
		public const int PROCESSOR_ARM820        =2080;    // Windows CE - 0x820
		public const int PROCESSOR_ARM920        =2336;    // Windows CE - 0x920
		public const int PROCESSOR_ARM_7TDMI     =70001;   // Windows CE

		public const int PROCESSOR_ARCHITECTURE_INTEL =0;
		public const int PROCESSOR_ARCHITECTURE_MIPS  =1;
		public const int PROCESSOR_ARCHITECTURE_ALPHA =2;
		public const int PROCESSOR_ARCHITECTURE_PPC   =3;
		public const int PROCESSOR_ARCHITECTURE_SHX   =4;
		public const int PROCESSOR_ARCHITECTURE_ARM   =5;
		public const int PROCESSOR_ARCHITECTURE_IA64  =6;
		public const int PROCESSOR_ARCHITECTURE_ALPHA64 =7;
		public const int PROCESSOR_ARCHITECTURE_UNKNOWN =0xFFFF;

		#endregion

		[DllImport("coredll")]
		public static extern bool SystemParametersInfo ( 
			uint uiAction, 
			uint uiParam, 
			ref RECT pvParam, 
			uint fWinIni);
		[DllImport("coredll")]
		public static extern bool SystemParametersInfo ( 
			uint uiAction, 
			uint uiParam, 
			StringBuilder pvParam, 
			uint fWinIni);
		
		[DllImport("coredll")]
		public static extern int GetSystemMetrics(int nIndex); 

		[DllImport("Coredll")]		
		public static extern bool KernelIoControl(UInt32 dwIoControlCode, IntPtr lpInBuf, UInt32 nInBufSize, byte[] buf, UInt32 nOutBufSize, [In, Out] uint lpBytesReturned);	

		const int IOCTL_PROCESSOR_INFORMATION = 0x01010064;

		[DllImport("Coredll")]		
		public static extern void GetSystemInfo( ref SYSTEM_INFO SystemInfo); 

		[DllImport("Coredll")]		
		public static extern bool QueryInstructionSet(
			uint dwInstructionSet,
			out CpuInstructionSet CurrentInstructionSet
			);

		public int PROCESSOR_INSTRUCTION_CODE(int arch, int core, int feature)      
		{
			return ((arch) << 24 | (core) << 16 | (feature));
		}

		const int PROCESSOR_X86_32BIT_CORE               = 1;

		const int PROCESSOR_MIPS16_CORE                  = 1;
		const int PROCESSOR_MIPSII_CORE                  = 2;
		const int PROCESSOR_MIPSIV_CORE                  = 3;

		const int PROCESSOR_HITACHI_SH3_CORE             = 1;
		const int PROCESSOR_HITACHI_SH4_CORE             = 2;

		const int PROCESSOR_ARM_V4_CORE                  = 1;
		const int PROCESSOR_ARM_V4I_CORE                 = 2;
		const int PROCESSOR_ARM_V4T_CORE                 = 3;

		const int PROCESSOR_FEATURE_NOFP                 = 0;
		const int PROCESSOR_FEATURE_FP                   = 1;
		const int PROCESSOR_FEATURE_DSP                  = PROCESSOR_FEATURE_FP;

		const int PROCESSOR_QUERY_INSTRUCTION             = 0; //PROCESSOR_INSTRUCTION_CODE(0,0,0);
		const int PROCESSOR_X86_32BIT_INSTRUCTION		  = 0x00010001; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_INTEL, PROCESSOR_X86_32BIT_CORE, PROCESSOR_FEATURE_FP);
		const int PROCESSOR_MIPS_MIPS16_INSTRUCTION       = 0x01010000; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_MIPS,  PROCESSOR_MIPS16_CORE, PROCESSOR_FEATURE_NOFP);
		const int PROCESSOR_MIPS_MIPSII_INSTRUCTION       = 0x01020000; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_MIPS,  PROCESSOR_MIPSII_CORE, PROCESSOR_FEATURE_NOFP);
		const int PROCESSOR_MIPS_MIPSIIFP_INSTRUCTION     = 0x01020001; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_MIPS,  PROCESSOR_MIPSII_CORE, PROCESSOR_FEATURE_FP);
		const int PROCESSOR_MIPS_MIPSIV_INSTRUCTION       = 0x01030000; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_MIPS,  PROCESSOR_MIPSIV_CORE, PROCESSOR_FEATURE_NOFP);
		const int PROCESSOR_MIPS_MIPSIVFP_INSTRUCTION     = 0x01030001; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_MIPS,  PROCESSOR_MIPSIV_CORE, PROCESSOR_FEATURE_FP);
		const int PROCESSOR_HITACHI_SH3_INSTRUCTION       = 0x04010000; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_SHX,   PROCESSOR_HITACHI_SH3_CORE, PROCESSOR_FEATURE_NOFP);
		const int PROCESSOR_HITACHI_SH3DSP_INSTRUCTION    = 0x04010001; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_SHX,   PROCESSOR_HITACHI_SH3_CORE, PROCESSOR_FEATURE_DSP);
		const int PROCESSOR_HITACHI_SH4_INSTRUCTION       = 0x04020001; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_SHX,   PROCESSOR_HITACHI_SH4_CORE, PROCESSOR_FEATURE_FP);

		const int PROCESSOR_ARM_V4_INSTRUCTION            = 0x05010000; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_ARM,   PROCESSOR_ARM_V4_CORE, PROCESSOR_FEATURE_NOFP);
		const int PROCESSOR_ARM_V4FP_INSTRUCTION          = 0x05010001; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_ARM,   PROCESSOR_ARM_V4_CORE, PROCESSOR_FEATURE_FP);
		const int PROCESSOR_ARM_V4I_INSTRUCTION           = 0x05020000; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_ARM,   PROCESSOR_ARM_V4I_CORE, PROCESSOR_FEATURE_NOFP);
		const int PROCESSOR_ARM_V4IFP_INSTRUCTION         = 0x05020001; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_ARM,   PROCESSOR_ARM_V4I_CORE, PROCESSOR_FEATURE_FP);
		const int PROCESSOR_ARM_V4T_INSTRUCTION           = 0x05030000; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_ARM,   PROCESSOR_ARM_V4T_CORE, PROCESSOR_FEATURE_NOFP);
		const int PROCESSOR_ARM_V4TFP_INSTRUCTION         = 0x05030001; //PROCESSOR_INSTRUCTION_CODE(PROCESSOR_ARCHITECTURE_ARM,   PROCESSOR_ARM_V4T_CORE, PROCESSOR_FEATURE_FP);

		public enum CpuInstructionSet
		{
			X86 = PROCESSOR_X86_32BIT_INSTRUCTION,
			SH3 = PROCESSOR_HITACHI_SH3_INSTRUCTION,
			SH4 = PROCESSOR_HITACHI_SH4_INSTRUCTION,
			MIPSV4 = PROCESSOR_MIPS_MIPSIV_INSTRUCTION,
			MIPSV4_FP = PROCESSOR_MIPS_MIPSIVFP_INSTRUCTION,
			MIPSVII = PROCESSOR_MIPS_MIPSII_INSTRUCTION,
			MIPSVII_FP = PROCESSOR_MIPS_MIPSIIFP_INSTRUCTION,
			MIPS16 = PROCESSOR_MIPS_MIPS16_INSTRUCTION,
			ARMV4 = PROCESSOR_ARM_V4_INSTRUCTION,
			ARMV4T = PROCESSOR_ARM_V4T_INSTRUCTION,
		}

	}
}
