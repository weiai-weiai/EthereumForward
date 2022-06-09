namespace EthereumForward.JSON
{
    public class ConfigEntity
    {
        /// <summary>
        /// Web端口
        /// </summary>
        public int WebPort { get; set; }
        /// <summary>
        /// 转发信息
        /// </summary>
        public List<ForwardItem> Forward { get; set; }
    }
    public class ForwardItem
    {
        /// <summary>
        /// 这里是指本地协议
        /// </summary>
        public string ServerAgreement { get; set; }
        /// <summary>
        /// 这里是指本地端口号
        /// </summary>
        public int ServerPort { get; set; }
        /// <summary>
        /// 这里是指远端协议
        /// </summary>
        public string ClientAgreement { get; set; }
        /// <summary>
        /// 这里是指远端端口号
        /// </summary>
        public int ClientPort { get; set; }
        /// <summary>
        /// 这里是指远端服务器地址
        /// </summary>
        public string ClientIp { get; set; }
        /// <summary>
        /// 是否为纯转发
        /// </summary>
        public string IsPureForwarding { get; set; }
        /// <summary>
        /// 证书地址
        /// </summary>
        public string CertificateUrl { get; set; }
    }

}
