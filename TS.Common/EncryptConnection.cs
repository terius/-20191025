using System;
using System.Collections.Generic;
using System.Configuration;

using System.Text;


namespace TS.Common
{
    public static class EncryptConnection
    {
        public static void EncryptConnectionString(bool encrypt)
        {
            Configuration configFile = null;
            try
            {

                ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap();
                fileMap.ExeConfigFilename = "GZTranService.exe.config"; 

                // Open the configuration file and retrieve the connectionStrings section.
                configFile = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None); //ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                ConnectionStringsSection configSection = configFile.GetSection("connectionStrings") as ConnectionStringsSection;

                if ((!(configSection.ElementInformation.IsLocked)) && (!(configSection.SectionInformation.IsLocked)))
                {
                    if (encrypt && !configSection.SectionInformation.IsProtected)//encrypt is false to unencrypt
                    {
                        configSection.SectionInformation.ProtectSection("DataProtectionConfigurationProvider");
                    }

                    if (!encrypt && configSection.SectionInformation.IsProtected)//encrypt is true so encrypt
                    {
                        configSection.SectionInformation.UnprotectSection();
                    }

                    //re-save the configuration file section
                    configSection.SectionInformation.ForceSave = true;

                    // Save the current configuration.
                    configFile.Save();
                }
            }
            catch (System.Exception ex)
            {
                throw (ex);
            }
            finally
            {
            }
        }
    }
}
