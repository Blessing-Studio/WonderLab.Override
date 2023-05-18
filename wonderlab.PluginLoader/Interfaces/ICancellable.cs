namespace wonderlab.PluginLoader.Interfaces
{
    /// <summary>
    /// ��ʾ�¼��ɱ�ȡ��
    /// </summary>
    public interface ICancellable
    {
        /// <summary>
        /// ����Ƿ�ȡ��
        /// </summary>
        /// <returns></returns>
        public bool IsCancel()
        {
            return IsCanceled;
        }
        /// <summary>
        /// ȡ���¼�
        /// </summary>
        public void Cancel()
        {
            IsCanceled = true;
        }
        /// <summary>
        /// �����Ƿ�ȡ��
        /// </summary>
        /// <param name="IsCancel">�Ƿ�ȡ��</param>
        public void SetCancel(bool IsCancel)
        {
            IsCanceled = IsCancel;
        }
        /// <summary>
        /// �Ƿ�ȡ��
        /// </summary>
        public bool IsCanceled { get; set; }
    }
}
