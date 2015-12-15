namespace Tests.Support
{
    public class SampleEvent
    {
        public string Message { get; private set; }

        public SampleEvent(string message)
        {
            Message = message;
        }
    }
}
