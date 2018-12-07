
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TNT.HtmlToPdf
{
    /// <summary>
    /// Pdf 格式的 IActionResult
    /// </summary>
    public class ViewAsPdf : AsPdfResultBase
    {

        /// <summary>
        /// 视图名称
        /// </summary>
        public new string ViewName { get; set; } = "";


        /// <summary>
        /// 模板名称
        /// </summary>
        public string MasterName { get; set; } = "";

        /// <summary>
        /// 视图模型
        /// </summary>
        public new object Model { get; set; }

        /// <summary>
        /// 视图数据
        /// </summary>
        public new ViewDataDictionary ViewData { get; set; }

        /// <summary>
        /// 实例化一个 Pdf 格式的 IActionResult
        /// </summary>
        /// <param name="viewData">视图数据</param>
        public ViewAsPdf(ViewDataDictionary viewData = null) {
            this.WkhtmlPath = string.Empty;
            MasterName = string.Empty;
            ViewName = string.Empty;
            Model = null;
            ViewData = viewData;
        }

        /// <summary>
        /// 实例化一个 Pdf 格式的 IActionResult
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="viewData">视图数据</param>
        public ViewAsPdf(string viewName, ViewDataDictionary viewData = null)
            : this(viewData) {
            ViewName = viewName;
        }

        /// <summary>
        /// 实例化一个 Pdf 格式的 IActionResult
        /// </summary>
        /// <param name="model">视图模型</param>
        /// <param name="viewData">视图数据</param>
        public ViewAsPdf(object model, ViewDataDictionary viewData = null)
            : this(viewData) {
            Model = model;
        }

        /// <summary>
        /// 实例化一个 Pdf 格式的 IActionResult
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="model">视图模型</param>
        /// <param name="viewData">视图数据</param>
        public ViewAsPdf(string viewName, object model, ViewDataDictionary viewData = null)
            : this(viewData) {
            ViewName = viewName;
            Model = model;
        }

        /// <summary>
        /// 实例化一个 Pdf 格式的 IActionResult
        /// </summary>
        /// <param name="viewName">视图名称</param>
        /// <param name="masterName">模板名称</param>
        /// <param name="model">视图模型</param>
        public ViewAsPdf(string viewName, string masterName, object model)
            : this(viewName, model) {
            MasterName = masterName;
        }

        /// <summary>
        /// 获取Url
        /// </summary>
        /// <param name="context">action 上下文</param>
        /// <returns></returns>
        protected override string GetUrl(ActionContext context) {
            return string.Empty;
        }

        /// <summary>
        /// 获取视图
        /// </summary>
        /// <param name="context">action 上下文</param>
        /// <param name="viewName">视图名称</param>
        /// <param name="masterName">模板名称</param>
        /// <returns></returns>
        protected virtual ViewEngineResult GetView(ActionContext context, string viewName, string masterName) {
            var engine = context.HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;

            var getViewResult = engine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
            if (getViewResult.Success) {
                return getViewResult;
            }

            var findViewResult = engine.FindView(context, viewName, isMainPage: true);
            if (findViewResult.Success) {
                return findViewResult;
            }

            var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
            var errorMessage = string.Join(
                Environment.NewLine,
                new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

            throw new InvalidOperationException(errorMessage);
        }

        /// <summary>
        /// 调用驱动程序
        /// </summary>
        /// <param name="context">action 上下文</param>
        /// <returns></returns>
        protected override async Task<byte[]> CallTheDriver(ActionContext context) {
            // 如果未提供视图名称，则使用 action 的名称
            string viewName = ViewName;
            if (string.IsNullOrEmpty(ViewName)) {
                viewName = ((Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor)context.ActionDescriptor).ActionName;
            }

            ViewEngineResult viewResult = GetView(context, viewName, MasterName);
            var html = new StringBuilder();

            //string html = context.GetHtmlFromView(viewResult, viewName, Model);
            ITempDataProvider tempDataProvider = context.HttpContext.RequestServices.GetService(typeof(ITempDataProvider)) as ITempDataProvider;

            var viewDataDictionary = new ViewDataDictionary(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new ModelStateDictionary()) {
                Model = this.Model
            };
            if (this.ViewData != null) {
                foreach (var item in this.ViewData) {
                    viewDataDictionary.Add(item);
                }
            }
            using (var output = new StringWriter()) {
                var view = viewResult.View;
                var tempDataDictionary = new TempDataDictionary(context.HttpContext, tempDataProvider);
                var viewContext = new ViewContext(
                    context,
                    viewResult.View,
                    viewDataDictionary,
                    tempDataDictionary,
                    output,
                    new HtmlHelperOptions());

                await view.RenderAsync(viewContext);

                html = output.GetStringBuilder();
            }


            string baseUrl = string.Format("{0}://{1}", context.HttpContext.Request.Scheme, context.HttpContext.Request.Host);
            var htmlForWkhtml = Regex.Replace(html.ToString(), "<head>", string.Format("<head><base href=\"{0}\" />", baseUrl), RegexOptions.IgnoreCase);

            byte[] fileContent = WkhtmltopdfDriver.ConvertHtml(this.WkhtmlPath, this.GetConvertOptions(), htmlForWkhtml);
            return fileContent;
        }
    }
}
