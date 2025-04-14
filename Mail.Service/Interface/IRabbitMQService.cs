namespace Mail.Service.Interface
{
    public interface IRabbitMQService
    {
        void Start(CancellationToken cancellationToken);
    }
}
