﻿using System;
using System.Collections.Generic;
using System.Linq;
using Epi.Cloud.Common.Configuration;
using Epi.Cloud.Interfaces.MetadataInterfaces;
using Epi.FormMetadata.DataStructures;
using Epi.FormMetadata.Constants;

namespace Epi.Cloud.Common.Metadata
{
    public class MetadataAccessor
    {
        public static class StaticCache
        {
            public static void ClearAll()
            {
                lock (Gate)
                {
                    FormDigests = null;
                    PageMetadata = new Dictionary<string, Dictionary<int, Page>>();
                    PageDigests = null;
                    FieldDigests = new Dictionary<string, FieldDigest[]>();
                    ProjectMetadataProvider = null;
                    PageFieldAttributes = null;
                }
            }
            public static object Gate = new object();

            // Form Digests [FormId]
            public static FormDigest[] FormDigests = null;

            // Page[FormId][PageId]
            public static Dictionary<string, Dictionary<int, Page>> PageMetadata = new Dictionary<string, Dictionary<int, Page>>();

            // PageDigests[FormId][PageId]
            public static PageDigest[][] PageDigests = null;

            // FieldDigests[FormId][PageId]
            public static Dictionary<string, FieldDigest[]> FieldDigests = new Dictionary<string, FieldDigest[]>();

            public static IProjectMetadataProvider ProjectMetadataProvider = null;

            // FieldAttributes[FormId][PageId][FieldName]
            public static Dictionary<string, Dictionary<int, Dictionary<string, FieldAttributes>>> PageFieldAttributes = null;

            public static Dictionary<string, string[]> FormIdPathFromRoot = new Dictionary<string, string[]>();
            public static Dictionary<string, string[]> FormNamePathFromRoot = new Dictionary<string, string[]>();
        }

        [ThreadStatic]
        protected string _formId;


        public MetadataAccessor()
        {
        }

        public MetadataAccessor(string surveyId)
        {
            _formId = surveyId;
        }

        public static void ClearStaticCache()
        {
            StaticCache.ClearAll();
        }

        public string CurrentFormId { get { return _formId; } set { _formId = value; } }

        public static IProjectMetadataProvider ProjectMetadataProvider
        {
            get
            {
                lock (StaticCache.Gate)
                {
                    if (StaticCache.ProjectMetadataProvider == null)
                    {
                        StaticCache.ProjectMetadataProvider = DependencyHelper.GetService<IProjectMetadataProvider>();
                    }
                    return StaticCache.ProjectMetadataProvider;
                }
            }

            protected set
            {
                lock (StaticCache.Gate)
                {
                    StaticCache.ProjectMetadataProvider = value;
                }
            }
        }

        public FormDigest[] FormDigests
        {
            get
            {
                lock (StaticCache.Gate)
                {
                    if (StaticCache.FormDigests == null) StaticCache.FormDigests = ProjectMetadataProvider.GetFormDigestsAsync().Result;

                    GenerateFormPaths();

                    return StaticCache.FormDigests;
                }
            }
        }

        public PageDigest[][] PageDigests
        {
            get
            {
                lock (StaticCache.Gate)
                {
                    if (StaticCache.PageDigests == null) StaticCache.PageDigests = (PageDigest[][])ProjectMetadataProvider.GetProjectPageDigestsAsync().Result.Clone();
                    return StaticCache.PageDigests;
                }
            }
        }

		public FieldDigest[] GetFieldDigests(string formId)
        {
            lock (StaticCache.Gate)
            {
                FieldDigest[] fieldDigests = null;
                if (!StaticCache.FieldDigests.TryGetValue(formId, out fieldDigests))
                    StaticCache.FieldDigests[formId] = fieldDigests = ProjectMetadataProvider.GetFieldDigestsAsync(formId).Result;
                return fieldDigests;
            }
        }

        private void GenerateFormPaths()
        {
            lock (StaticCache.Gate)
            {
                foreach (var fd in StaticCache.FormDigests)
                {
                    var formIdPathFromRoot = new List<string>();
                    var formNamePathFromRoot = new List<string>();
                    var fdx = fd;
                    do
                    {
                        formIdPathFromRoot.Insert(0, fdx.FormId);
                        formNamePathFromRoot.Insert(0, fdx.FormName);
                        var isRootForm = fdx.IsRootForm;
                        fdx = isRootForm ? null : StaticCache.FormDigests.Where(x => x.FormId == fdx.ParentFormId).SingleOrDefault();
                    } while (fdx != null);

                    StaticCache.FormIdPathFromRoot[fd.FormId] = formIdPathFromRoot.ToArray();
                    StaticCache.FormNamePathFromRoot[fd.FormId] = formNamePathFromRoot.ToArray();
                }
            }
        }

        public string GetFormIdByViewId(int viewId)
        {
            var formDigest = FormDigests.SingleOrDefault(d => d.ViewId == viewId);
            return formDigest != null ? formDigest.FormId : null;
        }

