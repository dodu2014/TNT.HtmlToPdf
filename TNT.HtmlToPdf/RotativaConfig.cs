using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TNT.HtmlToPdf
{
    public static class RotativaConfiguration
    {
        private static string _RotativaPath;
        internal static string RotativaPath {
            get {
                if (string.IsNullOrEmpty(_RotativaPath)) {
#if NET45
                    _RotativaUrl = System.Configuration.ConfigurationManager.AppSettings["RotativaUrl"];
#endif
                }
                return _RotativaPath;
            }
        }
        /// <summary>
        /// 设置 wkhtmltopdf 路径环境
        /// </summary>
        /// <param name="env">IHostingEnvironment  对象</param>
        /// <param name="wkhtmltopdfRelativePath">可选项. wkhtmltopdf.exe 相对目录的路径. 默认是 "/Rotativa". 在此下载 https://wkhtmltopdf.org/downloads.html</param>
        public static void Setup(IHostingEnvironment env, string wkhtmltopdfRelativePath = "Rotativa") {
            var rotativaPath = Path.Combine(env.WebRootPath, wkhtmltopdfRelativePath);

            if (!Directory.Exists(rotativaPath)) {
                throw new ApplicationException("Folder containing wkhtmltopdf.exe not found, searched for " + rotativaPath);
            }

            _RotativaPath = rotativaPath;
        }

    }
}
