using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace XClipboard.Views
{
    /// <summary>
    /// ClipboardView.xaml 的交互逻辑
    /// </summary>
    public partial class ClipboardView : UserControl
    {
        public ClipboardView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 个性化数据表
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyName == "updatatime")
                e.Cancel = true;
            else
                e.Column.Visibility = Visibility.Visible;

            if (e.PropertyName == "content")
                e.Column.MaxWidth = 300;


            Dictionary<string, string> d = new()
            {
                {"content","内容" },
                {"app","来自" },
                {"className","分类" },
                {"contentType","类型" },
                {"createtime","创建时间 "},
                {"updatatime","更新时间 "},
            };

            e.Column.Header = d[e.PropertyName];
        }

    }
}