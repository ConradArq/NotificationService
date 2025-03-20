﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace NotificationService.Shared.Resources {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class ValidationMessages {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ValidationMessages() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("NotificationService.Shared.Resources.ValidationMessages", typeof(ValidationMessages).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Request body cannot be empty..
        /// </summary>
        public static string EmptyRequestBody {
            get {
                return ResourceManager.GetString("EmptyRequestBody", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The field cannot exceed {0} characters..
        /// </summary>
        public static string FieldMaxLengthError {
            get {
                return ResourceManager.GetString("FieldMaxLengthError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The field must be greater than 0..
        /// </summary>
        public static string FieldMustBeGreaterThanZeroError {
            get {
                return ResourceManager.GetString("FieldMustBeGreaterThanZeroError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The field must be a value between {0} and {1}..
        /// </summary>
        public static string FieldRangeError {
            get {
                return ResourceManager.GetString("FieldRangeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be a valid date in the format yyyy-MM-dd..
        /// </summary>
        public static string InvalidDateError {
            get {
                return ResourceManager.GetString("InvalidDateError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be a valid decimal number..
        /// </summary>
        public static string InvalidDecimalError {
            get {
                return ResourceManager.GetString("InvalidDecimalError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be a valid email address..
        /// </summary>
        public static string InvalidEmailError {
            get {
                return ResourceManager.GetString("InvalidEmailError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The following email addresses are invalid: {0}.
        /// </summary>
        public static string InvalidEmailListError {
            get {
                return ResourceManager.GetString("InvalidEmailListError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to If the subject or body is not provided, the template ID must be provided..
        /// </summary>
        public static string InvalidEmailTemplateError {
            get {
                return ResourceManager.GetString("InvalidEmailTemplateError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be a valid {0}. Valid values are: {1}..
        /// </summary>
        public static string InvalidEnumError {
            get {
                return ResourceManager.GetString("InvalidEnumError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The field is not valid..
        /// </summary>
        public static string InvalidFieldError {
            get {
                return ResourceManager.GetString("InvalidFieldError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid form field format. Original Message: {0}.
        /// </summary>
        public static string InvalidFormFieldFormat {
            get {
                return ResourceManager.GetString("InvalidFormFieldFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be a valid integer..
        /// </summary>
        public static string InvalidIntegerError {
            get {
                return ResourceManager.GetString("InvalidIntegerError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The value must be a comma-separated list of integers..
        /// </summary>
        public static string InvalidIntegerListError {
            get {
                return ResourceManager.GetString("InvalidIntegerListError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid JSON format. Original Message: {0}.
        /// </summary>
        public static string InvalidJsonFormat {
            get {
                return ResourceManager.GetString("InvalidJsonFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Invalid JSON property format. Original Message: {0}.
        /// </summary>
        public static string InvalidJsonPropertyFormat {
            get {
                return ResourceManager.GetString("InvalidJsonPropertyFormat", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The query parameter &apos;{0}&apos; is not a valid value for the type &apos;{1}&apos;..
        /// </summary>
        public static string InvalidQueryParameter {
            get {
                return ResourceManager.GetString("InvalidQueryParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The route parameter &apos;{0}&apos; is not a valid value for the type &apos;{1}&apos;..
        /// </summary>
        public static string InvalidRouteParameter {
            get {
                return ResourceManager.GetString("InvalidRouteParameter", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to bind model for the given input..
        /// </summary>
        public static string ModelBindingError {
            get {
                return ResourceManager.GetString("ModelBindingError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to At least one recipient must be provided: UserId, RoleId, or To..
        /// </summary>
        public static string RequiredEmailNotificationRecipientError {
            get {
                return ResourceManager.GetString("RequiredEmailNotificationRecipientError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The field is required and cannot be empty..
        /// </summary>
        public static string RequiredFieldError {
            get {
                return ResourceManager.GetString("RequiredFieldError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to At least one recipient must be provided: UserId or RoleId..
        /// </summary>
        public static string RequiredPushNotificationRecipientError {
            get {
                return ResourceManager.GetString("RequiredPushNotificationRecipientError", resourceCulture);
            }
        }
    }
}
