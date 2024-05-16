using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Mids_Reborn.Forms.Controls
{
    public partial class AdvListView : ListView
    {
        public event EventHandler? DataSourceChanged;

        private object? _dataSource;

        [DefaultValue(null)]
        [RefreshProperties(RefreshProperties.Repaint)]
        [AttributeProvider(typeof(IListSource))]
        public object? DataSource
        {
            get => _dataSource;
            set
            {
                _dataSource = value;
                ColumnMappings = new List<ColumnMapping>();
                BindListView();
                DataSourceChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        internal List<ColumnMapping>? ColumnMappings { get; set; }
        
        public AdvListView()
        {
            InitializeComponent();
        }

        public void AddColumnMapping(int columnIndex, Func<object, object?> dataRetriever, Func<object?, object?>? transformer1 = null, Func<object?, object>? transformer2 = null, Func<object?, object?>? tagFunction = null)
        {
            ColumnMappings ??= new List<ColumnMapping>();

            // Check if the mapping for the specified column index already exists and update it if needed
            var existingMapping = ColumnMappings.FirstOrDefault(mapping => mapping.ColumnIndex == columnIndex);
            if (existingMapping != null)
            {
                existingMapping.DataRetriever = dataRetriever;
                existingMapping.Transformer1 = transformer1;
                existingMapping.Transformer2 = transformer2;
                existingMapping.TagFunction = tagFunction;
            }
            else
            {
                ColumnMappings.Add(new ColumnMapping
                {
                    ColumnIndex = columnIndex,
                    DataRetriever = dataRetriever,
                    Transformer1 = transformer1,
                    Transformer2 = transformer2,
                    TagFunction = tagFunction
                });
            }

            // Rebind the ListView to reflect the changes
            BindListView();
        }

        private void BindListView()
        {
            // Clear existing items in the ListView
            Items.Clear();

            // Check if there's no data source or no columns specified
            if (_dataSource == null || ColumnMappings == null || ColumnMappings.Count == 0)
            {
                return;
            }

            // Ensure that the data source is of type IEnumerable<object>
            if (_dataSource is not IEnumerable<object> dataObjects)
            {
                throw new ArgumentException("DataSource must implement IEnumerable<object>");
            }

            // Loop through each data object in the data source
            var enumerable = dataObjects.ToList();
            foreach (var dataObject in enumerable)
            {
                // Create a ListViewItem to represent this data object
                var item = new ListViewItem();
                
                // Loop through each column mapping
                foreach (var mapping in ColumnMappings)
                {
                    var columnIndex = mapping.ColumnIndex;

                    // Use the custom data retriever delegate to get the data from the object
                    var dataValue = mapping.DataRetriever(dataObject);
                    var transformedValue1 = mapping.Transformer1?.Invoke(dataValue) ?? dataValue;

                    string columnText;

                    if (mapping.Transformer2 != null)
                    {
                        // Use the second transformer if specified
                        var transformedValue2 = mapping.Transformer2(dataValue);
                        columnText = $"{transformedValue1} - {transformedValue2}";
                    }
                    else
                    {
                        columnText = transformedValue1?.ToString() ?? string.Empty;
                    }

                    // Add the column text to the ListViewItem
                    if (item.SubItems.Count <= columnIndex)
                    {
                        item.SubItems.Add(columnText);
                    }
                    else
                    {
                        item.SubItems[columnIndex].Text = columnText;
                    }

                    // Set the Tag value if applicable
                    if (mapping.TagFunction != null)
                    {
                        item.Tag = mapping.TagFunction(dataObject);
                    }
                }

                // Check to make sure the image list is not null and is not empty prior to assigning an image to an item
                if (SmallImageList is not null && SmallImageList.Images.Count > 0)
                {
                    var imageIndex = enumerable.IndexOf(dataObject);
                    if (imageIndex > -1) item.ImageIndex = imageIndex;
                }

                // Add the ListViewItem to the ListView
                Items.Add(item);
            }
        }

        internal class ColumnMapping
        {
            public int ColumnIndex { get; init; }
            public Func<object, object?> DataRetriever { get; set; }
            public Func<object?, object?>? Transformer1 { get; set; }
            public Func<object?, object>? Transformer2 { get; set; }
            public Func<object?, object?>? TagFunction { get; set; }
        }
    }
}
