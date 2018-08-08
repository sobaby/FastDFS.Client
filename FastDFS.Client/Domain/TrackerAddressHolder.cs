using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace FastDFS.Client.Domain
{
    class TrackerAddressHolder
    {
        /// <summary>
        /// 连接地址
        /// </summary>
        private IPEndPoint address;

        /// <summary>
        /// 当前是否有效
        /// </summary>
        private bool available;

        /// <summary>
        /// 上次无效时间
        /// </summary>
        private DateTime lastUnavailableTime;

        public TrackerAddressHolder(IPEndPoint address)
        {
            this.address = address;
        }

        /// <summary>
        /// 有效
        /// </summary>
        public void SetActive()
        {
            this.available = true;
        }

        /// <summary>
        /// 无效
        /// </summary>
        public void SetInActive()
        {
            this.available = false;
            this.lastUnavailableTime = DateTime.Now;
        }
        
        /// <summary>
        /// 是否有效
        /// </summary>
        /// <returns></returns>
        public bool IsAvailable()
        {
            return available;
        }
        /// <summary>
        /// 获取失效时间
        /// </summary>
        /// <returns></returns>

        public DateTime GetLastUnavailableTime()
        {
            return lastUnavailableTime;
        }



        /// <summary>
        /// 是否可以尝试连接
        /// </summary>
        /// <param name="retryAfterSecend">在n秒后重试</param>
        /// <returns></returns>
        public bool canTryToConnect(int retryAfterSecend)
        {
            // 如果是有效连接
            if (this.available)
            {
                return true;
                // 如果连接无效，并且达到重试时间
            }
            else if ((DateTime.Now - lastUnavailableTime).TotalSeconds > retryAfterSecend)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 连接地址
        /// </summary>
        /// <returns></returns>
        public IPEndPoint GetAddress()
        {
            return address;
        }
    }
}
