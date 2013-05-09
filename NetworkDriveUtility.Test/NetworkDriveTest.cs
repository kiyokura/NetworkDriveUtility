using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetworkDriveUtility.Test
{
    /// <summary>
    /// NetworkDrive の概要の説明
    /// </summary>
    [TestClass]
    public class NetworkDriveTest
    {
        // 実行する環境でここは書き換える
        private const string DriveLetterMappingTest = "S:";
        private const string DriveLetterDeleteTest = "T:";
        private const string UncPathForTestMapping = @"\\localhost\unctest";
        private const string UserName = @"localhost\TestUser";
        private const string UserPassword = "test001";



        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            // ドライブが存在しないか確認
            var drives = System.IO.DriveInfo.GetDrives();
            if (drives.Where(x => x.Name == DriveLetterMappingTest + "\\").Any())
            {
                throw new Exception(string.Format("ドライブ {0} が存在しています。ドライブを削除するか、DriveLetterMappingTestの値を変更してください" , DriveLetterMappingTest));
            }
            if (drives.Where(x => x.Name == DriveLetterDeleteTest + "\\").Any())
            {
                throw new Exception(string.Format("ドライブ {0} が存在しています。ドライブを削除するか、DriveLetterDeleteTestの値を変更してください" , DriveLetterDeleteTest));
            }
        }

        [TestMethod]
        public void ネットワークドライブを作成するテスト()
        {
            NetworkDriveInfo drive = NetworkDriveUtility.NetworkDrive.MapNetworkDrive(DriveLetterMappingTest, UncPathForTestMapping, UserName, UserPassword);
            Assert.AreEqual<string>(DriveLetterMappingTest, drive.DriveLetter);
            Assert.AreEqual<string>(UncPathForTestMapping, drive.RemotePath);
        }

        [TestMethod]
        public void ネットワークドライブを削除するテスト()
        {
            // ドライブレターDriveLetterDeleteTestでドライブを作成する
            NetworkDriveInfo drive = NetworkDriveUtility.NetworkDrive.MapNetworkDrive(DriveLetterDeleteTest, UncPathForTestMapping,null,null);

            // 割り当てたドライブを削除する
            var ret = NetworkDriveUtility.NetworkDrive.UnMapNetworkDrive(DriveLetterDeleteTest, NetworkDrive.WNetCancelConnection2Flags.NON, true);
            Assert.IsTrue(ret);
        }

        [ClassCleanup]
        public static void MyClassCleanup()
        {
            NetworkDriveUtility.NetworkDrive.UnMapNetworkDrive(DriveLetterMappingTest, NetworkDrive.WNetCancelConnection2Flags.NON, true);
        }
    }
}