        public string GetRootFormIdByViewId(int viewId)
        {
            var formDigest = FormDigests.SingleOrDefault(d => d.ViewId == viewId);
            return formDigest != null ? formDigest.RootFormId : null;
        }
        public string GetParentFormIdByViewId(int viewId)
        {
            var formDigest = FormDigests.SingleOrDefault(d => d.ViewId == viewId);
            return formDigest != null ? formDigest.ParentFormId : null;
        }

        public string GetRootFormId(string formId)
        {
            var formDigest = FormDigests.SingleOrDefault(d => d.FormId == formId);
            return formDigest != null ? formDigest.RootFormId : null;
        }
        public string GetRootFormName(string formId)
        {
            string rootFormName = null;
            var formDigest = FormDigests.SingleOrDefault(d => d.FormId == formId);
            if (formDigest != null)
            {
                if (formDigest.IsRootForm)
                    rootFormName = formDigest.FormName;
                else
                {
                    var rootFormDigest = GetFormDigest(formDigest.RootFormId);
                    rootFormName = rootFormDigest != null ? rootFormDigest.FormName : null;
                }
            }
            return rootFormName;
        }

        public string GetParentFormId(string formId)
        {
            var formDigest = FormDigests.SingleOrDefault(d => d.FormId == formId);
            return formDigest != null ? formDigest.ParentFormId : null;
        }

        public string GetParentFormName(string formId)
        {
            string parentFormName = null;
            var formDigest = FormDigests.SingleOrDefault(d => d.FormId == formId);
            if (formDigest != null && formDigest.IsChildForm)
            {
                var parentFormDigest = GetFormDigest(formDigest.ParentFormId);
                parentFormName = parentFormDigest != null ? parentFormDigest.FormName : null;
            }
            return parentFormName;
        }

        public string GetFormName(string formId)
        {
            var formDigest = FormDigests.SingleOrDefault(d => d.FormId == formId);
            return formDigest != null ? formDigest.FormName : null;
        }

        public Dictionary<int, Page> GetAllPageMetadatasByFormId(string formId)
        {
            lock (StaticCache.Gate)
            {
                Dictionary<int, Page> pageMetadatas = null;
                if (!StaticCache.PageMetadata.TryGetValue(formId, out pageMetadatas))
                {
                    StaticCache.PageMetadata[formId] = pageMetadatas = new Dictionary<int, Page>();
                }
                return pageMetadatas;
            }
        }

        public Page GetPageMetadataByPageId(string formId, int pageId)
        {
            lock (StaticCache.Gate)
            {
                Dictionary<int, Page> pageMetadatas = null;
                if (!StaticCache.PageMetadata.TryGetValue(formId, out pageMetadatas))
                {
                    StaticCache.PageMetadata[formId] = pageMetadatas = new Dictionary<int, Page>();
                }

                Page pageMetadata = null;
                if (!pageMetadatas.TryGetValue(pageId, out pageMetadata))
                {
                    pageMetadatas[pageId] = pageMetadata = ProjectMetadataProvider.GetPageMetadataAsync(formId, pageId).Result;
                }

                return pageMetadata;
            }
        }

        public Dictionary<string, FieldAttributes> GetPageFieldAttributesByPageId(string formId, int pageId)
        {
            lock (StaticCache.Gate)
            {
                if (StaticCache.PageFieldAttributes == null)
                {
                    StaticCache.PageFieldAttributes = new Dictionary<string, Dictionary<int, Dictionary<string, FieldAttributes>>>();
                }

                Dictionary<int, Dictionary<string, FieldAttributes>> pageFieldAttributesByPageId = null;
                if (!StaticCache.PageFieldAttributes.TryGetValue(formId, out pageFieldAttributesByPageId))
                {
                    StaticCache.PageFieldAttributes[formId] = pageFieldAttributesByPageId = new Dictionary<int, Dictionary<string, FieldAttributes>>();
                }

                Dictionary<string, FieldAttributes> pageFieldAttributes;
                if (!pageFieldAttributesByPageId.TryGetValue(pageId, out pageFieldAttributes))
                {
                    var formDigest = FormDigests.Single(d => d.FormId == formId);
                    var pageMetadata = GetPageMetadataByPageId(formId, pageId);
                    pageFieldAttributesByPageId[pageId] = pageFieldAttributes = FieldAttributes.MapFieldMetadataToFieldAttributes(pageMetadata, formDigest.CheckCode);
                }
                return pageFieldAttributes;
            }
        }

        public int PageIdFromPageNumber(string formId, int pageNumber)
        {
            PageDigest pageDigest = GetPageDigestByPageNumber(formId, pageNumber);
            var pageId = pageDigest != null ? pageDigest.PageId : 0;
            return pageId;
        }

        public int GetFormPageCount(string formId)
        {
            var pageDigests = GetPageDigests(formId);
            return pageDigests != null ? pageDigests.Count() : 0;
        }

