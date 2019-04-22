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

            ConnectionManager.InitializeForConfigSection(config);

            do
            {
                // Console.WriteLine("\r\n1.Init");
                Console.WriteLine("\r\n");
                Console.WriteLine("1.GetAllStorageNode");
                Console.WriteLine("2.GetStorageNode");
                Console.WriteLine("3.UploadFile");
                Console.WriteLine("4.RemoveFile");

                Console.Write("请输入命令：");
                var cmd = Console.ReadLine();

                switch (cmd)
                {
                    case "1":
                        var list  = FastDFSClient.GetStorageNodes(config.GroupName);
                        foreach (var item in list)
                        {
                            Console.WriteLine(item.EndPoint.ToString());
                            Console.WriteLine(item.StorePathIndex);
                        }
                        break;

                    case "2":
                        storageNode = FastDFSClient.GetStorageNode(config.GroupName);
                        Console.WriteLine(storageNode.GroupName);
                        Console.WriteLine(storageNode.EndPoint);
                        Console.WriteLine(storageNode.StorePathIndex);
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
