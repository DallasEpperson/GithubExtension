using System;
using System.Configuration;

namespace GithubExtension
{
    public class GithubExtensionSettings : ApplicationSettingsBase
    {
        [UserScopedSetting()]
        [DefaultSettingValue("")]
        public string GitExeLocation
        {
            get
            {
                return (string)this["GitExeLocation"];
            }
            set
            {
                this["GitExeLocation"] = (string)value;
            }
        }
    }
}
