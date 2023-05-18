namespace wonderlab.PluginLoader.Interfaces
{
    /// <summary>
    /// 表示事件可被取消
    /// </summary>
    public interface ICancellable
    {
        /// <summary>
        /// 检查是否被取消
        /// </summary>
        /// <returns></returns>
        public bool IsCancel()
        {
            return IsCanceled;
        }
        /// <summary>
        /// 取消事件
        /// </summary>
        public void Cancel()
        {
            IsCanceled = true;
        }
        /// <summary>
        /// 设置是否取消
        /// </summary>
        /// <param name="IsCancel">是否取消</param>
        public void SetCancel(bool IsCancel)
        {
            IsCanceled = IsCancel;
        }
        /// <summary>
        /// 是否取消
        /// </summary>
        public bool IsCanceled { get; set; }
    }
}
