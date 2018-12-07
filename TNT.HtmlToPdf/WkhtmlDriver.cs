using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace TNT.HtmlToPdf
{
    /// <summary>
    /// Wkhtml 生成器
    /// </summary>
    public abstract class WkhtmlDriver
    {
        /// <summary>
        /// 将给定URL或HTML字符串转换为PDF.
        /// </summary>
        /// <param name="wkhtmlPath">wkthmltopdf\wkthmltoimage 库的路径</param>
        /// <param name="switches">传递的命令参数.</param>
        /// <param name="html">包含应转换为PDF的HTML代码的字符串.</param>
        /// <param name="wkhtmlExe"></param>
        /// <returns>作为字节数组的PDF.</returns>
        protected static byte[] Convert(string wkhtmlPath, string switches, string html, string wkhtmlExe) {
            // switches:
            //     "-q"  - silent output, only errors - no progress messages
            //     " -"  - switch output to stdout
            //     "- -" - switch input to stdin and output to stdout
            switches = "-q " + switches + " -";

            // 从给定的HTML字符串生成PDF，而不是从URL生成
            if (!string.IsNullOrEmpty(html)) {
                switches += " -";
                html = SpecialCharsEncode(html);
            }

            var proc = new Process {
                StartInfo = new ProcessStartInfo {
                    FileName = Path.Combine(wkhtmlPath, wkhtmlExe),
                    Arguments = switches,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    RedirectStandardInput = true,
                    WorkingDirectory = wkhtmlPath,
                    CreateNoWindow = true
                }
            };
            proc.Start();

            // 从给定的HTML字符串生成PDF，而不是从URL生成
            if (!string.IsNullOrEmpty(html)) {
                using (var sIn = proc.StandardInput) {
                    sIn.WriteLine(html);
                }
            }

            using (var ms = new MemoryStream()) {
                using (var sOut = proc.StandardOutput.BaseStream) {
                    byte[] buffer = new byte[4096];
                    int read;

                    while ((read = sOut.Read(buffer, 0, buffer.Length)) > 0) {
                        ms.Write(buffer, 0, read);
                    }
                }

                string error = proc.StandardError.ReadToEnd();

                if (ms.Length == 0) {
                    throw new Exception(error);
                }

                proc.WaitForExit();

                return ms.ToArray();
            }
        }

        /// <summary>
        /// 编码所有特殊字符
        /// </summary>
        /// <param name="text">Html text</param>
        /// <returns>特殊字符编码的HTML</returns>
        private static string SpecialCharsEncode(string text) {
            var chars = text.ToCharArray();
            var result = new StringBuilder(text.Length + (int)(text.Length * 0.1));

            foreach (var c in chars) {
                var value = System.Convert.ToInt32(c);
                if (value > 127)
                    result.AppendFormat("&#{0};", value);
                else
                    result.Append(c);
            }

            return result.ToString();
        }
    }
}