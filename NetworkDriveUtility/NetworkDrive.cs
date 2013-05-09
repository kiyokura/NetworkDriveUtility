using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace NetworkDriveUtility
{
    /// <summary>
    /// ネットワークドライブ・クラス
    /// </summary>
    public class NetworkDrive
    {
        #region Win32ApiResource
        /// <summary>
        /// 接続の種類のフラグ
        /// </summary>
        public enum WNetCancelConnection2Flags
        {
            /// <summary>
            /// 接続に関する情報を更新しません。
            /// レジストリに恒久的な接続として記憶されている接続は、次のログオン時に復元が試みられます。
            /// 恒久的な接続として記憶されていない接続に対しては、CONNECT_UPDATE_PROFILE フラグの設定を無視します。
            /// </summary>
            NON = 0,

            /// <summary>
            /// ユーザープロファイルを「恒久的な接続でなくなった」という情報に更新します。
            /// 以後はログオン時に、その接続の復元を試みません。（ リモート名を使って資源を切断する場合、恒久的な接続には影響しません。）
            /// </summary>
            CONNECT_UPDATE_PROFILE = 1
        }
        private enum ResourceScope
        {
            RESOURCE_CONNECTED = 1,
            RESOURCE_GLOBALNET,
            RESOURCE_REMEMBERED,
            RESOURCE_RECENT,
            RESOURCE_CONTEXT
        }

        private enum ResourceType
        {
            RESOURCETYPE_ANY,
            RESOURCETYPE_DISK,
            RESOURCETYPE_PRINT,
            RESOURCETYPE_RESERVED
        }

        private enum ResourceUsage
        {
            RESOURCEUSAGE_CONNECTABLE = 0x00000001,
            RESOURCEUSAGE_CONTAINER = 0x00000002,
            RESOURCEUSAGE_NOLOCALDEVICE = 0x00000004,
            RESOURCEUSAGE_SIBLING = 0x00000008,
            RESOURCEUSAGE_ATTACHED = 0x00000010,
            RESOURCEUSAGE_ALL = (RESOURCEUSAGE_CONNECTABLE | RESOURCEUSAGE_CONTAINER | RESOURCEUSAGE_ATTACHED),
        }

        private enum ResourceDisplayType
        {
            RESOURCEDISPLAYTYPE_GENERIC,
            RESOURCEDISPLAYTYPE_DOMAIN,
            RESOURCEDISPLAYTYPE_SERVER,
            RESOURCEDISPLAYTYPE_SHARE,
            RESOURCEDISPLAYTYPE_FILE,
            RESOURCEDISPLAYTYPE_GROUP,
            RESOURCEDISPLAYTYPE_NETWORK,
            RESOURCEDISPLAYTYPE_ROOT,
            RESOURCEDISPLAYTYPE_SHAREADMIN,
            RESOURCEDISPLAYTYPE_DIRECTORY,
            RESOURCEDISPLAYTYPE_TREE,
            RESOURCEDISPLAYTYPE_NDSCONTAINER
        }

        [StructLayout(LayoutKind.Sequential)]
        private class NETRESOURCE
        {
            public ResourceScope dwScope = 0;
            public ResourceType dwType = 0;
            public ResourceDisplayType dwDisplayType = 0;
            public ResourceUsage dwUsage = 0;
            public string lpLocalName = null;
            public string lpRemoteName = null;
            public string lpComment = null;
            public string lpProvider = null;
        }

        [DllImport("mpr.dll")]
        private static extern int WNetAddConnection2(NETRESOURCE lpNetResource, string lpPassword, string lpUsername, int dwFlags);

        [DllImport("mpr.dll")]
        private static extern int WNetCancelConnection2(string name, WNetCancelConnection2Flags flags, bool force);
        #endregion

        /// <summary>
        /// ネットワークドライブ割り当て
        /// </summary>
        /// <param name="driveLetter">割り当てドライブレター</param>
        /// <param name="uncPath">割り当て元ととなるネットワークリソースのUNC（例：\\severname\sharename）</param>
        /// <param name="userName">接続に利用するユーザ名（OSへのログオンユーザ情報を利用する場合はnullを指定）</param>
        /// <param name="userPassword">接続に利用するユーザのパスワード（OSへのログオンユーザ情報を利用する場合はnullを指定）</param>
        /// <returns></returns>
        public static NetworkDriveInfo MapNetworkDrive(string driveLetter, string uncPath, string userName, string userPassword)
        {
            NETRESOURCE myNetResource = new NETRESOURCE();
            myNetResource.lpLocalName = driveLetter;
            myNetResource.lpRemoteName = uncPath;
            myNetResource.lpProvider = null;
            int result = WNetAddConnection2(myNetResource, userPassword,userName, 0);
            if (!result.Equals(0))
            {
                throw new Win32Exception((int)result);
            }

            return NetworkDriveInfo.GetNewtworkDriveInfo().Where(x => x.DriveLetter == driveLetter).FirstOrDefault();
        }

        /// <summary>
        /// ネットワークドライブの切断
        /// </summary>
        /// <param name="driveLetter">切断対象のドライブレター</param>
        /// <param name="flags">接続の種類のフラグ</param>
        /// <param name="force">切断を強制するか（trueで強制切断）</param>
        /// <returns></returns>
        public static bool UnMapNetworkDrive(string driveLetter, WNetCancelConnection2Flags flags, bool force)
        {
            int result = WNetCancelConnection2(driveLetter, flags, force);
            if (!result.Equals(0))
            {
                throw new Win32Exception((int)result);
            }
            return true;
        }


    }
}
