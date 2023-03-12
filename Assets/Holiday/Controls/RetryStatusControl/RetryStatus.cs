namespace Extreal.SampleApp.Holiday.Controls.RetryStatusControl
{
    public struct RetryStatus
    {
        public enum RunState
        {
            Retrying,
            Success,
            Failure
        }

        public RunState State { get; private set; }
        public string Message { get; private set; }

        public RetryStatus(RunState state, string message = null)
        {
            State = state;
            Message = message;
        }
    }
}
