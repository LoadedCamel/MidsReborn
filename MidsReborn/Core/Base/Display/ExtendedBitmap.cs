using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Mids_Reborn.Core.Base.Display
{
    /// <summary>
    /// A wrapper for System.Drawing.Bitmap that provides robust resource management,
    /// state tracking, and a correct cloning implementation.
    /// </summary>
    public class ExtendedBitmap : IDisposable, ICloneable
    {
        private Bitmap? _bits;
        private Graphics? _surface;
        private PropertyCache? _cache;
        private bool _isDisposed;
        private bool _isInitialized;
        private bool _isNew;

        #region Constructors

        /// <summary>
        /// Initializes an empty ExtendedBitmap, ready for lazy initialization.
        /// </summary>
        public ExtendedBitmap()
        {
            _cache = new PropertyCache();
            _isNew = true;
        }

        /// <summary>
        /// Initializes a new ExtendedBitmap with the specified dimensions.
        /// </summary>
        public ExtendedBitmap(int width, int height)
        {
            _cache = new PropertyCache { Size = new Size(width, height) };
            Initialize();
        }

        /// <summary>
        /// Initializes a new ExtendedBitmap with the specified dimensions.
        /// </summary>
        public ExtendedBitmap(Size imageSize) : this(imageSize.Width, imageSize.Height) { }

        /// <summary>
        /// Initializes a new ExtendedBitmap from an existing Bitmap object.
        /// </summary>
        public ExtendedBitmap(Bitmap bitmap)
        {
            ArgumentNullException.ThrowIfNull(bitmap);
            _cache = new PropertyCache();
            InitializeFromBitmap(bitmap, isFromNewSource: true);
        }

        /// <summary>
        /// Initializes a new ExtendedBitmap from an existing Image object.
        /// </summary>
        public ExtendedBitmap(Image image)
        {
            ArgumentNullException.ThrowIfNull(image);
            _cache = new PropertyCache();
            // Create a new Bitmap from the Image to ensure we have our own copy
            InitializeFromBitmap(new Bitmap(image), isFromNewSource: true);
        }

        /// <summary>
        /// Initializes a new ExtendedBitmap from a file path.
        /// </summary>
        public ExtendedBitmap(string file)
        {
            ArgumentNullException.ThrowIfNull(file);
            _cache = new PropertyCache();
            InitializeFromBitmap(new Bitmap(file), isFromNewSource: true);
        }

        /// <summary>
        /// Initializes a new ExtendedBitmap from a stream.
        /// </summary>
        public ExtendedBitmap(Stream stream)
        {
            ArgumentNullException.ThrowIfNull(stream);
            _cache = new PropertyCache();
            InitializeFromBitmap(new Bitmap(stream), isFromNewSource: true);
        }

        /// <summary>
        /// The copy constructor. Creates a deep clone of another ExtendedBitmap.
        /// </summary>
        public ExtendedBitmap(ExtendedBitmap other)
        {
            ArgumentNullException.ThrowIfNull(other);

            // If the source is uninitialized or disposed, create a similar empty object
            if (!other._isInitialized || other._isDisposed)
            {
                _cache = new PropertyCache();
                _isNew = true;
                return;
            }

            // Create a deep copy using the source bitmap's clone method
            var clonedBitmap = (Bitmap)other._bits!.Clone();
            _cache = new PropertyCache(other._cache!); // Use PropertyCache copy constructor

            InitializeFromBitmap(clonedBitmap, isFromNewSource: false);

            // Restore the state from the original
            _isInitialized = other._isInitialized;
            _isNew = other._isNew;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the underlying Bitmap object. Returns null if not initialized.
        /// </summary>
        public Bitmap? Bitmap => _bits;

        /// <summary>
        /// Gets the Graphics surface for drawing on the bitmap.
        /// Lazily initializes the bitmap if it hasn't been created yet.
        /// Accessing this property marks the bitmap as not new.
        /// </summary>
        public Graphics? Graphics
        {
            get
            {
                if (_isDisposed) return null;

                if (!_isInitialized && !Initialize())
                {
                    return null; // Initialization failed
                }

                // Create the Graphics object only if it doesn't already exist
                if (_surface == null)
                {
                    _surface = Graphics.FromImage(_bits!);
                    _surface.SmoothingMode = SmoothingMode.AntiAlias;
                    _surface.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    _surface.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    _surface.CompositingQuality = CompositingQuality.HighQuality;
                    _surface.Clip = new Region(_cache!.Bounds);
                    _cache!.UpdateFromGraphics(_surface);
                }

                _isNew = false;
                return _surface;
            }
        }

        /// <summary>
        /// Gets or sets the size of the bitmap. Setting a new size will re-initialize the bitmap.
        /// </summary>
        public Size Size
        {
            get => _cache?.Size ?? Size.Empty;
            set
            {
                if (_cache == null || value == _cache.Size) return;
                _cache.Size = value;
                Initialize();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Creates a deep clone of the object.
        /// </summary>
        public object Clone()
        {
            return new ExtendedBitmap(this);
        }

        /// <summary>
        /// Centralized method to create a blank bitmap based on properties in the cache.
        /// </summary>
        private bool Initialize()
        {
            if (_isDisposed || _cache is null) return false;
            if (_cache.Size.Width <= 0 || _cache.Size.Height <= 0) return false;

            var newBitmap = new Bitmap(_cache.Size.Width, _cache.Size.Height, _cache.BitDepth);
            InitializeFromBitmap(newBitmap, isFromNewSource: true);
            return true;
        }

        /// <summary>
        /// Centralized initialization logic. All constructors and methods that create or
        /// replace the bitmap should flow through here.
        /// </summary>
        private void InitializeFromBitmap(Bitmap newBitmap, bool isFromNewSource)
        {
            // Dispose previous resources
            _surface?.Dispose();
            _bits?.Dispose();

            // Set the new bitmap and create its graphics surface
            _bits = newBitmap;
            _surface = null;

            // Update the cache with properties from the new bitmap
            _cache!.UpdateFromBitmap(_bits);

            // Update state flags
            if (isFromNewSource)
            {
                _isNew = true;
            }
            _isInitialized = true;
        }

        #endregion

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed) return;

            if (disposing)
            {
                // Dispose managed resources
                _surface?.Dispose();
                _bits?.Dispose();
                _cache?.Dispose();
            }

            // Clear references and update state
            _surface = null;
            _bits = null;
            _cache = null;
            _isInitialized = false;
            _isDisposed = true;
        }

        #endregion

        #region Private Inner Class: PropertyCache

        private class PropertyCache : IDisposable
        {
            public PixelFormat BitDepth = PixelFormat.Format32bppArgb;
            public Rectangle Bounds;
            public Region? Clip;
            public Rectangle ClipRect;
            public Size Size;

            // Default constructor
            public PropertyCache() { }

            // Copy constructor for cloning
            public PropertyCache(PropertyCache other)
            {
                BitDepth = other.BitDepth;
                Bounds = other.Bounds;
                Clip = other.Clip?.Clone(); // Region must be cloned
                ClipRect = other.ClipRect;
                Size = other.Size;
            }

            public void UpdateFromBitmap(Bitmap bitmap)
            {
                Size = bitmap.Size;
                Bounds = new Rectangle(Point.Empty, Size);
                BitDepth = bitmap.PixelFormat;
            }

            public void UpdateFromGraphics(Graphics graphics)
            {
                Clip?.Dispose();
                Clip = graphics.Clip;
                ClipRect = Rectangle.Truncate(graphics.ClipBounds);
            }

            public void Dispose()
            {
                Clip?.Dispose();
            }
        }

        #endregion
    }
}