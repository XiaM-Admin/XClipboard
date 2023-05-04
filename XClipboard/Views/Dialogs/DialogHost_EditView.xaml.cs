using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using XClipboard.ViewModels.Dialogs;

namespace XClipboard.Views.Dialogs
{
    /// <summary>
    /// DialogHost_EditView.xaml 的交互逻辑
    /// </summary>
    public partial class DialogHost_EditView : UserControl
    {
        public DialogHost_EditView()
        {
            InitializeComponent();
            FruitTextBox.Focus();
            FruitTextBox.SelectAll();
            FruitTextBox.KeyDown += FruitTextBox_KeyDown;
        }

        private void FruitTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OK_Button.Focus();
            }
        }
    }
}