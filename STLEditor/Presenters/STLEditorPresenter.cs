using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public void Load(string pSTLFilePath)
        {
            if (string.IsNullOrWhiteSpace(pSTLFilePath) || !File.Exists(pSTLFilePath))
                throw new ArgumentException($"{nameof(STLEditorPresenter)} path invalid or file doesn't exist");
            _stl.Load(pSTLFilePath);
        }

        public List<DataGridViewRow> GetDataGridViewRows()
        {
            var list = new List<DataGridViewRow>();

            return list;
        }

        public List<DataGridViewColumn> GetDataGridViewColumns()
        {
            var list = new List<DataGridViewColumn>();

            return list;
        }
    }
}
