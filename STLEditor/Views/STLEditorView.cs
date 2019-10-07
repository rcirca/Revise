using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Revise.Files.STL;
using STLEditor.Util;
using STLEditor.Views;

namespace STLEditor
{
    public partial class STLEditorView : Form
    {
        public STLEditorView()
        {
            InitializeComponent();
            _openFileDialog.Multiselect = true;
            _saveButton.Enabled = false;
            _tabControl.MouseClick += OnTabControlMouseClick;
        }

        public void OnLoadFile(object sender, EventArgs pEvent)
        {

            var result =_openFileDialog.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            SetupTabWithGrid(_openFileDialog.FileNames);
            _saveButton.Enabled = _tabControl.TabCount > 0;
        }

        public void OnSaveFile(object sender, EventArgs pEvent)
        {
            var presenter = _tabControl.SelectedTab.Tag as STLEditorPresenter;

            presenter?.Save();
        }

        public void SetupTabWithGrid(string[] pFiles)
        {
            foreach (var file in pFiles)
               SetupTabWithGrid(file);
        }

        public void SetupTabWithGrid(string pFileName)
        {
            var presenter = new STLEditorPresenter();
            if (presenter.Load(pFileName))
            {
                var tab = new STLTabPageView(presenter);
                var dataGrid = new DataGridView();
                SetupGrid(tab, dataGrid);
                tab.Controls.Add(dataGrid);
                tab.Controls[0].Dock = DockStyle.Fill;
                tab.Tag = presenter;
                _tabControl.TabPages.Add(tab);
                _tabControl.SelectedTab = tab;
            }
        }

        public void SetupGrid(STLTabPageView pTabPage, DataGridView pDataGridView)
        {
            if (!SystemInformation.TerminalServerSession)
            {
                var type = pDataGridView.GetType();
                var propertyInfo = type.GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                propertyInfo?.SetValue(pDataGridView, true, null);
            }

            pDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            var listTuple = pTabPage.Presenter.GetColumnsForLanguage(StringTableLanguage.English);

            foreach (var col in listTuple)
                pDataGridView.Columns.Add(col.colName, col.colDisplay);

            var arr = pTabPage.Presenter.GetRowsForLanguage(pDataGridView, StringTableLanguage.English).ToArray();
            pDataGridView.Rows.AddRange(arr);

            pDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            foreach (DataGridViewColumn column in pDataGridView.Columns)
            {
                if (string.Equals(column.Name, StringIdentifiers.ID))
                {
                    column.MinimumWidth = 45;
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                else if (string.Equals(column.Name, StringIdentifiers.KEY))
                {
                    column.MinimumWidth = 100;
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
                }
                else
                {
                    column.MinimumWidth = 125;
                    column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                    column.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
                }
            }
        }

        public void OnDragDrop(object pSender, DragEventArgs pEventArgs)
        {
            if (!pEventArgs.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            var files = (string[])pEventArgs.Data.GetData(DataFormats.FileDrop);

            try
            {
                SetupTabWithGrid(files);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void OnDragEnter(object pSender, DragEventArgs pEventArgs)
        {
            pEventArgs.Effect = pEventArgs.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Link : DragDropEffects.None;
        }


        private void OnTabControlMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                for (var i = 0; i < _tabControl.TabCount; i++)
                {
                    var rekt = _tabControl.GetTabRect(i);
                    if (rekt.Contains(e.Location))
                    {
                        _tabControl.SelectedIndex = i;

                        var contextMenu = BuildContextMenuForTab(_tabControl.SelectedTab);
                        contextMenu.Show(_tabControl, e.Location);
                        
                        break;
                    }
                }
            }
        }

        private void UpdateLanguage(object pSender, EventArgs pArgs)
        {
            if (_tabControl.SelectedTab is STLTabPageView tab && pSender is MenuItem menuItem)
            {
                var presenter = tab.Presenter;

                if (!presenter.ShowAllLanguages)
                {
                    if(string.Equals(menuItem.Text, presenter.SelectedLanguage.ToString()))
                        return;
                    if (string.Equals(menuItem.Text, "All Languages"))
                    {
                        //update to show all
                    }

                    //update to new language
                }

                if (presenter.ShowAllLanguages)
                {
                    if (string.Equals(menuItem.Text, "All Languages"))
                        return;

                    //update to specific language
                }
            }
        }

        private void OnCloseTab(object pSender, EventArgs pArgs)
        {
            var selectedTab = _tabControl.SelectedTab;
            _tabControl.TabPages.Remove(selectedTab);
            selectedTab.Dispose();
        }

        private ContextMenu BuildContextMenuForTab(TabPage pTab)
        {
            var tab = pTab as STLTabPageView;
            var contextMenu = new ContextMenu();

            contextMenu.MenuItems.Add("Save", OnSaveFile);
            contextMenu.MenuItems.Add("-");
            for(var i = 0; i < tab?.Presenter.LanguageCount; i++)
            {
                contextMenu.MenuItems.Add(((StringTableLanguage) i).ToString(), UpdateLanguage);
            }

            contextMenu.MenuItems.Add("All Languages");
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add("Close", OnCloseTab);

            return contextMenu;
        }
    }
}
