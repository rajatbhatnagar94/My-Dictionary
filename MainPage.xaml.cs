using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Dictionary2.Resources;
using Coding4Fun.Toolkit.Controls;
using System.IO;
using Windows.Storage;
using System.Text;
using Windows.Storage.Streams;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization.Json;
using System.Windows.Media;
using Windows.Networking.Connectivity;
using Windows.Phone.Speech.Synthesis;
using Windows.System;


namespace Dictionary2
{
    public partial class MainPage : PhoneApplicationPage
    {
        public MainPage()
        {
            InitializeComponent();
            BuildLocalizedApplicationBar();
            appPlayButton.Click += appPlayButton_Click;
            ApplicationBar.Buttons.Add(appBarButton);
            Deserialize();
        }
        bool foundBig = false;
        bool foundSmall = false;
        async void speechsynth()
        {
            SpeechSynthesizer speech = new SpeechSynthesizer();
            try
            {
                VoiceInformation v = InstalledVoices.Default;
                speech.SetVoice(v);
                await speech.SpeakTextAsync(dictionaryContents.Word + "\n" + dictionaryContents.Definition);
            }
            catch (Exception)
            { }
        }

        void appPlayButton_Click(object sender, EventArgs e)
        {

            if (content.Text == "Please enter a word!" || content.Text == "" || content.Text == "Sorry, No such word found")
            {
                return;
            }

            speechsynth();
        }

        private async void Deserialize()
        {
            
            var jsonSerializer = new DataContractJsonSerializer(typeof(SavedData));
            try
            {
                using (var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(DictionaryContents.fileName))
                {
                    if (!(myStream == null))
                        App.ViewModel = (SavedData)jsonSerializer.ReadObject(myStream);
                    myStream.Close();
                }
            }
            catch (FileNotFoundException){ }
            catch (SerializationException){ }
            catch (Exception) { }
        }
        
        ApplicationBarIconButton appBarButton,appPlayButton;
        DictionaryContents dictionaryContents = new DictionaryContents();
        WebBrowser webv = new WebBrowser();
        string html = "";

        private void BuildLocalizedApplicationBar()
        {
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized; 
            appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/save.png", UriKind.Relative));
            appBarButton.IsEnabled = false;
            appBarButton.Text = AppResources.AppBarButtonText;
            appBarButton.Click += appBarButton_Click;
            ApplicationBarMenuItem appBarMenuVocab = new ApplicationBarMenuItem(AppResources.AppBarMenuVocab);
            ApplicationBar.MenuItems.Add(appBarMenuVocab);
            ApplicationBarMenuItem appBarMenuAbout = new ApplicationBarMenuItem(AppResources.AppBarMenuAbout);
            ApplicationBar.MenuItems.Add(appBarMenuAbout);
            appBarMenuVocab.Click += appBarMenuVocab_Click;
            appBarMenuAbout.Click += appBarMenuAbout_Click;
            appPlayButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/play.png", UriKind.Relative));
            appPlayButton.Text = "Play";
            ApplicationBar.Buttons.Add(appPlayButton);
            appPlayButton.IsEnabled = false;
        }

