using System.Diagnostics;
using System.Windows;
using WpfTourPlanner.BusinessLayer;
using WpfTourPlanner.Models.Exceptions;
using WpfTourPlanner.Stores;
using WpfTourPlanner.Util;
using WpfTourPlanner.ViewModels;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace WpfTourPlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            log4net.Config.XmlConfigurator.Configure();
            InitializeComponent();
            NavigationStore navigationStore = new NavigationStore();
            try
            {
                navigationStore.CurrentViewModel =
                    new HomeViewModel(TourPlannerFactory.GetTourPlannerManager(), navigationStore);
            }
            catch (ConfigException e)
            {
                UtilMethods.ShowErrorMsgBox(e.Message);
                Debug.WriteLine(e);
                throw;
            }

            this.DataContext = new MainViewModel(navigationStore);
        }
    }
}