using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace SpaceAnalyzer.Views
{
    public partial class TileUserControl : UserControl
    {
        Dictionary<string, decimal> myFileSizeDatas;
        // public TilesWindow(Dictionary<string, decimal> fileSizeData)
        // {
        //     InitializeComponent();
        //     myFileSizeDatas=fileSizeData;
        //     // foreach (KeyValuePair<string, decimal> pair in fileSizeData)
        //     // {
        //     //     MessageBox.Show(pair.Key + " has size of " + string.Format("{0:0.00}", pair.Value));
        //     // }
        // }
        public TileUserControl()
        {
            InitializeComponent();
            // this.Resources["sizeColor"]=new SolidColorBrush(System.Windows.Media.Colors.Green);
            // this.Resources["typeColor"]=new SolidColorBrush(System.Windows.Media.Colors.HotPink);
        }        
        public List<string> ListProperty
        {
            get
            {
                return (List<string>)GetValue(ListDependencyProperty);
            }
            set
            {
                SetValue(ListDependencyProperty, value);
            }
        }
        public static DependencyProperty ListDependencyProperty = DependencyProperty.Register("ListProperty", typeof(List<string>), typeof(TileUserControl), new PropertyMetadata(new List<string>(){"-","-","0 bytes"}));
        
    }
}
