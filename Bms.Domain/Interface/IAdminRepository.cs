namespace Bms.Domain.Interface
{
    /// <summary>
    /// 这个接口帮助我们从Console类解耦"输出"方法
    /// 我们不需要关心怎样输出，只要知道能输出即可
    /// </summary>
    public interface IAdminRepository
    {
        void Write(string content);
    }
}