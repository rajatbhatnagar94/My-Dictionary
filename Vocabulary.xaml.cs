using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;
using System.IO;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using System.Collections;
using System.ComponentModel;

namespace Dictionary2
{
    public partial class Vocabulary : PhoneApplicationPage
    {
        public Vocabulary()
        {
            InitializeComponent();
            CreateAppBar();
            ApplicationBar.Buttons.Add(select);
            Restore();
        }

        
        ApplicationBarIconButton select;
        ApplicationBarIconButton delete;
    
        private void CreateAppBar()
        {
            ApplicationBar = new ApplicationBar();
            select = new ApplicationBarIconButton(new Uri("/Toolkit.Content/ApplicationBar.Select.png", UriKind.RelativeOrAbsolute));
            select.Text = "select";
            select.Click += select_Click;

            delete = new ApplicationBarIconButton();
            delete.IconUri = new Uri("/Toolkit.Content/ApplicationBar.Delete.png", UriKind.RelativeOrAbsolute);
            delete.Text = "delete";
            delete.Click += delete_Click;
        }

        async void delete_Click(object sender, EventArgs e)
        {
           /* IList source = LongList.ItemsSource as IList;
            while (LongList.SelectedItems.Count > 0)
            {
                source.Remove((DictionaryContents)LongList.SelectedItems[0]);
            }
            LongListMultiSelector l = sender as LongListMultiSelector;
            */
            /*
            IList source = LongList.ItemsSource as IList;
           

            
            while (LongList.SelectedItems.Count > 0)
            {
                source.Remove((DictionaryContents)LongList.SelectedItems[0]);
            }
            */
            DictionaryContents[] selectedItems = new DictionaryContents[LongList.SelectedItems.Count];
            for (int i = 0; i < LongList.SelectedItems.Count; i++)
            {
                selectedItems[i] = (DictionaryContents)LongList.SelectedItems[i];
            }

         //   LongList.IsSelectionEnabled = false;

            for (int i = 0; i < selectedItems.Length; i++)
            {
                App.ViewModel.Items.Remove(selectedItems[i]);
            }


            var serializer = new DataContractJsonSerializer(typeof(SavedData));
            using (var stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync(
                         DictionaryContents.fileName,
                          CreationCollisionOption.ReplaceExisting))
            {
                serializer.WriteObject(stream, App.ViewModel);
            }
          //  select.IsEnabled = true;
        }

        void select_Click(object sender, EventArgs e)
        {
            LongList.EnforceIsSelectionEnabled = true;
            ApplicationBar.Buttons.Add(delete);
            ApplicationBar.Buttons.Remove(select);
            LongList.Visibility = Visibility.Visible;
            SmallList.Visibility = Visibility.Collapsed;
        }

        
        async void Restore()
        {
            /*string content = String.Empty;
              var myStream2 = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(DictionaryContents.fileName);
              using (StreamReader reader = new StreamReader(myStream2))
            {
                content = await reader.ReadToEndAsync();
            }
            */
            
           //Deserializing json code
            var jsonSerializer = new DataContractJsonSerializer(typeof(SavedData));
            try
            {
                using (var myStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(DictionaryContents.fileName))
                {
                    App.ViewModel = (SavedData)jsonSerializer.ReadObject(myStream);
                }
            }
            catch(Exception)
            {

            }
         
            DataContext = App.ViewModel;
         
            if (App.ViewModel.Items.Count == 0)
                select.IsEnabled = false;
            else 
                select.IsEnabled = true;
        }

      
       
       
        private void LongList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LongList.SelectedItems.Count > 0)
                delete.IsEnabled = true;
            else
                delete.IsEnabled = false;
            
        }

        private void LongList_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (LongList.EnforceIsSelectionEnabled)
            { 
                return;
            }
            select_Click(sender,e);
        }

        private void SmallList_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongListSelector selectedItem = sender as LongListSelector;
            if (selectedItem == null)
            {
                return;
            }
            DictionaryContents d = selectedItem.SelectedItem as DictionaryContents;
            NavigationService.Navigate(new Uri("/DetailsPage.xaml?selectedItem=" + d.BigDefinition + "&selectedWord=" + d.Word, UriKind.Relative));
          
        }

        private void SmallList_Hold(object sender, System.Windows.Input.GestureEventArgs e)
        {
            LongList.Visibility = Visibility.Visible;
            SmallList.Visibility = Visibility.Collapsed;
            LongList_Hold(sender, e);
        }
    }
}