        public int[] GetFormPageIds(string formId)
        {
            var pageDigests = GetPageDigests(formId);
            return pageDigests != null ? pageDigests.Select(pd => pd.PageId).ToArray() : null;
        }

        public FieldDigest GetFieldDigestByFieldName(string formId, string fieldName)
        {
            fieldName = fieldName.ToLower();
            var fieldDigests = GetFieldDigests(formId);
            FieldDigest fieldDigest = null;
            fieldDigest = fieldDigests != null ? fieldDigests.Where(fd => fd.FieldName == fieldName).SingleOrDefault() : null;
            return fieldDigest;
        }

		public FieldDigest[] GetFieldDigestsByFieldNames(string formId, IEnumerable<string> fieldNames)
		{
			formId = formId.ToLower();
			var fieldNameList = fieldNames.Select(n => n.ToLower()).ToArray();
			var fieldDigests = GetFieldDigests(formId).Where(fd => fieldNameList.Contains(fd.FieldName));
			return fieldDigests != null ? fieldDigests.ToArray() : null;
		}

        public Page GetCurrentFormPageMetadataByPageId(int pageId)
        {
            return GetPageMetadataByPageId(_formId, pageId);
        }

        public FormDigest GetCurrentFormDigest()
        {
            return GetFormDigest(_formId);
        }

        public FormDigest GetFormDigest(string formId)
        {
            return FormDigests.SingleOrDefault(f => f.FormId == formId);
        }

        public FormDigest GetFormDigestByFormName(string formName)
        {
            formName = formName.ToLower();
            return FormDigests.SingleOrDefault(f => f.FormName.ToLower() == formName);
        }

        public PageDigest[] GetCurrentFormPageDigests()
        {
            return GetPageDigests(_formId);
        }

        public PageDigest[] GetPageDigests(string formId)
        {
            var pageDigests = PageDigests.SingleOrDefault(d => d[0].FormId == formId);
            return pageDigests;
        }

        public PageDigest GetPageDigestByPageNumber(string formId, int pageNumber)
        {
            var pageDigests = PageDigests.SingleOrDefault(d => d[0].FormId == formId);
            var pageDigest = pageDigests != null ? pageDigests.Single(d => d.PageNumber == pageNumber) : null;
            return pageDigest;
        }

        public PageDigest GetPageDigestByPageId(string formId, int pageId)
        {
            PageDigest pageDigest = null;
            var pageDigests = PageDigests.SingleOrDefault(d => d[0].FormId == formId);
            if (pageDigests != null)
            {
                pageDigest = pageId > 0 ? pageDigests.Single(d => d.PageId == pageId) : pageDigests.FirstOrDefault();
            }
            return pageDigest;
        }

        public FieldDigest[] GetFieldDigestsWithPageNumber(string formId, int pageNumber)
        {
            var pagePosition = pageNumber - 1;
            return GetFieldDigests(formId).Where(d => d.Position == pagePosition).ToArray();
        }

        public FieldDigest[] GetFieldDigestsNotWithPageNumber(string formId, int pageNumber)
        {
            var pagePosition = pageNumber - 1;
            return GetFieldDigests(formId).Where(d => d.Position != pagePosition).ToArray();
        }

        public AbridgedFieldInfo GetFieldInfoByFieldName(string formId, int pageId, string fieldName)
        {
            fieldName = fieldName.ToLower();
            var fieldDigest = GetPageDigestByPageId(formId, pageId);
            var fieldInfo = fieldDigest.Fields.Where(f => f.FieldName == fieldName).SingleOrDefault();
            return fieldInfo;
        }

        public FieldDataType GetFieldDataTypeByFieldName(string formId, int pageId, string fieldName)
        {
            var fieldInfo = GetFieldInfoByFieldName(formId, pageId, fieldName);
            return fieldInfo != null ? fieldInfo.DataType : FieldDataType.Undefined;
        }

        public FieldAttributes GetFieldAttributes(FieldDigest fieldDigest)
        {
            var formId = fieldDigest.FormId;
            var pageId = fieldDigest.PageId;
            var fieldName = fieldDigest.Field.FieldName;
            var fieldAttributes = GetFieldAttributesByPageId(formId, pageId, fieldName);
            return fieldAttributes;
        }

        public FieldAttributes GetFieldAttributesByPageId(string formId, string pageId, string fieldName)
        {
            return GetFieldAttributesByPageId(formId, Convert.ToInt32(pageId), fieldName);
        }

        public FieldAttributes GetFieldAttributesByPageId(string formId, int pageId, string fieldName)
        {
            fieldName = fieldName.ToLower();
            var pageFieldAttributes = GetPageFieldAttributesByPageId(formId, pageId);
            FieldAttributes fieldAttributes = null;
            fieldAttributes = pageFieldAttributes.TryGetValue(fieldName, out fieldAttributes) ? fieldAttributes : null;
            return fieldAttributes;
        }
    }
}
