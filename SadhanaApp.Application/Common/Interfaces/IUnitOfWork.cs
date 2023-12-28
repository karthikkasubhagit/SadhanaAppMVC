namespace SadhanaApp.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        ISadhanaRepository SadhanaRepository { get; }
        IServiceRepository ServiceRepository { get; }
        IUserRepository UserRepository { get; }
        void Save();
    }
}
