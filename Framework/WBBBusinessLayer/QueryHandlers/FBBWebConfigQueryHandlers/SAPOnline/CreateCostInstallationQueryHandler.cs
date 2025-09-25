using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.FBBWebConfigQueries.SAPOnline;
using WBBData.DbIteration;
using WBBData.Repository;
using WBBEntity.Extensions;
using WBBEntity.Models;
using WBBEntity.PanelModels.FBBWebConfigModels;

namespace WBBBusinessLayer.QueryHandlers.FBBWebConfigQueryHandlers.SAPOnline
{
    public class CreateCostInstallationQueryHandler : IQueryHandler<CreateCostInstallationQuery, ReturnCreate>
    {
        private readonly ILogger _logger;
        private readonly IWBBUnitOfWork _uow;
        private readonly IEntityRepository<FBSS_INSTALLATION_COST> _costInstall;
        private readonly IEntityRepository<FBSS_FIXED_ASSET_CONFIG> _fixAssConfig;
        private readonly IEntityRepository<FBB_HISTORY_LOG> _historyLog;

        public CreateCostInstallationQueryHandler(
            ILogger logger,
            IEntityRepository<FBSS_INSTALLATION_COST> costInstall,
            IEntityRepository<FBSS_FIXED_ASSET_CONFIG> fixAssConfig,
            IEntityRepository<FBB_HISTORY_LOG> historyLog,
            IWBBUnitOfWork uow)
        {
            _logger = logger;
            _costInstall = costInstall;
            _fixAssConfig = fixAssConfig;
            _historyLog = historyLog;
            _uow = uow;
        }

        #region old
        //public ReturnCreate Handle(CreateCostInstallationQuery query)
        //{
        //    var r = new ReturnCreate();
        //    try
        //    {
        //        var _cost = new FBSS_INSTALLATION_COST();
        //        _cost.ID = query.ID;
        //        _cost.SERVICE = query.SERVICE.ToSafeString();
        //        _cost.VENDOR = query.VENDOR.ToSafeString();
        //        _cost.ORDER_TYPE = query.ORDER_TYPE.ToSafeString();
        //        _cost.INTERNET = query.RATE;
        //        _cost.PLAYBOX = query.PLAYBOX;
        //        _cost.VOIP = query.VOIP;
        //        _cost.EFFECTIVE_DATE = query.EFFECTIVE_DATE;
        //        _cost.EXPIRE_DATE = query.EXPIRE_DATE;
        //        _cost.REMARK = query.REMARK.ToSafeString();
        //        _cost.INS_OPTION = query.INS_OPTION;
        //        _cost.LENGTH_FR = query.LENGTH_FR;
        //        _cost.LENGTH_TO = query.LENGTH_TO;
        //        _cost.OUT_DOOR_PRICE = query.OUT_DOOR_PRICE;
        //        _cost.IN_DOOR_PRICE = query.IN_DOOR_PRICE;
        //        _cost.CREATE_DATE = query.CREATE_DATE;
        //        _cost.CREATE_BY = query.CREATE_BY;
        //        _cost.UPDATED_DATE = query.UPDATED_DATE;
        //        _cost.UPDATED_BY = query.UPDATED_BY;
        //        _cost.ADDRESS_ID = query.ADDRESS_ID;
        //        _cost.TOTAL_PRICE = query.TOTAL_PRICE;

