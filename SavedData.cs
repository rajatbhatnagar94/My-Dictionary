using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using System.Collections.ObjectModel;

namespace Dictionary2
{
    public class SavedData 
    {
         public SavedData() 
        {
            Items = new ObservableCollection<DictionaryContents>();
        }

        public ObservableCollection<DictionaryContents> Items { get; set; }
                  
        public bool IsDataLoaded { get; set; }

        public void LoadData()
        {
            
            //this.Items.Add(new DictionaryContents(){Word="Hel", Definition="Greeting"});
            IsDataLoaded = true;
        }

        
    }
}
