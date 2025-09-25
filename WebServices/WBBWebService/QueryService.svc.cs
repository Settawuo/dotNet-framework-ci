using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Activation;
using WBBContract;
using WBBWebService.CompositionRoot;

namespace WBBWebService
{
    [ServiceContract]
    [ServiceKnownType("GetKnownTypes")]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class QueryService
    {
        [OperationContract]
        public object Execute(dynamic query)
        {
            Type queryType = query.GetType();
            Type resultType = GetQueryResultType(queryType);
            Type queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);

            dynamic queryHandler = Bootstrapper.GetInstance(queryHandlerType);

            return queryHandler.Handle(query);
        }

        [OperationContract]
        public object Version()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            string version = fvi.FileVersion + ".17/07/2023-v002";

            return version;
        }

        public static IEnumerable<Type> GetKnownTypes(ICustomAttributeProvider provider)
        {
            var contractAssembly = typeof(IQuery<>).Assembly;

            var queryTypes = (
                from type in contractAssembly.GetExportedTypes()
                where TypeIsQueryType(type)
                select type)
                .ToList();

            var resultTypes =
                from queryType in queryTypes
                select GetQueryResultType(queryType);

            return queryTypes.Union(resultTypes).ToArray();
        }

        private static bool TypeIsQueryType(Type type)
        {
            return GetQueryInterface(type) != null;
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