using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dictionary2
{
    public class DictionaryContents
    {
        public string Word { get; set; }
        public List<string> custom_entry_pos { get; set; }
        public string Definition { get; set; }
        public string BigDefinition { get; set; }
        public List<string> custom_pos { get; set; }
        public const string key = "Dictionarykey";
        public const string fileName = "Dictionary.json";
        public DictionaryContents()
        {
            custom_entry_pos = new List<string>();
            custom_pos = new List<string>();
        }

        public List<string> Find(string html, string find,string find2, string end,string end2)
        {

            List<string> WordMeaning = new List<string>();
            try
            {
                for (int i = 0; i < html.Length - find.Length - 1; i++)
                {
                    bool a=html.Substring(i, find.Length).Equals(find), b2=html.Substring(i, find2.Length).Equals(find2);
                    if ( a||b2 )
                    {
                        StringBuilder b = new StringBuilder();
                        int j;
                        if (a)
                        {
                            j = i + find.Length;
                            while (!(html.Substring(j, end.Length).Equals(end)))
                            {
                                b.Append(html.Substring(j, 1).ToUpper());
                                j++;
                            }
                            i = j + end.Length-1;
                        }
                        else
                        {
                            j = i + find2.Length;
                            while (!(html.Substring(j, end2.Length).Equals(end2)))
                            {
                                b.Append(html.Substring(j, 1));
                                j++;
                            }
                            b.Append("\n");
                            i = j + end2.Length-1;
                        }
                        
                        WordMeaning.Add(b.ToString());
                    }
                }
                return WordMeaning;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<string> FindSingle(string html, string find, string end)
        {
            List<string> WordMeaning = new List<string>();
            try
            {
                for (int i = 0; i < html.Length - find.Length - 1; i++)
                {
                    if (html.Substring(i, find.Length).Equals(find))
                    {
                        StringBuilder b = new StringBuilder();
                        int j;

                        j = i + find.Length;
                        while (!(html.Substring(j, end.Length).Equals(end)))
                        {
                            b.Append(html.Substring(j, 1));
                            j++;
                        }
                        i = j + end.Length - 1;
                        WordMeaning.Add(b.ToString());
                    }
                }
                return WordMeaning;
            }
            catch (Exception)
            {
                return null; ;
            }

        }

        
    }

   
}
