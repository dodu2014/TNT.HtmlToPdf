
namespace TNT.HtmlToPdf.Options
{
    /// <summary>
    /// 页面大小.
    /// </summary>
    public enum Size
    {
        /// <summary>
        /// 841 x 1189 mm
        /// </summary>
        A0,

        /// <summary>
        /// 594 x 841 mm
        /// </summary>
        A1,

        /// <summary>
        /// 420 x 594 mm
        /// </summary>
        A2,

        /// <summary>
        /// 297 x 420 mm
        /// </summary>
        A3,

        /// <summary>
        /// 210 x 297 mm
        /// </summary>
        A4,

        /// <summary>
        /// 148 x 210 mm
        /// </summary>
        A5,

        /// <summary>
        /// 105 x 148 mm
        /// </summary>
        A6,

        /// <summary>
        /// 74 x 105 mm
        /// </summary>
        A7,

        /// <summary>
        /// 52 x 74 mm
        /// </summary>
        A8,

        /// <summary>
        /// 37 x 52 mm
        /// </summary>
        A9,

        /// <summary>
        /// 1000 x 1414 mm
        /// </summary>
        B0,

        /// <summary>
        /// 707 x 1000 mm
        /// </summary>
        B1,

        /// <summary>
        /// 500 x 707 mm
        /// </summary>
        B2,

        /// <summary>
        /// 353 x 500 mm
        /// </summary>
        B3,

        /// <summary>
        /// 250 x 353 mm
        /// </summary>
        B4,

        /// <summary>
        /// 176 x 250 mm
        /// </summary>
        B5,

        /// <summary>
        /// 125 x 176 mm
        /// </summary>
        B6,

        /// <summary>
        /// 88 x 125 mm
        /// </summary>
        B7,

        /// <summary>
        /// 62 x 88 mm
        /// </summary>
        B8,

        /// <summary>
        /// 33 x 62 mm
        /// </summary>
        B9,

        /// <summary>
        /// 31 x 44 mm
        /// </summary>
        B10,

        /// <summary>
        /// 163 x 229 mm
        /// </summary>
        C5E,

        /// <summary>
        /// 105 x 241 mm - 美国通用10信封
        /// </summary>
        Comm10E,

        /// <summary>
        /// 110 x 220 mm
        /// </summary>
        Dle,

        /// <summary>
        /// 190.5 x 254 mm
        /// </summary>
        Executive,

        /// <summary>
        /// 210 x 330 mm
        /// </summary>
        Folio,

        /// <summary>
        /// 431.8 x 279.4 mm
        /// </summary>
        Ledger,

        /// <summary>
        /// 215.9 x 355.6 mm
        /// </summary>
        Legal,

        /// <summary>
        /// 215.9 x 279.4 mm
        /// </summary>
        Letter,

        /// <summary>
        /// 279.4 x 431.8 mm
        /// </summary>
        Tabloid
    }

    /// <summary>
    /// 纸张方向.
    /// </summary>
    public enum Orientation
    {
        /// <summary>
        /// 横向
        /// </summary>
        Landscape,
        /// <summary>
        /// 纵向
        /// </summary>
        Portrait
    }

    /// <summary>
    /// 图像输出格式
    /// </summary>
    public enum ImageFormat
    {
        /// <summary>
        /// jpeg
        /// </summary>
        jpeg,
        /// <summary>
        /// png
        /// </summary>
        png
    }

    /// <summary>
    /// 内容配置
    /// </summary>
    public enum ContentDisposition
    {
        /// <summary>
        /// 内容作为附件存在，默认的
        /// </summary>
        Attachment = 0,
        /// <summary>
        /// 内容以内联方式存在
        /// </summary>
        Inline
    }
}
