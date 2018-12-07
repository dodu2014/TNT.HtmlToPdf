using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TNT.HtmlToPdf.Options
{
    /// <summary>
    /// 页边距选项
    /// </summary>
    public class Margins
    {
        /// <summary>
        /// 底部页边距，单位mm.
        /// </summary>
        [OptionFlag("-B")] public int? Bottom;

        /// <summary>
        /// 左边页边距，单位mm.
        /// </summary>
        [OptionFlag("-L")] public int? Left;

        /// <summary>
        /// 右边页边距，单位mm.
        /// </summary>
        [OptionFlag("-R")] public int? Right;

        /// <summary>
        /// 顶部页边距，单位mm.
        /// </summary>
        [OptionFlag("-T")] public int? Top;

        /// <summary>
        /// 构造页边距设置
        /// </summary>
        public Margins() {
        }

        /// <summary>
        /// 构造页边距设置.
        /// </summary>
        /// <param name="top">顶部页边距，单位mm.</param>
        /// <param name="right">右边页边距，单位mm.</param>
        /// <param name="bottom">底部页边距，单位mm.</param>
        /// <param name="left">左边页边距，单位mm.</param>
        public Margins(int top, int right, int bottom, int left) {
            Top = top;
            Right = right;
            Bottom = bottom;
            Left = left;
        }

        /// <summary>
        /// 转化为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString() {
            var result = new StringBuilder();

            FieldInfo[] fields = GetType().GetFields();
            foreach (FieldInfo fi in fields) {
                OptionFlag of = fi.GetCustomAttributes(typeof(OptionFlag), true).FirstOrDefault() as OptionFlag;
                if (of == null) continue;

                object value = fi.GetValue(this);
                if (value != null)
                    result.AppendFormat(CultureInfo.InvariantCulture, " {0} {1}", of.Name, value);
            }

            return result.ToString().Trim();
        }
    }
}