        //        if (query.ACTION == WBBContract.Commands.ActionType.Insert)
        //        {
        //            var cos = _costInstall.Get(a => a.SERVICE == query.SERVICE && a.VENDOR == query.VENDOR && a.ORDER_TYPE == query.ORDER_TYPE && a.INS_OPTION == query.INS_OPTION);
        //            var cos_novendor = _costInstall.Get(a => a.SERVICE == query.SERVICE && a.ORDER_TYPE == query.ORDER_TYPE && a.INS_OPTION == query.INS_OPTION);
        //            int aa = cos.Count();
        //            int bb = cos_novendor.Count();
        //            var anyItem = true;
        //            if (cos.Any())
        //            {
        //                foreach (var co in cos)
        //                {
        //                    if (
        //                        ((query.EFFECTIVE_DATE >= co.EFFECTIVE_DATE) && (co.EXPIRE_DATE == null))
        //                        || ((query.EFFECTIVE_DATE <= co.EFFECTIVE_DATE) && query.EXPIRE_DATE == null)
        //                        )
        //                    {
        //                        anyItem = false;
        //                    }
        //                    else if (
        //                                (
        //                                    (query.EFFECTIVE_DATE >= co.EFFECTIVE_DATE) && (query.EFFECTIVE_DATE <= co.EXPIRE_DATE)
        //                                    ||
        //                                    (query.EXPIRE_DATE >= co.EFFECTIVE_DATE) && (query.EXPIRE_DATE <= co.EXPIRE_DATE)
        //                                )
        //                                ||
        //                                (
        //                                    (co.EFFECTIVE_DATE >= query.EFFECTIVE_DATE) && (co.EFFECTIVE_DATE <= query.EXPIRE_DATE)
        //                                    ||
        //                                    (co.EXPIRE_DATE >= query.EFFECTIVE_DATE) && (co.EXPIRE_DATE <= query.EXPIRE_DATE)
        //                                )
        //                            )
        //                    {
        //                        anyItem = false;
        //                    }
        //                }
        //            }
        //            else if (cos_novendor.Any())
        //            {
        //                foreach (var co in cos_novendor)
        //                {
        //                    if (
        //                            (
        //                                ((query.EFFECTIVE_DATE >= co.EFFECTIVE_DATE) && (co.EXPIRE_DATE == null))
        //                                || ((query.EFFECTIVE_DATE <= co.EFFECTIVE_DATE) && query.EXPIRE_DATE == null)
        //                            )
        //                            &&
        //                            (
        //                                (
        //                                    (query.LENGTH_FR >= co.LENGTH_FR) && (query.LENGTH_FR <= co.LENGTH_TO)
        //                                    ||
        //                                    (query.LENGTH_TO >= co.LENGTH_FR) && (query.LENGTH_TO <= co.LENGTH_TO)
        //                                )
        //                                ||
        //                                (
        //                                    (co.LENGTH_FR >= query.LENGTH_FR) && (co.LENGTH_FR <= query.LENGTH_TO)
        //                                    ||
        //                                    (co.LENGTH_TO >= query.LENGTH_FR) && (co.LENGTH_TO <= query.LENGTH_TO)
        //                                )
        //                            )
        //                        )
        //                    {
        //                        anyItem = false;
        //                    }
        //                    else if (
        //                                (
        //                                    (
        //                                        (query.EFFECTIVE_DATE >= co.EFFECTIVE_DATE) && (query.EFFECTIVE_DATE <= co.EXPIRE_DATE)
        //                                        ||
        //                                        (query.EXPIRE_DATE >= co.EFFECTIVE_DATE) && (query.EXPIRE_DATE <= co.EXPIRE_DATE)
        //                                    )
        //                                    ||
        //                                    (
        //                                        (co.EFFECTIVE_DATE >= query.EFFECTIVE_DATE) && (co.EFFECTIVE_DATE <= query.EXPIRE_DATE)
        //                                        ||
        //                                        (co.EXPIRE_DATE >= query.EFFECTIVE_DATE) && (co.EXPIRE_DATE <= query.EXPIRE_DATE)
        //                                    )
        //                                )
        //                                &&
        //                                (
        //                                    (
        //                                        (query.LENGTH_FR >= co.LENGTH_FR) && (query.LENGTH_FR <= co.LENGTH_TO)
        //                                        ||
        //                                        (query.LENGTH_TO >= co.LENGTH_FR) && (query.LENGTH_TO <= co.LENGTH_TO)
        //                                    )
        //                                    ||
        //                                    (
        //                                        (co.LENGTH_FR >= query.LENGTH_FR) && (co.LENGTH_FR <= query.LENGTH_TO)
        //                                        ||
        //                                        (co.LENGTH_TO >= query.LENGTH_FR) && (co.LENGTH_TO <= query.LENGTH_TO)
        //                                    )
        //                                )
        //                            )
        //                    {
        //                        anyItem = false;
        //                    }
        //                }
        //            }

