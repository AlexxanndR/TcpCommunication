using MVVM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace Receiver.MVVM.ViewModel
{
    class ReceiverViewModel : ObservableObject
    {
        private void NavigateToPage(string pageName)
        {
            foreach (Window window in Application.Current.Windows)
                if (window.GetType() == typeof(MainWindow))
                    (window as MainWindow).MainWindowFrame.Navigate(new Uri(string.Format("{0}{1}{2}", "MVVM/View/", pageName, "View.xaml"), UriKind.RelativeOrAbsolute));
        }

        public ReceiverViewModel() { }

        public RelayCommand MenuSelectionCommand
        {
            get
            {
                return new RelayCommand((pageName) => NavigateToPage(pageName as string));
            }
        }
    }
}
