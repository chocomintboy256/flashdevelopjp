using System;
using System.ComponentModel;
using PluginCore.Localization;
using PluginCore.Managers;

namespace FDjpPlugin.Resources
{
    [AttributeUsage(AttributeTargets.All)]
    public class LocalizedCategoryAttribute : CategoryAttribute
    {
        public LocalizedCategoryAttribute(String key) : base(key) { }

        /// <summary>
        /// Gets the localized string
        /// </summary>
        protected override String GetLocalizedString(String key)
        {
            return LocaleHelper.GetString(key);
        }

    }

    [AttributeUsage(AttributeTargets.All)]
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public LocalizedDisplayNameAttribute(String key) : base(key) { }

        /// <summary>
        /// Gets the localized string
        /// </summary>
        public override string DisplayName
        {
            get
            {
                String key = base.DisplayName;
                DisplayNameValue = LocaleHelper.GetString(key);
                if (DisplayNameValue == null) DisplayNameValue = String.Empty;
                TraceManager.Add(key + ":" + DisplayNameValue);
                return DisplayNameValue;
            }
        }

    }

    [AttributeUsage(AttributeTargets.All)]
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        private Boolean initialized = false;

        public LocalizedDescriptionAttribute(String key) : base(key) { }

        /// <summary>
        /// Gets the description of the string
        /// </summary>
        public override string Description
        {
            get
            {
                if (!initialized)
                {
                    String key = base.Description;
                    DescriptionValue = LocaleHelper.GetString(key);
                    if (DescriptionValue == null) DescriptionValue = String.Empty;
                    initialized = true;
                }
                return DescriptionValue;
            }
        }

    }

    [AttributeUsage(AttributeTargets.All)]
    public class StringValueAttribute : Attribute
    {
        private String value;

        public StringValueAttribute(String value)
        {
            this.value = value;
        }

        /// <summary>
        /// Gets the string value of the class
        /// </summary>
        public String Value
        {
            get { return this.value; }
        }

    }

}