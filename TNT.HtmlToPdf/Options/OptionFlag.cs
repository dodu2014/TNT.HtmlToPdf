using System;

namespace TNT.HtmlToPdf.Options
{
    /// <summary>
    /// 配置选项标识，用来标识用在 Wkhtmltopdf 工具内的命令参数标识
    /// </summary>
    public class OptionFlag : Attribute
    {
        /// <summary>
        /// 标识名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 通过名称构造一个新实例
        /// </summary>
        /// <param name="name">标识名称</param>
        public OptionFlag(string name) {
            Name = name;
        }
    }
}
