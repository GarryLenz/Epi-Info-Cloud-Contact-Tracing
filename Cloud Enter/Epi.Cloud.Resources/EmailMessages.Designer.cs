﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Epi.Cloud.Resources.EmailMessages {
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
    internal class EmailMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal EmailMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Epi.Cloud.Resources.EmailMessages", typeof(EmailMessages).Assembly);
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
        ///   Looks up a localized string similar to This is a test.
        /// </summary>
        internal static string AMessageForUnitTests {
            get {
                return ResourceManager.GetString("AMessageForUnitTests", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to An Epi Info Cloud Enter account has been created for your organization..
        /// </summary>
        internal static string InsertUser_Subject {
            get {
                return ResourceManager.GetString("InsertUser_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You recently updated your password for Epi Info™ Cloud Enter. \n \n If you have not accessed password help, please contact the administrator for you organization. \n \n.
        /// </summary>
        internal static string PasswordChanged_Body {
            get {
                return ResourceManager.GetString("PasswordChanged_Body", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your Epi Info Cloud Enter Password has been updated.
        /// </summary>
        internal static string PasswordChanged_Subject {
            get {
                return ResourceManager.GetString("PasswordChanged_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You recently accessed our Forgot Password service for Epi Info™ Cloud Enter. \n \n Your new temporary password is: {0}\n \n If you have not accessed password help, please contact the administrator. \n \nLog in with your temporary password. You will then be asked to create a new password..
        /// </summary>
        internal static string ResetPassword_Body {
            get {
                return ResourceManager.GetString("ResetPassword_Body", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your Epi Info Cloud Enter Password.
        /// </summary>
        internal static string ResetPassword_Subject {
            get {
                return ResourceManager.GetString("ResetPassword_Subject", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You account info has been updated in Epi Info™ Cloud Enter system..
        /// </summary>
        internal static string UpdateUserInfo_Body {
            get {
                return ResourceManager.GetString("UpdateUserInfo_Body", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Your Epi Info Cloud Enter Account info has been updated.
        /// </summary>
        internal static string UpdateUserInfo_Subject {
            get {
                return ResourceManager.GetString("UpdateUserInfo_Subject", resourceCulture);
            }
        }
    }
}