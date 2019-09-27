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
using STLEditor.Util;

namespace STLEditor
{
    public class STLEditorPresenter
    {
        private readonly StringTableFile _stl;
        private string _fileName;
        public STLEditorPresenter(StringTableFile pStringTableFile)
        {
            _stl = pStringTableFile;
        }
        public StringTableFile STL => _stl;

        public List<StringTableRow> Rows => _stl.Rows;
        public List<StringTableKey> Keys => _stl.Keys;

        public bool Load(string pSTLFilePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(pSTLFilePath))
                    throw new ArgumentException($"{nameof(STLEditorPresenter)} path invalid {pSTLFilePath}");

                if (!File.Exists(pSTLFilePath))
                    throw new ArgumentException($"{nameof(STLEditorPresenter)} File doesn't exist: {pSTLFilePath}");

                _stl.Load(pSTLFilePath);
                _fileName = Path.GetFileName(pSTLFilePath);
                return true;
            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK);
                return false;
            }
        }

        public List<DataGridViewRow> GetRowsWithAllLanguages(DataGridView pDataGridView)
        {
            var list = new List<DataGridViewRow>();
            for (var i = 0; i < _stl.RowCount; i++)
            {
                var key = _stl.Keys[i];
                var row = _stl.Rows[i];

                var dataGridRow = new DataGridViewRow();
                dataGridRow.CreateCells(pDataGridView);
                dataGridRow.Cells[0].Value = key.ID;
                dataGridRow.Cells[1].Value = key.Key;
                var index = 2;
                for (var x = 0; x < _stl.LanguageCount; x++)
                {
                    var stringLang = (StringTableLanguage) x;
                    dataGridRow.Cells[index++].Value = row.GetText(stringLang);

                    if (_stl.TableType == StringTableType.Normal)
                        continue;

                    dataGridRow.Cells[index++].Value = row.GetDescription(stringLang);

                
                    if (_stl.TableType != StringTableType.Quest)
                    {
                        if (row.GetStartMessage().Length > 0 || row.GetEndMessage().Length > 0)
                            MessageBox.Show($"Start: {row.GetStartMessage()} End: {row.GetEndMessage()}", _fileName,
                                MessageBoxButtons.OK);
                        continue;
                    }

                    dataGridRow.Cells[index++].Value = row.GetStartMessage(stringLang);
                    dataGridRow.Cells[index++].Value = row.GetEndMessage(stringLang);
                }

                list.Add(dataGridRow);
            }
            return list;
        }

        public List<(string colName, string colDisplay)> GetColumnForIdAndKey()
        {
            var list = new List<(string, string)>();
            list.Add((StringIdentifiers.ID, "ID"));
            list.Add((StringIdentifiers.KEY, "Key"));
            return list;
        }

        public List<(string colName, string colDisplay)> GetColumnsForAllLanguages()
        {
            var list = GetColumnForIdAndKey();

            for (var i = 0; i < _stl.LanguageCount; i++)
            {
                var stringLanguage = ((StringTableLanguage) i).ToString();
                var columnName = $"{StringIdentifiers.LANGUAGE_TEXT}{stringLanguage}";
                var langFormat = $"Text ({stringLanguage})";
                list.Add((columnName, langFormat));

                if (_stl.TableType == StringTableType.Normal)
                    continue;

                columnName = $"{StringIdentifiers.DESCRIPTION_TEXT}{stringLanguage}";
                langFormat = $"Description ({stringLanguage})";
                list.Add((columnName, langFormat));

                if (_stl.TableType != StringTableType.Quest)
                    continue;

                columnName = $"{StringIdentifiers.START_MSG}{stringLanguage}";
                langFormat = $"Start Message ({stringLanguage})";
                list.Add((columnName, langFormat));

                columnName = $"{StringIdentifiers.END_MSG}{stringLanguage}";
                langFormat = $"End Message ({stringLanguage})";
                list.Add((columnName, langFormat));
            }

            return list;
        }

        public List<(string colName, string colDisplay)> GetColumnsForLanguage(StringTableLanguage pLanguage)
        {
            var list = GetColumnForIdAndKey();
            var stringLanguage = pLanguage.ToString();
            var columnNameText = $"{StringIdentifiers.LANGUAGE_TEXT}{stringLanguage}";
            var textLangFormat = $"Text ({stringLanguage})";

            list.Add((columnNameText, textLangFormat));

            if (_stl.TableType != StringTableType.Normal)
            {
                var columnDescText = $"{StringIdentifiers.DESCRIPTION_TEXT}{stringLanguage}";
                var descLangFormatFormat = $"Description ({stringLanguage})";

                list.Add((columnDescText, descLangFormatFormat));

                if (_stl.TableType == StringTableType.Quest)
                {
                    var colStartText = $"{StringIdentifiers.START_MSG}{stringLanguage}";
                    var colEndText = $"{StringIdentifiers.END_MSG}{stringLanguage}";
                    var startLangFormat = $"Start Message ({stringLanguage})";
                    var endLangFormat = $"End Message ({stringLanguage})";
                    list.Add((colStartText, startLangFormat));
                    list.Add((colEndText, endLangFormat));
                }
            }
            return list;
        }
    }
}
