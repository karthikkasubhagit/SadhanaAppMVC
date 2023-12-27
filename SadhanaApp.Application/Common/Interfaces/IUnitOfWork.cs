namespace SadhanaApp.Application.Common.Interfaces
{
    public interface IUnitOfWork
    {
        public ISadhanaRepository SadhanaRepository { get; }
        public IServiceRepository ServiceRepository { get; }
        public IUserRepository UserRepository { get; }
    }
}
