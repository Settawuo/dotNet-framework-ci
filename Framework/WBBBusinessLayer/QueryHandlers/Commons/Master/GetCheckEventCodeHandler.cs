using System;
using System.Linq;
using WBBContract;
using WBBContract.Queries.Commons.Master;
using WBBData.Repository;
using WBBEntity.Models;
using WBBEntity.PanelModels;

namespace WBBBusinessLayer.QueryHandlers.Commons.Master
{
    public class GetCheckEventCodeHandler : IQueryHandler<GetCheckEventCodeQuery, EventCodeModel>
    {
        private readonly ILogger _logger;
        private readonly IEntityRepository<FBB_EVENT_CODE> _eventCodeService;
        private readonly IEntityRepository<FBB_EVENT_SUBCONTRACT> _FBB_EVENT_SUBCONTRACT;

        public GetCheckEventCodeHandler(ILogger logger, IEntityRepository<FBB_EVENT_CODE> eventCodeService, IEntityRepository<FBB_EVENT_SUBCONTRACT> FBB_EVENT_SUBCONTRACT)
        {
            _logger = logger;
            _eventCodeService = eventCodeService;
            _FBB_EVENT_SUBCONTRACT = FBB_EVENT_SUBCONTRACT;
        }

        public EventCodeModel Handle(GetCheckEventCodeQuery query)
        {
            #region Query
            //select 'Y' correct_code
            //from fbb_event_code c
            //where c.event_code = p_event_code
            //and trunc(sysdate) between c.effective_date and c.expire_date;
            #endregion

            var EventcodeModel = new EventCodeModel();

            try
            {
                DateTime EFFECTIVE_DATE_TH = new DateTime();
                DateTime EXPIRE_DATE_TH = new DateTime();

                DateTime EFFECTIVE_DATE_EN = new DateTime();
                DateTime EXPIRE_DATE_EN = new DateTime();

                DateTime CurrentDate = DateTime.Now;
                var date = DateTime.Now.Date;

                _logger.Info("Get_FBB_EVENT_SUBCONTRACT");

                var Event = from e in _eventCodeService.Get()
                            where e.EVENT_CODE.ToUpper() == query.Event_Code.ToUpper()
                            && (date >= e.EFFECTIVE_DATE && (e.EXPIRE_DATE == null || date <= e.EXPIRE_DATE))
                            select e;

                if (Event.Any())
                {
                    var EventCode = Event.Select(l => l.EVENT_CODE).FirstOrDefault();

                    var SUBCONTRACT = from f in _FBB_EVENT_SUBCONTRACT.Get()
                                      where f.EVENT_CODE == EventCode
                                      select new { f.EVENT_START_DATE, f.EVENT_END_DATE };

                    var effective_date = (from d in SUBCONTRACT select d.EVENT_START_DATE).Min();
                    var expire_date = (from d in SUBCONTRACT select d.EVENT_END_DATE).Max();

                    if (!effective_date.HasValue)
                    {
                        EFFECTIVE_DATE_EN = DateTime.Now;
                    }
                    else
                    {
                        if (effective_date.GetValueOrDefault() < CurrentDate)
                        {
                            EFFECTIVE_DATE_EN = CurrentDate;
                        }
                        else
                        {
                            EFFECTIVE_DATE_EN = effective_date.GetValueOrDefault();
                        }
                    }
                    EFFECTIVE_DATE_TH = EFFECTIVE_DATE_EN.AddYears(543);

                    if (!expire_date.HasValue)
                    {
                        EXPIRE_DATE_EN = DateTime.Now;
                    }
                    else
                    {
                        if (expire_date.GetValueOrDefault() < CurrentDate)
                        {
                            EXPIRE_DATE_EN = CurrentDate;
                        }
                        else
                        {
                            EXPIRE_DATE_EN = expire_date.GetValueOrDefault();
                        }
                    }
                    EXPIRE_DATE_TH = EXPIRE_DATE_EN.AddYears(543);

                    EventcodeModel.eventCode = EventCode;
                    EventcodeModel.technology = Event.Select(l => l.TECHNOLOGY).FirstOrDefault();
                    EventcodeModel.EFFECTIVE_DATE_EN1 = EFFECTIVE_DATE_EN.ToString("yyyy/MM/dd");
                    EventcodeModel.EFFECTIVE_DATE_EN2 = EFFECTIVE_DATE_EN.ToString("dd/MM/yyyy");
                    EventcodeModel.EFFECTIVE_DATE_TH1 = EFFECTIVE_DATE_TH.ToString("yyyy/MM/dd");
                    EventcodeModel.EFFECTIVE_DATE_TH2 = EFFECTIVE_DATE_TH.ToString("dd/MM/yyyy");
                    EventcodeModel.EXPIRE_DATE_EN = EXPIRE_DATE_EN.ToString("yyyy/MM/dd");
                    EventcodeModel.EXPIRE_DATE_TH = EXPIRE_DATE_TH.ToString("yyyy/MM/dd");

                    //Update 16.4
                    var plug_and_play_flag = Event.Select(l => l.PLUG_AND_PLAY_FLAG).FirstOrDefault();
                    EventcodeModel.plug_and_play_flag = plug_and_play_flag;

                    return EventcodeModel;
                }
            }
            catch (Exception ex)
            {
                _logger.Info(ex.Message.ToString());

            }
            return EventcodeModel;

        }
    }
}
