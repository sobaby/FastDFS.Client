using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDFS.Client.Exception
{

    [Serializable]
    public class FdfsConnectException : System.Exception
    {
        public FdfsConnectException() { }
        public FdfsConnectException(string message) : base(message) { }
        public FdfsConnectException(string message, System.Exception inner) : base(message, inner) { }
        protected FdfsConnectException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
