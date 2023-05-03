using System.Text;

namespace Net.WebApi.Demo.Common;

/// <summary>
/// 本地文件日志记录对象
/// </summary>
public class Log
{
    /// <summary>
    /// 日志记录锁
    /// </summary>
    private static readonly object Lock = new();

    /// <summary>
    /// 记录日志
    /// </summary>
    /// <param name="level"></param>
    /// <param name="title"></param>
    /// <param name="content"></param>
    private static void LogWrite(string level, string title, string? content = null)
    {
        try
        {
            lock (Lock)
            {
                // 文件夹
                var dir = Path.Combine(AppContext.BaseDirectory, "logs");
                Directory.CreateDirectory(dir);

                // 文件
                var path = Path.Combine(dir, $"{DateTime.Now:yyyy-MM-dd}_{level}.log");

                // 日志内容
                content = $"{DateTime.Now:HH:mm:ss}\t{title}\r\n{(string.IsNullOrEmpty(content) ? "" : content)}\r\n\r\n";
                var buffer = Encoding.UTF8.GetBytes(content);

                // 文本流
                using var fs = !File.Exists(path)
                    ? File.Create(path)
                    : new FileStream(path, FileMode.Append);

                fs.Write(buffer);
                fs.Flush();
                fs.Dispose();
                fs.Close();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"日志记录异常: {ex.Message}");
        }
    }

    /// <summary>
    /// 普通日志
    /// </summary>
    /// <param name="title"></param>
    /// <param name="content"></param>
    public static void Info(string title, string? content = null) => Task.Run(() => LogWrite("Info", title, content));

    /// <summary>
    /// 警告日志
    /// </summary>
    /// <param name="title"></param>
    /// <param name="content"></param>
    public static void Warn(string title, string? content = null) => Task.Run(() => LogWrite("Warn", title, content));

    /// <summary>
    /// 错误日志
    /// </summary>
    /// <param name="title"></param>
    /// <param name="content"></param>
    public static void Error(string title, string? content = null) => Task.Run(() => LogWrite("Error", title, content));

    /// <summary>
    /// 自定义日志
    /// </summary>
    /// <param name="mark"></param>
    /// <param name="title"></param>
    /// <param name="content"></param>
    public static void Custom(string mark, string title, string? content = null) => Task.Run(() => LogWrite(mark, title, content));
}