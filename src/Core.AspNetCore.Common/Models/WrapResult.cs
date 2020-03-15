using System;

namespace Core.Web.Models
{

    [Serializable]
    public class WrapResult<TResult> : WrapResultBase
    {
        public WrapResult()
        {
        }

        public WrapResult(TResult result) : base(true)
        {
            this.Result = result;
        }

        public WrapResult(bool success) : base(success)
        {
        }

        public WrapResult(ErrorInfo error) : base(error)
        {

        }
        public TResult Result { get; set; }
    }

    [Serializable]
    public class WrapResult : WrapResult<object>
    {
        public WrapResult() : base(default(object))
        {
        }

        public WrapResult(ErrorInfo error) : base(error)
        {
        }
    }
}
