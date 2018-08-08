using System;

namespace FastDFS.Client.Exception
{
    public class FDFSException : System.Exception
    {
        public FDFSException(string msg) :
            base(msg)
        {

        }
    }
}
