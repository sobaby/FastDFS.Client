FastDFS.Client
==============

FastDFS.Client for .netframework 4.5


example
==============
var config = FastDfsManager.GetConfigSection();

StorageNode storageNode = null;

var fileName = "";

do
{
	Console.WriteLine("1.Init");
	Console.WriteLine("2.GetStorageNode");
	Console.WriteLine("3.UploadFile");
	Console.WriteLine("4.RemoveFile");

	Console.Write("please input command[1,2,3,4]:");
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