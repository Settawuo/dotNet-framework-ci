using AIRNETEntity.Models;
using System;
using System.Globalization;
using System.Linq;
using System.Threading;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class SaveChangeStatusBuildingCommandHandler : ICommandHandler<SaveChangeStatusBuildingCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IAirNetUnitOfWork _uowAir;
        private readonly IEntityRepository<FBB_FBSS_LISTBV> _LISTBVervice;
        private readonly IAirNetEntityRepository<AIR_PACKAGE_FTTR> _AIR_PACKAGE_FTTR;

        public SaveChangeStatusBuildingCommandHandler(ILogger logger,
            IWBBUnitOfWork uow,
            IAirNetUnitOfWork uowAir,
            IEntityRepository<FBB_FBSS_LISTBV> LISTBVervice,
            IAirNetEntityRepository<AIR_PACKAGE_FTTR> AIR_PACKAGE_FTTR
        )
        {
            _logger = logger;
            _uow = uow;
            _uowAir = uowAir;
            _LISTBVervice = LISTBVervice;
            _AIR_PACKAGE_FTTR = AIR_PACKAGE_FTTR;
        }

        public void Handle(SaveChangeStatusBuildingCommand command)
        {
            bool SaveStatus = true;
            try
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                var dateNow = DateTime.Now.Date;
                var dateTimeNow = DateTime.Now;
                var strDateNow = dateNow.ToString("dd/MM/yyyy"); ;
                if (command.ACTIVE_FLAG != "" || command.IS_SAVE_REASON)
                {
                    try
                    {
                        var UpdateListBV = _LISTBVervice.Get(c => c.ADDRESS_ID == command.ADDRESS_ID).ToList();
                        if (UpdateListBV != null && UpdateListBV.Count > 0)
                        {
                            foreach (var item in UpdateListBV)
                            {
                                item.UPDATED_BY = command.UPDATE_BY;
                                item.UPDATED_DATE = dateTimeNow;
                                if (command.ACTIVE_FLAG != "")
                                {
                                    item.ACTIVE_FLAG = command.ACTIVE_FLAG;
                                }

                                if (command.IS_SAVE_REASON)
                                {
                                    item.REASON = command.REASON;
                                }

                                _LISTBVervice.Update(item);
                                _uow.Persist();
                            }
                        }
                        SaveStatus = true;
                    }
                    catch (Exception ex1)
                    {
                        _logger.Info(ex1.Message);
                        command.Return_Desc = "Error SaveChangeStatusBuildingCommandHandler " + ex1.Message;
                        SaveStatus = false;
                    }
                }
                if (command.FTTR_FLAG != "")
                {
                    if (command.FTTR_FLAG == "Y")
                    {
                        try
                        {
                            var dataInsert = new AIR_PACKAGE_FTTR()
                            {
                                ADDRESS_ID = command.ADDRESS_ID,
                                PRODUCT_SUBTYPE = "FTTR",
                                OWNER_PRODUCT = "AWN",
                                CUSTOMER_TYPE = "",
                                EFFECTIVE_DTM = dateNow,
                                EXPIRE_DTM = dateNow.AddYears(20),
                                SFF_PROMOTION_BILL_ENG = "",
                                SFF_PROMOTION_BILL_THA = "",
                                SFF_PROMOTION_CODE = "",
                                UPD_BY = command.UPDATE_BY,
                                UPD_DTM = dateTimeNow
                            };
                            _AIR_PACKAGE_FTTR.Create(dataInsert);
                            _uowAir.Persist();


                            var UpdateListBV = _LISTBVervice.Get(c => c.ADDRESS_ID == command.ADDRESS_ID).ToList();
                            if (UpdateListBV != null && UpdateListBV.Count > 0)
                            {
                                foreach (var item in UpdateListBV)
                                {
                                    item.UPDATED_BY = command.UPDATE_BY;
                                    item.UPDATED_DATE = dateTimeNow;

                                    _LISTBVervice.Update(item);
                                    _uow.Persist();
                                }
                            }

                            SaveStatus = true;
                        }
                        catch (Exception ex2)
                        {
                            SaveStatus = false;
                            _logger.Info(ex2.Message);
                            command.Return_Desc = "Error SaveChangeStatusBuildingCommandHandler " + ex2.Message;

                        }
                    }
                    else
                    {
                        try
                        {
                            var dataDelete = _AIR_PACKAGE_FTTR.Get(c => c.ADDRESS_ID == command.ADDRESS_ID).FirstOrDefault();
                            _AIR_PACKAGE_FTTR.Delete(dataDelete);
                            _uowAir.Persist();

                            var UpdateListBV = _LISTBVervice.Get(c => c.ADDRESS_ID == command.ADDRESS_ID).ToList();
                            if (UpdateListBV != null && UpdateListBV.Count > 0)
                            {
                                foreach (var item in UpdateListBV)
                                {
                                    item.UPDATED_BY = command.UPDATE_BY;
                                    item.UPDATED_DATE = dateTimeNow;

                                    _LISTBVervice.Update(item);
                                    _uow.Persist();
                                }
                            }

                            SaveStatus = true;
                        }
                        catch (Exception ex2)
                        {
                            _logger.Info(ex2.Message);
                            command.Return_Desc = "Error SaveChangeStatusBuildingCommandHandler " + ex2.Message;
                            SaveStatus = false;
                        }
                    }
                }

                if (SaveStatus)
                {
                    command.Return_Code = 1;
                    command.Return_Desc = "Saved Complete.";
                    command.Return_UpdateDate = strDateNow;
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message);
                command.Return_Desc = "Error SaveChangeStatusBuildingCommandHandler " + ex.Message;

            }
        }

    }
}
