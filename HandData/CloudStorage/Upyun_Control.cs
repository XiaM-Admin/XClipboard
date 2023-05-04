using DataHandler.Common;
using DataHandler.Models;
using SKIT.FlurlHttpClient.Upyun.Uss;
using SKIT.FlurlHttpClient.Upyun.Uss.Models;
using System.IO;
using System.Threading.Tasks;

namespace DataHandler.CloudStorage
{
    public class Upyun_Control
    {
        public UpyunUssClient Client { get; set; }
        public string BucketName { get; private set; }
        public string Host { get; }

        /// <summary>
        /// 又拍云类
        /// </summary>
        /// <param name="OperatorName">用户名</param>
        /// <param name="OperatorPassword">密码</param>
        /// <param name="BucketName">存储桶名</param>
        /// <param name="Host">访问域名</param>
        public Upyun_Control(string OperatorName, string OperatorPassword, string BucketName, string Host)
        {
            var options = new UpyunUssClientOptions()
            {
                OperatorName = OperatorName,
                OperatorPassword = OperatorPassword
            };
            Client = new UpyunUssClient(options);
            this.BucketName = BucketName;
            this.Host = Host;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="filePath">文件\图片路径</param>
        /// <param name="UpLoadKey">图床路径Key</param>
        /// <returns></returns>
        public async Task<UpLoadResponse> UpLoadImage(string filePath, string UpLoadKey = "")
        {
            var postArray = StorageCommon.GetImageBytes(filePath);
            if (postArray.Length == 0)
                return new UpLoadResponse()
                {
                    Title = "图片不存在",
                    IsSuccessful = false,
                    Text = $"路径{filePath}的图片不存在，请检查后重试。",
                    Code = -1
                };

            if (UpLoadKey.EndsWith("/"))
                UpLoadKey += Path.GetFileName(filePath);
            else
                UpLoadKey += "/" + Path.GetFileName(filePath);

            var request = new UploadFileRequest()
            {
                BucketName = BucketName,
                FileKey = UpLoadKey,
                FileBytes = postArray
            };

            var response = await Client.ExecuteUploadFileAsync(request);
            if (response.IsSuccessful())
                return new UpLoadResponse()
                {
                    Title = "图片上传完成",
                    IsSuccessful = true,
                    Text = $"路径{filePath}的图片上传成功。",
                    Code = response.RawStatus,
                    ImageUrl = this.Host + "/" + UpLoadKey
                };
            else
                return new UpLoadResponse()
                {
                    Title = "图片上传失败",
                    IsSuccessful = false,
                    Text = $"路径{filePath}的图片上传失败，Code:{response.RawStatus},\r\n{response.ErrorCode},\r\n{response.ErrorMessage}",
                    Code = response.RawStatus
                };
        }

        public async Task<bool> UpTestAsync(string UpLoadKey)
        {
            UpLoadKey += "/test.png";
            var request = new UploadFileRequest()
            {
                BucketName = BucketName,
                FileKey = UpLoadKey,
                FileBytes = System.Array.Empty<byte>()
            };
            var response = await Client.ExecuteUploadFileAsync(request);
            if (response.IsSuccessful()) return true;
            return false;
        }
    }
}