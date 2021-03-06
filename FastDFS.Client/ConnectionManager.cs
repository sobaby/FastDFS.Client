using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using FastDFS.Client.Common;
using FastDFS.Client.Config;
using FastDFS.Client.Domain;
using System.Text;
using FastDFS.Client.Exception;

namespace FastDFS.Client
{
    /// <summary>
    /// 链接管理池
    /// </summary>
    public sealed class ConnectionManager
    {
        #region 私有字段
        
        /// <summary>
        /// 
        /// </summary>
        private static List<IPEndPoint> _listTrackers = new List<IPEndPoint>();

        private static TrackerLocator locator ;


        #endregion

        #region 公共静态字段

        public static Dictionary<IPEndPoint, Pool> TrackerPools = new Dictionary<IPEndPoint, Pool>();
        public static Dictionary<IPEndPoint, Pool> StorePools = new Dictionary<IPEndPoint, Pool>();

        #endregion

        #region 公共静态方法

        private static bool Initialize(List<IPEndPoint> trackers)
        {
            foreach (var point in trackers)
            {
                if (!TrackerPools.ContainsKey(point))
                {
                    var pool = new Pool(point, FDFSConfig.TrackerMaxConnection);
                    TrackerPools.Add(point, pool);
                }
            }
            locator = new TrackerLocator(trackers);

            _listTrackers = trackers;
            return true;
        }

        public static bool InitializeForConfigSection(FastDfsConfig config)
        {
            if (config != null)
            {
                var trackers = new List<IPEndPoint>();

                foreach (var ipInfo in config.FastDfsServer)
                {
                    trackers.Add(new IPEndPoint(IPAddress.Parse(ipInfo.IpAddress), ipInfo.Port));
                }

                return Initialize(trackers);
            }

            return false;
        }

        public static Connection GetTrackerConnection(IPEndPoint ip)
        {
            var pool = TrackerPools[ip];

            return pool.GetConnection();
        }

        public static Connection GetStorageConnection(IPEndPoint endPoint)
        {
            lock ((StorePools as ICollection).SyncRoot)
            {
                if (!StorePools.ContainsKey(endPoint))
                {
                    StorePools.Add(endPoint, new Pool(endPoint, FDFSConfig.StorageMaxConnection));
                }
            }

            return StorePools[endPoint].GetConnection();
        }

        public static byte[] GetResponse(FDFSRequest request)
        {
            Connection conn = null;
            IPEndPoint address = null;
            try
            {
                try
                {
                    if (request.Connection == null)
                    {
                        address = locator.GetTrackerAddress();
                        conn = GetTrackerConnection(address);
                    }
                    else
                    {
                        conn = request.Connection;
                    }
                    //打开
                    conn.OpenConnection();
                    locator.SetActive(address);
                }
                catch (SocketException ex)
                {
                    locator.SetInActive(address);
                    throw new FdfsConnectException("connect failed", ex);
                }

                var stream = conn.GetStream();

                var headerBuffer = request.Header.ToByte();

                stream.Write(headerBuffer, 0, headerBuffer.Length);
                stream.Write(request.Body, 0, request.Body.Length);

                var header = new FDFSHeader(stream);
                
                if (header.Status != 0)
                    throw new FDFSException(string.Format("Get Response Error,Error Code:{0}", header.Status));

                var body = new byte[header.Length];
                if (header.Length != 0) stream.Read(body, 0, (int)header.Length);

                return body;
            }
            finally
            {
                //关闭
                //Connection.Close();
                if (conn!=null)
                    conn.ReleaseConnection();
            }
        }

        #endregion
        
    }
}