﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Epi.Cloud.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class CosmosDBSp {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal CosmosDBSp() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Epi.Cloud.Resources.CosmosDBSp", typeof(CosmosDBSp).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function getGridContent(query, sortKey, isSortAscending, continuationToken, skip) {
        ///    var context = getContext();
        ///    var response = context.getResponse();
        ///    var collection = context.getCollection();
        ///    var collectionLink = collection.getSelfLink();
        ///    var responses = [];
        ///    var nextContinuationToken;
        ///    var currentContinuationToken;
        ///    var responseSize = 0;
        ///    var isMaxSizeReached = false;
        ///
        ///    if (!sortKey) {
        ///        sortkey = &quot;_ts&quot;;
        ///        isSortAscending = false;
        ///    }
        ///
        ///    var trace = &quot;query [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string getGridContent {
            get {
                return ResourceManager.GetString("getGridContent", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function SharingRules(ruleId, isHostOrgUser, userOrgId, responseOrgId)
        ///{
        ///    switch (ruleId)
        ///    {
        ///        case 1:
        ///        default:
        ///            // Organization users can only access the data of there organization.
        ///            return userOrgId == responseOrgId;
        ///
        ///        case 2:
        ///            // All users in host organization will have access to all data of all organizations
        ///            // and other Organization users can only access the data of there organization.
        ///            return isHostOrgUser || [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string udfSharingRules {
            get {
                return ResourceManager.GetString("udfSharingRules", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to function WildCardCompare(input, pattern, singleWildcard = &apos;?&apos;, multipleWildcard = &apos;*&apos;)
        ///{
        ///    input = input.toLowerCase();
        ///    pattern = pattern.toLowerCase();
        ///
        ///    var inputLength = input.length;
        ///    var patternLength = pattern.length;
        ///
        ///    // Stack containing input positions that should be tested for further matching
        ///    //var inputPosStack = new int[(input.Length + 1) * (pattern.Length + 1)];
        ///    var inputPosStack = [];
        ///
        ///    // Stack containing pattern positions that should be tested for furth [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string udfWildCardCompare {
            get {
                return ResourceManager.GetString("udfWildCardCompare", resourceCulture);
            }
        }
    }
}
