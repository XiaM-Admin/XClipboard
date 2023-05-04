namespace XClipboard.Common.Models
{
    /// <summary>
    /// 系统运行状态
    /// </summary>
    public class ProgramState
    {
        private bool clipboard;
        private string color;
        private bool isRunTrue;
        private bool localStorage;
        private string text;

        /// <summary>
        /// 剪贴板状态
        /// </summary>
        public bool Clipboard
        {
            get { return clipboard; }
            set { clipboard = value; }
        }

        /// <summary>
        /// 按钮等颜色
        /// </summary>
        public string Color
        {
            get { return color; }
            set { color = value; }
        }

        /// <summary>
        /// 是否能正常运行
        /// </summary>
        public bool IsRunTrue
        {
            get { return isRunTrue; }
            set { isRunTrue = value; }
        }

        /// <summary>
        /// 本地数据库状态
        /// </summary>
        public bool LocalStorage
        {
            get { return localStorage; }
            set { localStorage = value; }
        }

        /// <summary>
        /// 提示文字
        /// </summary>
        public string Text
        {
            get { return text; }
            set { text = value; }
        }
    }
}