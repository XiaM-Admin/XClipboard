# XClipboard
这是一个剪贴板监控程序，您剪贴板的一切数据都可以被`XClipboard`记录，存储在本地的数据库中，所以你不需要担心自己的数据泄露！您的所有数据不会离开您的电脑。

## 为什么要做这个项目？
除了剪贴板记录的功能，我还加入了图床功能，为了自己更方便的博客记录，您只管截图，剩下的交给`XClipboard`。XClipboard会自动将您剪贴板的图片做上传处理，保存至您想存放的云存储桶中（又拍云...），若想使用这张图片，XClipboard为您转换了多种链接格式（`MarkDown`、`BBCode`、`HTML`等）。

## 项目结构
采用WPF MVVM框架构建，使用Prism、MaterialDesignInXamlToolkit等开源库实现各种功能。

![](https://p.x-tools.top/Images/2023 05-12 22-24-19.png)

![2023 05-07 20-55-18.png](https://p.x-tools.top/Images/2023%2005-07%2020-55-18.png)

![](https://p.x-tools.top/Images/2023 05-12 22-29-44.png)

## 建议

- 我不建议使用Win10 & 11 的系统截图工具，因为这会导致本程序连续保存多次该截图，建议使用免费的Snipaste或者picpick来作为截图工具使用！