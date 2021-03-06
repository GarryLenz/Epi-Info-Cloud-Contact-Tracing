﻿using Epi.Cloud.Common.BusinessObjects;
using Epi.Cloud.Common.DTO;

namespace Epi.Cloud.MVC.Extensions
{
    public static class FormInfoBOExtensions
    {
        public static FormInfoDTO ToFormInfoDTO(this FormInfoBO BO)
        {
            return new FormInfoDTO
            {
                IsSQLProject = BO.IsSQLProject,
                FormId = BO.FormId,
                FormNumber = BO.FormNumber,
                FormName = BO.FormName,
                OrganizationName = BO.OrganizationName,
                OrganizationId = BO.OrganizationId,
                IsDraftMode = BO.IsDraftMode,
                UserId = BO.UserId,
                IsOwner = BO.IsOwner,
                OwnerFName = BO.OwnerFName,
                OwnerLName = BO.OwnerLName,
                IsShareable = BO.IsShareable,
                IsShared = BO.IsShared,
                HasDraftModeData = BO.HasDraftModeData
            };
        }
    }
}
