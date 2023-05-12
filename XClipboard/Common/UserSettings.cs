using DataHandler.CloudStorage;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using System.Windows;
using XClipboard.Common.Models;
using XClipboard.Core;
using Microsoft.Win32;
using XClipboard.ClipboardHistory;
using System.Net.NetworkInformation;

namespace XClipboard.Common
{
    public class UserSettings
    {
        private JsonSteeingsModels steeingsModels { get; set; }
        public JsonSettings JsonSettings { get; set; }

        public UserSettings()
        {
            //var Value = JsonConvert.SerializeObject(Object);
            //JsonSteeings settings = JsonConvert.DeserializeObject<JsonSteeings>(Value);
            //JsonSettings = new JsonSettings()
            //{
            //    ClipboardSettings = new()
            //    {
            //        duplicate_data = true,
            //        image_save_path = "Data\\Images\\",
            //        handleData = new()
            //        {
            //            Text = true,
            //            Image = true,
            //            Files = true
            //        }
            //    },
            //    ImageurlSettings = new()
            //    {
            //        CopyUrlMode = "Url",
            //        DefaultOSS = "无",
            //        UpyunSettings = new()
            //        {
            //            Bucket = "Bucket",
            //            Host = "Host",
            //            Password = "Password",
            //            UpPath = "Images/",
            //            User = "User"
            //        }
            //    },
            //    SystemSettings = new()
            //    {
            //        IsListen = true,
            //        Show_Number = "5",
            //        Startup = false,
            //        Upload_Alert = false,
            //        Upload_Auto = false
            //    }
            //};
            //var Value = JsonConvert.SerializeObject(JsonSettings);
            var dbobj = Program_State.GetDBService();
            JsonSteeingsModels jsonSteeingsModels = new();
            if (!dbobj.CheckTable(DBService_Core.UserSettingName, jsonSteeingsModels)) throw new Exception("UserSettings Init Error.1");
            JsonSteeingsModels task = dbobj.GetDataByID<JsonSteeingsModels>(dbobj.GetDataNumber(DBService_Core.UserSettingName).Result, DBService_Core.UserSettingName).Result;
            if (task is null)
            {
                //创建默认配置
                JsonSettings = JsonConvert.DeserializeObject<JsonSettings>(DBService_Core.DefaultJsonSettings);
                //写配置数据库
                steeingsModels = new()
                {
                    Json = DBService_Core.DefaultJsonSettings,
                    CreateDate = DateTime.Now
                };
                dbobj.InsertData(steeingsModels, DBService_Core.UserSettingName);
            }
            else
            {
                steeingsModels = task;
                JsonSettings = JsonConvert.DeserializeObject<JsonSettings>(task.Json);
            }
        }

        public async Task<int> SaveAsync()
        {
            //数据检查
            var dbobj = Program_State.GetDBService();
            JsonSteeingsModels task = dbobj.GetDataByID<JsonSteeingsModels>(dbobj.GetDataNumber(DBService_Core.UserSettingName).Result, DBService_Core.UserSettingName).Result;
            JsonSettings JsonSettings_New = JsonConvert.DeserializeObject<JsonSettings>(task.Json);
            if (JsonSettings_New.Equals(JsonSettings))
                throw new Exception("设置未被更改.");

            //处理数据
            await checkSetAsync(JsonSettings);
            SetAutoStart(JsonSettings.SystemSettings.Startup);

            if (!JsonSettings.SystemSettings.IsListen)
                Program_State.GetAppState().IsClipboardServiceRunning.Clipboard = false;
            else
                Program_State.GetAppState().IsClipboardServiceRunning.Clipboard = true;

            var Value = JsonConvert.SerializeObject(JsonSettings);
            steeingsModels = new()
            {
                Json = Value,
                CreateDate = DateTime.Now
            };
            int ret = await dbobj.InsertDataAsync(steeingsModels, DBService_Core.UserSettingName);
            return ret;
        }

        /// <summary>
        /// 自动开机设置
        /// </summary>
        /// <returns></returns>
        private void SetAutoStart(bool isStart)
        {
            //利用注册表实现开机应用自启动
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (isStart)
                rk.SetValue("XClipboard", System.Windows.Forms.Application.ExecutablePath + " --autostart");
            else
                rk.DeleteValue("XClipboard", false);
        }

        private async Task<bool> checkSetAsync(JsonSettings js)
        {
            //系统设置检查
            if (Convert.ToInt16(js.SystemSettings.Show_Number) < 5 || Convert.ToInt16(js.SystemSettings.Show_Number) > 10)
                js.SystemSettings.Show_Number = "5";
            //剪贴板设置检查
            if (js.ClipboardSettings.image_save_path is null || js.ClipboardSettings.image_save_path == "")
                js.ClipboardSettings.image_save_path = "Data\\Images\\";
            //图片上传设置检查
            if (js.ImageurlSettings.DefaultOSS == "又拍云")
            {
                UpyunSettings upsets = js.ImageurlSettings.UpyunSettings;
                //又拍云检查
                Upyun_Control upyun_Control = new(upsets.User, upsets.Password, upsets.Bucket, upsets.Host);
                var ret = await upyun_Control.UpTestAsync(upsets.UpPath);
                if (!ret) throw new Exception("又拍云上传测试失败！");
            }

            return true;
        }


    }
}