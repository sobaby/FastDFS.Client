using System;
using System.Net.Sockets;

namespace FastDFS.Client.Common
{
    public class Connection : TcpClient
    {
        public Connection()
        {
            InUse = false;
        }

        public Pool Pool { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime LastUseTime { get; set; }

        public bool InUse { get; set; }

        public void Open()
        {
            if (InUse)
            {
                throw new FDFSException("the connection is already in user");
            }

            InUse = true;
            LastUseTime = DateTime.Now;
        }

        public new void Close()
        {
            Pool.CloseConnection(this);
        }

        public void Release()
        {
            Pool.ReleaseConnection(this);
        }
    }
}