using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using Revise.Files.STL;
using STLEditor.Util;

namespace STLEditor
{
    public partial class STLEditorView : Form
    {
        private STLEditorPresenter _presenter;
        public STLEditorView()
        {
            InitializeComponent();
            _presenter = new STLEditorPresenter(new StringTableFile());
            _openFileDialog.Multiselect = true;
            _saveButton.Enabled = false;
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
            if (_presenter.Load(pFileName))
            {
                var tab = new TabPage(Path.GetFileName(pFileName));
                var dataGrid = new DataGridView();
                SetupGrid(dataGrid);
                tab.Controls.Add(dataGrid);
                tab.Controls[0].Dock = DockStyle.Fill;
                tab.Tag = _presenter;
                
                _tabControl.TabPages.Add(tab);
                _tabControl.SelectTab(tab);
            }
        }

        public void SetupGrid(DataGridView pDataGridView)
        {
            if (!SystemInformation.TerminalServerSession)
            {
                var type = pDataGridView.GetType();
                var propertyInfo = type.GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                propertyInfo?.SetValue(pDataGridView, true, null);
            }

            pDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

            var listTuple = _presenter.GetColumnsForLanguage(StringTableLanguage.English);

            foreach (var col in listTuple)
                pDataGridView.Columns.Add(col.colName, col.colDisplay);

            var arr = _presenter.GetRowsForLanguage(pDataGridView, StringTableLanguage.English).ToArray();
            pDataGridView.Rows.AddRange(arr);

            pDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            pDataGridView.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
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
    }
}
