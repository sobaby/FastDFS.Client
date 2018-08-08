using System;

namespace FastDFS.Client.Common
{
    public class FDFSRequest
    {
        #region 公共属性

        public FDFSHeader Header { get; set; }

        public byte[] Body { get; set; }

        public Connection Connection { get; set; }

        #endregion

        #region 公共方法

        public byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region 公共虚方法

        public virtual FDFSRequest GetRequest(params object[] paramList)
        {
            throw new NotImplementedException();
        }

        public virtual byte[] GetResponse()
        {
            var body = ConnectionManager.GetResponse(this);

            return body;
        }

        #endregion
    }
}