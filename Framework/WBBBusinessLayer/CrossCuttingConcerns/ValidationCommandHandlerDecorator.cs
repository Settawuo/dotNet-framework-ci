using WBBContract;

namespace WBBBusinessLayer.CrossCuttingConcerns
{
    public class ValidationCommandHandlerDecorator<TCommand> : ICommandHandler<TCommand>
    {
        private readonly ICommandHandler<TCommand> handler;
        private readonly IValidator validator;
        private readonly ILogger logger;

        public ValidationCommandHandlerDecorator(ICommandHandler<TCommand> handler,
            IValidator validator, ILogger logger)
        {
            this.handler = handler;
            this.validator = validator;
            this.logger = logger;
        }

        void ICommandHandler<TCommand>.Handle(TCommand command)
        {
            try
            {
                // validate the supplied command.
                this.validator.ValidateObject(command);
                this.logger.Info(typeof(TCommand).Name + " is valid.");
            }
            catch (System.ComponentModel.DataAnnotations.ValidationException faultEx)
            {
                this.logger.Info(typeof(TCommand).Name + " is not valid.");
                throw new System.ServiceModel.FaultException(faultEx.Message, new System.ServiceModel.FaultCode("ValidationError"));
            }

            // forward the (valid) command to the real command handler.
            this.handler.Handle(command);
        }
    }
}
