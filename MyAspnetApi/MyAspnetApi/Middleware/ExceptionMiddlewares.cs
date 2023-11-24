using MyAspnetCore.Exceptions;
using MyAspnetCore.Resources;

namespace MyAspnetApi.Middleware
{
    public class ExceptionMiddlewares
    {
        #region Fields

        private readonly RequestDelegate _next;

        #endregion

        public ExceptionMiddlewares(RequestDelegate next)
        {
            _next = next;
        }

        #region Methods

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HanldeExceptionAsync(context, ex);
            }
        }
        public async Task HanldeExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";

            if (ex is ValidateException validateException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                //List<string> UserMsg = new List<string>();
                //UserMsg.Add(ResourceVN.UserMsg_NotFound);
                await context.Response.WriteAsync(
                    text: new BaseException()
                    {
                        ErrCode = context.Response.StatusCode,
                        DevMsg = ex.Message,
                        UserMsg = ex.Message,
                        TraceId = context.TraceIdentifier,
                        MoreInfo = ex.HelpLink,
                        ErrorMsgs = validateException.ErrorMsgs,
                    }.ToString() ?? ""
                    );
            }
            else if (ex is NotFoundException notFoundException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                //List<string> UserMsg = new List<string>();
                //UserMsg.Add(ResourceVN.UserMsg_InValid);
                await context.Response.WriteAsync(
                    text: new BaseException()
                    {
                        ErrCode = context.Response.StatusCode,
                        DevMsg = ex.Message,
                        UserMsg = ex.Message,
                        TraceId = context.TraceIdentifier,
                        MoreInfo = ex.HelpLink,
                        ErrorMsgs = notFoundException.ErrorMsgs,
                    }.ToString() ?? ""
                    );
            }
            else
            {
                // Lỗi server hoặc lỗi khác
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                //List<string> UserMsg = new List<string>();
                //UserMsg.Add(ResourceVN.UserMsg_System);
                await context.Response.WriteAsync(
                    text: new BaseException()
                    {
                        ErrCode = context.Response.StatusCode,
                        DevMsg = ex.Message,
                        UserMsg = ex.Message,
                        TraceId = context.TraceIdentifier,
                        MoreInfo = ex.HelpLink,
                        ErrorMsgs = new List<string> { ResourceVN.Err_Msg_System },
                    }.ToString() ?? ""
                    );
            }

        }
        #endregion
    }
}
