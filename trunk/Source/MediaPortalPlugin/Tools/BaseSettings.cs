using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace PandoraMusicBox.MediaPortalPlugin.Tools {
    internal class SettingAttribute : Attribute { }

    internal class BaseSettings {
        public string FileName {
            get;
            protected set;
        }

        public string Namespace {
            get;
            protected set;
        }

        public void LoadSettings() {
            try {
                using (MediaPortal.Profile.Settings xmlreader = new MediaPortal.Profile.Settings(GetFileFullPath())) {
                    // loop through each property in the class
                    foreach (PropertyInfo currProperty in GetType().GetProperties())
                        foreach (object currAttr in currProperty.GetCustomAttributes(true))
                            if (currAttr.GetType() == typeof(SettingAttribute)) {
                                // we have found a property with the SettingAttribute applied
                                if (currProperty.PropertyType == typeof(string)) {
                                    string value = xmlreader.GetValueAsString(Namespace, currProperty.Name, "");
                                    currProperty.GetSetMethod().Invoke(this, new object[] { value });
                                }

                                break;
                            }
                }
            }
            catch (Exception) {
            }
        }

        public void SaveSettings() {
            try {
                using (MediaPortal.Profile.Settings xmlwriter = new MediaPortal.Profile.Settings(GetFileFullPath(), false)) {
                    // loop through each property in the class
                    foreach (PropertyInfo currProperty in GetType().GetProperties())
                        foreach (object currAttr in currProperty.GetCustomAttributes(true))
                            if (currAttr.GetType() == typeof(SettingAttribute)) {
                                // we have found a property with the SettingAttribute applied
                                if (currProperty.PropertyType == typeof(string)) {
                                    object value = currProperty.GetGetMethod().Invoke(this, new object[] { });
                                    xmlwriter.SetValue(Namespace, currProperty.Name, value);
                                }

                                break;
                            }
                }
            }
            catch (Exception) {

            }
        }

        private string GetFileFullPath() {
            return MediaPortal.Configuration.Config.GetFile(MediaPortal.Configuration.Config.Dir.Config, FileName);
        }
    }
}
