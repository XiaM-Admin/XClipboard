using log4net;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using XClipboard.ClipboardHistory;
using XClipboard.Common;
using XClipboard.Common.Models;
using XClipboard.Core;
using XClipboard.Modules.ModuleName;
using XClipboard.Services;
using XClipboard.Services.Interfaces;
using XClipboard.ViewModels;
using XClipboard.ViewModels.Dialogs;
using XClipboard.ViewModels.Settings;
using XClipboard.Views;
using XClipboard.Views.Dialogs;
using XClipboard.Views.Settings;

namespace XClipboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        /// <summary>
        /// 重构启动代码
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            log4net.Config.XmlConfigurator.Configure();
            ILog log = LogManager.GetLogger("App");//获取一个日志记录器

            log.Info("Start App.");

            // 实例化 AppState 类并设置为应用程序属性
            AppState appState = new();
            this.Properties["AppState"] = appState;

            // 实例化 数据库 对象
            DBService DbObj = new("Data\\Clipboardb.db", DBService_Core.ClipboarName);
            this.Properties["DBObj"] = DbObj;
            DbObj.Connect(DBService_Core.ClipboarName, new Clipboardb_Models(), e =>
            {
                appState.IsClipboardServiceRunning.LocalStorage = e;
                if (e)
                    log.Info($"IsClipboardServiceRunning：{e}");
                else
                    log.Warn($"IsClipboardServiceRunning：{e}");
            });

            // 初始化配置
            appState.userSettings = new();

            //开启剪贴板
            var table = DbObj.CheckTable(DBService_Core.ImgaeurlName, new Imageurldb_Models());
            log.Info($"DBService {DBService_Core.ImgaeurlName}：{table}，Run UpdataClipboards()");
            appState.UpdataClipboards();

            ClipboardService.Start();
            base.OnStartup(e);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IMessageService, MessageService>();

            containerRegistry.RegisterForNavigation<HomeView, HomeViewModels>();
            containerRegistry.RegisterForNavigation<ClipboardView, ClipboardViewModels>();
            containerRegistry.RegisterForNavigation<ImgesUrlView, ImgesUrlViewModels>();
            containerRegistry.RegisterForNavigation<SettingsView, SettingsViewModels>();
            containerRegistry.RegisterForNavigation<SystemView, SystemViewModels>();

            containerRegistry.RegisterDialog<TextShowView, TextShowViewModels>();
            containerRegistry.RegisterDialog<ProgramInfoView, ProgramInfoViewModels>();
            containerRegistry.RegisterDialog<ImageShowView, ImageShowViewModels>();
            containerRegistry.RegisterDialog<DialogHost_EditView, DialogHost_EditViewModels>();
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleNameModule>();
        }
    }
}