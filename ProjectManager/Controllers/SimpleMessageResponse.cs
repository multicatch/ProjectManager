namespace ProjectManager.Controllers
{
    public class SimpleMessageResponse
    {
        public string Message { get; set; }

        public SimpleMessageResponse(string message)
        {
            Message = message;
        }
    }
}