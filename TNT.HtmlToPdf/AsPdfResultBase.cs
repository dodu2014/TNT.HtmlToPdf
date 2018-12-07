using Microsoft.AspNetCore.Mvc;
using TNT.HtmlToPdf.Options;
using System;
using System.Linq;
using System.Text;

namespace TNT.HtmlToPdf
{
    public abstract class AsPdfResultBase : AsResultBase
    {
        protected AsPdfResultBase() {
            this.PageMargins = new Margins();
        }
        /// <summary>
        /// 设置页面大小.
        /// </summary>
        [OptionFlag("-s")]
        public Size? PageSize { get; set; }

        /// <summary>
        /// 设置页面宽度，单位毫米 mm.
        /// </summary>
        /// <remarks>Has priority over <see cref="PageSize"/> but <see cref="PageHeight"/> has to be also specified.</remarks>
        [OptionFlag("--page-width")]
        public double? PageWidth { get; set; }

        /// <summary>
        /// 设置页面高度，单位毫米 mm.
        /// </summary>
        /// <remarks>Has priority over <see cref="PageSize"/> but <see cref="PageWidth"/> has to be also specified.</remarks>
        [OptionFlag("--page-height")]
        public double? PageHeight { get; set; }

        /// <summary>
        /// 设置页面方向，单位毫米 mm.
        /// </summary>
        [OptionFlag("-O")]
        public Orientation? PageOrientation { get; set; }

        /// <summary>
        /// 设置页面边距.
        /// </summary>
        public Margins PageMargins { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="switches"></param>
        /// <returns></returns>
        protected override byte[] WkhtmlConvert(string switches) {
            return WkhtmltopdfDriver.Convert(WkhtmlPath, switches);
        }

        /// <summary>
        /// 获取文档类型
        /// </summary>
        /// <returns></returns>
        protected override string GetContentType() {
            return "application/pdf";
        }

        /// <summary>
        /// wkhtmltopdf 库路径.
        /// </summary>
        [Obsolete("使用 WkhtmltopdfPath 代替 CookieName.", false)]
        public string WkhtmltopdfPath {
            get {
                return this.WkhtmlPath;
            }
            set {
                this.WkhtmlPath = value;
            }
        }

        /// <summary>
        /// 指示PDF是否应该以较低的质量生成.
        /// </summary>
        [OptionFlag("-l")]
        public bool IsLowQuality { get; set; }

        /// <summary>
        /// 要打印到PDF文件中的拷贝数.
        /// </summary>
        [OptionFlag("--copies")]
        public int? Copies { get; set; }

        /// <summary>
        /// 指示是否应该以灰度级生成PDF.
        /// </summary>
        [OptionFlag("-g")]
        public bool IsGrayScale { get; set; }

        /// <summary>
        /// 获取转换配置选项
        /// </summary>
        /// <returns></returns>
        protected override string GetConvertOptions() {
            var result = new StringBuilder();

            if (this.PageMargins != null)
                result.Append(this.PageMargins.ToString());

            result.Append(" ");
            result.Append(base.GetConvertOptions());

            return result.ToString().Trim();
        }
    }
}