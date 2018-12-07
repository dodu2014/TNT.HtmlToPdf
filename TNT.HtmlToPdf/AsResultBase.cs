using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using TNT.HtmlToPdf.Options;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace TNT.HtmlToPdf
{
    /// <summary>
    /// ����� ViewResult ����
    /// </summary>
    public abstract class AsResultBase : ViewResult //IActionResult
    {
        /// <summary>
        /// ����ʵ��
        /// </summary>
        protected AsResultBase() {
            this.WkhtmlPath = string.Empty;
            this.FormsAuthenticationCookieName = ".ASPXAUTH";
        }

        /// <summary>
        /// �⽫��Ϊ���ɵ�PDF�ļ������Ʒ��͵������.
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// wkhtmltopdf\wkhtmltoimage ��·��.
        /// </summary>
        public string WkhtmlPath { get; set; }

        /// <summary>
        /// �ɱ���֤ʹ�õ������֤cookie���Զ�������.
        /// </summary>
        [Obsolete("ʹ�� FormsAuthenticationCookieName ���� CookieName.")]
        public string CookieName {
            get { return this.FormsAuthenticationCookieName; }
            set { this.FormsAuthenticationCookieName = value; }
        }

        /// <summary>
        /// �ɱ���֤ʹ�õ������֤cookie���Զ�������.
        /// </summary>
        public string FormsAuthenticationCookieName { get; set; }

        /// <summary>
        /// �����Զ��� headers.
        /// </summary>
        [OptionFlag("--custom-header")]
        public Dictionary<string, string> CustomHeaders { get; set; }

        /// <summary>
        /// ���� cookies.
        /// </summary>
        [OptionFlag("--cookie")]
        public Dictionary<string, string> Cookies { get; set; }

        /// <summary>
        /// Sets post values.
        /// </summary>
        [OptionFlag("--post")]
        public Dictionary<string, string> Post { get; set; }

        /// <summary>
        /// ָʾҳ���Ƿ��������JavaScript.
        /// </summary>
        [OptionFlag("-n")]
        public bool IsJavaScriptDisabled { get; set; }

        /// <summary>
        /// ��С�����С.
        /// </summary>
        [OptionFlag("--minimum-font-size")]
        public int? MinimumFontSize { get; set; }

        /// <summary>
        /// ���ô��������.
        /// </summary>
        [OptionFlag("-p")]
        public string Proxy { get; set; }

        /// <summary>
        /// HTTP��֤�û���.
        /// </summary>
        [OptionFlag("--username")]
        public string UserName { get; set; }

        /// <summary>
        /// HTTP��֤����.
        /// </summary>
        [OptionFlag("--password")]
        public string Password { get; set; }

        /// <summary>
        /// �����Ҫ��һ����ǰ��֧�ֵĿ��أ���ʹ�ô˷���.
        /// </summary>
        [OptionFlag("")]
        public string CustomSwitches { get; set; }

        /// <summary>
        /// �ڷ������˱����·��
        /// </summary>
        [Obsolete(@"ʹ��BuildFile(this.ControllerContext)�������棬��ʹ�����ɵĶ���������ִ������Ĳ���.")]
        public string SaveOnServerPath { get; set; }

        /// <summary>
        /// �ĵ�������ʽ
        /// </summary>
        public ContentDisposition ContentDisposition { get; set; }

        /// <summary>
        /// ��ȡUrl
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        protected abstract string GetUrl(ActionContext context);

        /// <summary>
        /// ��OpthFraveGrar���Է���Ϊ���Դ��ݸ�WKHTMLTOPDF�����Ƶ�һ��.
        /// </summary>
        /// <returns>����ֱ�Ӵ��ݸ�WKHTMLTOPDF�����Ƶ������в���.</returns>
        protected virtual string GetConvertOptions() {
            var result = new StringBuilder();

            var fields = this.GetType().GetProperties();
            foreach (var fi in fields) {
                var of = fi.GetCustomAttributes(typeof(OptionFlag), true).FirstOrDefault() as OptionFlag;
                if (of == null)
                    continue;

                object value = fi.GetValue(this, null);
                if (value == null)
                    continue;

                if (fi.PropertyType == typeof(Dictionary<string, string>)) {
                    var dictionary = (Dictionary<string, string>)value;
                    foreach (var d in dictionary) {
                        result.AppendFormat(" {0} {1} {2}", of.Name, d.Key, d.Value);
                    }
                } else if (fi.PropertyType == typeof(bool)) {
                    if ((bool)value)
                        result.AppendFormat(CultureInfo.InvariantCulture, " {0}", of.Name);
                } else {
                    result.AppendFormat(CultureInfo.InvariantCulture, " {0} {1}", of.Name, value);
                }
            }

            return result.ToString().Trim();
        }

        private string GetWkParams(ActionContext context) {
            var switches = string.Empty;

            string authenticationCookie = null;
            if (context.HttpContext.Request.Cookies != null && context.HttpContext.Request.Cookies.Keys.Contains(FormsAuthenticationCookieName)) {
                authenticationCookie = context.HttpContext.Request.Cookies[FormsAuthenticationCookieName];
            }
            if (authenticationCookie != null) {
                //var authCookieValue = authenticationCookie.Value;
                switches += " --cookie " + this.FormsAuthenticationCookieName + " " + authenticationCookie;
            }

            switches += " " + this.GetConvertOptions();

            var url = this.GetUrl(context);
            switches += " " + url;

            return switches;
        }

        /// <summary>
        /// ����������
        /// </summary>
        /// <param name="context">action ������</param>
        /// <returns></returns>
        protected virtual async Task<byte[]> CallTheDriver(ActionContext context) {
            var switches = this.GetWkParams(context);
            var fileContent = this.WkhtmlConvert(switches);
            return fileContent;
        }
        //protected abstract Task<byte[]> CallTheDriver(ActionContext context);

        protected abstract byte[] WkhtmlConvert(string switches);

        public async Task<byte[]> BuildFile(ActionContext context) {
            if (context == null)
                throw new ArgumentNullException("context");

            //if (this.WkhtmlPath == string.Empty)
            //    this.WkhtmlPath = context.HttpContext.Server.MapPath("~/Rotativa");

            this.WkhtmlPath = RotativaConfiguration.RotativaPath;

            var fileContent = await CallTheDriver(context);

            if (string.IsNullOrEmpty(SaveOnServerPath) == false) {
                File.WriteAllBytes(this.SaveOnServerPath, fileContent);
            }

            return fileContent;
        }

        public async override Task ExecuteResultAsync(ActionContext context) {
            var fileContent = await this.BuildFile(context);

            var response = this.PrepareResponse(context.HttpContext.Response);

            await response.Body.WriteAsync(fileContent, 0, fileContent.Length);
        }

        private static string SanitizeFileName(string name) {
            string invalidChars = Regex.Escape(new string(Path.GetInvalidPathChars()) + new string(Path.GetInvalidFileNameChars()));
            string invalidCharsPattern = string.Format(@"[{0}]+", invalidChars);

            string result = Regex.Replace(name, invalidCharsPattern, "_");
            return result;
        }

        protected HttpResponse PrepareResponse(HttpResponse response) {
            response.ContentType = this.GetContentType();

            if (!String.IsNullOrEmpty(this.FileName)) {
                var contentDisposition = this.ContentDisposition == ContentDisposition.Attachment
                    ? "attachment"
                    : "inline";

                response.Headers.Add("Content-Disposition", string.Format("{0}; filename=\"{1}\"", contentDisposition, SanitizeFileName(this.FileName)));
            }
            //response.Headers.Add("Content-Type", this.GetContentType());

            return response;
        }

        protected abstract string GetContentType();
    }
}