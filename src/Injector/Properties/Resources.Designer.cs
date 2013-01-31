﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ZeroInstall.Injector.Properties {
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
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("ZeroInstall.Injector.Properties.Resources", typeof(Resources).Assembly);
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
        ///   Looks up a localized string similar to Feed: {0}
        ///- Valid signature from {1}
        ///- {2}
        ///Do you want to trust this key to sign feeds from &apos;{3}&apos;?
        ///.
        /// </summary>
        internal static string AskKeyTrust {
            get {
                return ResourceManager.GetString("AskKeyTrust", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to {0} bindings are currently not supported on this operating system..
        /// </summary>
        internal static string BindingNotSupportedOnCurrentOS {
            get {
                return ResourceManager.GetString("BindingNotSupportedOnCurrentOS", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to &lt;environment&gt; bindings must contain either a &apos;value&apos; or an &apos;insert&apos; attribute..
        /// </summary>
        internal static string EnvironmentBindingValueInvalid {
            get {
                return ResourceManager.GetString("EnvironmentBindingValueInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error loading feed preferences for &apos;{0}&apos;. Reverting to default values..
        /// </summary>
        internal static string ErrorLoadingFeedPrefs {
            get {
                return ResourceManager.GetString("ErrorLoadingFeedPrefs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error loading interface preferences for &apos;{0}&apos;. Reverting to default values..
        /// </summary>
        internal static string ErrorLoadingInterfacePrefs {
            get {
                return ResourceManager.GetString("ErrorLoadingInterfacePrefs", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error loading the trust database. Reverting to default values..
        /// </summary>
        internal static string ErrorLoadingTrustDB {
            get {
                return ResourceManager.GetString("ErrorLoadingTrustDB", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The output of the external solver could not be processed..
        /// </summary>
        internal static string ExternalSolverOutputErrror {
            get {
                return ResourceManager.GetString("ExternalSolverOutputErrror", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Error while downloading feed &apos;{0}&apos;:
        ///{1}
        ///Switching to mirror..
        /// </summary>
        internal static string FeedDownloadErrorSwitchToMirror {
            get {
                return ResourceManager.GetString("FeedDownloadErrorSwitchToMirror", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The feed &apos;{0}&apos; is not cached and Zero Install is currently in off-line mode..
        /// </summary>
        internal static string FeedNotCachedOffline {
            get {
                return ResourceManager.GetString("FeedNotCachedOffline", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The feed &apos;{0}&apos; was not signed with any trusted keys..
        /// </summary>
        internal static string FeedNoTrustedSignatures {
            get {
                return ResourceManager.GetString("FeedNoTrustedSignatures", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &lt;interface&gt; uri attribute (&apos;{0}&apos;) does not match the URL the feed was downloaded from (&apos;{1}&apos;)..
        /// </summary>
        internal static string FeedUriMismatch {
            get {
                return ResourceManager.GetString("FeedUriMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The &lt;interface&gt; uri attribute missing. Should be &apos;{0}&apos;..
        /// </summary>
        internal static string FeedUriMissing {
            get {
                return ResourceManager.GetString("FeedUriMissing", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to File not found: {0}.
        /// </summary>
        internal static string FileNotFound {
            get {
                return ResourceManager.GetString("FileNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Illegal character in {0} binding name..
        /// </summary>
        internal static string IllegalCharInBindingName {
            get {
                return ResourceManager.GetString("IllegalCharInBindingName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to find implementation for interface &apos;{0}&apos; in the selection..
        /// </summary>
        internal static string ImplementationNotInSelection {
            get {
                return ResourceManager.GetString("ImplementationNotInSelection", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Missing name in {0} binding..
        /// </summary>
        internal static string MissingBindingName {
            get {
                return ResourceManager.GetString("MissingBindingName", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No interface was specified..
        /// </summary>
        internal static string MissingInterfaceID {
            get {
                return ResourceManager.GetString("MissingInterfaceID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The graphical policy editor is not available in command-line mode..
        /// </summary>
        internal static string NoAuditInCli {
            get {
                return ResourceManager.GetString("NoAuditInCli", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to At least one implementation must be passed..
        /// </summary>
        internal static string NoImplementationsPassed {
            get {
                return ResourceManager.GetString("NoImplementationsPassed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The key information server was unable to provide any additional information about this key..
        /// </summary>
        internal static string NoKeyInfoServerData {
            get {
                return ResourceManager.GetString("NoKeyInfoServerData", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to No selected version.
        /// </summary>
        internal static string NoSelectedVersion {
            get {
                return ResourceManager.GetString("NoSelectedVersion", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to (not cached).
        /// </summary>
        internal static string NotCached {
            get {
                return ResourceManager.GetString("NotCached", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The interface &apos;{0}&apos; doesn&apos;t start with &apos;http:&apos; and doesn&apos;t exist as a file &apos;{1}&apos; either..
        /// </summary>
        internal static string NotInterfaceID {
            get {
                return ResourceManager.GetString("NotInterfaceID", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was a problem loading a configuration file.
        ///You can delete the file &apos;{0}&apos; to fix the problem..
        /// </summary>
        internal static string ProblemLoadingConfigFile {
            get {
                return ResourceManager.GetString("ProblemLoadingConfigFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was a problem loading the configuration value &apos;{0}&apos;.
        ///You can delete the file &apos;{1}&apos; to fix the problem. Other settings may also be lost..
        /// </summary>
        internal static string ProblemLoadingConfigValue {
            get {
                return ResourceManager.GetString("ProblemLoadingConfigValue", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to A possible replay attack was detected. The new feed&apos;s modification time is before old version.
        ///Feed ID: {0}
        ///Old time: {1}
        ///New time: {2}.
        /// </summary>
        internal static string ReplayAttack {
            get {
                return ResourceManager.GetString("ReplayAttack", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Buggy implementation.
        /// </summary>
        internal static string SelectionCandidateNoteBuggy {
            get {
                return ResourceManager.GetString("SelectionCandidateNoteBuggy", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Incompatible operating system or CPU.
        /// </summary>
        internal static string SelectionCandidateNoteIncompatibleArchitecture {
            get {
                return ResourceManager.GetString("SelectionCandidateNoteIncompatibleArchitecture", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Insecure implementation.
        /// </summary>
        internal static string SelectionCandidateNoteInsecure {
            get {
                return ResourceManager.GetString("SelectionCandidateNoteInsecure", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Implementation not in cache (currently in off-line mode).
        /// </summary>
        internal static string SelectionCandidateNoteNotCached {
            get {
                return ResourceManager.GetString("SelectionCandidateNoteNotCached", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Non-executable source implementation.
        /// </summary>
        internal static string SelectionCandidateNoteSource {
            get {
                return ResourceManager.GetString("SelectionCandidateNoteSource", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Implementation version too old or too new.
        /// </summary>
        internal static string SelectionCandidateNoteVersionMismatch {
            get {
                return ResourceManager.GetString("SelectionCandidateNoteVersionMismatch", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The solver encountered an unexpected problem..
        /// </summary>
        internal static string SolverProblem {
            get {
                return ResourceManager.GetString("SolverProblem", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Try using the command &apos;0install-win&apos; instead of &apos;0install&apos;..
        /// </summary>
        internal static string Try0installWin {
            get {
                return ResourceManager.GetString("Try0installWin", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to cache the downloaded application catalog..
        /// </summary>
        internal static string UnableToCacheCatalog {
            get {
                return ResourceManager.GetString("UnableToCacheCatalog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to load cached application catalog from disk..
        /// </summary>
        internal static string UnableToLoadCachedCatalog {
            get {
                return ResourceManager.GetString("UnableToLoadCachedCatalog", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to download GnuPG key file for &apos;{0}&apos;..
        /// </summary>
        internal static string UnableToLoadKeyFile {
            get {
                return ResourceManager.GetString("UnableToLoadKeyFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to parse key information for &apos;{0}&apos;..
        /// </summary>
        internal static string UnableToParseKeyInfo {
            get {
                return ResourceManager.GetString("UnableToParseKeyInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Unable to retrieve key information for &apos;{0}&apos;..
        /// </summary>
        internal static string UnableToRetrieveKeyInfo {
            get {
                return ResourceManager.GetString("UnableToRetrieveKeyInfo", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The feed is signed with untrusted keys!.
        /// </summary>
        internal static string UntrustedKeys {
            get {
                return ResourceManager.GetString("UntrustedKeys", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The working directory has already been changed by a previous command..
        /// </summary>
        internal static string WokringDirDuplicate {
            get {
                return ResourceManager.GetString("WokringDirDuplicate", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The working directory contains an invalid path (potentially a security risk)..
        /// </summary>
        internal static string WorkingDirInvalidPath {
            get {
                return ResourceManager.GetString("WorkingDirInvalidPath", resourceCulture);
            }
        }
    }
}
