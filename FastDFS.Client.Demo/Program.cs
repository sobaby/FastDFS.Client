using System;
using System.IO;
using FastDFS.Client.Common;
using FastDFS.Client.Config;

namespace FastDFS.Client.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = FastDfsManager.GetConfigSection();

            StorageNode storageNode = null;

            var fileName = "";

            do
            {
                Console.WriteLine("\r\n1.Init");
                Console.WriteLine("2.GetStorageNode");
                Console.WriteLine("3.UploadFile");
                Console.WriteLine("4.RemoveFile");

                Console.Write("请输入命令：");
                var cmd = Console.ReadLine();

                switch (cmd)
                {
                    case "1":
                        ConnectionManager.InitializeForConfigSection(config);
                        break;

                    case "2":
                        storageNode = FastDFSClient.GetStorageNode(config.GroupName);
                        Console.WriteLine(storageNode.EndPoint);
                        break;

                    case "3":
                        fileName = FastDFSClient.UploadFile(storageNode, File.ReadAllBytes("test.jpg"), "jpg");
                        Console.WriteLine(fileName);
                        break;

                    case "4":
                        FastDFSClient.RemoveFile(config.GroupName, fileName);
                        break;
                }

            } while (true);

        }
    }
}
