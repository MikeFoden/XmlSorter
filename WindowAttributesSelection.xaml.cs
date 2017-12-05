using System.ComponentModel;
using System.Windows;
using XmlSorter.DataObjects;

namespace XmlSorter
{
	/// <summary>
	/// Sorting attributes selection window.
	/// </summary>
	public partial class WindowAttributesSelection : Window
    {
        #region Constructors

        public WindowAttributesSelection(AttributesBinding AttributesBindingInstance)
        {
            this.InitializeComponent();
            ListBoxAttributes.ItemsSource = AttributesBindingInstance;
            ListBoxAttributes.Items.SortDescriptions.Add(new SortDescription("Key", ListSortDirection.Ascending));
        }

        #endregion

        #region Events' Handlers

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            //Visibility = System.Windows.Visibility.Hidden;
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        #endregion
    }
}