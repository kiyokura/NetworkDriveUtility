using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;

namespace NetworkDriveUtility
{
    /// <summary>
    /// ネットワークドライブ情報
    /// </summary>
    public class NetworkDriveInfo
    {
        #region property
        /// <summary>
        /// ドライブレター
        /// </summary>
        /// <remarks>
        /// コロン付のドライブレター（ex. C:）
        /// </remarks>
        public string DriveLetter { get; private set; }

        /// <summary>
        /// リモートのパスのUNC表現
        /// </summary>
        public string RemotePath { get; private set; }

        /// <summary>
        /// 接続状態を表す文字列
        /// </summary>
        public string ConnectionState { get; private set; }

        /// <summary>
        /// 接続しているユーザ名
        /// </summary>
        public string UserName { get; private set; }
        #endregion

        /// <summary>
        /// ネットワークドライブ情報取得
        /// </summary>
        /// <returns></returns>
        public static List<NetworkDriveInfo> GetNewtworkDriveInfo()
        {
            var managementClass = new ManagementClass("Win32_NetworkConnection");
            var managementObj = managementClass.GetInstances();
            var l = new List<NetworkDriveInfo>();
            foreach (var mo in managementObj)
            {
                var info = new NetworkDriveInfo()
                {
                    DriveLetter = string.Format("{0}", mo["LocalName"]),
                    RemotePath = string.Format("{0}", mo["RemotePath"]),
                    ConnectionState = string.Format("{0}", mo["ConnectionState"]),
                    UserName = string.Format("{0}", mo["UserName"])
                };
                l.Add(info);

                Debug.WriteLine("AccessMask:\t{0}", mo["AccessMask"]);
                Debug.WriteLine("Caption:\t{0}", mo["Caption"]);
                Debug.WriteLine("Comment:\t{0}", mo["Comment"]);
                Debug.WriteLine("ConnectionState:\t{0}", mo["ConnectionState"]);
                Debug.WriteLine("ConnectionType:\t{0}", mo["ConnectionType"]);
                Debug.WriteLine("Description:\t{0}", mo["Description"]);
                Debug.WriteLine("DisplayType:\t{0}", mo["DisplayType"]);
                Debug.WriteLine("InstallDate:\t{0}", mo["InstallDate"]);
                Debug.WriteLine("LocalName:\t{0}", mo["LocalName"]);
                Debug.WriteLine("Name:\t{0}", mo["Name"]);
                Debug.WriteLine("Persistent:\t{0}", mo["Persistent"]);
                Debug.WriteLine("ProviderName:\t{0}", mo["ProviderName"]);
                Debug.WriteLine("RemoteName:\t{0}", mo["RemoteName"]);
                Debug.WriteLine("RemotePath:\t{0}", mo["RemotePath"]);
                Debug.WriteLine("ResourceType:\t{0}", mo["ResourceType"]);
                Debug.WriteLine("Status:\t{0}", mo["Status"]);
                Debug.WriteLine("UserName:\t{0}", mo["UserName"]);
                Debug.WriteLine("-----------------------------------------");
            }

            return l;
        }
    }
}
