using System.Runtime.InteropServices;
using System;

public static class UNC
{
    //
    //Dll import declarations
    //
    [DllImport("mpr.dll")]
    private static extern int WNetGetUniversalName(string lpLocalPath, int dwInfoLevel, ref UNIVERSAL_NAME_INFO lpBuffer, ref int lpBufferSize);

    [DllImport("mpr", CharSet = CharSet.Auto)]
    private static extern int WNetGetUniversalName(string lpLocalPath, int dwInfoLevel, IntPtr lpBuffer, ref int lpBufferSize);

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    private struct UNIVERSAL_NAME_INFO
    {
        [MarshalAs(UnmanagedType.LPTStr)]
        public string lpUniversalName;
    }

    //
    //Constants
    //
    private const int NO_ERROR = 0;
    private const int ERROR_MORE_DATA = 234;
    private const int ERROR_NOT_CONNECTED = 2250;
    private const int UNIVERSAL_NAME_INFO_LEVEL = 1;

    //
    //GetUNCPath function
    //
    static public bool TryGetUNCPath(string mappedDrive, out string UNCPath)
    {
        UNCPath = "";

        UNIVERSAL_NAME_INFO rni = new UNIVERSAL_NAME_INFO();
        int bufferSize = Marshal.SizeOf(rni);

        int nRet = WNetGetUniversalName(
        mappedDrive, UNIVERSAL_NAME_INFO_LEVEL,
        ref rni, ref bufferSize);

        if (ERROR_MORE_DATA == nRet)
        {
            IntPtr pBuffer = Marshal.AllocHGlobal(bufferSize); ;
            try
            {
                nRet = WNetGetUniversalName(mappedDrive, UNIVERSAL_NAME_INFO_LEVEL, pBuffer, ref bufferSize);

                if (NO_ERROR == nRet)
                {
                    rni = (UNIVERSAL_NAME_INFO)Marshal.PtrToStructure(pBuffer,
                    typeof(UNIVERSAL_NAME_INFO));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(pBuffer);
            }
        }
        switch (nRet)
        {
            case NO_ERROR:
                UNCPath = rni.lpUniversalName;
                return true;

            case ERROR_NOT_CONNECTED:
                //Local file-name
                return false;

            default:
                return false;
        }
    }
}