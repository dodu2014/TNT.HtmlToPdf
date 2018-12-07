using System.IO;
using System.Runtime.InteropServices;

namespace TNT.HtmlToPdf
{
    /// <summary>
    /// Wkhtmltopdf 生成器
    /// </summary>
    public class WkhtmltopdfDriver : WkhtmlDriver
    {
        /// <summary>
        /// wkhtmltopdf 至在 windows 环境下有exe扩展名.
        /// </summary>
        private static readonly string wkhtmlExe =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "wkhtmltopdf.exe" : "wkhtmltopdf";

        /// <summary>
        /// 将HTML字符串转换为PDF.
        /// </summary>
        /// <param name="wkhtmltopdfPath">wkthmltopdf 路径.</param>
        /// <param name="switches">传递的命令参数.</param>
        /// <param name="html">包含应转换为PDF的HTML代码的字符串.</param>
        /// <returns>作为字节数组的PDF.</returns>
        public static byte[] ConvertHtml(string wkhtmltopdfPath, string switches, string html) {
            return Convert(wkhtmltopdfPath, switches, html, wkhtmlExe);
        }

        /// <summary>
        /// 将给定URL转换成PDF.
        /// </summary>
        /// <param name="wkhtmltopdfPath">wkthmltopdf 路径.</param>
        /// <param name="switches">传递的命令参数.</param>
        /// <returns>作为字节数组的PDF.</returns>
        public static byte[] Convert(string wkhtmltopdfPath, string switches) {
            return Convert(wkhtmltopdfPath, switches, null, wkhtmlExe);
        }
    }
}

