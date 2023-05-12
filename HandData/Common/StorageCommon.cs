using DataHandler.CloudStorage;
using DataHandler.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DataHandler.Common
{
    public static class StorageCommon
    {
        /// <summary>
        /// 获取图片Bytes字节
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static byte[] GetImageBytes(string filePath)
        {
            if (!File.Exists(filePath))
                return Array.Empty<byte>();

            FileStream fs = new(filePath, FileMode.Open, FileAccess.Read);
            BinaryReader r = new(fs);
            byte[] postArray = r.ReadBytes((int)fs.Length);
            return postArray;
        }

        /// <summary>
        /// 将图片转换为缩略图
        /// </summary>
        /// <param name="image"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static Bitmap CreateThumbnail(Image image, int width, int height)
        {
            var thumbnail = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(thumbnail))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.DrawImage(image, 0, 0, width, height);
            }

            return thumbnail;
        }

        /// <summary>
        /// 将缩略图转换为Base64编码
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static string ImageToBase64(Bitmap image)
        {
            using var memoryStream = new MemoryStream();
            image.Save(memoryStream, ImageFormat.Png);
            return Convert.ToBase64String(memoryStream.ToArray());
        }

        /// <summary>
        /// 将Base64编码转换为图片数据
        /// </summary>
        /// <param name="base64"></param>
        /// <returns></returns>
        public static BitmapImage Base64ToBitmapImage(string base64)
        {
            var bitmapImage = new BitmapImage();
            byte[] bytes = Convert.FromBase64String(base64);

            using (var memoryStream = new MemoryStream(bytes))
            {
                memoryStream.Position = 0;
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
        /// <summary>
        /// 取文件字节大小
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string GetFileSizeString(string filePath)
        {
            long fileSizeInBytes = new FileInfo(filePath).Length;

            if (fileSizeInBytes < 1024)
            {
                return $"{fileSizeInBytes} bytes";
            }
            else if (fileSizeInBytes < 1024 * 1024)
            {
                return $"{(double)fileSizeInBytes / 1024:0.##} KB";
            }
            else
            {
                return $"{(double)fileSizeInBytes / (1024 * 1024):0.##} MB";
            }
        }
    }
}