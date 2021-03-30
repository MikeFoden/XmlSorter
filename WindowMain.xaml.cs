using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Navigation;
using System.Xml;
using System.Xml.Linq;
using Microsoft.Win32;
using IOPath = System.IO.Path;
using XmlSorter.DataObjects;
using XmlSorter.Helpers;

namespace XmlSorter
{
    /// <summary>
    /// Startup and main window of the application
    /// </summary>
    public partial class WindowMain : Window
    {
        #region Fields

        private string _tempSourceFilePath;
        private string _tempTargetFilePath;
        private string _originalSourceFilePath;
        private OpenFileDialog _openFileDialogInstance;
        private SaveFileDialog _saveFileDialogInstance;
        private volatile bool _isAttributesBeingRead;
        private bool _clearRigthBrowser;
        private WindowAttributesSelection _windowAttributesSelectionInstance;
        private readonly AttributesBinding _attributesBindingInstance = new AttributesBinding();
        private IEnumerable<string> _filteredSortingAttibutes;

        #endregion

        #region Constructors

        public WindowMain()
        {
            InitializeComponent();
        }

        #endregion

        #region Events' Handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MaintainControlsAvailability();
        }

        private void ButtonSourceBrowse_Click(object sender, RoutedEventArgs e)
        {
          if (!ShowOpenDialog()) { return; }

          TextBlockSouurcePath.Text = _openFileDialogInstance.FileName;
          _originalSourceFilePath = _openFileDialogInstance.FileName;
          _tempSourceFilePath = IOPath.Combine(IOPath.GetTempPath(), "Source.xml");
          _tempTargetFilePath = IOPath.Combine(IOPath.GetTempPath(), "Target.xml");
          MaintainControlsAvailability();
          _clearRigthBrowser = true;
          WebBrowserAfter.NavigateToString("<HTML></HTML>");
          new Thread(ReadAttributes).Start();
        }

