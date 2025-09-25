using System.Security.Principal;
using WBBContract;

namespace WBBBusinessLayer.CrossCuttingConcerns
{
    public class AuthorizationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> decoratedHandler;
        private readonly IPrincipal currentUser;
        private readonly ILogger logger;

        public AuthorizationCommandHandlerDecorator(ICommandHandler<TCommand> decoratedHandler,
            IPrincipal currentUser, ILogger logger)
        {
            this.decoratedHandler = decoratedHandler;
            this.currentUser = currentUser;
            this.logger = logger;
        }

        public void Handle(TCommand query)
        {
            this.Authorize();

            this.decoratedHandler.Handle(query);
        }

        private void Authorize()
        {
            // Some useful authorization logic here.
            if (typeof(TCommand).Namespace.Contains("Admin") && !this.currentUser.IsInRole("Admin"))
            {
                throw new AuthorizationException();
            }

            this.logger.Info("User " + this.currentUser.Identity.Name + " has been authorized to execute " +
                typeof(TCommand).Name);
        }
    }
}