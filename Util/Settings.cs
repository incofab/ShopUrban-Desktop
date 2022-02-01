using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ShopUrban.Util
{
    [Obsolete("This file is not used")]
    class Settings
    {
        public const string KEY_LAST_ANNOUNCEMENT_CHECK = "last_announcement_check";
        public const string KEY_LAST_ANNOUNCEMENT_ID = "last_announcement_id";

        // Used by settings file
        public readonly static string BASE_FOLDER2 = Environment.GetFolderPath(
            Environment.SpecialFolder.CommonApplicationData)
            +"/app/";

        public readonly static string BASE_FOLDER = Path.GetDirectoryName(
            Assembly.GetEntryAssembly().Location)+@"\Assets\";

        public readonly static string UTME_BASE_FOLDER = BASE_FOLDER + @"Content\utme\";
        public readonly static string UTME_QUESTIONS_FOLDER = UTME_BASE_FOLDER + @"questions";
        public readonly static string UTME_IMAGES_FOLDER = UTME_BASE_FOLDER + @"images";

        public readonly static string LITERATURE_BASE_FOLDER = BASE_FOLDER + @"Content\utme_literature\";
        public readonly static string LITERATURE_QUESTIONS_FOLDER = LITERATURE_BASE_FOLDER + @"questions";
        public readonly static string LITERATURE_IMAGES_FOLDER = LITERATURE_BASE_FOLDER + @"images";

        public readonly static string OTHER_CONTENTS_FOLDER = BASE_FOLDER + @"Content\others\";
        public readonly static string OTHER_CONTENTS_FOLDER_JAMB_SYLLABUS = OTHER_CONTENTS_FOLDER + @"Jamb Syllabus";
        public readonly static string OTHER_CONTENTS_FOLDER_LITERATURE = OTHER_CONTENTS_FOLDER + @"Literature Content";
        public readonly static string OTHER_CONTENTS_FOLDER_STUDY_MATERIALS = OTHER_CONTENTS_FOLDER + @"study materials";
        public readonly static string OTHER_CONTENTS_FOLDER_TLC = OTHER_CONTENTS_FOLDER + @"The Life Changer";

        private readonly static string FILE_NAME = BASE_FOLDER2 + Helpers.md5("settings.txt");

        private const string KEY_ACTIVATION_KEY = "activation_key";
        private const string KEY_IS_EXTRACTED = "is_extracted";

        private JObject settingsData = new JObject();

        public Settings()
        {
            if(!Directory.Exists(BASE_FOLDER2))
            {
                Directory.CreateDirectory(BASE_FOLDER2);
            }

            if (!File.Exists(FILE_NAME))
            {
                this.saveSettings();
            }

            loadSettings();
        }

        private static Settings settingsObj;
        public static Settings getInstance()
        {
            if (settingsObj == null)
            {
                settingsObj = new Settings();
            }
            return settingsObj;
        }

        public JObject getSettingsData() { return settingsData; }

        private void loadSettings()
        {
            string data = File.ReadAllText(FILE_NAME);
            
            if (data == null) return;

            settingsData = JObject.Parse(data);
        }

        public void saveSettings()
        {
            if (settingsData == null) return;
            
            File.WriteAllText(FILE_NAME, settingsData.ToString());
        }

        public void setActivationKey(string activationKey)
        {
            settingsData[KEY_ACTIVATION_KEY] = activationKey;
            //settingsData.Add(KEY_ACTIVATION_KEY, activationKey);
        }

        public void setIsExtracted(bool isExtracted)
        {
            settingsData[KEY_IS_EXTRACTED] = isExtracted;
            //settingsData.Add(KEY_IS_EXTRACTED, isExtracted);
        }

        public string getActivationKey()
        {
            if (settingsData.ContainsKey(KEY_ACTIVATION_KEY))
            {
                return settingsData.GetValue(KEY_ACTIVATION_KEY).ToString();
            }
            return "";
        }
        public bool isExtracted()
        {
            if (settingsData.ContainsKey(KEY_IS_EXTRACTED) 
                && Directory.Exists(UTME_BASE_FOLDER))
            {
                return (bool)settingsData.GetValue(KEY_IS_EXTRACTED);
            }
            return false;
        }


    }
}
