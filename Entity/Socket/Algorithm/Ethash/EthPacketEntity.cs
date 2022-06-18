namespace EthereumForward.Entity.Socket.Algorithm.Ethash
{
    public class EthPacketEntity
    {
        /// <summary>
        /// 通讯ID
        /// </summary>
        public int id { get; set; }
        /// <summary>
        /// 调用接口
        /// </summary>
        public string method { get; set; }
        /// <summary>
        /// 提交数据
        /// </summary>
        public List<string> @params { get; set; }
        /// <summary>
        /// jsonrpc版本
        /// </summary>
        public string jsonrpc { get; set; }
        /// <summary>
        /// 矿机名
        /// </summary>
        public string worker { get; set; }

    }
}
