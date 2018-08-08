using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FastDFS.Client.Common
{
    /// <summary>
    ///  基于ArrayList的循环链表类
    ///  第一次调用next() 将返回第一个元素，调用previous() 将返回最后一个元素
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class CircularList<T> : List<T>
    {

        /**
         * serialVersionUID
         */
        private static readonly long serialVersionUID = 1L;

        private int index = -1;

        /**
         * 重置，之后第一次调用next()将返回第一个元素，调用previous()将返回最后一个元素
         */
        public void Reset()
        {
            lock (this)
            {
                index = -1;
            }
        }

        /**
         * 下一个元素
         * 
         * @return
         */
        public T Next()
        {
            CheckEmpty();

            lock (this)
            {
                index++;
                if (index >= this.Count)
                {
                    index = 0;
                }
                return this[index];
            }

        }

        public T Current()
        {
            CheckEmpty();

            lock (this)
            {
                if (index < 0)
                {
                    index = 0;
                }
                return this[index];
            }
        }

        /**
         * 上一个元素
         * 
         * @return
         */
        public T previous()
        {
            CheckEmpty();

            lock (this)
            {
                index--;
                if (index < 0)
                {
                    index = this.Count - 1;
                }
                return this[index];
            }
        }

        private void CheckEmpty()
        {
            if (this.Count == 0)
            {
                throw new IndexOutOfRangeException("空的列表，无法获取元素");
            }
        }

    }
}