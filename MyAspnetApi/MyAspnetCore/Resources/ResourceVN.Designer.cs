﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MyAspnetCore.Resources {
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
    public class ResourceVN {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal ResourceVN() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("MyAspnetCore.Resources.ResourceVN", typeof(ResourceVN).Assembly);
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
        ///   Looks up a localized string similar to Email đã tồn tại.
        /// </summary>
        public static string EmailExist {
            get {
                return ResourceManager.GetString("EmailExist", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email không được để trống.
        /// </summary>
        public static string EmailNotEmpty {
            get {
                return ResourceManager.GetString("EmailNotEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Có lỗi xảy ra vui lòng liên hệ HUST để được trợ giúp.
        /// </summary>
        public static string Err_Exception {
            get {
                return ResourceManager.GetString("Err_Exception", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Lỗi từ hẹ thống.
        /// </summary>
        public static string Err_Msg_System {
            get {
                return ResourceManager.GetString("Err_Msg_System", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Không tìm thấy.
        /// </summary>
        public static string Err_NotFound {
            get {
                return ResourceManager.GetString("Err_NotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dữ liệu đầu vào không hợp lệ.
        /// </summary>
        public static string Err_ValidateData {
            get {
                return ResourceManager.GetString("Err_ValidateData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ngày sing không hợp lệ.
        /// </summary>
        public static string InvalidDob {
            get {
                return ResourceManager.GetString("InvalidDob", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Email không hợp lệ.
        /// </summary>
        public static string InvalidEmail {
            get {
                return ResourceManager.GetString("InvalidEmail", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tài khoản hoặc mật khẩu không chính xác.
        /// </summary>
        public static string NotFoundAcc {
            get {
                return ResourceManager.GetString("NotFoundAcc", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Mật khẩu không được để trống.
        /// </summary>
        public static string PasswordNotEmpty {
            get {
                return ResourceManager.GetString("PasswordNotEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tên không được để trống.
        /// </summary>
        public static string UserNameNotEmpty {
            get {
                return ResourceManager.GetString("UserNameNotEmpty", resourceCulture);
            }
        }
    }
}
