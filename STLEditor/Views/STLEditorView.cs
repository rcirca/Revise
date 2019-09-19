using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private const string HARD_CODED_STL_FILE = "Files/STR_ITEMTYPE.STL";
        private const string LANG_6 = "Files/List_EventString_6_Languages";
        private STLEditorPresenter _presenter;
        public STLEditorView()
        {
            InitializeComponent();
            if (!SystemInformation.TerminalServerSession)
            {
                Type dgvType = _stlDataGrid.GetType();
                PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                    BindingFlags.Instance | BindingFlags.NonPublic);
                pi.SetValue(_stlDataGrid, true, null);
            }
            _presenter = new STLEditorPresenter(new StringTableFile());

        }

        public void OnLoadFile(object sender, EventArgs pEvent)
        {
            var result =_openFileDialog.ShowDialog(this);
            if (result != DialogResult.OK)
                return;
            try
            {
                _presenter.Load(_openFileDialog.FileName);
                UpdateGrid();
            }
            catch (Exception e)
            {
                MessageBox.Show($"File format mismatch: {_openFileDialog.FileName}. \n Make sure it's a STL file", "Error",
                    MessageBoxButtons.OK);
                Console.WriteLine(e.StackTrace);
            }
        }

        public void UpdateGrid()
        {
            _stlDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            _stlDataGrid.Columns.Add("ID", "ID");
            _stlDataGrid.Columns.Add("Key", "String ID");
            var maxLangCount = 0;
            var rowData = new List<string>();
            for (var i = 0; i < _presenter.RowCount; i++)
            {
                var row = _presenter.Rows;
                var key = _presenter.Keys;
                rowData.Add($"{key[i].ID}");
                rowData.Add(key[i].Key);
                PopulateLanguages(rowData, row[i], ref maxLangCount);

                _stlDataGrid.Rows.Add(rowData.ToArray());
                rowData.Clear();
            }

            _stlDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            _stlDataGrid.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.ColumnHeader);
            foreach (DataGridViewColumn column in _stlDataGrid.Columns)
            {
                if (string.Equals(column.Name, "ID"))
                    column.MinimumWidth = 45;
                else if (string.Equals(column.Name, "Key"))
                    column.MinimumWidth = 100;
                else
                    column.MinimumWidth = 125;
            }
        }

        private void PopulateLanguages(List<string> pRowData, StringTableRow pRow, ref int pMaxLangCount)
        {
            var list = pRow.GetAllTextLanguage();
            var langCount = 0;
            foreach (var lang in list)
            {
                if (pMaxLangCount < list.Count)
                {
                    _stlDataGrid.Columns.Add($"Lang{langCount}", $"Text ({((StringTableLanguage)langCount).ToString()})");
                    pMaxLangCount++;
                }
                pRowData.Add(lang);
                langCount++;
            }
        }

        private void PopulateDescription()
        {

        }

        private void BuildRows()
        {

        }
    }
}
