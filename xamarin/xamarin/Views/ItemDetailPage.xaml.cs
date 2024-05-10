using System.ComponentModel;
using Xamarin.Forms;
using xamarin.ViewModels;

namespace xamarin.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}
