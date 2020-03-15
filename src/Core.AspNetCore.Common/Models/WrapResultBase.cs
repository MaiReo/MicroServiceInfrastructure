namespace Core.Web.Models
{
    public abstract class WrapResultBase
    {

        public WrapResultBase()
        {
            Success = true;
        }

        public WrapResultBase(bool success)
        {
            Success = success;
        }

        public WrapResultBase(ErrorInfo error)
        {
            this.Error = error;
        }

        /// <summary>
        /// Indicates success status of the result.
        /// Set <see cref="Error"/> if this value is false.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error details (Must and only set if <see cref="Success"/> is false).
        /// </summary>
        public ErrorInfo Error { get; set; }
    }
}
