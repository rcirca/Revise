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

        private void OnLoadFile(object sender, EventArgs pEvent)
        {

            var result =_openFileDialog.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            SetupTabWithGrid(_openFileDialog.FileNames);
            _saveButton.Enabled = _tabControl.TabCount > 0;
        }

        private void OnSaveFile(object sender, EventArgs pEvent)
        {
            ((STLTabPageView)_tabControl.SelectedTab)?.Presenter?.Save();
        }

        private void SetupTabWithGrid(string[] pFiles)
        {
            foreach (var file in pFiles)
               SetupTabWithGrid(file);
        }

        private void SetupTabWithGrid(string pFileName)
        {
            var presenter = new STLEditorPresenter();
            if (presenter.Load(pFileName))
            {
                var tab = new STLTabPageView(presenter);
                SetupGrid(tab);
                _tabControl.TabPages.Add(tab);
                _tabControl.SelectedTab = tab;
            }
        }

        private void SetupGrid(STLTabPageView pTabPage, StringTableLanguage pLanguage = StringTableLanguage.English)
        {
            var dataGrid = CreateDataGridWithDoubleBuffer();

            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            var listTuple = pTabPage.Presenter.GetColumnsForLanguage(pLanguage);

            foreach (var col in listTuple)
                dataGrid.Columns.Add(col.colName, col.colDisplay);

            var arr = pTabPage.Presenter.GetRowsForLanguage(dataGrid, pLanguage).ToArray();
            dataGrid.Rows.AddRange(arr);

            SetupColumns(dataGrid);

            pTabPage.DataGrid = dataGrid;
        }

        private void SetupGridForAllLanguages(STLTabPageView pTabPage)
        {
            var dataGrid = CreateDataGridWithDoubleBuffer();

            dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            var listTuple = pTabPage.Presenter.GetColumnsForAllLanguages();

            foreach (var col in listTuple)
                dataGrid.Columns.Add(col.colName, col.colDisplay);

            var arr = pTabPage.Presenter.GetRowsWithAllLanguages(dataGrid).ToArray();
            dataGrid.Rows.AddRange(arr);

            SetupColumns(dataGrid);

            pTabPage.DataGrid = dataGrid;
        }

        private void ResetGrid(STLTabPageView pTabPage)
        {
            pTabPage.Controls.Clear();
        }

        private void OnDragDrop(object pSender, DragEventArgs pEventArgs)
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

        private void OnDragEnter(object pSender, DragEventArgs pEventArgs)
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

        private void ChangeLanguage(STLTabPageView pTab, StringTableLanguage pLanguage)
        {

            var presenter = pTab.Presenter;

            if (pLanguage == presenter.SelectedLanguage)
                return;

            ResetGrid(pTab);
            SetupGrid(pTab, pLanguage);
        }

        private void ShowAllLanguages(STLTabPageView pTab)
        {
            var presenter = pTab.Presenter;
            if (presenter.ShowAllLanguages)
                return;

            ResetGrid(pTab);
            SetupGridForAllLanguages(pTab);
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
                var language = ((StringTableLanguage) i);
                contextMenu.MenuItems.Add(language.ToString(), (sender, e) => ChangeLanguage(tab, language));
            }

            contextMenu.MenuItems.Add("All Languages", (sender, e) => ShowAllLanguages(tab));
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add("Close", OnCloseTab);

            return contextMenu;
        }

        private DataGridView CreateDataGridWithDoubleBuffer()
        {
            var dataGrid = new DataGridView();
            if (!SystemInformation.TerminalServerSession)
            {
                var type = dataGrid.GetType();
                var propertyInfo = type.GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                propertyInfo?.SetValue(dataGrid, true, null);
            }

            return dataGrid;
        }

        private void SetupColumns(DataGridView pDataGrid)
        {

            pDataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;

            foreach (DataGridViewColumn column in pDataGrid.Columns)
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
    }
}
