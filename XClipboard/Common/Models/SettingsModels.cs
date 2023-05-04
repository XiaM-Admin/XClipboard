using System.Collections.Generic;

namespace XClipboard.Common.Models
{
    public abstract class SettingsModels
    {
        /// <summary>
        /// 设置标题
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 展示类型
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Label ComboBox 设置项
    /// </summary>
    public class TextBlockComboBoxSetting : SettingsModels
    {
        /// <summary>
        /// Label ComboBox 设置项
        /// </summary>
        /// <param name="v1">标题</param>
        /// <param name="list">列内容</param>
        /// <param name="v2">选中值</param>
        public TextBlockComboBoxSetting(string v1, List<string> list, string v2)
        {
            this.Type = "ComboBox";
            this.Label = v1;
            this.ComboBoxItems = list;
            this.Value = v2;
        }

        /// <summary>
        /// 选中值
        /// </summary>
        public string Value { get; set; }

        public List<string> ComboBoxItems { get; set; }
    }

    /// <summary>
    /// Label TextBox 设置 填空
    /// </summary>
    public class TextBlockTextBoxSetting : SettingsModels
    {
        /// <summary>
        /// Label TextBox 设置 填空
        /// </summary>
        /// <param name="v1">标题</param>
        /// <param name="v2">内容</param>
        public TextBlockTextBoxSetting(string v1, string v2)
        {
            this.Type = "TextBox";
            this.Label = v1;
            this.Value = v2;
        }

        /// <summary>
        /// 内容
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 可选框 设置
    /// </summary>
    public class TextBlockCheckBoxSetting : SettingsModels
    {
        /// <summary>
        /// 可选框 设置
        /// </summary>
        /// <param name="l1">标题</param>
        /// <param name="value">true or false</param>
        public TextBlockCheckBoxSetting(string l1, bool value)
        {
            this.Label = l1;
            this.Type = "CheckBox";
            this.Value = value;
        }

        public bool Value { get; set; }
    }
}