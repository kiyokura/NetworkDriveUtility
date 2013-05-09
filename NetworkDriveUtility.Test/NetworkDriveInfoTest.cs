using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace NetworkDriveUtility.Test
{
    [TestClass]
    public class NetworkDriveInfoTest
    {
        // 実行する環境でここは書き換える
        private const string DriveLetterMappingTest = "N:";
        private const string UncPathForTestMapping = @"\\localhost\unctest";
        private const string UserName = @"localhost\TestUser";

        [ClassInitialize]
        public static void MyClassInitialize(TestContext testContext)
        {
            // ドライブが存在しないか確認
            var drives = System.IO.DriveInfo.GetDrives();
            if (!drives.Where(x => x.Name == DriveLetterMappingTest + "\\").Any())
            {
                throw new Exception(string.Format("ドライブ {0} が存在していません。ドライブを作成してください。", DriveLetterMappingTest));
            }

            var drive = drives.Where(x => x.Name == DriveLetterMappingTest + "\\").First();
            if (!(drive.DriveType == System.IO.DriveType.Network))
            {
                throw new Exception(string.Format("ドライブ {0} はネットワークドライブではありません。DriveLetterMappingTestの設定を変更し、テスト条件に合うネットワークドライブを作成してください。", DriveLetterMappingTest));
            }
        }


        [TestMethod]
        public void Testネットワークドライブのリモートパスが取得できる()
        {
            // あらかじめ、ネットワークドライブを作成しておく
            // ローカルPCに、Nドライブにunctestという名前で共有フォルダを作っておく前提
            var drives = NetworkDriveUtility.NetworkDriveInfo.GetNewtworkDriveInfo();
            var drive = drives.Where(x => x.DriveLetter == DriveLetterMappingTest).First();

            Assert.AreEqual<string>(UncPathForTestMapping, drive.RemotePath);
        }

        [TestMethod]
        public void TestネットワークドライブのConnectionStateが取得できる()
        {
            // あらかじめ、ネットワークドライブを作成しておく
            // ローカルPCに、Nドライブにunctestという名前で共有フォルダを作っておく前提
            var drives = NetworkDriveUtility.NetworkDriveInfo.GetNewtworkDriveInfo();
            var drive = drives.Where(x => x.DriveLetter == DriveLetterMappingTest).First();

            Assert.AreEqual<string>("Connected", drive.ConnectionState);
        }

        [TestMethod]
        public void Testネットワークドライブの接続ユーザ名が取得できる()
        {
            // あらかじめ、ネットワークドライブを作成しておく
            // ローカルPCに、Nドライブにunctestという名前で共有フォルダを作っておく前提
            // さらに、ローカルユーザとしてTestUserというユーザをつくり、localhost\TestUserで接続するようにネットワークドライブを設定しておくこと。
            var drives = NetworkDriveUtility.NetworkDriveInfo.GetNewtworkDriveInfo();
            var drive = drives.Where(x => x.DriveLetter == DriveLetterMappingTest).First();

            Assert.AreEqual<string>(UserName, drive.UserName);

        }
    }
}
