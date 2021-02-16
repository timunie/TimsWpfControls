using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TimsWpfControls.ExtensionMethods;

namespace TimsWpfControls.Helper
{
    public static class FileHelper
    {
        public static bool IsFullyQualifiedAdvanced(string fileName)
        {
            try
            {
                return IsFullyQualifiedAdvanced(new Uri(fileName));
            }
            catch
            {
                return false;
            }
        }

        public static bool IsFullyQualifiedAdvanced(Uri uri)
        {
            return uri.Host == Dns.GetHostEntry(uri.Host).HostName;
        }


        public static string GetFullyQualifiedAdvanced(string fileName)
        {
            try
            {
                var uri = new Uri(fileName);

                if (!uri.IsUnc)
                {
                    return fileName;
                }
                else if (IsFullyQualifiedAdvanced(uri))
                {
                    return fileName;
                }
                else
                {
                    return fileName.ReplaceFirst(uri.Host, Dns.GetHostEntry(uri.Host).HostName);
                }
            }
            catch
            {
                return null;
            }
        }


        [DllImport("mpr.dll")]
        static extern int WNetGetUniversalNameA(string lpLocalPath, int dwInfoLevel, IntPtr lpBuffer, ref int lpBufferSize);

        // I think max length for UNC is actually 32,767
        public static string LocalPathToUNC(string localPath, int maxLen = 2000)
        {
            IntPtr lpBuff;

            // Allocate the memory
            try
            {
                lpBuff = Marshal.AllocHGlobal(maxLen);
            }
            catch (OutOfMemoryException)
            {
                return null;
            }

            try
            {
                int res = WNetGetUniversalNameA(localPath, 1, lpBuff, ref maxLen);

                if (res != 0)
                    return GetFullyQualifiedAdvanced(localPath);

                // lpbuff is a structure, whose first element is a pointer to the UNC name (just going to be lpBuff + sizeof(int))
                var result = Marshal.PtrToStringAnsi(Marshal.ReadIntPtr(lpBuff));

                return GetFullyQualifiedAdvanced(result);
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                Marshal.FreeHGlobal(lpBuff);
            }
        }
    }
}
