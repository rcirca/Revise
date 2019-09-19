using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Revise.Files.STL;

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

        }

        public void OnLoadFile(object sender, EventArgs pEvent)
        {

            var result =_openFileDialog.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            SetupTabWithGrid(_openFileDialog.FileNames);

            //try
            //{
            //    _presenter.Load(_openFileDialog.FileName);
            //    SetupGrid();
            //}
            //catch (Exception e)
            //{
            //    MessageBox.Show($"File format mismatch: {_openFileDialog.FileName}. \n Make sure it's a STL file", "Error",
            //        MessageBoxButtons.OK);
            //    Console.WriteLine(e.StackTrace);
            //}
        }

        public void SetupTabWithGrid(string[] pFiles)
        {
            foreach (var file in pFiles)
            {
                SetupTabWithGrid(file);
            }
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

            var listTuple = _presenter.GetDataGridViewColumns();

            foreach (var col in listTuple)
                pDataGridView.Columns.Add(col.colName, col.colDisplay);

            var arr = _presenter.GetDataGridViewRows(pDataGridView).ToArray();
            pDataGridView.Rows.AddRange(arr);

            pDataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            pDataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            foreach (DataGridViewColumn column in pDataGridView.Columns)
            {
                if (string.Equals(column.Name, "id"))
                    column.MinimumWidth = 45;
                else if (string.Equals(column.Name, "string_id"))
                    column.MinimumWidth = 100;
                else
                    column.MinimumWidth = 125;
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
