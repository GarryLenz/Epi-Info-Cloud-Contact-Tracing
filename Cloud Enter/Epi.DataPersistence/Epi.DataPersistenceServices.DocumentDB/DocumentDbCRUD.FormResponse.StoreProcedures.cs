﻿using System;
using System.Collections.Generic;
using Epi.Cloud.Resources;
using Epi.Cloud.Resources.Constants;
using Epi.PersistenceServices.DocumentDB;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Epi.DataPersistenceServices.DocumentDB
{
    public partial class DocumentDbCRUD
    {
        public int? continuationToken = null;
        private const string SPGetRecordsBySurveyId = "GetRecordsBySurveyId";

        /// <summary>
        /// Execute DB SP-Get all records by surveyID 
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="spId"></param>
        /// <param name="surveyID"></param>
        /// <returns></returns>
        private List<FormResponseProperties> GetAllRecordsBySurveyId(string collectionId, string spId, string query)
        {
            return ExecuteSPAsync(collectionId, spId, query);
        }

        internal class OrderByResult
        {
            public Document[] Result { get; set; }
            public int? Continuation { get; set; }
        }

        /// <summary>
        /// ExecuteSPAsync
        /// </summary>
        /// <param name="collectionId"></param>
        /// <param name="spId"></param>
        /// <param name="surveyId"></param>
        /// <returns></returns>
        private List<FormResponseProperties> ExecuteSPAsync(string collectionId, string spId, string query)
        {
            RequestOptions option = new RequestOptions();
            var formResponseList = new List<FormResponseProperties>();
            var formResponse = new FormResponseProperties();
            // Create SP Uri
            string spUri = UriFactory.CreateStoredProcedureUri(DatabaseName, collectionId, spId).ToString();
            try
            {
                do
                {
                    var spResponse = Client.ExecuteStoredProcedureAsync<OrderByResult>(spUri, query).Result;
                    foreach (var doc in spResponse.Response.Result)
                    {
                        formResponse = (dynamic)doc;
                        formResponseList.Add(formResponse);
                    }
                } while (continuationToken != null);

                return formResponseList;
            }
            catch (Exception ex)
            {
                var errorCode = ((DocumentClientException)ex.InnerException).Error.Code;
                if (errorCode == "NotFound")
                {
                    spUri = UriFactory.CreateDocumentCollectionUri(DatabaseName, collectionId).ToString();
                    var createSPResponse = CreateSPAsync(spUri, spId);

                    //Execute SP 
                    ExecuteSPAsync(collectionId, spId, query);
                }
            }
            return null;
        }

        /// <summary>
        /// Create Document Db Stored Procedure
        /// </summary>
        /// <param name="spSelfLink"></param>
        /// <param name="spId"></param>
        /// <returns></returns>
        private StoredProcedure CreateSPAsync(string spSelfLink, string spId)
        {

            try
            {
                StoredProcedure sproc = new StoredProcedure();
                var sprocBody = ResourceProvider.GetResourceString(ResourceNamespaces.DocumentDBSp, DocumentDBSPKeys.GetAllRecordsBySurveyID);
                var sprocDefinition = new StoredProcedure
                {
                    Id = spId,
                    Body = sprocBody
                };
                var result = Client.CreateStoredProcedureAsync(spSelfLink, sprocDefinition).Result;
                return sproc;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }
}