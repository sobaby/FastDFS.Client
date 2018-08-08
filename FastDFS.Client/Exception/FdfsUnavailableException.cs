using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDFS.Client.Exception
{

    [Serializable]
    public class FdfsUnavailableException : System.Exception
    {
        public FdfsUnavailableException() { }
        public FdfsUnavailableException(string message) : base(message) { }
        public FdfsUnavailableException(string message, System.Exception inner) : base(message, inner) { }
        protected FdfsUnavailableException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
