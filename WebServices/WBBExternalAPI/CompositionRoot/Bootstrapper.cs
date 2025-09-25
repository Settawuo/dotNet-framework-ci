using Microsoft.Extensions.Configuration;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading;
using WBBBusinessLayer;
using WBBBusinessLayer.AuthenDBServices;
using WBBBusinessLayer.CommandHandlers;
using WBBBusinessLayer.CommandHandlers.ExWebServices.FbbCpGw;
using WBBBusinessLayer.CommandHandlers.WebServices;
using WBBBusinessLayer.QueryHandlers.ExWebApi;
using WBBBusinessLayer.QueryHandlers.ExWebServices.FbbCpGw;
using WBBBusinessLayer.QueryHandlers.WebService;
using WBBBusinessLayer.QueryHandlers.WebServices;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Commands.ExWebServices.FbbCpGw;
using WBBContract.Commands.WebServices;
using WBBContract.Queries.ExWebApi;
using WBBContract.Queries.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.PanelModels.ExWebApiModel;
using WBBEntity.PanelModels.WebServiceModels;
using WBBExternalAPI.Code;
using WBBExternalAPI.Extension;
using AIRNETEntity.StoredProc;
using WBBEntity.PanelModels;

namespace WBBExternalAPI.CompositionRoot
{
    public static class Bootstrapper
    {
        private static Container container;

        public static object GetInstance(Type serviceType)
        {
            return container.GetInstance(serviceType);
        }

        public static T GetInstance<T>() where T : class
        {
            return container.GetInstance<T>();
        }

        public static void Bootstrap()
        {
            // Did you know the container can diagnose your configuration? Go to: http://bit.ly/YE8OJj.
            container = new Container();

            container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

            // Add Service
            container.Register(typeof(ICommandHandler<TransferFileToStorageCommand>), typeof(TransferFileToStorageCommandHandler));
            container.Register(typeof(IQueryHandler<GetListWebsiteConfigurationQuery, GetListWebsiteConfigurationQueryModel>), typeof(GetListWebsiteConfigurationQueryHandler));
            container.Register(typeof(IQueryHandler<RetrieveAddressQuery, RetrieveAddressQueryModel>), typeof(RetrieveAddressQueryHandler));
            container.Register(typeof(IQueryHandler<GetDataFBBPreRegisterQuery, GetDataFBBPreRegisterQueryModel>), typeof(GetDataFBBPreRegisterQueryHandler));


            //container.Register(typeof(IQueryHandler<,>), businessLayerAssemblies);

            //R24.03 Max DisplayChangePro
            container.Register(typeof(IQueryHandler<GetPackageSequenceQuery, GetPackageSequenceModel>), typeof(GetPackageSequenceHandler));
            container.Register(typeof(IQueryHandler<GetTableAirFbbQuery, List<GetTableAirFbbModel>>), typeof(GetTableAirFbbHandler));
            container.Register(typeof(IQueryHandler<GetDetailOrderFeeTopupReplaceQuery, DetailOrderFeeTopupReplaceModel>), typeof(GetDetailOrderFeeTopupReplaceQueryHandler));
            container.Register(typeof(IQueryHandler<GetMeshCheckTechnologyQuery, MeshCheckTechnologyModel>), typeof(GetMeshCheckTechnologyQueryHandler));
            //end R24.03 Max DisplayChangePro

            //R24.03 Max Sale portal
            container.Register(typeof(IQueryHandler<GetListNoCoverageQuery, AutoCheckCoverageModel>), typeof(GetListNoCoverageQueryHandler));
            container.Register(typeof(IQueryHandler<GetAppointmentTrackingQuery, List<AppointmentDisplayTrackingList>>), typeof(GetAppointmentTrackingQueryHandler));
            container.Register(typeof(IQueryHandler<ChangeCountInstallDateQuery, ChangeCountInstallDateQueryModel>), typeof(ChangeCountInstallDateQueryHandler));
            container.Register(typeof(IQueryHandler<CheckOrderCancelQuery, CheckOrderCancelModel>), typeof(CheckOrderCancelQueryHandler));
            container.Register(typeof(IQueryHandler<SalePortalLeaveMessageQuery, List<SalePortalLeaveMessageList>>), typeof(SalePortalLeaveMessageQueryHandler));
            container.Register(typeof(IQueryHandler<SalePortalLeaveMessageByRefferenceNoQuery, List<SalePortalLeaveMessageList>>), typeof(SalePortalLeaveMessageByRefferenceNoQueryHandler));
            container.Register(typeof(IQueryHandler<SalePortalLeaveMessageByRefferenceNoQuery_IM, List<SalePortalLeaveMessageList>>), typeof(SalePortalLeaveMessageByRefferenceNoQueryHandler_IM));
            container.Register(typeof(IQueryHandler<WBBContract.Queries.ExWebServices.FbbCpGw.GetPreRegisterQuery, WBBEntity.PanelModels.ExWebServiceModels.GetPreRegisterQueryData>), typeof(GetPreRegisterQueryHandler));

            container.Register(typeof(ICommandHandler<InsertAppointmentTrackingCommand>), typeof(InsertAppointmentTrackingCommandHandler));
            container.Register(typeof(ICommandHandler<UpdatePreregisterStatusCommand>), typeof(UpdatePreregisterStatusCommandHandler));
            container.Register(typeof(ICommandHandler<UpdateFBBSaleportalPreRegisterByOrdermcCommand>), typeof(UpdateFBBSaleportalPreRegisterByOrdermcCommandHandler));
            //end R24.03 Max Sale portal


            //R24.03 Max Check Change Package Ontop
            container.Register(typeof(IQueryHandler<CheckChangePackageOntopQuery, CheckChangePackageOntopModel>), typeof(CheckChangePackageOntopQueryHandler));
            container.Register(typeof(IQueryHandler<GetPathPdfQuery, GetPathPdfModel>), typeof(GetPathPdfQueryHeandle));
            container.Register(typeof(IQueryHandler<GetTrackingQuery, List<TrackingModel>>), typeof(GetTrackingQueryHandler));
            container.Register(typeof(IQueryHandler<WBBContract.WebService.GetListChangePackageQuery, List<ListChangePackageModel>>), typeof(GetListChangePackageQueryHandler));
            //end R24.03 Max Check Change Package Ontop

            // R24.04 Max Sale portal
            container.Register(typeof(IQueryHandler<InstallLeaveMessageQuery, InstallLeaveMessageModel>), typeof(InstallLeaveMessageQueryHandler));
            container.Register(typeof(IQueryHandler<PackageTopupInternetNotUseQuery, PackageTopupInternetNotUseModel>), typeof(PackageTopupInternetNotUseQueryHandler));
            //end R24.04 Max Sale portal
            RegisterWcfSpecificDependencies();

            container.Verify();
        }