        //            if (anyItem)
        //            {
        //                var getcost = (from c in _costInstall.Get() select c);
        //                var idmax = getcost.Max(c => c.ID);
        //                if (idmax != null)
        //                {
        //                    _cost.ID = idmax + 1;
        //                }
        //                else
        //                {
        //                    _cost.ID = 1;
        //                }
        //                if (query.VENDOR.ToSafeString().Substring(0, 1) == "V")
        //                {
        //                    _cost.VENDOR = "V_" + _cost.ID.ToSafeString();
        //                }
        //                _costInstall.Create(_cost);
        //                r.RETURN_CODE = 0;
        //                r.RETURN_MSG = "success";
        //            }
        //            else { r.RETURN_CODE = -1; r.RETURN_MSG = "duplicate"; }

        //            //var getcost = (from c in _costInstall.Get() select c);
        //            //var idmax = getcost.Max(c => c.ID);
        //            //if (idmax != null)
        //            //{
        //            //    _cost.ID = idmax + 1;
        //            //}
        //            //else
        //            //{
        //            //    _cost.ID = 1;
        //            //}
        //            //_costInstall.Create(_cost);
        //            //r.RETURN_CODE = 0;
        //            //r.RETURN_MSG = "success";
        //        }
        //        else if (query.ACTION == WBBContract.Commands.ActionType.Update)
        //        {
        //            var cos = _costInstall.Get(a => a.SERVICE == query.SERVICE && a.VENDOR == query.VENDOR && a.ORDER_TYPE == query.ORDER_TYPE && a.INS_OPTION == query.INS_OPTION && a.ID != query.ID);
        //            var cos_novendor = _costInstall.Get(a => a.SERVICE == query.SERVICE && a.ORDER_TYPE == query.ORDER_TYPE && a.INS_OPTION == query.INS_OPTION && a.ID != query.ID);
        //            var anyItem = true;
        //            if (cos.Any())
        //            {
        //                foreach (var co in cos)
        //                {
        //                    if (
        //                        ((query.EFFECTIVE_DATE >= co.EFFECTIVE_DATE) && (co.EXPIRE_DATE == null))
        //                        || ((query.EFFECTIVE_DATE <= co.EFFECTIVE_DATE) && query.EXPIRE_DATE == null)
        //                        )
        //                    {
        //                        anyItem = false;
        //                    }
        //                    else if (
        //                                (
        //                                    (query.EFFECTIVE_DATE >= co.EFFECTIVE_DATE) && (query.EFFECTIVE_DATE <= co.EXPIRE_DATE)
        //                                    ||
        //                                    (query.EXPIRE_DATE >= co.EFFECTIVE_DATE) && (query.EXPIRE_DATE <= co.EXPIRE_DATE)
        //                                )
        //                                ||
        //                                (
        //                                    (co.EFFECTIVE_DATE >= query.EFFECTIVE_DATE) && (co.EFFECTIVE_DATE <= query.EXPIRE_DATE)
        //                                    ||
        //                                    (co.EXPIRE_DATE >= query.EFFECTIVE_DATE) && (co.EXPIRE_DATE <= query.EXPIRE_DATE)
        //                                )
        //                            )
        //                    {
        //                        anyItem = false;
        //                    }
        //                }
        //            }
        //            else if (cos_novendor.Any())
        //            {
        //                foreach (var co in cos_novendor)
        //                {
        //                    if (
        //                            (
        //                                ((query.EFFECTIVE_DATE >= co.EFFECTIVE_DATE) && (co.EXPIRE_DATE == null))
        //                                || ((query.EFFECTIVE_DATE <= co.EFFECTIVE_DATE) && query.EXPIRE_DATE == null)
        //                            )
        //                            &&
        //                            (
        //                                (
        //                                    (query.LENGTH_FR >= co.LENGTH_FR) && (query.LENGTH_FR <= co.LENGTH_TO)
        //                                    ||
        //                                    (query.LENGTH_TO >= co.LENGTH_FR) && (query.LENGTH_TO <= co.LENGTH_TO)
        //                                )
        //                                ||
        //                                (
        //                                    (co.LENGTH_FR >= query.LENGTH_FR) && (co.LENGTH_FR <= query.LENGTH_TO)
        //                                    ||
        //                                    (co.LENGTH_TO >= query.LENGTH_FR) && (co.LENGTH_TO <= query.LENGTH_TO)
        //                                )
        //                            )
        //                        )
        //                    {
        //                        anyItem = false;
        //                    }
        //                    else if (
        //                                (
        //                                    (
        //                                        (query.EFFECTIVE_DATE >= co.EFFECTIVE_DATE) && (query.EFFECTIVE_DATE <= co.EXPIRE_DATE)
        //                                        ||
        //                                        (query.EXPIRE_DATE >= co.EFFECTIVE_DATE) && (query.EXPIRE_DATE <= co.EXPIRE_DATE)
        //                                    )
        //                                    ||
        //                                    (
        //                                        (co.EFFECTIVE_DATE >= query.EFFECTIVE_DATE) && (co.EFFECTIVE_DATE <= query.EXPIRE_DATE)
        //                                        ||
        //                                        (co.EXPIRE_DATE >= query.EFFECTIVE_DATE) && (co.EXPIRE_DATE <= query.EXPIRE_DATE)
        //                                    )
        //                                )
        //                                &&
        //                                (
        //                                    (
        //                                        (query.LENGTH_FR >= co.LENGTH_FR) && (query.LENGTH_FR <= co.LENGTH_TO)
        //                                        ||
        //                                        (query.LENGTH_TO >= co.LENGTH_FR) && (query.LENGTH_TO <= co.LENGTH_TO)
        //                                    )
        //                                    ||
        //                                    (
        //                                        (co.LENGTH_FR >= query.LENGTH_FR) && (co.LENGTH_FR <= query.LENGTH_TO)
        //                                        ||
        //                                        (co.LENGTH_TO >= query.LENGTH_FR) && (co.LENGTH_TO <= query.LENGTH_TO)
        //                                    )
        //                                )
        //                            )
        //                    {
        //                        anyItem = false;
        //                    }
        //                }
        //            }

