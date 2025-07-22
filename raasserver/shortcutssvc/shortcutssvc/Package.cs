/* Copyright (c) 2025 Elbitin
 *
 * This file is part of RAAS Server.
 *
 * RAAS Server is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 *
 * RAAS Server is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License along with RAAS Server. If not, see <https://www.gnu.org/licenses/>.
 */
﻿using Elbitin.Applications.RAAS.Common.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Elbitin.Applications.RAAS.RAASServer.ShortcutsSvc
{
    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10", IsNullable = false)]
    public partial class Package
    {
        public static Package DeserializeXml(String xmlData)
        {
            return (Package)SerializationHelper.DeserializeXml(xmlData, typeof(Package));
        }

        public static Package DeserializeXmlFileWithRetries(String xmlPath)
        {
            return (Package)SerializationHelper.DeserializeXmlFileWithRetries(xmlPath, typeof(Package));
        }

        public string SerializeXml()
        {
            return SerializationHelper.SerializeXml(typeof(Package), this);
        }

        public bool SerializeXmlFile(string packageXmlPath)
        {
            return SerializationHelper.SerializeXmlFile(packageXmlPath, typeof(Package), this);
        }


        private PackageIdentity identityField;

        private PackageProperties propertiesField;

        private PackageDependencies dependenciesField;

        private PackageCapabilities capabilitiesField;

        private PackageResources resourcesField;

        private PackageApplication[] applicationsField;

        private string ignorableNamespacesField;

        /// <remarks/>
        public PackageIdentity Identity
        {
            get
            {
                return this.identityField;
            }
            set
            {
                this.identityField = value;
            }
        }

        /// <remarks/>
        public PackageProperties Properties
        {
            get
            {
                return this.propertiesField;
            }
            set
            {
                this.propertiesField = value;
            }
        }

        /// <remarks/>
        public PackageDependencies Dependencies
        {
            get
            {
                return this.dependenciesField;
            }
            set
            {
                this.dependenciesField = value;
            }
        }

        /// <remarks/>
        public PackageCapabilities Capabilities
        {
            get
            {
                return this.capabilitiesField;
            }
            set
            {
                this.capabilitiesField = value;
            }
        }

        /// <remarks/>
        public PackageResources Resources
        {
            get
            {
                return this.resourcesField;
            }
            set
            {
                this.resourcesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Application", IsNullable = false)]
        public PackageApplication[] Applications
        {
            get
            {
                return this.applicationsField;
            }
            set
            {
                this.applicationsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string IgnorableNamespaces
        {
            get
            {
                return this.ignorableNamespacesField;
            }
            set
            {
                this.ignorableNamespacesField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public partial class PackageIdentity
    {

        private string nameField;

        private string publisherField;

        private string versionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Publisher
        {
            get
            {
                return this.publisherField;
            }
            set
            {
                this.publisherField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Version
        {
            get
            {
                return this.versionField;
            }
            set
            {
                this.versionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public partial class PackageProperties
    {

        private string displayNameField;

        private string publisherDisplayNameField;

        private string logoField;

        private string supportedUsersField;

        /// <remarks/>
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        public string PublisherDisplayName
        {
            get
            {
                return this.publisherDisplayNameField;
            }
            set
            {
                this.publisherDisplayNameField = value;
            }
        }

        /// <remarks/>
        public string Logo
        {
            get
            {
                return this.logoField;
            }
            set
            {
                this.logoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10")]
        public string SupportedUsers
        {
            get
            {
                return this.supportedUsersField;
            }
            set
            {
                this.supportedUsersField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public partial class PackageDependencies
    {

        private PackageDependenciesTargetDeviceFamily targetDeviceFamilyField;

        /// <remarks/>
        public PackageDependenciesTargetDeviceFamily TargetDeviceFamily
        {
            get
            {
                return this.targetDeviceFamilyField;
            }
            set
            {
                this.targetDeviceFamilyField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public partial class PackageDependenciesTargetDeviceFamily
    {

        private string nameField;

        private string minVersionField;

        private string maxVersionTestedField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MinVersion
        {
            get
            {
                return this.minVersionField;
            }
            set
            {
                this.minVersionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string MaxVersionTested
        {
            get
            {
                return this.maxVersionTestedField;
            }
            set
            {
                this.maxVersionTestedField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public partial class PackageCapabilities
    {

        private Capability capabilityField;

        private Capability1[] capability1Field;

        private PackageCapabilitiesDeviceCapability[] deviceCapabilityField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10/windowscapabiliti" +
            "es")]
        public Capability Capability
        {
            get
            {
                return this.capabilityField;
            }
            set
            {
                this.capabilityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("Capability", Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabil" +
            "ities")]
        public Capability1[] Capability1
        {
            get
            {
                return this.capability1Field;
            }
            set
            {
                this.capability1Field = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("DeviceCapability")]
        public PackageCapabilitiesDeviceCapability[] DeviceCapability
        {
            get
            {
                return this.deviceCapabilityField;
            }
            set
            {
                this.deviceCapabilityField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10/windowscapabiliti" +
        "es")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10/windowscapabiliti" +
        "es", IsNullable = false)]
    public partial class Capability
    {

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabil" +
        "ities")]
    [System.Xml.Serialization.XmlRootAttribute("Capability", Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabil" +
        "ities", IsNullable = false)]
    public partial class Capability1
    {

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public partial class PackageCapabilitiesDeviceCapability
    {

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public partial class PackageResources
    {

        private PackageResourcesResource resourceField;

        /// <remarks/>
        public PackageResourcesResource Resource
        {
            get
            {
                return this.resourceField;
            }
            set
            {
                this.resourceField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public partial class PackageResourcesResource
    {

        private string languageField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Language
        {
            get
            {
                return this.languageField;
            }
            set
            {
                this.languageField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public partial class PackageApplication
    {

        private VisualElements visualElementsField;

        private PackageApplicationExtensions extensionsField;

        private string idField;

        private string executableField;

        private string entryPointField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10")]
        public VisualElements VisualElements
        {
            get
            {
                return this.visualElementsField;
            }
            set
            {
                this.visualElementsField = value;
            }
        }

        /// <remarks/>
        public PackageApplicationExtensions Extensions
        {
            get
            {
                return this.extensionsField;
            }
            set
            {
                this.extensionsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Id
        {
            get
            {
                return this.idField;
            }
            set
            {
                this.idField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Executable
        {
            get
            {
                return this.executableField;
            }
            set
            {
                this.executableField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string EntryPoint
        {
            get
            {
                return this.entryPointField;
            }
            set
            {
                this.entryPointField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10", IsNullable = false)]
    public partial class VisualElements
    {

        private VisualElementsSplashScreen splashScreenField;

        private string appListEntryField;

        private string displayNameField;

        private string square150x150LogoField;

        private string square44x44LogoField;

        private string descriptionField;

        private string backgroundColorField;

        /// <remarks/>
        public VisualElementsSplashScreen SplashScreen
        {
            get
            {
                return this.splashScreenField;
            }
            set
            {
                this.splashScreenField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string AppListEntry
        {
            get
            {
                return this.appListEntryField;
            }
            set
            {
                this.appListEntryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Square150x150Logo
        {
            get
            {
                return this.square150x150LogoField;
            }
            set
            {
                this.square150x150LogoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Square44x44Logo
        {
            get
            {
                return this.square44x44LogoField;
            }
            set
            {
                this.square44x44LogoField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BackgroundColor
        {
            get
            {
                return this.backgroundColorField;
            }
            set
            {
                this.backgroundColorField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10")]
    public partial class VisualElementsSplashScreen
    {

        private string imageField;

        private bool optionalField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Image
        {
            get
            {
                return this.imageField;
            }
            set
            {
                this.imageField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(Form = System.Xml.Schema.XmlSchemaForm.Qualified, Namespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10/5")]
        public bool Optional
        {
            get
            {
                return this.optionalField;
            }
            set
            {
                this.optionalField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10")]
    public partial class PackageApplicationExtensions
    {

        private Extension extensionField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Namespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10")]
        public Extension Extension
        {
            get
            {
                return this.extensionField;
            }
            set
            {
                this.extensionField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10", IsNullable = false)]
    public partial class Extension
    {

        private ExtensionProtocol protocolField;

        private string categoryField;

        /// <remarks/>
        public ExtensionProtocol Protocol
        {
            get
            {
                return this.protocolField;
            }
            set
            {
                this.protocolField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Category
        {
            get
            {
                return this.categoryField;
            }
            set
            {
                this.categoryField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10")]
    public partial class ExtensionProtocol
    {

        private string displayNameField;

        private string nameField;

        private string returnResultsField;

        /// <remarks/>
        public string DisplayName
        {
            get
            {
                return this.displayNameField;
            }
            set
            {
                this.displayNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ReturnResults
        {
            get
            {
                return this.returnResultsField;
            }
            set
            {
                this.returnResultsField = value;
            }
        }
    }


}