        async void appBarButton_Click(object sender, EventArgs e)
        {
            foreach (var item in App.ViewModel.Items)
            {
                if(item.Word.ToLower().Equals(dictionaryContents.Word.ToLower()))
                {
                    content.Text += "\n\nThis word is Already Saved!";
                    appBarButton.IsEnabled = false;
                    return;
                }

            }

            App.ViewModel.Items.Insert(0, dictionaryContents);
            var serializer = new DataContractJsonSerializer(typeof(SavedData));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                         DictionaryContents.fileName,
                          CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream,App.ViewModel);
                stream.Close();
            }
            appBarButton.IsEnabled = false;
        }

        async void appBarMenuAbout_Click(object sender, EventArgs e)
        {
            await Launcher.LaunchUriAsync(new Uri("http://1drv.ms/1qD6TBx")); 
        }

        void appBarMenuVocab_Click(object sender, EventArgs e)
        {
            //Show the saved Vocabulary
            NavigationService.Navigate(new Uri("/Vocabulary.xaml", UriKind.Relative));
            
        }


       public static bool IsInternet()
        {
            try
            {
                ConnectionProfile connections = NetworkInformation.GetInternetConnectionProfile();
                bool internet = connections != null && connections.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.InternetAccess;
                return internet;
            }
            catch (NotImplementedException)
            {
                return false;
            }
        }
      
       void completed()   
       {
            ApplicationBar.Mode = ApplicationBarMode.Default;
            appBarButton.IsEnabled = true;
            bar.IsIndeterminate = false;
            appPlayButton.IsEnabled = true;

            if (content.Text == "Word not found!")
                return;

            dictionaryContents.Word = SearchText.Text;
            dictionaryContents.BigDefinition = "";
            dictionaryContents.Definition = "";

            if (!html.Contains("<div class='custom_entry'>"))
            {
                wiki2(1);

                /*  string find2 = "<ol class=\"sense\">";
                string end2 = "</ol>";
                dictionaryContents.Word = SearchText.Text;
                //dictionaryContents.custom_entry_pos = dictionaryContents.Find(html,find1,find2,end1,end2);
                dictionaryContents.custom_entry_pos = dictionaryContents.FindSingle(html, find2, end2);
                for (int i = 0; i < dictionaryContents.custom_entry_pos.Count; i++)
                {
                    dictionaryContents.custom_entry_pos[i] = dictionaryContents.custom_entry_pos[i].Replace("<li> ", "\n");
                    dictionaryContents.custom_entry_pos[i] = dictionaryContents.custom_entry_pos[i].Replace("</li>", "\n");
                    dictionaryContents.custom_entry_pos[i] = dictionaryContents.custom_entry_pos[i].Replace("<li>", "\n");
                    dictionaryContents.custom_entry_pos[i] = dictionaryContents.custom_entry_pos[i].Replace("<span class=\"ex\">", "\nExample: ");
                    dictionaryContents.custom_entry_pos[i] = dictionaryContents.custom_entry_pos[i].Replace("<em>", "'");
                    dictionaryContents.custom_entry_pos[i] = dictionaryContents.custom_entry_pos[i].Replace("</em>", "'");
                    dictionaryContents.custom_entry_pos[i] = dictionaryContents.custom_entry_pos[i].Replace("</span>", "");
                    dictionaryContents.custom_entry_pos[i] = dictionaryContents.custom_entry_pos[i].Replace("<div class=\"sds-list\">", "\n");
                    dictionaryContents.custom_entry_pos[i] = dictionaryContents.custom_entry_pos[i].Replace("</div>", "\n");
                    dictionaryContents.custom_entry_pos[i] = dictionaryContents.custom_entry_pos[i].Replace("<span class=\"cls\">", "\n");
                }


                if (!dictionaryContents.custom_entry_pos.Equals(null))
                {
                    for (int i = 0; i < dictionaryContents.custom_entry_pos.Count; i++)
                    {
                        if (dictionaryContents.custom_entry_pos[i].Contains("<"))
                        {
                            int j = dictionaryContents.custom_entry_pos[i].IndexOf("<");
                            int k = dictionaryContents.custom_entry_pos[i].IndexOf(">");
                            dictionaryContents.custom_entry_pos[i]= dictionaryContents.custom_entry_pos[i].Remove(j, k - j + 1);
                            i--;
                        }
                    }

                    for (int i = 0; i < dictionaryContents.custom_entry_pos.Count; i++)
                    {
                        content.Text += dictionaryContents.custom_entry_pos[i] + "\n";

                    } dictionaryContents.Definition = content.Text;
                }*/
            }
            else
            {
                string find2 = "<div class='custom_entry'>";
                //string find2 = "<div class=\"custom_entry\">";
                string end2 = "</div>";
                string end = "</span>";
                string find = "<span class=\"custom_entry_pos\">";

                dictionaryContents.custom_entry_pos = dictionaryContents.Find(html,find, find2, end, end2);

                if(!dictionaryContents.custom_entry_pos.Equals(null))
                for (int i = 0; i < dictionaryContents.custom_entry_pos.Count; i++)
                dictionaryContents.Definition += dictionaryContents.custom_entry_pos[i] + "\n";
               

                if(dictionaryContents.Definition.Contains('<'))
                {
                    for (int i = 0; i < dictionaryContents.custom_entry_pos.Count; i++)
                    {
                        if (dictionaryContents.custom_entry_pos[i].Contains("<"))
                        {
                            int j = dictionaryContents.custom_entry_pos[i].IndexOf("<");
                            int k = dictionaryContents.custom_entry_pos[i].IndexOf(">");
                            dictionaryContents.custom_entry_pos[i] = dictionaryContents.custom_entry_pos[i].Remove(j, k - j + 1);
                            i--;
                        }
                    }
                    dictionaryContents.Definition = "";
                    for (int i = 0; i < dictionaryContents.custom_entry_pos.Count; i++)
                    dictionaryContents.Definition += dictionaryContents.custom_entry_pos[i] + "\n";
                }

                if(dictionaryContents.Definition!="")
                foundSmall = true;
                wiki2(0);
            }
            
           if(foundSmall)
           {
               content.Text = dictionaryContents.Definition;
           }
           if(!foundSmall)
           {
               content.Text = "Sorry Word meaning not found!";
               appBarButton.IsEnabled = false;
           }

           // big();

           //If no definition found in Details dictionary
           if(!foundBig&&!foundSmall)
           {
               content.Text = "Sorry Word meaning not found!";
               appBarButton.IsEnabled = false;
           }

           if (dictionaryContents.Definition == "")
           {
               content.Text = "Sorry word meaning not found!";
               appBarButton.IsEnabled = false;
           }
           /*if(dictionaryContents.custom_pos.Count==0)
           {
               func();
               if (dictionaryContents.Definition == "" && dictionaryContents.custom_pos.Count != 0) 
               {
                   dictionaryContents.Definition = dictionaryContents.BigDefinition;
                   content.Text = dictionaryContents.Definition;
               }
           }
           */
           /*if (dictionaryContents.Definition == "")
            {
                content.Text = "Sorry word meaning not found!";
                appBarButton.IsEnabled = false;
            }*/
            
    }
       /* private void func()
       {
           string Find3 = "<div class=\"sense\">";
           string end3 = "</div>";
           dictionaryContents.Word = SearchText.Text;
           dictionaryContents.custom_pos = dictionaryContents.FindSingle(html, Find3, end3);
           for (int i = 0; i < dictionaryContents.custom_pos.Count; i++)
           {
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<li> ", "\n");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("</li>", "\n");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<li>", "\n");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<span class=\"ex\">", "\nExample: ");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<em>", "'");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("</em>", "'");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("</span>", "");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<div class=\"sds-list\">", "\n");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("</div>", "\n");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<span class=\"cls\">", "\n");
           }

           if (!dictionaryContents.custom_pos.Equals(null))
           {
               for (int i = 0; i < dictionaryContents.custom_pos.Count; i++)
               {
                   if (dictionaryContents.custom_pos[i].Contains("<"))
                   {
                       int j = dictionaryContents.custom_pos[i].IndexOf("<");
                       int k = dictionaryContents.custom_pos[i].IndexOf(">");
                       dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Remove(j, k - j + 1);
                       i--;
                   }
               }

               for (int i = 0; i < dictionaryContents.custom_pos.Count; i++)
               {
                   dictionaryContents.BigDefinition += dictionaryContents.custom_pos[i] + "\n";
               }
           }
       }
        */

        private void wiki2(int l)
        {
            string find1 = "<a href=\"http://wiktionary.yourdictionary.com/\" title=\"Wiktionary\">";
            string end1 = "<hr>";
            List<string> mystring =  dictionaryContents.FindSingle(html, find1, end1);
            wiki(l,mystring);
        }

        private void wiki(int m,List<string> mystring)
        {
            string find1 = "custom_entry_pos wiktionary-heading'>";
            string find2 = "<ol>";
            string end1 = "</span>";
            string end2 = "</ol>";
            if (mystring.Count == 0)
            return;
            string a = mystring[0];
            dictionaryContents.custom_pos = dictionaryContents.Find(a, find1, find2, end1, end2);
            
            for (int i = 0; i < dictionaryContents.custom_pos.Count; i++)
            {
                dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("ORIGIN", "");
                dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("ANAGRAMS", "");
                dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("ANTONYMS", "");
                dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("RELATED TERMS", "");
                dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("USAGE NOTES", "");
                dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<li> ", "\n");
                dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("</li>", "\n");
               // dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<li>", "\n");
                dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<i>", "\nExample: '");
                dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("</i>", "'\n");            
            }

            if (!dictionaryContents.custom_pos.Equals(null))
                for (int i = 0; i < dictionaryContents.custom_pos.Count; i++)
                    dictionaryContents.BigDefinition += dictionaryContents.custom_pos[i] + "\n";

            if (dictionaryContents.BigDefinition.Contains('<'))
            {
                for (int i = 0; i < dictionaryContents.custom_pos.Count; i++)
                {
                    if (dictionaryContents.custom_pos[i].Contains("<"))
                    {
                        int j = dictionaryContents.custom_pos[i].IndexOf("<");
                        int k = dictionaryContents.custom_pos[i].IndexOf(">");
                        dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Remove(j, k - j + 1);
                        i--;
                    }
                }
                dictionaryContents.BigDefinition = "";
                for (int i = 0; i < dictionaryContents.custom_pos.Count; i++)
                dictionaryContents.BigDefinition += dictionaryContents.custom_pos[i] + "\n";
            }
            if(m==1)
            {
                dictionaryContents.custom_entry_pos = dictionaryContents.custom_pos;
                dictionaryContents.Definition = dictionaryContents.BigDefinition;
                foundSmall = true;
            }

            if(dictionaryContents.BigDefinition!="")
            foundBig = true;

        }
       private void SearchText_GotFocus(object sender, RoutedEventArgs e)
       {
           SearchText.SelectAll();
       }


       private void EnterkeyPressed(object sender, System.Windows.Input.KeyEventArgs e)
       {
            if(e.Key.ToString().Equals("Enter"))
            {
                Search_ActionIconTapped(sender, e);
                bar.Focus();
            }
       }

     /*  void big()
       {
           if (content.Text == "Please enter a word!" || content.Text == "" || content.Text == "Sorry, No such word found")
           {
               return;
           }

           string find2 = "<ol class=\"sense\">";
           string end2 = "</ol>";
           
           dictionaryContents.Word = SearchText.Text;
           dictionaryContents.custom_pos = dictionaryContents.FindSingle(html, find2, end2);
           for (int i = 0; i < dictionaryContents.custom_pos.Count; i++)
           {
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<li> ", "\n");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("</li>", "\n");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<li>", "\n");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<span class=\"ex\">", "\nExample: ");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<em>", "'");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("</em>", "'");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("</span>", "");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<div class=\"sds-list\">", "\n");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("</div>", "\n");
               dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Replace("<span class=\"cls\">", "\n");
           }


           if (!dictionaryContents.custom_pos.Equals(null))
           {
               for (int i = 0; i < dictionaryContents.custom_pos.Count; i++)
               {
                   if (dictionaryContents.custom_pos[i].Contains("<"))
                   {
                       int j = dictionaryContents.custom_pos[i].IndexOf("<");
                       int k = dictionaryContents.custom_pos[i].IndexOf(">");
                       dictionaryContents.custom_pos[i] = dictionaryContents.custom_pos[i].Remove(j, k - j + 1);
                       i--;
                   }
               }

               for (int i = 0; i < dictionaryContents.custom_pos.Count; i++)
               {
                   dictionaryContents.BigDefinition += dictionaryContents.custom_pos[i] + "\n";
               }
           }

       }
        */

       private void ScrollViewer_Tap(object sender, System.Windows.Input.GestureEventArgs e)
       {
           NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + dictionaryContents.BigDefinition + "&selectedWord="+ dictionaryContents.Word, UriKind.Relative));
       }

       private void Search_ActionIconTapped(object sender, EventArgs e)
       {
           if (!IsInternet())
           {
               MessageBox.Show("An Internet Connection is needed to use the dictionary...");
               return;
           }


           dictionaryContents = new DictionaryContents();
           if (SearchText.Text == "")
           {
               content.Text = "Please enter a word!";
               return;
           }
           bar.Focus();
           content.Text = "";
          // webv.Navigate(new Uri("http://www.yourdictionary.com/" + SearchText.Text));
           appBarButton.IsEnabled = false;
           AccessTheWebAsync();

    
       }

       void AccessTheWebAsync()
       {
           try
           {
               HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://www.yourdictionary.com/" + SearchText.Text);
               request.AllowAutoRedirect = true;
               request.BeginGetResponse(new AsyncCallback(ReadWebRequestCallback), request);
               Dispatcher.BeginInvoke(() =>bar.IsIndeterminate = true);
           }
           catch(WebException)
           {
               Dispatcher.BeginInvoke(() => content.Text = "Word not found!"); 
           }
       }

       private void ReadWebRequestCallback(IAsyncResult callbackResult)
       {
           try
           {
               HttpWebRequest myRequest = (HttpWebRequest)callbackResult.AsyncState;
               HttpWebResponse myResponse = (HttpWebResponse)myRequest.EndGetResponse(callbackResult);


               using (StreamReader httpwebStreamReader = new StreamReader(myResponse.GetResponseStream()))
               {
                   html = httpwebStreamReader.ReadToEnd();
                   Dispatcher.BeginInvoke(() =>bar.IsIndeterminate = false);
                   Dispatcher.BeginInvoke(() => appBarButton.IsEnabled = true);
                   Dispatcher.BeginInvoke(() =>completed());
               }
           }
           catch (WebException)
           {   
               Dispatcher.BeginInvoke(() => content.Text = "Word not found!");
               Dispatcher.BeginInvoke(() =>bar.IsIndeterminate = false);
           }
       }
 
    }
}