        //            if (anyItem)
        //            {
        //                if (query.VENDOR.ToSafeString().Substring(0, 1) == "V")
        //                {
        //                    _cost.VENDOR = "V_" + query.ID.ToString();
        //                }
        //                _costInstall.Update(_cost);
        //                r.RETURN_CODE = 0;
        //                r.RETURN_MSG = "success";
        //            }
        //            else { r.RETURN_CODE = -1; r.RETURN_MSG = "duplicate"; }
        //            //_costInstall.Update(_cost);
        //            //r.RETURN_CODE = 0;
        //            //r.RETURN_MSG = "success";

        //        }
        //        else if (query.ACTION == WBBContract.Commands.ActionType.Delete)
        //        {
        //            var cos = _costInstall.Get(a => a.ID == query.ID);

        //            if (cos.Any())
        //            {
        //                foreach (var co in cos)
        //                {
        //                    _costInstall.Delete(co);
        //                    r.RETURN_CODE = 0;
        //                    r.RETURN_MSG = "success";
        //                }
        //            }
        //        }
        //        _uow.Persist();
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.Info(ex.GetErrorMessage());
        //        r.RETURN_CODE = -1;
        //        r.RETURN_MSG = ex.GetErrorMessage();
        //    }

        //    return r;

        //}
        #endregion

