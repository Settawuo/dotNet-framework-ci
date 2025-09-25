using System;
using System.Collections.Generic;
using System.Linq;
using WBBEntity.Extensions;
using WBBEntity.PanelModels;
using WBBWeb.Models;

namespace WBBWeb.Controllers.ConfigLovs
{
    public class ConfigLovHelpers : WBBController
    {

        public List<LovScreenValueModel> GetLovScreenByPageCode(string pageCode)
        {
            try
            {
                List<LovValueModel> config;
                if (pageCode == null)
                {
                    config = LovData.Where(l => l.LovValue5 == null && l.Type == "SCREEN").ToList();
                }
                else if (pageCode == "ALLPAGE")
                {
                    config = LovData.Where(l => l.Type == "SCREEN").ToList();
                }
                else
                {
                    config = LovData.Where(l =>
                        (!string.IsNullOrEmpty(l.Type) && l.Type == "SCREEN")
                            && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(pageCode))).ToList();
                }
                List<LovScreenValueModel> screenValue;
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

        public List<LovScreenValueModel> GetLovByTypePageCode(string typeName, string pageCode)
        {
            try
            {
                var config = LovData.Where(l =>
                    (!string.IsNullOrEmpty(l.Type) && l.Type == typeName)
                    && (!string.IsNullOrEmpty(l.LovValue5) && l.LovValue5.Equals(pageCode))).ToList();

                List<LovScreenValueModel> screenValue;
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

        public List<DropdownModel> GetLovDropdownListByType(string lovType)
        {
            try
            {
                var dropDown = base.LovData
                    .Where(l => l.Type.Equals(lovType))
                    .Select(l => new DropdownModel
                    {
                        Text = (SiteSession.CurrentUICulture.IsThaiCulture() ? l.LovValue1 : l.LovValue2),
                        Value = l.Name,
                        DefaultValue = l.DefaultValue,
                    }).ToList();
                return dropDown;
            }
            catch (Exception ex)
            {
                Logger.Info(ex.GetErrorMessage());
                return new List<DropdownModel>();
            }
        }

        public List<FbbConstantModel> GetFbbConstantModel(string fbbConstType)
        {
            var data = LovData
               .Where(l => l.Type.Equals(fbbConstType))
               .Select(l => new FbbConstantModel
               {
                   Field = l.Name,
                   Validation = l.LovValue1,
                   SubValidation = l.LovValue2
               }).ToList();

            return data;
        }
    }
}
