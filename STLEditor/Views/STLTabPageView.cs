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

        public STLTabPageView(STLEditorPresenter pPresenter) : base(pPresenter.Filename)
        {
            _stlPresenter = pPresenter;
            MouseClick += OnMouseClick;
            this.ContextMenu = new ContextMenu();
            ContextMenu.MenuItems.Add(new MenuItem("Hello"));
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                MessageBox.Show("RightClicked");
            }
        }

        public STLEditorPresenter Presenter => _stlPresenter;
    }
}
