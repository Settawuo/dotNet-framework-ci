using System.Linq;
using FBBPAYG_LoadFile_3BBReport.CommandServices;
using WBBContract;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using WBBContract;

namespace FBBPAYG_LoadFile_3BBReport.Code
{
    

    public sealed class WcfServiceCommandHandlerProxy<TCommand> : ICommandHandler<TCommand>
    {
        public void Handle(TCommand command)
        {
            using (var service = new CommandServiceClient())
            {
                object result = service.Execute(command);

                Update(source: result, destination: command);
            }
        }

        private static void Update(object source, object destination)
        {
            var properties =
                from property in destination.GetType().GetProperties()
                where property.CanRead && property.CanWrite
                select property;

            foreach (var property in properties)
            {
                object value = property.GetValue(source, null);

                property.SetValue(destination, value, null);
            }
        }
    }
}