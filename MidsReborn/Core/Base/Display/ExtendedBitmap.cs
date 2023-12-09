using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Mids_Reborn.Core.Base.Display
{
    public class ExtendedBitmap : IDisposable, ICloneable
    {
        private Bitmap? _bits;
        private bool _isDisposed;
        private bool _isInitialized;
        private bool _isNew;

        private Graphics? _surface;
        private PropertyCache? _cache;

        public ExtendedBitmap()
        {
            _cache = new PropertyCache();
            _isNew = true;
            _isInitialized = false;
        }

        public ExtendedBitmap(Size imageSize)
        {
            _cache = new PropertyCache
            {
                Size = imageSize
            };
            Initialize();
        }

        public ExtendedBitmap(int width, int height)
        {
            _cache = new PropertyCache
            {
                Size = new Size(width, height)
            };
            Initialize();
        }

        public ExtendedBitmap(string file)
        {
            _cache = new PropertyCache();
            Initialize(file);
        }

        public ExtendedBitmap(Bitmap bitmap) => _bits = bitmap;

        public ExtendedBitmap(Image image)
        {
            _cache = new PropertyCache
            {
                Size = image.Size,
                BitDepth = image.PixelFormat
            };
            Initialize(image);
        }

        public ExtendedBitmap(Stream stream)
        {
            _cache = new PropertyCache();
            Initialize(stream);
        }

        public Graphics? Graphics
        {
            get
            {
                Graphics graphics;
                if (_isInitialized)
                {
                    _isNew = false;
                    graphics = _surface;
                }
                else if (Initialize())
                {
                    _isNew = false;
                    graphics = _surface;
                }
                else
                {
                    graphics = null;
                }
                return graphics;
            }
        }

        private bool CanInitialize
        {
            get
            {
                if (_isDisposed)
                {
                    return false;
                }

                if ((_cache!.Size.Width > 0) & (_cache.Size.Height > 0))
                {
                    return true;
                }

                if (!((_cache.Bounds.Width > 0) & (_cache.Bounds.Height > 0))) return false;
                _cache.Size.Width = _cache.Bounds.Width;
                _cache.Size.Height = _cache.Bounds.Height;
                return true;
            }
        }

        public Bitmap? Bitmap => !_isInitialized ? Initialize() ? _bits : null : _bits; 

        private Region Clip
        {
            get => (_isInitialized ? _cache!.Clip : new Region())!;
            set
            {
                if (!_isInitialized) return;
                if (_surface != null)
                {
                    _surface.Clip = value;
                    _cache?.Update(ref _surface);
                }
                _isNew = false;
            }
        }

        public Rectangle ClipRect => _isInitialized ? _cache!.ClipRect : new Rectangle();

        public Size Size
        {
            get => _isInitialized ? _cache!.Size : new Size();
            set
            {
                if (value.Width == _cache!.Size.Width && value.Height == _cache.Size.Height) return;
                _cache.Size = value;
                Initialize();
            }
        }

        private bool Initialize()
        {
            if (!CanInitialize) return false;
            _surface?.Dispose();
            _bits?.Dispose();
            _bits = new Bitmap(_cache!.Size.Width, _cache.Size.Height, _cache.BitDepth);
            _surface = Graphics.FromImage(_bits);
            _cache.Update(ref _bits);
            _surface.Clip = new Region(_cache.Bounds);
            _cache.Update(ref _surface);
            _isNew = true;
            _isInitialized = true;
            return true;
        }

        private void Initialize(Image file)
        {
            _surface?.Dispose();
            _bits?.Dispose();
            _bits = new Bitmap(file);
            _surface = Graphics.FromImage(_bits);
            _cache?.Update(ref _bits);
            _surface.Clip = new Region(_cache!.Bounds);
            _cache.Update(ref _surface);
            _isNew = true;
            _isInitialized = true;
        }

        private void Initialize(string file)
        {
            _surface?.Dispose();
            _bits?.Dispose();
            _bits = new Bitmap(file);
            _surface = Graphics.FromImage(_bits);
            _cache?.Update(ref _bits);
            _surface.Clip = new Region(_cache!.Bounds);
            _cache.Update(ref _surface);
            _isNew = true;
            _isInitialized = true;
        }

        private void Initialize(Stream stream)
        {
            _surface?.Dispose();
            _bits?.Dispose();
            _bits = new Bitmap(stream);
            _surface = Graphics.FromImage(_bits);
            _cache?.Update(ref _bits);
            _surface.Clip = new Region(_cache!.Bounds);
            _cache.Update(ref _surface);
            _isNew = true;
            _isInitialized = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed) return;
            _isNew = false;
            _isInitialized = false;
            _surface?.Dispose();
            _bits?.Dispose();
            _cache?.Clip?.Dispose();
            _isDisposed = true;
        }

        public object Clone()
        {
            object obj;
            if (!_isInitialized)
            {
                obj = new ExtendedBitmap();
            }
            else
            {
                var bitmapExt = new ExtendedBitmap(Size)
                {
                    _cache = _cache
                };
                if (_bits == null) return new ExtendedBitmap();
                bitmapExt._surface?.DrawImageUnscaled(_bits, new Point(0, 0));
                bitmapExt.Clip = Clip;
                bitmapExt._isInitialized = _isInitialized;
                bitmapExt._isNew = _isNew;
                obj = bitmapExt;

            }

            return obj;

        }

        private class PropertyCache
        {
            private Point _location;

            public PixelFormat BitDepth = PixelFormat.Format32bppArgb;
            public Rectangle Bounds;
            public Region? Clip;
            public Rectangle ClipRect;
            public Size Size;

            public void Update(ref Bitmap args)
            {
                Size = args.Size;
                _location = new Point(0, 0);
                Bounds = new Rectangle(_location, Size);
                BitDepth = args.PixelFormat;
            }

            public void Update(ref Graphics args)
            {
                Clip?.Dispose();
                Clip = args.Clip;
                ClipRect = RectConvert(args.ClipBounds);
            }

            private static Rectangle RectConvert(RectangleF iRect)
            {
                return new Rectangle(
                    iRect.X <= 2147483648.0
                        ? iRect.X >= (double) int.MinValue ? Convert.ToInt32(iRect.X) : int.MinValue
                        : int.MaxValue,
                    iRect.Y <= 2147483648.0
                        ? iRect.Y >= (double) int.MinValue ? Convert.ToInt32(iRect.Y) : int.MinValue
                        : int.MaxValue,
                    iRect.Width <= 2147483648.0
                        ? iRect.Width >= (double) int.MinValue ? Convert.ToInt32(iRect.Width) : int.MinValue
                        : int.MaxValue,
                    iRect.Height <= 2147483648.0
                        ? iRect.Height >= (double) int.MinValue ? Convert.ToInt32(iRect.Height) : int.MinValue
                        : int.MaxValue);
            }
        }
    }
}
