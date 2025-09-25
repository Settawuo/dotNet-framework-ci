//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Quartz;
//using Quartz.Impl;
//using WBBBusinessLayer;
//using WBBWebService.CompositionRoot;
//using WBBWebService.Code;
//using WBBContract.Queries.WebServices.FBSS;
//using WBBContract;
//using WBBContract.Commands.WebServices.FBSS;
//using WBBEntity.PanelModels.WebServiceModels;
//using WBBEntity.Extensions;

//namespace WBBWebService
//{
//    public class ScheduleConfig : IJob
//    {
//        private ILogger _logger;     

//        public void Execute(IJobExecutionContext context)
//        {
//            _logger = Bootstrapper.GetInstance<DebugLogger>();
//            JobKey key = context.JobDetail.Key;
//            _logger.Info("SimpleJob says: " + key + " executing at " + SystemTime.Now());
//            //var data = QueryBuild();
//            ////var data = new List<FBSSChangedAddressInfo>();
//            //if (data.Count != 0)
//            //{
//            //    //AlterChangeBuild(data);

//            //}
//        }

//        private List<FBSSChangedAddressInfo> createTest()
//        {
//            var result = new List<FBSSChangedAddressInfo>();

//            var Language = new List<string> { "T", "E" };
//            var FBSSAddressType = new List<string> { "B", "V", "O" };     

//            var FBSSChangeAddressAction = new List<string> { "M", "N", "D"};
//            var BuildNo = new List<string> { "A", "B", "C", "D" };
//            var random = new Random();

//            for(var i = 0; i<30 ; i++)
//            {                  
//                var temp = new FBSSChangedAddressInfo()
//                {                    
//                    Language =  Language[random.Next(Language.Count())],//.ParseEnum<FBSSLanguageFlag>(),
//                    AddressType = FBSSAddressType[random.Next(FBSSAddressType.Count())],//.ParseEnum<FBSSAddressType>(),
//                    PostalCode = "10300",
//                    AddressId = i.ToSafeString(),
//                    BuildingName = "AIS Tower"+i,
//                    BuildingNo = BuildNo[random.Next(BuildNo.Count())],
//                    ChangedAction = FBSSChangeAddressAction[random.Next(FBSSChangeAddressAction.Count())],//.ParseEnum<FBSSChangeAddressAction>()                   
//                };

//                result.Add(temp);
//            }
//            return result;
//        }

//        private List<FBSSChangedAddressInfo> QueryBuild()
//        {
//            var query = new GetFBSSChangedBuilding
//            {
//                //StartDate = DateTime.Parse("01/11/2014"),
//                //EndDate = DateTime.Parse("07/11/2014")
//            };

//            Type queryType = query.GetType();
//            Type resultType = GetQueryResultType(queryType);
//            Type queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, resultType);
//            dynamic queryHandler = Bootstrapper.GetInstance(queryHandlerType);
//            var result = queryHandler.Handle(query);

//            return result;
//        }

//        private void AlterChangeBuild(List<FBSSChangedAddressInfo> data)
//        {
//            data = createTest();
//            var command = new AlterChangedBuildingCommand
//            {
//                FBSSChangedAddressInfos = data,
//                ActionBy = "Bot"
//            };

//            Type commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(command.GetType());
//            dynamic commandHandler = Bootstrapper.GetInstance(commandHandlerType);
//            commandHandler.Handle(command);

//        }


//        private static bool TypeIsQueryType(Type type)
//        {
//            return GetQueryInterface(type) != null;
//        }

//        private static Type GetQueryResultType(Type queryType)
//        {
//            return GetQueryInterface(queryType).GetGenericArguments()[0];
//        }

//        private static Type GetQueryInterface(Type type)
//        {
//            return (
//                from @interface in type.GetInterfaces()
//                where @interface.IsGenericType
//                where typeof(IQuery<>).IsAssignableFrom(@interface.GetGenericTypeDefinition())
//                select @interface)
//                .SingleOrDefault();
//        }


//    }
//}