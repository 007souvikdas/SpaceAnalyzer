using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SpaceAnalyzer.Views
{
    public partial class FilePreviewUserControl : UserControl
    {
        public FilePreviewUserControl()
        {
            InitializeComponent();
        }
        public string FileName
        {
            get
            {
                return (string)GetValue(FileNameDependencyProperty);
            }
            set
            {
                SetValue(FileNameDependencyProperty, value);
            }
        }
        public static DependencyProperty FileNameDependencyProperty = DependencyProperty.Register("FileName", typeof(string), typeof(FilePreviewUserControl), new PropertyMetadata(""));

        public string FileSize
        {
            get
            {
                return (string)GetValue(FileSizeDependencyProperty);
            }
            set
            {
                SetValue(FileSizeDependencyProperty, value);
            }
        }
        public static DependencyProperty FileSizeDependencyProperty = DependencyProperty.Register("FileSize", typeof(string), typeof(FilePreviewUserControl), new PropertyMetadata("0 byte"));

        public string FilePath
        {
            get
            {
                return (string)GetValue(FilePathDependencyProperty);
            }
            set
            {
                SetValue(FilePathDependencyProperty, value);
            }
        }
        public static DependencyProperty FilePathDependencyProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(FilePreviewUserControl), new PropertyMetadata(""));
        public BitmapImage BitmapImageSource
        {
            get
            {
                return (BitmapImage)GetValue(BitmapImageSourceDependencyProperty);
            }
            set
            {
                SetValue(BitmapImageSourceDependencyProperty, value);
            }
        }
        public static DependencyProperty BitmapImageSourceDependencyProperty = DependencyProperty.Register("BitmapImageSource", typeof(BitmapImage), typeof(FilePreviewUserControl), new PropertyMetadata(new BitmapImage()));

    }
}