        private static void RegisterWcfSpecificDependencies()
        {
            container.Register<ILogger, DebugLogger>();
            container.Register<IPrincipal>(() => Thread.CurrentPrincipal);

            var logger = new DebugLogger(container);

            try
            {
                var dbConnString = "";
                if (Configurations.GetContext.GetConnectionString("Context") != null)
                {
                    dbConnString = Configurations.GetContext.GetConnectionString("Context");
                    //logger.Info("Load ConnectionStrings in Web config:");
                }

                var dbConnStringAirNet = "";
                if (Configurations.GetContext.GetConnectionString("AirNetContext") != null)
                {
                    dbConnStringAirNet = Configurations.GetContext.GetConnectionString("AirNetContext");
                }

                container.Register<IWBBDbFactory>(() => new DbFactory(dbConnString, Configurations.DbSchema), Lifestyle.Scoped);
                container.Register<IAIRDbFactory>(() => new AirNetDbFactory(dbConnStringAirNet, Configurations.DBSchemaAIRNET), Lifestyle.Scoped);

                container.Register<IWBBUnitOfWork, UnitOfWork>();
                container.Register<IAirNetUnitOfWork, AirNetUnitOfWork>();

                container.Register(typeof(IEntityRepository<>), typeof(EntityRepository<>));
                container.Register(typeof(IAirNetEntityRepository<>), typeof(AirNetEntityRepository<>));

                container.RegisterDecorator(typeof(IQueryHandler<,>), typeof(LifetimeScopeQueryHandlerProxy<,>));
                container.RegisterDecorator(typeof(ICommandHandler<>), typeof(LifetimeScopeCommandHandlerProxy<>));

            }
            catch (Exception ex)
            {
                logger.Info(ex.Message);
                throw ex;
            }
            // todo : deploy
        }
    }
}