        private void OpenFileDialogInstance_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = !ValidateXml(_openFileDialogInstance.FileName);
        }

        private void CheckBoxOverwriteSourceFile_Checked(object sender, RoutedEventArgs e)
        {
            MaintainControlsAvailability();
        }

        private void ButtonTargetBrowse_Click(object sender, RoutedEventArgs e)
        {
            if(ShowSaveDialog())
            {
                TextBlockTargetPath.Text = _saveFileDialogInstance.FileName;
                MaintainControlsAvailability();
            }
        }

        private void CheckBoxSortBySpecificAttributes_Checked(object sender, RoutedEventArgs e)
        {
            MaintainControlsAvailability();
        }

        private void CheckBoxSortByTagName_Checked(object sender, RoutedEventArgs e)
        {
            MaintainControlsAvailability();
        }

        private void CheckBoxSortAttributes_Checked(object sender, RoutedEventArgs e)
        {
            MaintainControlsAvailability();
        }

        private void ButtonSelectAttributes_Click(object sender, RoutedEventArgs e)
        {
            SelectAttributes();
        }

        private void ButtonSort_Click(object sender, RoutedEventArgs e)
        {
            _filteredSortingAttibutes = _attributesBindingInstance.GetSelected();
            var xe = XElement.Load(_tempSourceFilePath);
            SortElement(xe);
            xe.Save(_tempTargetFilePath);
            WebBrowserAfter.NavigateSafely(_tempTargetFilePath);
        }

        private void WebBrowserBefore_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            e.Cancel = !string.Equals(e.Uri.GetBrowserSafeUri(),
              new Uri(_tempSourceFilePath).GetBrowserSafeUri(), StringComparison.CurrentCultureIgnoreCase);
        }

        private void WebBrowserAfter_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            if(_clearRigthBrowser)
            {
                _clearRigthBrowser = false;
            }
            else
            {
                e.Cancel = !string.Equals(e.Uri.GetBrowserSafeUri(),
                  new Uri(_tempTargetFilePath).GetBrowserSafeUri(), StringComparison.CurrentCultureIgnoreCase);
            }
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            System.IO.File.Copy(_tempTargetFilePath, TextBlockTargetPath.Text, true);
        }
       
        #endregion

        #region Helper Functions

        private void SortElement(XElement xe)
        {
            var nodesToBePreserved = xe.Nodes().Where(p => p.GetType() != typeof(XElement));
            if(CheckBoxSortAttributes.IsChecked.Value)
            {
                xe.ReplaceAttributes(xe.Attributes().OrderBy(x => x.Name.LocalName));
            }
            if(!CheckBoxSortBySpecificAttributes.IsChecked.Value || !_filteredSortingAttibutes.Any())
            {
                xe.ReplaceNodes((xe.Elements().OrderBy(x => x.Name.LocalName).Union((nodesToBePreserved).OrderBy(p => p.ToString()))).OrderBy(n => n.NodeType.ToString()));
            }
            else
            {
                foreach(var att in _filteredSortingAttibutes)
                {
                    xe.ReplaceNodes((xe.Elements().OrderBy(x => x.Name.LocalName).ThenBy(x => (string)x.Attribute(att)).Union((nodesToBePreserved).OrderBy(p => p.ToString()))).OrderBy(n => n.NodeType.ToString()));
                }
            }
            foreach(var xc in xe.Elements())
            {
                SortElement(xc);
            }
        }

        private void ReadAttributes()
        {
            _isAttributesBeingRead = true;
            if(_attributesBindingInstance != null)
            {
                Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate
                    {
                        _attributesBindingInstance.Clear();
                    }));
            }
            var xe = XElement.Load(_originalSourceFilePath);
            xe.Save(_tempSourceFilePath);
            Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(delegate
                {
                    WebBrowserBefore.NavigateSafely(_tempSourceFilePath);
                }));
            var doc = new XmlDocument();
            doc.Load(_tempSourceFilePath);
            const string stringXPath = "//@*";
            foreach(XmlAttribute att in doc.SelectNodes(stringXPath))
            {
                _attributesBindingInstance.Add(att.Name);
            }
            _isAttributesBeingRead = false;
        }

        private void SelectAttributes()
        {
            while(_isAttributesBeingRead)
            {

            }
            if(_windowAttributesSelectionInstance == null)
            {
                _windowAttributesSelectionInstance = new WindowAttributesSelection(_attributesBindingInstance);
                _windowAttributesSelectionInstance.IsVisibleChanged += new DependencyPropertyChangedEventHandler(delegate
                    {
                        MaintainControlsAvailability();
                    });
            }
            _windowAttributesSelectionInstance.Show(this);
            MaintainControlsAvailability();
        }

        private bool ShowOpenDialog()
        {
            if(_openFileDialogInstance == null)
            {
                _openFileDialogInstance = new OpenFileDialog();
                _openFileDialogInstance.FileOk += new System.ComponentModel.CancelEventHandler(OpenFileDialogInstance_FileOk);
            }
            var result = _openFileDialogInstance.ShowDialog();
            return result.HasValue && result.Value;
        }

        private bool ShowSaveDialog()
        {
            if(_saveFileDialogInstance == null)
            {
                _saveFileDialogInstance = new SaveFileDialog();
            }
            var result = _saveFileDialogInstance.ShowDialog();
            return result.HasValue && result.Value;
        }

        private void MaintainControlsAvailability()
        {
            if(!IsLoaded)
            {
                return;
            }
            GroupBoxTarget.IsEnabled = TextBlockSouurcePath.Text.Length > 0;

            LabelTargetPath.IsEnabled = !CheckBoxOverwriteSourceFile.IsChecked.Value;
            TextBlockTargetPath.IsEnabled = !CheckBoxOverwriteSourceFile.IsChecked.Value;
            ButtonTargetBrowse.IsEnabled = !CheckBoxOverwriteSourceFile.IsChecked.Value;
            TextBlockTargetPath.Text = (CheckBoxOverwriteSourceFile.IsChecked.Value) ? TextBlockSouurcePath.Text : (_saveFileDialogInstance == null) ? string.Empty : _saveFileDialogInstance.FileName;

            GroupBoxOptions.IsEnabled = GroupBoxTarget.IsEnabled;
            GroupBoxPreview.IsEnabled = GroupBoxTarget.IsEnabled;
            ButtonSelectAttributes.IsEnabled = CheckBoxSortBySpecificAttributes.IsChecked.Value && (_windowAttributesSelectionInstance == null || !_windowAttributesSelectionInstance.IsVisible);
            GroupBoxActions.IsEnabled = TextBlockSouurcePath.Text.Length > 0;
            ButtonSort.IsEnabled = CheckBoxSortByTagName.IsChecked.Value || CheckBoxSortAttributes.IsChecked.Value || CheckBoxSortBySpecificAttributes.IsChecked.Value;
            ButtonSave.IsEnabled = TextBlockTargetPath.Text.Length > 0;
            if(!CheckBoxSortBySpecificAttributes.IsChecked.Value && _windowAttributesSelectionInstance != null && _windowAttributesSelectionInstance.IsVisible)
            {
                _windowAttributesSelectionInstance.Hide();
            }
        }

        private static bool ValidateXml(string filePath)
        {
            try
            {
                new XmlDocument().Load(filePath);
                return true;
            }
            catch
            {
                MessageBox.Show("Invalid xml file");
                return false;
            }
        }

        #endregion
    }
}