        public ReturnCreate Handle(CreateCostInstallationQuery query)
        {
            // rebuild
            var historyLog = new FBB_HISTORY_LOG();
            string _PARAMS = " ID : " + query.ID.ToSafeString();
            _PARAMS += " ORDER_TYPE : " + query.ORDER_TYPE.ToSafeString();
            _PARAMS += " SERVICE : " + query.SERVICE.ToSafeString();
            _PARAMS += " INS_OPTION : " + query.INS_OPTION.ToSafeString();
            _PARAMS += " VENDOR : " + query.VENDOR.ToSafeString();
            _PARAMS += " EFFECTIVE_DATE : " + query.EFFECTIVE_DATE.ToSafeString();
            _PARAMS += " EXPIRE_DATE : " + query.EXPIRE_DATE.ToSafeString();
            _PARAMS += " ACTION : " + query.ACTION.ToSafeString();

            var r = new ReturnCreate();
            try
            {
                var _cost = new FBSS_INSTALLATION_COST();
                _cost.ID = query.ID;
                _cost.SERVICE = query.SERVICE.ToSafeString();
                _cost.VENDOR = query.VENDOR.ToSafeString();
                _cost.ORDER_TYPE = query.ORDER_TYPE.ToSafeString();
                _cost.INTERNET = query.RATE;
                _cost.PLAYBOX = query.PLAYBOX;
                _cost.VOIP = query.VOIP;
                _cost.EFFECTIVE_DATE = query.EFFECTIVE_DATE;
                _cost.EXPIRE_DATE = query.EXPIRE_DATE;
                _cost.REMARK = query.REMARK.ToSafeString();
                _cost.INS_OPTION = query.INS_OPTION;
                _cost.LENGTH_FR = query.LENGTH_FR;
                _cost.LENGTH_TO = query.LENGTH_TO;
                _cost.OUT_DOOR_PRICE = query.OUT_DOOR_PRICE;
                _cost.IN_DOOR_PRICE = query.IN_DOOR_PRICE;
                _cost.CREATE_DATE = query.CREATE_DATE;
                _cost.CREATE_BY = query.CREATE_BY;
                _cost.UPDATED_DATE = query.UPDATED_DATE;
                _cost.UPDATED_BY = query.UPDATED_BY;
                _cost.ADDRESS_ID = query.ADDRESS_ID;
                _cost.TOTAL_PRICE = query.TOTAL_PRICE;

                bool _Validate = Validate(query);
                if (query.ACTION == WBBContract.Commands.ActionType.Insert)
                {
                    if (_Validate == true)
                    {
                        var getcost = (from c in _costInstall.Get() select c);
                        var idmax = getcost.Max(c => c.ID);
                        idmax = (idmax == null) ? 0 : (idmax + 1);
                        _cost.ID = idmax;
                        _costInstall.Create(_cost);

                        query.ID = idmax;
                        r.RETURN_CODE = 0;
                        r.RETURN_MSG = "success";
                    }
                    else { r.RETURN_CODE = -1; r.RETURN_MSG = "duplicate"; }
                }
                else if (query.ACTION == WBBContract.Commands.ActionType.Update)
                {
                    if (_Validate == true)
                    {
                        _costInstall.Update(_cost);
                        r.RETURN_CODE = 0;
                        r.RETURN_MSG = "success";
                    }
                    else { r.RETURN_CODE = -1; r.RETURN_MSG = "duplicate"; }
                }
                else if (query.ACTION == WBBContract.Commands.ActionType.Delete)
                {
                    var cos = _costInstall.Get(a => a.ID == query.ID);
                    if (cos.Any())
                    {
                        foreach (var co in cos)
                        {
                            _costInstall.Delete(co);
                            r.RETURN_CODE = 0;
                            r.RETURN_MSG = "success";
                        }
                    }
                }
                _uow.Persist();
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.Message);

                //historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                //historyLog.ACTION = query.ACTION.ToSafeString();
                //historyLog.APPLICATION = "CostInstallation";
                //historyLog.DESCRIPTION = "Error Message : [" + ex.Message.ToSafeString() + "] InnerException : [" + ex.InnerException.ToSafeString() + "] Params : { " + _PARAMS + " }";
                //historyLog.REF_KEY = "Service : CreateCostInstallationQueryHandler";
                //historyLog.REF_NAME = "CreateCostInstallationQuery";
                //historyLog.CREATED_BY = query.CREATE_BY.ToSafeString();
                //historyLog.CREATED_DATE = DateTime.Now;
                //_historyLog.Create(historyLog);
                //_uow.Persist();

                r.RETURN_CODE = -1;
                r.RETURN_MSG = ex.GetErrorMessage();
            }
            if (
                (
                    query.ORDER_TYPE.ToSafeString().ToUpper() == "NEW"
                    && query.SERVICE.ToSafeString().ToUpper() == "FTTH"
                    && query.INS_OPTION.ToSafeString().ToUpper() == "DEFAULT"
                    && query.VENDOR.ToSafeString().ToUpper() == "DEFAULT"
                )
                || query.IS_DELETE_FIXASSCONFIG == "Y"
                )
            {
                Effect_fixAssConfig(query, r);
            }

            return r;

        }

