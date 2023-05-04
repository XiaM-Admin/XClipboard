namespace DataHandler.Models
{
    /// <summary>
    /// 上传返回Model
    /// </summary>
    public class UpLoadResponse
    {
        /// <summary>
        /// response Code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 成功后返回的图床链接
        /// </summary>
        public string ImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccessful { get; set; }

        /// <summary>
        /// 描述文本
        /// </summary>
        public string Text { get; set; } = string.Empty;

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; } = string.Empty;
    }
}