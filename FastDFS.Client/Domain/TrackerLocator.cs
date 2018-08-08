using FastDFS.Client.Common;
using FastDFS.Client.Exception;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FastDFS.Client.Domain
{
    /// <summary>
    /// 表示Tracker服务器位置
    /// </summary>
    class TrackerLocator
    {
        /// <summary>
        /// 10分钟以后重试连接
        /// </summary>
        private static readonly int DEFAULT_RETRY_AFTER_SECEND = 60;

        /// <summary>
        /// tracker服务配置地址列表
        /// </summary>
        private List<IPEndPoint> trackerList = new List<IPEndPoint>();

        /// <summary>
        /// 目录服务地址-为了加速处理，增加了一个map
        /// </summary>
        private Dictionary<IPEndPoint, TrackerAddressHolder> trackerAddressMap = new Dictionary<IPEndPoint, TrackerAddressHolder>();

        /// <summary>
        /// 轮询圈
        /// </summary>
        private CircularList<TrackerAddressHolder> trackerAddressCircular = new CircularList<TrackerAddressHolder>();

        /// <summary>
        /// 连接中断以后经过N秒重试
        /// </summary>
        private int retryAfterSecend = DEFAULT_RETRY_AFTER_SECEND;

        /// <summary>
        /// 初始化Tracker服务器地址
        /// 配置方式为 ip:port 如 192.168.1.2:21000
        /// </summary>
        /// <param name="trackerList"></param>
        public TrackerLocator(List<IPEndPoint> trackerList)
        {
            this.trackerList = trackerList;
            buildTrackerAddresses();
        }

        /// <summary>
        /// 分析TrackerAddress
        /// </summary>
        private void buildTrackerAddresses()
        {

            foreach (var item in this.trackerList)
            {
                TrackerAddressHolder holder = new TrackerAddressHolder(item);
                trackerAddressCircular.Add(holder);
                trackerAddressMap.Add(item, holder);
            }
        }

        /// <summary>
        /// 获取Tracker服务器地址
        /// </summary>
        /// <returns></returns>
        public IPEndPoint GetTrackerAddress()
        {
            TrackerAddressHolder holder;
            // 遍历连接地址,抓取当前有效的地址
            for (int i = 0; i < trackerAddressCircular.Count; i++)
            {
                holder = trackerAddressCircular.Next();
                if (holder.canTryToConnect(retryAfterSecend))
                {
                    return holder.GetAddress();
                }
            }
            throw new FdfsUnavailableException("找不到可用的tracker " + getTrackerAddressConfigString());
        }
        
        /// <summary>
        /// 获取配置地址列表
        /// </summary>
        /// <returns></returns>
        private string getTrackerAddressConfigString()
        {
            StringBuilder config = new StringBuilder();
            for (int i = 0; i < trackerAddressCircular.Count; i++)
            {
                TrackerAddressHolder holder = trackerAddressCircular.Next();
                IPEndPoint address = holder.GetAddress();
                config.Append(address.ToString()).Append(",");
            }
            return config.ToString();
        }
        /// <summary>
        /// 设置连接有效
        /// </summary>
        /// <param name="address"></param>
        public void SetActive(IPEndPoint address)
        {
            if (address == null) return;

            if (trackerAddressMap.TryGetValue(address, out TrackerAddressHolder holder))
            {
                holder.SetActive();
            }
        }

        /// <summary>
        /// 设置连接无效
        /// </summary>
        /// <param name="address"></param>
        public void SetInActive(IPEndPoint address)
        {
            if (address == null) return;

            if (trackerAddressMap.TryGetValue(address,out TrackerAddressHolder holder))
            {
                holder.SetInActive();
            }
        }
    }
}
