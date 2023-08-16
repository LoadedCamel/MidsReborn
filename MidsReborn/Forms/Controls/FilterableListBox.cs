using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class FilterableListBox : ListBox
    {
        private IBindingList? _originalDataSource;
        private Func<object, bool>? _filterPredicate;
        private bool _filtered;

        public FilterableListBox()
        {
            InitializeComponent();
        }

        public void SetDataSource<T>(IEnumerable<T> dataSource)
        {
            _originalDataSource = dataSource as IBindingList;
            Filter();
        }

        public void ApplyFilter(Func<object, bool> predicate)
        {
            _filterPredicate = predicate;
            Filter();
        }

        public void ClearFilter()
        {
            if (!_filtered) return;
            DataSource = _originalDataSource;
            _filtered = false;
        }

        private void Filter()
        {
            if (_originalDataSource == null) return;
            if (_filterPredicate == null)
            {
                DataSource = _originalDataSource;
            }
            else
            {
                var filteredItems = _originalDataSource.Cast<object>().Where(_filterPredicate).ToList();
                DataSource = filteredItems;
                _filtered = true;
            }
        }

        protected override void OnDataSourceChanged(EventArgs e)
        {
            base.OnDataSourceChanged(e);

            if (DataSource is IBindingList bindingList)
            {
                bindingList.ListChanged += (sender, args) => Filter();
            }
        }
    }
}
