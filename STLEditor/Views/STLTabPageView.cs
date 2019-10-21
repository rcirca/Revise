using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STLEditor.Views
{
    public class STLTabPageView : TabPage
    {
        private STLEditorPresenter _stlPresenter;

        private DataGridView _dataGrid;

        public STLTabPageView(STLEditorPresenter pPresenter) : base(pPresenter.Filename)
        {
            _stlPresenter = pPresenter;
        }

        public STLEditorPresenter Presenter => _stlPresenter;

        public DataGridView DataGrid
        {
            get => _dataGrid;

            set
            {
                if(Controls.Count > 0)
                    Controls.Clear();

                Controls.Add(value);
                Controls[0].Dock = DockStyle.Fill;
            }
        }
    }
}
