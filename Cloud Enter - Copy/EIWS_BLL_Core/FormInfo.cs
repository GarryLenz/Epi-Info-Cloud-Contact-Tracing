﻿using System.Collections.Generic;
using Epi.Web.Enter.Common.BusinessObject;
using Epi.Web.Enter.Interfaces.DataInterface;
using System.Configuration;

namespace Epi.Web.BLL
{
    public class FormInfo
    {
        private IFormInfoDao FormInfoDao;

        public FormInfo(IFormInfoDao pSurveyInfoDao)
        {
            this.FormInfoDao = pSurveyInfoDao;
        }

        public List<FormInfoBO> GetFormsInfo(int UserId, int CurrentOrgId)
        {
            //Owner Forms
            List<FormInfoBO> result = this.FormInfoDao.GetFormInfo(UserId, CurrentOrgId);
            return result;
        }

        public FormInfoBO GetFormInfoByFormId(string FormId, bool getMetadata, int UserId)
        {
            //Owner Forms
            FormInfoBO result = this.FormInfoDao.GetFormByFormId(FormId, getMetadata, UserId);

            if (ConfigurationManager.AppSettings["IsEWAVLiteIntegrationEnabled"].ToUpper() == "TRUE" && result.IsSQLProject)
            {
                bool toggleSwitchValue = this.FormInfoDao.GetEwavLiteToggleSwitch(FormId, UserId);

                result.EwavLiteToggleSwitch = toggleSwitchValue;
            }
            result.HasDraftModeData = this.FormInfoDao.HasDraftRecords(FormId);

            return result;
        }
    }
}