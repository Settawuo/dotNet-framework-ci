using AIRNETEntity.Extensions;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Xsl;
using WBBContract;
using WBBContract.Commands;
using WBBContract.Queries.WebServices;
using WBBContract.QueryModels.WebServices;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.QueryHandlers.WebServices
{
    [KnownType(typeof(GetChannelSalesCodeQueryModel))]
    [DataContract]
    public class GetChannelSalesCodeQueryHandler : IQueryHandler<GetChannelSalesCodeQuery, GetChannelSalesCodeQueryModel>
    {
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_INTERFACE_LOG> _intfLog;
        private readonly IEntityRepository<FBB_SPECIALIST> _channelSalesCodeRepository;

        public GetChannelSalesCodeQueryHandler(IWBBUnitOfWork uow,
            IEntityRepository<FBB_INTERFACE_LOG> intfLog, IEntityRepository<FBB_SPECIALIST> channelSalesCodeRepository)
        {
            _uow = uow;
            _intfLog = intfLog;
            _channelSalesCodeRepository = channelSalesCodeRepository;
        }

        public GetChannelSalesCodeQueryModel Handle(GetChannelSalesCodeQuery query)
        {
            var getChannelSalesCodeModel = new GetChannelSalesCodeQueryModel();
            InterfaceLogCommand log = null;

            try
            {
                log = InterfaceLogServiceHelper.StartInterfaceLog(_uow, _intfLog, query, query.channelSalesCode, "GetChannelSalesCode", "FBBService", null, "FBB", "");
                
                var result = _channelSalesCodeRepository.Get()
                            .Where(w => w.CHANNEL_SALES_CODE == query.channelSalesCode)
                            .Select(w => new
                            {
                                w.CHANNEL_SALES_NAME,
                                w.REMARK,
                                w.IS_STAFF,
                                w.IS_PARTNER
                            })
                            .FirstOrDefault();

                if (result != null)
                {
                    if (query.userType == "Staff" && result.IS_STAFF == 1)
                    {
                        getChannelSalesCodeModel.is_special = "Y"; // Special
                    }
                    else if (query.userType == "Partner" && result.IS_PARTNER == 1)
                    {
                        getChannelSalesCodeModel.is_special = "Y"; // Special
                    }
                    else
                    {
                        getChannelSalesCodeModel.is_special = "N"; // Not Special
                    }
                }
                else
                {
                    getChannelSalesCodeModel.is_special = "N"; // Not Special
                }



                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, getChannelSalesCodeModel, log, "Success", "", "");
            }
            catch (Exception ex)
            {

                InterfaceLogServiceHelper.EndInterfaceLog(_uow, _intfLog, getChannelSalesCodeModel, log, "Failed", ex.GetErrorMessage(), "");
            }


            return getChannelSalesCodeModel;
        }
    }
}