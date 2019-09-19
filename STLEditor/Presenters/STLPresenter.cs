using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Revise.Files;
using Revise.Files.STL;

namespace STLEditor
{
    public class STLEditorPresenter
    {
        private StringTableFile _stl;
        private const string HARD_CODED_STL_FILE = "Files/STR_ITEMTYPE.STL";
        private const string LANG_6 = "Files/List_EventString_6_Languages.STL";
        public STLEditorPresenter(StringTableFile pStringTableFile)
        {
            _stl = pStringTableFile;
        }

        public int RowCount => _stl.RowCount;

        public List<StringTableRow> Rows => _stl.Rows;
        public List<StringTableKey> Keys => _stl.Keys;

        public void LoadFile()
        {
            var stream = File.OpenRead(LANG_6);

            _stl = new StringTableFile();
            _stl.Load(stream);
        }

        public bool Load(string pSTLFilePath)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(pSTLFilePath))
                    throw new ArgumentException($"{nameof(STLEditorPresenter)} path invalid {pSTLFilePath}");

                if (!File.Exists(pSTLFilePath))
                    throw new ArgumentException($"{nameof(STLEditorPresenter)} File doesn't exist: {pSTLFilePath}");

                _stl.Load(pSTLFilePath);
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK);
                return false;
            }
        }

        public List<DataGridViewRow> GetDataGridViewRows(DataGridView pDataGridView)
        {
            var list = new List<DataGridViewRow>();
            for (var i = 0; i < _stl.RowCount; i++)
            {
                var key = _stl.Keys[i];
                var row = _stl.Rows[i];
                if (!string.IsNullOrWhiteSpace(row.GetStartMessage()))
                {

                }

                if (!string.IsNullOrWhiteSpace(row.GetEndMessage()))
                {

                }

                if (!string.IsNullOrWhiteSpace(row.GetDescription()))
                {

                }
                var dataGridRow = new DataGridViewRow();
                dataGridRow.CreateCells(pDataGridView);
                dataGridRow.Cells[0].Value = key.ID;
                dataGridRow.Cells[1].Value = key.Key;
                for (var j = 0; j < _stl.LanguageCount; j++)
                {
                    dataGridRow.Cells[j + 2].Value = row.GetText((StringTableLanguage) j);
                }

                list.Add(dataGridRow);
            }
            return list;
        }

        public List<(string colName, string colDisplay)> GetDataGridViewColumns()
        {
            var list = new List<(string, string)>();
            list.Add(("id", "ID"));
            list.Add(("string_id", "Key"));
            for (var i = 0; i < _stl.LanguageCount; i++)
            {
                var columnName = $"Language_{i}";
                var langFormat = $"Language({((StringTableLanguage) i).ToString()})";
                list.Add((columnName, langFormat));
            }

            return list;
        }
        
    }
}
