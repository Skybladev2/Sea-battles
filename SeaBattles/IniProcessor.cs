using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace SeaBattles
{
    /// <summary>
    /// Работает с .ini-файлами
    /// Хавает их целиком в память и пишет на диск тоже целиком.
    /// </summary>
    public class IniProcessor
    {
        //protected StreamReader reader = null;
        //protected StreamWriter writer = null;
        protected string path = null;
        private string currentKey = null;
        private string currentSectionName = null;

        protected List<KeyValuePair<string, List<KeyValuePair<string, string>>>> sections = new List<KeyValuePair<string, List<KeyValuePair<string, string>>>>();

        public IniProcessor(string path)
        {
            this.path = path;
        }

        public void ReadFile()
        {
            ReadFileInternal(this.path);
        }

        public void ReadFile(string path)
        {
            ReadFileInternal(path);
        }

        protected void ReadFileInternal(string path)
        {
            sections.Clear();
            using (StreamReader reader = new StreamReader(path))
            {
                string line = null;
                List<KeyValuePair<string, string>> currentSection = null;

                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    // название секции
                    if (line.StartsWith("[") && line.EndsWith("]"))
                    {
                        // пустые секции игнорируем
                        if (line.Length > 2)
                        {
                            string sectionName = line.Substring(1, line.Length - 2);

                            this.currentSectionName = sectionName;
                            if (sections.Exists(FindSection))
                            {
                                currentSection = null;
                            }
                            else
                            {
                                currentSection = new List<KeyValuePair<string, string>>();
                                sections.Add(new KeyValuePair<string, List<KeyValuePair<string, string>>>(sectionName, currentSection));
                            }
                        }
                    }
                    else if (currentSection != null)
                    {
                        int i;
                        if ((i = line.IndexOf('=')) > 0)
                        {
                            int j = line.Length - i - 1;
                            string key = line.Substring(0, i).Trim();
                            this.currentKey = key;

                            if (key.Length > 0)
                            {
                                // записываем только первый ключ
                                if (!currentSection.Exists(FindKey))
                                {
                                    string @value = (j > 0) ? (line.Substring(i + 1, j).Trim()) : ("");
                                    currentSection.Add(new KeyValuePair<string, string>(key, @value));
                                }
                            }
                        }
                    }
                }
            }
        }

        private bool FindKey(KeyValuePair<string, string> element)
        {
            if (element.Key == currentKey)
                return true;
            else
                return false;

        }

        private bool FindSection(KeyValuePair<string, List<KeyValuePair<string, string>>> section)
        {
            if (section.Key == currentSectionName)
                return true;
            else
                return false;
        }
    }
}
