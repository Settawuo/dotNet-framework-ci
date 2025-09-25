namespace WBBContract
{
    public interface ICommandHandler<TCommand>
    {
        void Handle(TCommand command);
    }
}