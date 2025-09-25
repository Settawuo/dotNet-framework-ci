using FBBPAYG_GenFile_DisneyPB.CommandServices;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using WBBContract;

namespace FBBPAYG_GenFile_DisneyPB.Code
{

    public sealed class WcfServiceCommandHandlerProxy<TCommand> : ICommandHandler<TCommand>
    {
        public void Handle(TCommand command)
        {
            using (CommandServiceClient commandServiceClient = new CommandServiceClient())
                WcfServiceCommandHandlerProxy<TCommand>.Update(commandServiceClient.Execute((object)command), (object)command);
        }

        private static void Update(object source, object destination)
        {
            foreach (PropertyInfo propertyInfo in ((IEnumerable<PropertyInfo>)destination.GetType().GetProperties()).Where<PropertyInfo>((Func<PropertyInfo, bool>) (property => property.CanRead && property.CanWrite)))
            {
                object obj = propertyInfo.GetValue(source, (object[])null);
                propertyInfo.SetValue(destination, obj, (object[])null);
            }
        }
    }
}
