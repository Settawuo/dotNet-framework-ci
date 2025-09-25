using Newtonsoft.Json;
using System;
using System.Linq;
using WBBContract;
using WBBExternalAPI.CompositionRoot;

namespace WBBExternalAPI.Contacts
{
    public static class CallBusinessLayer
    {
        public static object ExecuteCommand(dynamic command)
        {
            Type commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic commandHandler = Bootstrapper.GetInstance(commandHandlerType);

            commandHandler.Handle(command);

            // Instead of returning the output property of a command, we just return the complete command.
            // There is some overhead in this, but is of course much easier than returning a part of the command.
            return command;
        }

        public static T ExecuteCommand<T>(dynamic command)
        {
            Type commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());

            dynamic commandHandler = Bootstrapper.GetInstance(commandHandlerType);

            var result = commandHandler.Handle(command);

            string json = JsonConvert.SerializeObject(result);

            return JsonConvert.DeserializeObject<T>(json);
        }

        public static object ExecuteQuery(dynamic query)
        {
            Type queryType = query.GetType();
            Type resultType = GetQueryResultType(queryType);
            Type queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);

            dynamic queryHandler = Bootstrapper.GetInstance(queryHandlerType);

            return queryHandler.Handle(query);
        }

        public static T ExecuteQuery<T>(dynamic query)
        {
            Type queryType = query.GetType();
            Type resultType = GetQueryResultType(queryType);
            Type queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);

            dynamic queryHandler = Bootstrapper.GetInstance(queryHandlerType);

            var result = queryHandler.Handle(query);

            string json = JsonConvert.SerializeObject(result);

            return JsonConvert.DeserializeObject<T>(json);
        }

        private static Type GetQueryResultType(Type queryType)
        {
            return GetQueryInterface(queryType).GetGenericArguments()[0];
        }

        private static Type GetQueryInterface(Type type)
        {
            return (
                from @interface in type.GetInterfaces()
                where @interface.IsGenericType
                where typeof(IQuery<>).IsAssignableFrom(@interface.GetGenericTypeDefinition())
                select @interface)
                .SingleOrDefault();
        }
    }
}