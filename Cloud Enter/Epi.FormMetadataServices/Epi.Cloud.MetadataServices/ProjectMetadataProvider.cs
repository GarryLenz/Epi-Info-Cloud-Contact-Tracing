﻿using System.Collections.Generic;
using Epi.Cloud.MetadataServices.DataTypes;
using Epi.Cloud.MetadataServices.ProxiesService;

namespace Epi.Cloud.MetadataServices
{
    public class ProjectMetadataProvider
    {
        //Pass the page id and call the DBAccess API and get the project fileds.
        public List<MetadataFieldAttributes> GetProxy(string pageid)
        {
            FieldAttributeServiceProxy fieldat = new FieldAttributeServiceProxy();
            List<MetadataFieldAttributes> fieldattributes = new List<MetadataFieldAttributes>();
            var task = fieldat.GetProjectMetadataAsync(pageid);
            return task.Result;
        }
    }
}
