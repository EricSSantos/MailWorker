namespace Mail.Service.Interface
{
    public interface IMessageService
    {
        Task ProcessMessage(string messageJson);
    }
}
