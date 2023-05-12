using DataHandler.CloudStorage;
using DataHandler.Models;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XClipboard.Common
{
    public static class Auto_UpImage
    {
        public static async Task<UpLoadResponse> UpImageAsync(string OSS_Name, string filePath)
        {
            var appset = Program_State.GetJsonSettings().ImageurlSettings;

            UpLoadResponse ret = new();
            switch (OSS_Name)
            {
                case "又拍云":
                    Upyun_Control upyun_ = new(appset.UpyunSettings.User, appset.UpyunSettings.Password, appset.UpyunSettings.Bucket, appset.UpyunSettings.Host);
                    ret = await upyun_.UpLoadImage(filePath, appset.UpyunSettings.UpPath);
                    break;
                default:
                    break;
            }

            return ret;
        }
        /// <summary>
        /// 判断图片是否一致
        /// </summary>
        /// <param name="img">图片一</param>
        /// <param name="bmp">图片二</param>
        /// <returns>是否一致</returns>
        public static bool IsSameImg(Bitmap img, Bitmap bmp)
        {
            if (bmp == null || img == null) return false;
            //大小一致
            if (img.Width == bmp.Width && img.Height == bmp.Height)
            {
                //将图片一锁定到内存
                BitmapData imgData_i = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                IntPtr ipr_i = imgData_i.Scan0;
                int length_i = imgData_i.Width * imgData_i.Height * 3;
                byte[] imgValue_i = new byte[length_i];
                Marshal.Copy(ipr_i, imgValue_i, 0, length_i);
                img.UnlockBits(imgData_i);
                //将图片二锁定到内存
                BitmapData imgData_b = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
                IntPtr ipr_b = imgData_b.Scan0;
                int length_b = imgData_b.Width * imgData_b.Height * 3;
                byte[] imgValue_b = new byte[length_b];
                Marshal.Copy(ipr_b, imgValue_b, 0, length_b);
                img.UnlockBits(imgData_b);
                //长度不相同
                if (length_i != length_b)
                {
                    return false;
                }
                else
                {
                    //循环判断值
                    for (int i = 0; i < length_i; i++)
                    {
                        //不一致
                        if (imgValue_i[i] != imgValue_b[i])
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            else
                return false;
        }

        public static Bitmap ToBitmap(Image image)
        {
            Bitmap bitmap = new(image.Width, image.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(bitmap))
                graphics.DrawImage(image, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

            return bitmap;
        }
    }
}