        public bool Validate(CreateCostInstallationQuery query)
        {
            bool result = true;
            var _ID = query.ID;
            _ID = (_ID == null) ? 0 : _ID; // yah
            var _ORDER_TYPE = query.ORDER_TYPE.ToSafeString().ToUpper();
            var _SERVICE = query.SERVICE.ToSafeString().ToUpper();
            var _INS_OPTION = query.INS_OPTION.ToSafeString().ToUpper();
            var _VENDOR = query.VENDOR.ToSafeString().ToUpper();

            var _EFFECTIVE_DATE = query.EFFECTIVE_DATE;
            var _EXPIRE_DATE = query.EXPIRE_DATE;
            var _LENGTH_FR = query.LENGTH_FR;
            var _LENGTH_TO = query.LENGTH_TO;


            // standard
            var _cost = _costInstall.Get(w =>
                w.ID != _ID
                && w.ORDER_TYPE.ToUpper() == _ORDER_TYPE
                && w.SERVICE.ToUpper() == _SERVICE
                && w.INS_OPTION.ToUpper() == _INS_OPTION
                && w.VENDOR.ToUpper() == _VENDOR
                );

            var filter_date_1 = _cost.Where(w =>
                (_EFFECTIVE_DATE >= w.EFFECTIVE_DATE && w.EXPIRE_DATE == null)
                ||
                (_EFFECTIVE_DATE <= w.EFFECTIVE_DATE && _EXPIRE_DATE == null)
                );

            var filter_date_2 = _cost.Where(w =>
                    (
                        (_EFFECTIVE_DATE >= w.EFFECTIVE_DATE) && (_EFFECTIVE_DATE <= w.EXPIRE_DATE)
                        ||
                        (_EXPIRE_DATE >= w.EFFECTIVE_DATE) && (_EXPIRE_DATE <= w.EXPIRE_DATE)
                    )
                    ||
                    (
                        (w.EFFECTIVE_DATE >= _EFFECTIVE_DATE) && (w.EFFECTIVE_DATE <= _EXPIRE_DATE)
                        ||
                        (w.EXPIRE_DATE >= _EFFECTIVE_DATE) && (w.EXPIRE_DATE <= _EXPIRE_DATE)
                    )
                );

            var filter_date = filter_date_1.Union(filter_date_2);
            var filter_result = filter_date;

            if (_LENGTH_FR != null && _LENGTH_TO != null)
            {
                filter_result = filter_date.Where(w =>
                                                (
                                                    (_LENGTH_FR >= w.LENGTH_FR) && (_LENGTH_FR <= w.LENGTH_TO)
                                                    ||
                                                    (_LENGTH_TO >= w.LENGTH_FR) && (_LENGTH_TO <= w.LENGTH_TO)
                                                )
                                                ||
                                                (
                                                    (w.LENGTH_FR >= _LENGTH_FR) && (w.LENGTH_FR <= _LENGTH_TO)
                                                    ||
                                                    (w.LENGTH_TO >= _LENGTH_FR) && (w.LENGTH_TO <= _LENGTH_TO)
                                                )
                    );
            }

            //count
            int count_cost = _cost.Count();
            int count_filter_date_1 = filter_date_1.Count();
            int count_filter_date_2 = filter_date_2.Count();
            int count_filter_date = filter_date.Count();
            int count_filter_result = filter_result.Count();
            if (count_filter_result > 0)
            {
                result = false; // dup
            }

            return result;
        }

