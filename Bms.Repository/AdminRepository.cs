using Bms.Domain.Interface;
using System;

namespace Bms.Repository
{
    /// <summary>
    /// 这里IOutput接口的实现完成向控制台的输出。
    /// 技术上讲，我们可以实现IOutput接口完成Debug或者Trace或者
    /// 其他的输出方式
    /// </summary>
    public class AdminRepository : IAdminRepository
    {
        public void Write(string content)
        {
            Console.WriteLine(content);
        }
    }
}