namespace Extreal.SampleApp.Holiday.Controls.RetryStatusControl
{
    public readonly struct RetryStatus
    {
        public enum RunState
        {
            Retrying,
            Success,
            Failure
        }

        public RunState State { get; }
        public string Message { get; }

        public RetryStatus(RunState state, string message = null)
        {
            State = state;
            Message = message;
        }
    }
}
