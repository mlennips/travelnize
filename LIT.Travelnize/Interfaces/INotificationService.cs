namespace LIT.Travelnize.Interfaces
{
    public interface INotificationService
    {
        void Success(string message, params object[] args);
        void Error(string message, params object[] args);
        void Info(string message, params object[] args);
    }
}
