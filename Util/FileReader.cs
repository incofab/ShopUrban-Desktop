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
    class FileReader
    {
        public static readonly string APP_NAMESPACE = "ExamScholars";
        private static readonly string CONTENT_EXT = ".json";
        private static string CONTENT_BASE;// = APP_NAMESPACE+".Assets.Content.";

        //private MCrypt mCrypt;

        //private string assemblyPath;

        public FileReader(string activeContent)
        {
                //CONTENT_BASE = Settings.BASE_FOLDER+"Content/";
            //if (ExamSession.CONTENT_MODE_JAMB_LITERATURE.Equals(activeContent))
            //{
            //    CONTENT_BASE = Settings.LITERATURE_BASE_FOLDER;
            //}
            //else if (ExamSession.CONTENT_MODE_OTHER_CONTENT.Equals(activeContent))
            //{
            //    CONTENT_BASE = Settings.OTHER_CONTENTS_FOLDER;
            //}
            //else
            //{
            //    CONTENT_BASE = Settings.UTME_BASE_FOLDER;
            //}

            //CONTENT_BASE = getBasePath()+"/Assets/Content/";

            //mCrypt = new MCrypt();
        }

        private static string assemblyPath;
        public static string getBasePath()
        {
            if (assemblyPath != null) return assemblyPath;

            assemblyPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            //Console.WriteLine("assemblyPath 1 = " + assemblyPath);
            if (assemblyPath.EndsWith(@"bin/Debug"))// || assemblyPath.EndsWith(@"bin\Debug"))
            {
                //assemblyPath = Directory.GetParent(assemblyPath).FullName;
                assemblyPath = Directory.GetParent(assemblyPath).FullName;
            }

            return assemblyPath;
        }

        public string readAllCoursesFile()
        {
            //return readFile(@"C:/Users/Admin/source/repos/ExamdrillerDev/ExamdrillerDev/Assets/all_courses.json");
            return readFile(CONTENT_BASE+ "questions/all_courses" + CONTENT_EXT);
        }

        public Newtonsoft.Json.Linq.JObject getZcountData()
        {
            string zCountStr = readFile(CONTENT_BASE+ "questions/zcount" + CONTENT_EXT, false);

            if (zCountStr == null) return null;

            return Newtonsoft.Json.Linq.JObject.Parse(zCountStr);
        }

        public string readCourseYearsFile(string courseId)
        {
            string path = CONTENT_BASE + "questions/" + courseId + "/" + courseId.ToLower() + CONTENT_EXT;
            //ExamdrillerDev.Assets.Content.Physics.physics.json
            return readFile(path);
        }
        public string readCourseTopicFile(int courseId)
        {
            string path = CONTENT_BASE + "questions/" + courseId + "/t" + CONTENT_EXT;

            return readFile(path);
        }

        public string readQuestionsFile(int courseId, int courseYearId)
        {
            string path = CONTENT_BASE + "questions/" + courseId + "/" + courseId + "_" + courseYearId + CONTENT_EXT;

            return readFile(path);
        }

        public static string readFile(string path, bool isEncrypted = true)
        {
            //Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(path);
            //if (stream == null)
            if (!File.Exists(@path))
            {
                Console.WriteLine("File not found; Path = " + path);
                return null;
            }

            StreamReader streamReader = new StreamReader(@path);
            string s = streamReader.ReadToEnd();

            return isEncrypted ? s : s;
        }

        public static string getCourseImagePath(string imageName, string courseId, string year)
        {
            //return getBasePath()+"/Assets/Content/images/"+courseId+"/"+year+"/"+imageName;
            return CONTENT_BASE + $"images/{courseId}/{year}/{imageName}";
        }

    }
}
