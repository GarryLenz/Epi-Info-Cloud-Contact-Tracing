﻿using System.Collections.Generic;
using Epi.Web.Enter.Common.DTO;
using MvcDynamicForms;
using Epi.Cloud.DataEntryServices.Model;
using System;
using System.Threading.Tasks;
using Epi.Web.Enter.Common.Message;
using Epi.Cloud.Common.Metadata;
using Epi.Cloud.Interfaces.MetadataInterfaces;
using Epi.Cloud.Common.Constants;
using Epi.Cloud.DataEntryServices.Extensions;
using Epi.Cloud.Common.EntityObjects;

namespace Epi.Cloud.DataEntryServices.Facade
{
	public class SurveyDocumentDBFacade : MetadataAccessor, ISurveyStoreDocumentDBFacade
	{
		public SurveyDocumentDBFacade(IProjectMetadataProvider projectMetadataProvider)
		{
			ProjectMetadataProvider = projectMetadataProvider;
		}


		CRUDSurveyResponse _surveyResponse = new CRUDSurveyResponse();

		/// <summary>
		/// Insert survey question and answer to Document Db
		/// </summary>

		public bool DoesResponseExist(string responseId)
		{
			return _surveyResponse.DoesResponseIdExist(responseId);
		}

		public int GetFormResponseCount(string formId)
		{
			return _surveyResponse.GetFormResponseCount(formId);
		}

		public FormResponseDetail GetFormResponseState(string responseId)
		{
			var formResponseProperties = _surveyResponse.GetFormResponseState(responseId);
			return formResponseProperties != null ? formResponseProperties.ToFormResponseDetail() : null;
		}

		public bool UpdateResponseStatus(string responseId, int responseStatus)
		{
            return _surveyResponse.UpdateResponseStatus(responseId, responseStatus);
		}

		#region Insert Into Servey Response to DocumentDB
		public async Task<bool> InsertSurveyResponseToDocumentDBStoreAsync(SurveyInfoModel surveyInfoModel, string responseId, Form form, SurveyAnswerDTO surveyAnswerDTO, bool IsSubmited, bool IsSaved, int PageNumber, int userId)
		{
			FormDocumentDBEntity storeSurvey;
			string Dbname = surveyInfoModel.SurveyName;
			storeSurvey = GetDbAndCollectionName(form, Dbname);
			storeSurvey.GlobalRecordID = responseId;
			storeSurvey.PageResponsePropertiesList.Add(form.ToPageResponseProperties(responseId));
			bool response = await _surveyResponse.InsertToSurveyToDocumentDBAsync(storeSurvey, userId);
			return true;
		}
		#endregion


		#region ReadSurveyAnswerByResponseID,PageId 
		public PageResponseDetail ReadSurveyAnswerByResponseID(string formId, string responseId, int pageId)
		{
			var formName = GetFormDigest(formId).FormName;
			var pageResponseProperties = _surveyResponse.GetPageResponsePropertiesByResponseId(responseId, formId, pageId);
			var pageResponseDetail = pageResponseProperties != null
								   ? pageResponseProperties.ToPageResponseDetail(formId, formName)
								   : null;
			return pageResponseDetail;
		}

		#endregion

		#region DeleteSurveyByResponseId
		public SurveyAnswerResponse DeleteResponse(FormDocumentDBEntity SARequest)
		{
			SurveyAnswerResponse surveyAnsResponse = new SurveyAnswerResponse();
			//var tasks = _surveyResponse.DeleteDocumentByIdAsync(SARequest);
			//var result = tasks.Result;
			return surveyAnsResponse;
		}

		#endregion

		public FormsHierarchyDTO GetChildRecordByChildFormId(string childFormId, string relateParentId, IDictionary<int, FieldDigest> fields)
		{
			FormsHierarchyDTO formHierarchyDTO = new FormsHierarchyDTO();
			var childRecords = _surveyResponse.GetAllResponsesWithFieldNames(fields, relateParentId);
			formHierarchyDTO.ResponseIds = new List<SurveyAnswerDTO>();
			foreach (var item in childRecords)
			{
				SurveyAnswerDTO surveyAnswer = new SurveyAnswerDTO();
				surveyAnswer.ResponseId = item.ResponseId.ToString();
				surveyAnswer.SqlData = item.ResponseDetail.FlattenedResponseQA(key => key.ToLower());
				formHierarchyDTO.ResponseIds.Add(surveyAnswer);
			}
		  
			return formHierarchyDTO;
		}

