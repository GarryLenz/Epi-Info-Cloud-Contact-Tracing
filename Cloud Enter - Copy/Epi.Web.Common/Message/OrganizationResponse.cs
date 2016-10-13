﻿using System.Collections.Generic;
using Epi.Web.Enter.Common.DTO;
using System.Runtime.Serialization;

namespace Epi.Web.Enter.Common.Message
{
    [DataContract(Namespace = "http://www.yourcompany.com/types/")]
    public class OrganizationResponse : Epi.Web.Enter.Common.MessageBase.ResponseBase
    {

        /// <summary>
        /// Default Constructor for OrganizationResponse.
        /// </summary>
        public OrganizationResponse() { }

        /// <summary>
        /// Overloaded Constructor for OrganizationResponse. Sets CorrelationId.
        /// </summary>
        /// <param name="correlationId"></param>
        public OrganizationResponse(string correlationId) : base(correlationId) { }

        /// <summary>
        /// OrganizationInfo object.
        /// </summary>
        [DataMember]
        public List<OrganizationDTO> OrganizationList;

        [DataMember]
        public List<UserDTO> OrganizationUsersList;
    }
}