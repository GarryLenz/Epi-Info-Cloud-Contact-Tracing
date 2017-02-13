﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epi.FormMetadata.DataStructures;

namespace Epi.Cloud.Interfaces.MetadataInterfaces
{
    public interface IProjectMetadataProvider
    {
        string ProjectId { get; }
        Guid ProjectGuid { get; }
        string GetProjectId_RetrieveProjectIfNecessary();

        Task<Template> RetrieveProjectMetadataAsync(Guid projectId);

        //Task<Template> GetProjectMetadataAsync();
        //Pass the page id and call the DBAccess API and get the project fileds.
        //Task<Template> GetProjectMetadataAsync(string formId, ProjectScope scope);
        Task<Page> GetPageMetadataAsync(string formId, int pageId);
        Task<FormDigest[]> GetFormDigestsAsync();
        Task<FormDigest> GetFormDigestAsync(string formId);
        Task<PageDigest[][]> GetProjectPageDigestsAsync();
        Task<PageDigest[]> GetPageDigestsAsync(string formId);
        Task<FieldDigest> GetFieldDigestAsync(string formId, string fieldName);
        Task<FieldDigest[]> GetFieldDigestsAsync(string formId);
        Task<FieldDigest[]> GetFieldDigestsAsync(string formId, IEnumerable<string> fieldNames);
    }

    public enum ProjectScope
    {
        TemplateWithAllPages = -1,
        TemplateWithNoPages = 0
    }
}
