using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

namespace Dictionary2
{
    public partial class DetailsPage : PhoneApplicationPage
    {
        public DetailsPage()
        {
            InitializeComponent();
            
        }


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (DataContext == null)
            {
                string selectedDefinition = "", selectedWord = "";
                Word.Text = "More about the word";
                if (NavigationContext.QueryString.TryGetValue(" selectedWord ", out selectedWord))
                {
                    Word.Text = "More about \n" + selectedWord;
                }
                if (NavigationContext.QueryString.TryGetValue("selectedItem", out selectedDefinition))
                {
                    TextBlock []Definition = new TextBlock[20];
                    Style style = (Style)this.Resources["PhoneTextSmallStyle"];
                    int k = selectedDefinition.Length;
                    int c=0;
                    int r = 0;
                    r = selectedDefinition.Length / 1000;
                    int m = 0;
                    if(r==0)
                    {
                        Definition[c] = new TextBlock();
                        Definition[c].TextWrapping = TextWrapping.Wrap;
                        Definition[c].Text = selectedDefinition.Substring(0, selectedDefinition.Length);
                        Definition[c].Style = style;
                        Stack.Children.Add(Definition[c]);
                    }
                    else
                    {
                        while (r>0)
                        {
                            Definition[c] = new TextBlock();
                            Definition[c].TextWrapping = TextWrapping.Wrap;
                            Definition[c].Style = style;
                            Definition[c].Text = selectedDefinition.Substring(m, 1000);
                            m = m + 1000;
                            r--;
                            Stack.Children.Add(Definition[c]);
                            c++;
                        }
                        Definition[c] = new TextBlock();
                        Definition[c].TextWrapping = TextWrapping.Wrap;
                        Definition[c].Style = style;
                        Definition[c].Text = selectedDefinition.Substring(m, selectedDefinition.Length-m);
                        Stack.Children.Add(Definition[c]);
                    }
                    
                }
                
               
            }
        }
    }
}