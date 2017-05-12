﻿using System.Collections.Generic;
using System.Linq;
using Epi.Cloud.Common.Metadata;
using Epi.DataPersistence.Constants;
using Epi.DataPersistence.DataStructures;
using static Epi.PersistenceServices.DocumentDB.DataStructures;

namespace Epi.DataPersistence.Extensions
{
#if !DocDbV2
    public static class DocumentDBExtensions
    {
        public static PageResponseDetail ToPageResponseDetail(this PageResponseProperties pageResponseProperties, FormResponseDetail formResponseDetail, MetadataAccessor metadataAccessor = null)
        {
            return pageResponseProperties.ToPageResponseDetail(formResponseDetail.FormId, formResponseDetail.FormName, metadataAccessor);
        }

        public static PageResponseDetail ToPageResponseDetail(this PageResponseProperties pageResponseProperties, string formId, string formName, MetadataAccessor metadataAccessor = null)
        {
			metadataAccessor = metadataAccessor ?? new MetadataAccessor();
			var pageResponseDetail = new PageResponseDetail
			{
				GlobalRecordID = pageResponseProperties.GlobalRecordID,
				FormId = formId,
				FormName = formName,
				PageId = pageResponseProperties.PageId,
				PageNumber = metadataAccessor.GetPageDigestByPageId(formId, pageResponseProperties.PageId).PageNumber,
                ResponseQA = pageResponseProperties.ResponseQA
            };
            return pageResponseDetail;
        }

        public static FormResponseDetail ToFormResponseDetail(this HierarchicalDocumentResponseProperties hierarchicalDocumentResponseProperties)
        {
            var formResponseProperties = hierarchicalDocumentResponseProperties.FormResponseProperties;
            var pageResponsePropertiesList = hierarchicalDocumentResponseProperties.PageResponsePropertiesList;

            var formResponseDetail = formResponseProperties.ToFormResponseDetail(pageResponsePropertiesList);
            formResponseDetail.AddFormResponseDetailChildren(hierarchicalDocumentResponseProperties);

            return formResponseDetail;
        }

        private static void AddFormResponseDetailChildren(this FormResponseDetail formResponseDetail, HierarchicalDocumentResponseProperties hierarchicalDocumentResponseProperties)
        {
            foreach (var childHierarchicalDocumentResponseProperties in hierarchicalDocumentResponseProperties.ChildResponseList)
            {
                var childFormResponseDetail = childHierarchicalDocumentResponseProperties.ToFormResponseDetail();
				childFormResponseDetail.ParentFormId = formResponseDetail.FormId;
                formResponseDetail.AddChildFormResponseDetail(childFormResponseDetail);
            }
        }

        public static FormResponseDetail ToFormResponseDetail(this DocumentResponseProperties documentResponseProperties)
        {
            FormResponseDetail formResponseDetail = null;

            var formResponseProperties = documentResponseProperties.FormResponseProperties;
            if (formResponseProperties != null)
            {
                formResponseDetail = formResponseProperties.ToFormResponseDetail();
                if (documentResponseProperties.PageResponsePropertiesList != null && documentResponseProperties.PageResponsePropertiesList.Count > 0)
                {
					MetadataAccessor metadataAccessor = new MetadataAccessor();
                    foreach (var pageResponseProperties in documentResponseProperties.PageResponsePropertiesList)
                    {
						if (pageResponseProperties != null)
						{
							var pageResponseDetail = pageResponseProperties.ToPageResponseDetail(formResponseDetail, metadataAccessor);
							formResponseDetail.AddPageResponseDetail(pageResponseDetail);
						}
                    }
                }
            }

            return formResponseDetail;
        }

        public static FormResponseDetail ToFormResponseDetail(this FormResponseProperties formResponseProperties, List<PageResponseProperties> pageResponsePropertiesList = null)
        {
            var formResponseDetail = new FormResponseDetail
            {
                GlobalRecordID = formResponseProperties.GlobalRecordID,

                FormId = formResponseProperties.FormId,
                FormName = formResponseProperties.FormName,
                IsNewRecord = formResponseProperties.IsNewRecord,
                RecStatus = formResponseProperties.RecStatus,
                RelateParentResponseId = formResponseProperties.RelateParentId,
                FirstSaveLogonName = formResponseProperties.FirstSaveLogonName,
                LastSaveLogonName = formResponseProperties.LastSaveLogonName,
                FirstSaveTime = formResponseProperties.FirstSaveTime,
                LastSaveTime = formResponseProperties.LastSaveTime,

                LastActiveUserId = formResponseProperties.UserId,
                IsRelatedView = formResponseProperties.IsRelatedView,
                IsDraftMode = formResponseProperties.IsDraftMode,
                PageIds = formResponseProperties.PageIds,
                RequiredFieldsList = formResponseProperties.RequiredFieldsList,
                HiddenFieldsList = formResponseProperties.HiddenFieldsList,
                HighlightedFieldsList = formResponseProperties.HighlightedFieldsList,
                DisabledFieldsList = formResponseProperties.DisabledFieldsList
            };

            if (pageResponsePropertiesList != null && pageResponsePropertiesList.Count > 0)
            {
                foreach (var pageResponseProperties in pageResponsePropertiesList)
                {
					if (pageResponseProperties != null)
					{
						formResponseDetail.AddPageResponseDetail(pageResponseProperties.ToPageResponseDetail(formResponseDetail));
					}
                }
            }

            return formResponseDetail;
        }

		public static FormResponseProperties ToFormResponseProperties(this FormResponseDetail formResponseDetail)
		{
			var formResponseProperties = new FormResponseProperties
			{
				Id = formResponseDetail.GlobalRecordID,
				GlobalRecordID = formResponseDetail.GlobalRecordID,

				FormId = formResponseDetail.FormId,
				FormName = formResponseDetail.FormName,
                IsNewRecord = formResponseDetail.RecStatus == RecordStatus.InProcess ? formResponseDetail.IsNewRecord : false,
				RecStatus = formResponseDetail.RecStatus,
				RelateParentId = formResponseDetail.RelateParentResponseId,
				FirstSaveLogonName = formResponseDetail.FirstSaveLogonName,
				LastSaveLogonName = formResponseDetail.LastSaveLogonName,
				FirstSaveTime = formResponseDetail.FirstSaveTime,
				LastSaveTime = formResponseDetail.LastSaveTime,

				UserId = formResponseDetail.LastActiveUserId,
				IsRelatedView = formResponseDetail.IsRelatedView,
				IsDraftMode = formResponseDetail.IsDraftMode,

				PageIds = formResponseDetail.PageIds,

                HiddenFieldsList = formResponseDetail.HiddenFieldsList,
                HighlightedFieldsList = formResponseDetail.HighlightedFieldsList,
                DisabledFieldsList = formResponseDetail.DisabledFieldsList,
                RequiredFieldsList = formResponseDetail.RequiredFieldsList
			};


			formResponseProperties.PageIds.AddRange(formResponseDetail.PageResponseDetailList.Select(p => p.PageId).ToArray());
			formResponseProperties.PageIds = formResponseDetail.PageIds.Distinct().OrderBy(pid => pid).ToList();

			return formResponseProperties;
		}

		public static PageResponseProperties ToPageResponseProperties(this PageResponseDetail pageResponseDetail)
		{
			var pageResponseProperties = new PageResponseProperties
			{
				Id = pageResponseDetail.GlobalRecordID,
				GlobalRecordID = pageResponseDetail.GlobalRecordID,
				PageId = pageResponseDetail.PageId,
				ResponseQA = pageResponseDetail.ResponseQA
			};
			return pageResponseProperties;
		}
    }
#endif
}