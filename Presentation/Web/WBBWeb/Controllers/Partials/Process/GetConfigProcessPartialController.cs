using System;
using System.Collections.Generic;
using System.Linq;
using WBBContract.Queries.FBBWebConfigQueries;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Extension;
using WBBWeb.Models;

namespace WBBWeb.Controllers
{
    public partial class ProcessController : WBBController
    {
        public List<DropdownModel> GetDropDownConfig(string type)
        {
            return base.LovData
                        .Where(l => l.Type == type)
                        .Select(l => new DropdownModel
                        {
                            Text = SiteSession.CurrentUICulture.IsThaiCulture() ? l.LovValue1 : l.LovValue2,
                            Value = l.LovValue3,
                        })
                        .ToList();
        }

        public List<DropdownModel> GetConfigByType(string type)
        {
            return base.LovData
                        .Where(l => l.Type == type)
                        .Select(l => new DropdownModel
                        {
                            Text = l.Name,
                            Value = l.LovValue1,
                            Value2 = l.LovValue2,
                            Value3 = l.LovValue3,
                            Value4 = l.LovValue4,
                            Value5 = l.LovValue5,
                            DefaultValue = l.DefaultValue
                        })
                        .ToList();
        }

        public List<LovScreenValueModel> GetScreenConfig(string pageCode)
        {
            try
            {
                List<LovValueModel> config = null;
                if (pageCode == null)
                {
                    config = base.LovData.Where(l => l.LovValue5 == null && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG").ToList();
                }
                else if (pageCode == "ALLPAGE")
                {
                    config = base.LovData.Where(l => l.Type == "SCREEN").ToList();
                }
                else
                {
                    config = base.LovData.Where(l =>
                        (!string.IsNullOrEmpty(l.Type) && l.Type == "SCREEN" || l.Type == "VAS_CODE_CONFIG")
                            && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(pageCode))).ToList();
                }
                //config = config.Where(a => a.Name == "L_DETAIL_DISCOUNT_SINGLE_BILL_1").ToList();
                var screenValue = new List<LovScreenValueModel>();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue1,
                        LovValue3 = l.LovValue3,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                        Type = l.Type,
                        DefaultValue = l.DefaultValue,
                        Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                        DisplayValueJing = l.Text
                    }).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue2,
                        LovValue3 = l.LovValue3,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                        Type = l.Type,
                        DefaultValue = l.DefaultValue,
                        Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                        DisplayValueJing = l.Text
                    }).ToList();
                }

                return screenValue;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<LovScreenValueModel>();
            }
        }

        public List<LovScreenValueModel> GetLovConfigBytype(string typeCode = "")
        {
            try
            {
                List<LovValueModel> config = null;
                config = base.LovData.Where(l => !string.IsNullOrEmpty(l.Type) && l.Type.Equals(typeCode)).ToList();
                var screenValue = new List<LovScreenValueModel>();
                if (SiteSession.CurrentUICulture.IsThaiCulture())
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue1,
                        LovValue3 = l.LovValue3,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                        Type = l.Type,
                        DefaultValue = l.DefaultValue,
                        Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                        DisplayValueJing = l.Text
                    }).OrderBy(t => t.OrderByPDF).ToList();
                }
                else
                {
                    screenValue = config.Select(l => new LovScreenValueModel
                    {
                        Name = l.Name,
                        PageCode = l.LovValue5,
                        DisplayValue = l.LovValue2,
                        LovValue3 = l.LovValue3,
                        GroupByPDF = l.LovValue4,
                        OrderByPDF = l.OrderBy,
                        Type = l.Type,
                        DefaultValue = l.DefaultValue,
                        Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                        DisplayValueJing = l.Text
                    }).OrderBy(t => t.OrderByPDF).ToList();
                }

                return screenValue;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<LovScreenValueModel>();
            }
        }

        private List<FileFormatModel> GetFormatFile(QuickWinPanelModel model)
        {
            var lang = (SiteSession.CurrentUICulture.IsThaiCulture() ? "THAI" : "ENG");

            var query = new GetFormatFileNameQuery
            {
                language = lang,
                ID_CardType = model.CustomerRegisterPanelModel.L_CARD_TYPE.ToSafeString(),
                ID_CardNo = model.CustomerRegisterPanelModel.L_CARD_NO.ToSafeString(),
                ListFilename = model.CustomerRegisterPanelModel.ListImageFile
            };

            var result = _queryProcessor.Execute(query);

            return result;
        }

        public List<LovScreenValueModel> GetGeneralScreenConfig()
        {
            var screenData = GetScreenConfig(null);
            return screenData;
        }

        public List<LovScreenValueModel> GetCoverageScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CoveragePageCode);
            return screenData;
        }

        public List<LovScreenValueModel> GetDisplayPackageScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.DisplayPackagePageCode);
            return screenData;
        }

        public List<LovScreenValueModel> GetDisplayOneLove()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CheckDataOnelove);
            return screenData;
        }

        public List<LovScreenValueModel> GetCustRegisterScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.CustomerRegisterPageCode);
            return screenData;
        }

        public List<LovScreenValueModel> GetTopUpFixedlineScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.GetTopUpFixedlinePageCode);
            return screenData;
        }

        public List<LovScreenValueModel> GetChangePromotionScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.ChangePromotionPageCode);
            return screenData;
        }

        public List<LovScreenValueModel> GetSummaryScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.SummaryPageCode);
            return screenData;
        }

        public List<LovScreenValueModel> GetDisplay_Select_Type_Service()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.SelectType_Service);
            return screenData;
        }

        public List<LovScreenValueModel> GetVas_Select_Package_ScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.Vas_Package);
            return screenData;
        }

        public List<LovScreenValueModel> GetVasPopUpScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.DisplayPopupVasConfrim);
            return screenData;
        }

        public List<LovScreenValueModel> GetSelectRouter()
        {
            //R18.1 FTTB Sell Router
            var screenData = GetScreenConfig(WebConstants.LovConfigName.GetSelectRouterCode);
            return screenData;
        }

        public List<LovScreenValueModel> GetTopUpPlaybox_ScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.TopupPlayBox);
            return screenData;
        }

        public List<LovScreenValueModel> GetTopUpInternet_ScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.GetTopUpInternetPageCode);
            return screenData;
        }
        public List<LovScreenValueModel> GetMeshConfig_ScreenConfig()
        {
            var screenData = GetLovConfigBytype(WebConstants.LovConfigName.MeshConfig);
            return screenData;
        }

        public List<LovScreenValueModel> GetTopUpMesh_ScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.GetTopUpMeshPageCode);
            return screenData;
        }

        public List<LovScreenValueModel> GetPlugandplayMessage()
        {
            var data = base.LovData
                .Where(l => l.Type.Equals(WebConstants.LovConfigName.Plugandplay)).OrderBy(t => t.OrderBy).ToList();
            List<LovScreenValueModel> screenValue;
            if (SiteSession.CurrentUICulture.IsThaiCulture())
            {
                screenValue = data.Select(l => new LovScreenValueModel
                {
                    Name = l.Name,
                    PageCode = l.LovValue5,
                    DisplayValue = l.LovValue1,
                    LovValue3 = l.LovValue3,
                    GroupByPDF = l.LovValue4,
                    OrderByPDF = l.OrderBy,
                    Type = l.Type,
                    DefaultValue = l.DefaultValue,
                    Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                    DisplayValueJing = l.Text
                }).ToList();
            }
            else
            {
                screenValue = data.Select(l => new LovScreenValueModel
                {
                    Name = l.Name,
                    PageCode = l.LovValue5,
                    DisplayValue = l.LovValue2,
                    LovValue3 = l.LovValue3,
                    GroupByPDF = l.LovValue4,
                    OrderByPDF = l.OrderBy,
                    Type = l.Type,
                    DefaultValue = l.DefaultValue,
                    Blob = l.Image_blob != null ? Convert.ToBase64String(l.Image_blob, 0, l.Image_blob.Length) : "",
                    DisplayValueJing = l.Text
                }).ToList();
            }

            return screenValue;

        }

        public List<LovScreenValueModel> GetTopUpReplace_ScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.GetTopUpReplacePageCode);
            return screenData;
        }
        //23.06 IPCAMERA
        public List<LovScreenValueModel> GetIpCameraScreenConfig()
        {
            var screenData = GetScreenConfig(WebConstants.LovConfigName.GetIpCamera);
            return screenData;
        }
        //R23.05 CheckFraud
        private FileFormatFraudModel GetFormatFileFraud(QuickWinPanelModel model)
        {

            var query = new GetFormatFileNameFraudQuery
            {
                ID_CardNo = model.CustomerRegisterPanelModel.L_CARD_NO.ToSafeString(),
            };

            var result = _queryProcessor.Execute(query);

            return result;
        }
    }
}