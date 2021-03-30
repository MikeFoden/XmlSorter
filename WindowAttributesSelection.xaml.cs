using System.Collections;
using System.ComponentModel;
using System.Windows;

namespace XmlSorter
{
  /// <summary>
  /// Sorting attributes selection window.
  /// </summary>
  public partial class WindowAttributesSelection : Window
    {
        #region Constructors

        public WindowAttributesSelection(IEnumerable attributesBindingInstance)
        {
            InitializeComponent();
            ListBoxAttributes.ItemsSource = attributesBindingInstance;
            ListBoxAttributes.Items.SortDescriptions.Add(new SortDescription("Key", ListSortDirection.Ascending));
        }

        #endregion

        #region Events' Handlers

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        #endregion
    }
}