		#region Save FormParentProperties to DocumentDB
		/// <summary>
		/// First time store ResonseId,RecStatus, and SurveyId in DocumentDB
		/// </summary>
		/// <param name="ProjectMetaData"></param>
		/// <param name="Status"></param>
		/// <param name="UserId"></param>
		/// <param name="ResponseId"></param>
		/// <returns></returns>
		public bool SaveFormPropertiesToDocumentDB(SurveyAnswerRequest request)
		{

			FormDocumentDBEntity storeForm = new FormDocumentDBEntity(); 
			if(request.Criteria.IsDeleteMode)
			{
				request.SurveyAnswerList[0].Status = RecordStatus.Deleted;
			}

			var formName = GetFormDigest(request.Criteria.SurveyId).FormName;
			FormResponseProperties formData = new FormResponseProperties()
			{
				RecStatus = request.SurveyAnswerList[0].Status,
				FormId = request.Criteria.SurveyId,
				FormName = formName,
				GlobalRecordID = request.SurveyAnswerList[0].ResponseId,
				RelateParentId = request.SurveyAnswerList[0].RelateParentId,
				//IsRelatedView = ProjectMetaData.IsRelatedView,
				FirstSaveTime = DateTime.UtcNow,
				LastSaveTime = DateTime.UtcNow,
				UserId = request.Criteria.UserId,
				IsDraftMode = request.Criteria.IsDraftMode
			};

			storeForm.FormResponseProperties = new FormResponseProperties();
			storeForm.FormResponseProperties = formData;
			storeForm.GlobalRecordID = request.RequestId; 
			var saveTask = _surveyResponse.SaveSurveyQuestionInDocumentDBAsync(storeForm);
			
			//if(_FormData.RecStatus==2)
			//{
			//  //  var test = ReadFormInfo(_storeForm.GlobalRecordID);
			//    string Query= "Select * from FormInfo where FormInfo.GlobalRecordID ='"+_FormData.GlobalRecordID +"' and FormInfo.RecStatus!=0" ;
			//    var FormInfo =(FormProperties)_surveyResponse.ReadDataFromCollection(_FormData.GlobalRecordID, "FormInfo", Query);
			//    CURDServiceBus crudServicBus = new CURDServiceBus();
			//    if(FormInfo!=null)
			//    {
			//        //send form info to ServiceBus
			//        crudServicBus.SendMessagesToTopic(FormInfo.Id, JsonConvert.SerializeObject(FormInfo));
			//    }
			//}
			return true;
		}

		#endregion

		#region Get the Record by GlobalRecordID
		public SurveyAnswerResponse GetSurveyAnswerResponse(string responseId)
		{
			// TODO Implement GetSurveyAnswerResponse
			return null;
		}
		#endregion

		#region Get the Record by GlobalRecordID
		public SurveyAnswerResponse GetSurveyAnswerResponse(string responseId, int userId)
        { 
			SurveyAnswerResponse _surveyAnswerResponse = new SurveyAnswerResponse();
			_surveyAnswerResponse.SurveyResponseList = new List<SurveyAnswerDTO>();

			var formDocumentDbEntity = _surveyResponse.GetAllPageResponsesByResponseId(responseId);
			List<SurveyAnswerDTO> surveyResponseList = new List<SurveyAnswerDTO>();
			SurveyAnswerDTO surveyAnswerDTO = new SurveyAnswerDTO();
			surveyAnswerDTO.ResponseId = responseId;
			surveyAnswerDTO.SurveyId = formDocumentDbEntity.FormResponseProperties.FormId;

			surveyAnswerDTO.ResponseDetail = formDocumentDbEntity.ToFormResponseDetail();

			surveyAnswerDTO.DateCreated = formDocumentDbEntity.FormResponseProperties.FirstSaveTime;
			surveyAnswerDTO.DateUpdated = formDocumentDbEntity.FormResponseProperties.LastSaveTime;
			surveyAnswerDTO.RelateParentId = formDocumentDbEntity.FormResponseProperties.RelateParentId;
			surveyAnswerDTO.LastActiveUserId = formDocumentDbEntity.FormResponseProperties.UserId;
			surveyAnswerDTO.Status = RecordStatus.Saved;
			surveyResponseList.Add(surveyAnswerDTO); 
			_surveyAnswerResponse.SurveyResponseList = surveyResponseList;
			return _surveyAnswerResponse;
		}
		#endregion

		#region Get Forminfo for DataConsisitencyServiceAPI
		public List<string> GetFormInfo(string responseId)
		{ 
			var responseToDCSApi = _surveyResponse.ReadfullFormInfo(responseId);
			return responseToDCSApi;
		}
		#endregion

		#region Read All Records By SurveyID
		public IEnumerable<SurveyResponse> GetAllResponsesContainingFields(IDictionary<int, FieldDigest> gridFields)
		{
			return _surveyResponse.GetAllResponsesWithFieldNames(gridFields);
		}

		public FormResponseDetail GetFormResponseByResponseId(string responseId)
		{
			var response = _surveyResponse.GetAllPageResponsesByResponseId(responseId);
			var formResponseDetail = response.ToFormResponseDetail();
			return formResponseDetail;
		}
		#endregion

		#region GetCollectionName
		private FormDocumentDBEntity GetDbAndCollectionName(Form form, string DbName)
		{
			var formId = form.SurveyInfo.SurveyId;
			var pageId = Convert.ToInt32(form.PageId);
			var pageDigest = GetPageDigestByPageId(formId, pageId);

			FormDocumentDBEntity survey = new FormDocumentDBEntity();
			survey.FormName = pageDigest.FormName;
			survey.IsChildForm = pageDigest.IsRelatedView;
			survey.CollectionName = DbName + pageDigest.PageId;
			survey.DatabaseName = DbName;
			return survey;
		}
		#endregion
	}
}