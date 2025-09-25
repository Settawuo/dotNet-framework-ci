using ObjectDumper;
using System;
using System.Data;
using System.Linq;
using WBBContract;
using WBBContract.Commands;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;

namespace WBBBusinessLayer.CommandHandlers
{
    public class CoverageResultCommandHandler : ICommandHandler<CoverageResultCommand>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBB_COVERAGEAREA_RESULT> _covResultService;

        public CoverageResultCommandHandler(ILogger logger, IWBBUnitOfWork uow,
            IEntityRepository<FBB_COVERAGEAREA_RESULT> covResultService)
        {
            _logger = logger;
            _uow = uow;
            _covResultService = covResultService;
        }

        public void Handle(CoverageResultCommand command)
        {
            try
            {
                _logger.Info(command.DumpToString(command.GetType().Name));

                if (command.ActionType == ActionType.Insert)
                {
                    _logger.Info("Inserting Coverage Result.");

                    #region Insert FBB_COVERAGEAREA_RESULT

                    var data = new FBB_COVERAGEAREA_RESULT();
                    data.CVRID = command.CVRID;
                    data.NODENAME = command.NODENAME;
                    data.TOWER = command.TOWER;
                    data.FLOOR = command.FLOOR;
                    data.ISONLINENUMBER = command.ISONLINENUMBER;
                    data.ADDRESS_NO = command.ADDRESS_NO;
                    data.MOO = command.MOO;
                    data.SOI = command.SOI;
                    data.ROAD = command.ROAD;
                    data.COVERAGETYPE = command.COVERAGETYPE;
                    data.COVERAGERESULT = command.COVERAGERESULT;
                    data.LATITUDE = command.LATITUDE;
                    data.LONGITUDE = command.LONGITUDE;
                    data.PRODUCTTYPE = command.PRODUCTTYPE;
                    data.ZIPCODE_ROWID = command.ZIPCODE_ROWID;
                    data.OWNER_PRODUCT = command.OWNER;
                    //data.SELECTPRODUCT = command.PRODUCTTYPE;

                    data.TRANSACTION_ID = command.TRANSACTION_ID;
                    data.CREATED_BY = string.IsNullOrEmpty(command.ActionBy) ? "CUSTOMER" : command.ActionBy;
                    data.CREATED_DATE = DateTime.Now;

                    _covResultService.Create(data);
                    _uow.Persist();
                    command.RESULT_ID = data.RESULTID;

                    #endregion Insert FBB_COVERAGEAREA_RESULT
                }
                else
                {
                    #region Update

                    _logger.Info("Updat Coverage Result.");

                    var exCoverageResult = (from t in _covResultService.Get() where t.RESULTID == command.RESULT_ID select t).FirstOrDefault();
                    if (null == exCoverageResult)
                    {
                        _logger.Info("Cannot update FBB_COVERAGEAREA_RESULT with result id = " + command.RESULT_ID);
                        return;
                    }

                    exCoverageResult.PREFIXNAME = command.PREFIXNAME;
                    exCoverageResult.FIRSTNAME = command.FIRSTNAME;
                    exCoverageResult.LASTNAME = command.LASTNAME;
                    exCoverageResult.CONTACTNUMBER = command.CONTACTNUMBER;

                    exCoverageResult.RETURN_CODE = command.ReturnCode;
                    exCoverageResult.RETURN_MESSAGE = command.ReturnMessage;
                    exCoverageResult.RETURN_ORDER = command.ReturnOrder;

                    _covResultService.Update(exCoverageResult);
                    _uow.Persist();

                    #endregion Update
                }
            }
            catch (Exception ex)
            {
                _logger.Info("Error occured when handle CoverageResultCommand");
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.StackTrace);
            }
        }
    }
}