        public void Effect_fixAssConfig(CreateCostInstallationQuery query, ReturnCreate r)
        {
            var historyLog = new FBB_HISTORY_LOG();
            string _PARAMS = " ID : " + query.ID.ToSafeString();
            _PARAMS += " ORDER_TYPE : " + query.ORDER_TYPE.ToSafeString();
            _PARAMS += " SERVICE : " + query.SERVICE.ToSafeString();
            _PARAMS += " INS_OPTION : " + query.INS_OPTION.ToSafeString();
            _PARAMS += " VENDOR : " + query.VENDOR.ToSafeString();
            _PARAMS += " EFFECTIVE_DATE : " + query.EFFECTIVE_DATE.ToSafeString();
            _PARAMS += " EXPIRE_DATE : " + query.EXPIRE_DATE.ToSafeString();
            _PARAMS += " ACTION : " + query.ACTION.ToSafeString();

            try
            {
                if (r.RETURN_CODE == 0) // success
                {
                    var _fix_Z9 = new FBSS_FIXED_ASSET_CONFIG();
                    _fix_Z9.PROGRAM_CODE = "P024_" + query.ID.ToSafeString();
                    _fix_Z9.PROGRAM_NAME = "CONFIG_INS";
                    _fix_Z9.COM_CODE = "Z9";
                    _fix_Z9.ASSET_CLASS_GI = "ราคาเหมาจ่ายสำหรับเคส No Data";
                    _fix_Z9.ASSET_CLASS_INS = "";
                    _fix_Z9.COST_CENTER = "";
                    _fix_Z9.QUANTITY = query.RATE;
                    _fix_Z9.MOVEMENT_TYPE_OUT = "";
                    _fix_Z9.XREF1_HD = "";
                    _fix_Z9.EVA4_GI = "";
                    _fix_Z9.EVA4_INS = "";
                    _fix_Z9.DOCUMENT_TYPE = "";
                    _fix_Z9.CURRENCY = "";
                    _fix_Z9.RATE = query.RATE;
                    _fix_Z9.ACCOUNT = "";
                    _fix_Z9.CREATE_DATETIME = DateTime.Now;
                    _fix_Z9.MODIFY_DATETIME = DateTime.Now;
                    _fix_Z9.MOVEMENT_TYPE_IN = "";
                    _fix_Z9.EFFECTIVE_DATE = query.EFFECTIVE_DATE;
                    _fix_Z9.EXPIRE_DATE = query.EXPIRE_DATE;

                    var _fix_Z10 = new FBSS_FIXED_ASSET_CONFIG();
                    _fix_Z10.PROGRAM_CODE = "P024_" + query.ID.ToSafeString();
                    _fix_Z10.PROGRAM_NAME = "CONFIG_INS";
                    _fix_Z10.COM_CODE = "Z10";
                    _fix_Z10.ASSET_CLASS_GI = "ราคาเหมาจ่ายสำหรับเคส No Data for Reuse";
                    _fix_Z10.ASSET_CLASS_INS = "";
                    _fix_Z10.COST_CENTER = "";
                    _fix_Z10.QUANTITY = query.RATE;
                    _fix_Z10.MOVEMENT_TYPE_OUT = "";
                    _fix_Z10.XREF1_HD = "";
                    _fix_Z10.EVA4_GI = "";
                    _fix_Z10.EVA4_INS = "";
                    _fix_Z10.DOCUMENT_TYPE = "";
                    _fix_Z10.CURRENCY = "";
                    _fix_Z10.RATE = query.RATE;
                    _fix_Z10.ACCOUNT = "";
                    _fix_Z10.CREATE_DATETIME = DateTime.Now;
                    _fix_Z10.MODIFY_DATETIME = DateTime.Now;
                    _fix_Z10.MOVEMENT_TYPE_IN = "";
                    _fix_Z10.EFFECTIVE_DATE = query.EFFECTIVE_DATE;
                    _fix_Z10.EXPIRE_DATE = query.EXPIRE_DATE;

                    r.RETURN_CODE = 0;
                    r.RETURN_MSG = "success";

                    if (query.ACTION == WBBContract.Commands.ActionType.Insert)
                    {
                        _fixAssConfig.Create(_fix_Z9);
                        _fixAssConfig.Create(_fix_Z10);

                        r.RETURN_CODE = 0;
                        r.RETURN_MSG = "success";
                    }
                    else if (query.ACTION == WBBContract.Commands.ActionType.Update)
                    {
                        var _where = "P024_" + query.ID.ToSafeString();
                        var _fix = _fixAssConfig.Get(w => w.PROGRAM_CODE == _where).ToList();
                        if (query.IS_DELETE_FIXASSCONFIG == "Y")
                        {
                            // Delete
                            var _fix_delete9 = _fixAssConfig.Get(w => w.PROGRAM_CODE == _where && w.COM_CODE == "Z9").ToList();
                            if (_fix_delete9.Count() > 0)
                            {
                                _fix_Z10 = _fixAssConfig.Get(w => w.PROGRAM_CODE == _where && w.COM_CODE == "Z9").FirstOrDefault();
                                _fixAssConfig.Delete(_fix_Z10);
                            }

                            var _fix_delete10 = _fixAssConfig.Get(w => w.PROGRAM_CODE == _where && w.COM_CODE == "Z9").ToList();
                            if (_fix_delete10.Count() > 0)
                            {
                                _fix_Z10 = _fixAssConfig.Get(w => w.PROGRAM_CODE == _where && w.COM_CODE == "Z10").FirstOrDefault();
                                _fixAssConfig.Delete(_fix_Z10);
                            }
                        }
                        else
                        {
                            // Update
                            if (_fix.Count() > 0)
                            {
                                _fix_Z9 = _fixAssConfig.Get(w => w.PROGRAM_CODE == _where && w.COM_CODE == "Z9").FirstOrDefault();
                                _fix_Z9.QUANTITY = query.RATE;
                                _fix_Z9.RATE = query.RATE;
                                _fix_Z9.MODIFY_DATETIME = DateTime.Now;
                                _fix_Z9.EFFECTIVE_DATE = query.EFFECTIVE_DATE;
                                _fix_Z9.EXPIRE_DATE = query.EXPIRE_DATE;
                                _fixAssConfig.Update(_fix_Z9);

                                _fix_Z10 = _fixAssConfig.Get(w => w.PROGRAM_CODE == _where && w.COM_CODE == "Z10").FirstOrDefault();
                                _fix_Z10.QUANTITY = query.RATE;
                                _fix_Z10.RATE = query.RATE;
                                _fix_Z10.MODIFY_DATETIME = DateTime.Now;
                                _fix_Z10.EFFECTIVE_DATE = query.EFFECTIVE_DATE;
                                _fix_Z10.EXPIRE_DATE = query.EXPIRE_DATE;
                                _fixAssConfig.Update(_fix_Z10);
                            }
                            else
                            {
                                _fixAssConfig.Create(_fix_Z9);
                                _fixAssConfig.Create(_fix_Z10);
                            }
                        }
                        r.RETURN_CODE = 0;
                        r.RETURN_MSG = "success";
                    }
                    else if (query.ACTION == WBBContract.Commands.ActionType.Delete)
                    {
                        var _where = "P024_" + query.ID.ToSafeString();
                        var _fix_delete9 = _fixAssConfig.Get(w => w.PROGRAM_CODE == _where && w.COM_CODE == "Z9").ToList();

                        if (_fix_delete9.Count() > 0)
                        {
                            _fix_Z10 = _fixAssConfig.Get(w => w.PROGRAM_CODE == _where && w.COM_CODE == "Z9").FirstOrDefault();
                            _fixAssConfig.Delete(_fix_Z10);
                        }

                        var _fix_delete10 = _fixAssConfig.Get(w => w.PROGRAM_CODE == _where && w.COM_CODE == "Z9").ToList();
                        if (_fix_delete10.Count() > 0)
                        {
                            _fix_Z10 = _fixAssConfig.Get(w => w.PROGRAM_CODE == _where && w.COM_CODE == "Z10").FirstOrDefault();
                            _fixAssConfig.Delete(_fix_Z10);
                        }

                        r.RETURN_CODE = 0;
                        r.RETURN_MSG = "success";
                    }
                    _uow.Persist();
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.GetErrorMessage());
                _logger.Info(ex.Message);

                //historyLog.HISTORY_ID = historyLog.HISTORY_ID;
                //historyLog.ACTION = query.ACTION.ToSafeString();
                //historyLog.APPLICATION = "SupContractor";
                //historyLog.DESCRIPTION = "Error Message : [" + ex.Message.ToSafeString() + "] InnerException : [" + ex.InnerException.ToSafeString() + "] Params : { " + _PARAMS + " }";
                //historyLog.REF_KEY = "Service : CreateCostInstallationQueryHandler";
                //historyLog.REF_NAME = "Effect_fixAssConfig";
                //historyLog.CREATED_BY = query.CREATE_BY.ToSafeString();
                //historyLog.CREATED_DATE = DateTime.Now;
                //_historyLog.Create(historyLog);
                //_uow.Persist();

                r.RETURN_CODE = -1;
                r.RETURN_MSG = ex.GetErrorMessage();
            }
        }
    }
}
