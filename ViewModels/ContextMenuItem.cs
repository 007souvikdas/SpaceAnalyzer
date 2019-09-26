using System.Windows.Input;
namespace SpaceAnalyzer.ViewModels
{
    public class ContextMenuItem
    {
        public string Name { get; set; }
        public ICommand ContextCommand { get; set; }
    }
}