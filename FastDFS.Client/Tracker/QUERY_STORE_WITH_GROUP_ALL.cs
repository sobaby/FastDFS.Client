using System;
using FastDFS.Client.Common;
using FastDFS.Client.Exception;
using System.Collections.Generic;
using System.IO;
using FastDFS.Client.Domain;

namespace FastDFS.Client.Tracker
{
    /// <summary>
    /// query which storage server to store file
    /// 
    /// Reqeust 
    ///     Cmd: TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ALL 107
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    /// Response
    ///     Cmd: TRACKER_PROTO_CMD_RESP
    ///     Status: 0 right other wrong
    ///     Body: 
    ///     @ FDFS_GROUP_NAME_MAX_LEN bytes: group name
    ///     @ IP_ADDRESS_SIZE - 1 bytes: storage server ip address
    ///     @ FDFS_PROTO_PKG_LEN_SIZE bytes: storage server port
    ///     @ 1 byte: store path index on the storage server
    /// </summary>
    public class QUERY_STORE_WITH_GROUP_ALL : FDFSRequest
    {
        #region 单例对象

        private static readonly QUERY_STORE_WITH_GROUP_ALL _instance = new QUERY_STORE_WITH_GROUP_ALL();

        public static QUERY_STORE_WITH_GROUP_ALL Instance
        {
            get
            {
                return _instance;
            }
        }

        #endregion

        private QUERY_STORE_WITH_GROUP_ALL()
        {

        }

        public override FDFSRequest GetRequest(params object[] paramList)
        {
            if (paramList.Length == 0)
                throw new FDFSException("GroupName is null");

            var result = new QUERY_STORE_WITH_GROUP_ALL();

            var groupName = Util.StringToByte((string)paramList[0]);
            if (groupName.Length > Consts.FDFS_GROUP_NAME_MAX_LEN)
            {
                throw new FDFSException("GroupName is too long");
            }

            var body = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];
            Array.Copy(groupName, 0, body, 0, groupName.Length);
            result.Body = body;
            result.Header = new FDFSHeader(Consts.FDFS_GROUP_NAME_MAX_LEN, Consts.TRACKER_PROTO_CMD_SERVICE_QUERY_STORE_WITH_GROUP_ALL, 0);

            return result;
        }

        public class Response
        {
            public string GroupName;

            public List<StorageItem> StorageNode;
            public Response(byte[] responseByte)
            {
                var groupNameBuffer = new byte[Consts.FDFS_GROUP_NAME_MAX_LEN];

                Array.Copy(responseByte, groupNameBuffer, Consts.FDFS_GROUP_NAME_MAX_LEN);

                GroupName = Util.ByteToString(groupNameBuffer).TrimEnd('\0');

                int fixFieldsTotalSize = 23;

                if ((responseByte.Length - 1 - Consts.FDFS_GROUP_NAME_MAX_LEN) % fixFieldsTotalSize != 0)
                {
                    throw new IOException("fixFieldsTotalSize=" + fixFieldsTotalSize + "but byte array length: " + responseByte.Length
                            + " is invalid!");
                }
                // 计算反馈对象数量
                int count = (responseByte.Length -1 - Consts.FDFS_GROUP_NAME_MAX_LEN) / fixFieldsTotalSize;
                int offset = Consts.FDFS_GROUP_NAME_MAX_LEN;

                StorageNode = new List<StorageItem>(count);
                for (int i = 0; i < count; i++)
                {
                    StorageItem storage = new StorageItem();

                    var ipAddressBuffer = new byte[Consts.IP_ADDRESS_SIZE - 1];

                    Array.Copy(responseByte, offset, ipAddressBuffer, 0, Consts.IP_ADDRESS_SIZE - 1);

                    storage.IpStr = new string(FDFSConfig.Charset.GetChars(ipAddressBuffer)).TrimEnd('\0');

                    var portBuffer = new byte[Consts.FDFS_PROTO_PKG_LEN_SIZE];

                    Array.Copy(responseByte, offset + Consts.IP_ADDRESS_SIZE - 1, portBuffer, 0, Consts.FDFS_PROTO_PKG_LEN_SIZE);

                    storage.StoragePort = (int)Util.BufferToLong(portBuffer, 0);

                    storage.StorePathIndex = responseByte[responseByte.Length - 1];

                    StorageNode.Add(storage);

                    offset += fixFieldsTotalSize;
                }                
            }
        }
    }
}