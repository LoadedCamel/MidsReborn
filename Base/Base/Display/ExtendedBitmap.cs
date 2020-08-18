using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Base.Display
{
    public class ExtendedBitmap : IDisposable, ICloneable
    {
        private Bitmap _bits;
        private bool _isDisposed;

        private bool _isInitialised;
        private bool _isNew;

        private Graphics _surface;
        private Bitmap bitmap;

        protected PropertyCache Cache;

        private ExtendedBitmap()

        {
            Cache = new PropertyCache();
            _isNew = true;
            _isInitialised = false;
        }

        public ExtendedBitmap(Size imageSize)
        {
            Cache = new PropertyCache
            {
                Size = imageSize
            };
            Initialise();
        }

        public ExtendedBitmap(int x, int y)
        {
            Cache = new PropertyCache
            {
                Size = new Size(x, y)
            };
            Initialise();
        }

        public ExtendedBitmap(string fileName)
        {
            Cache = new PropertyCache();
            Initialise(fileName);
        }

        public ExtendedBitmap(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        public Graphics Graphics
        {
            get
            {
                Graphics graphics;
                if (_isInitialised)
                {
                    _isNew = false;
                    graphics = _surface;
                }
                else if (Initialise())
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

        public Bitmap Bitmap => !_isInitialised ? Initialise() ? _bits : null : _bits;

        private bool CanInitialise

        {
            get
            {
                bool flag;
                if (_isDisposed)
                {
                    flag = false;
                }
                else if ((Cache.Size.Width > 0) & (Cache.Size.Height > 0))
                {
                    flag = true;
                }
                else if ((Cache.Bounds.Width > 0) & (Cache.Bounds.Height > 0))
                {
                    Cache.Size.Width = Cache.Bounds.Width;
                    Cache.Size.Height = Cache.Bounds.Height;
                    flag = true;
                }
                else
                {
                    flag = false;
                }

                return flag;
            }
        }

        public Size Size
        {
            get => _isInitialised ? Cache.Size : new Size();
            set
            {
                if (value.Width == Cache.Size.Width && value.Height == Cache.Size.Height)
                    return;
                Cache.Size = value;
                Initialise();
            }
        }

        private Region Clip

        {
            get => _isInitialised ? Cache.Clip : new Region();
            set
            {
                if (!_isInitialised)
                    return;
                _surface.Clip = value;
                Cache.Update(ref _surface);
                _isNew = false;
            }
        }

        public Rectangle ClipRect => _isInitialised ? Cache.ClipRect : new Rectangle();

        public object Clone()
        {
            object obj;
            if (!_isInitialised)
            {
                obj = new ExtendedBitmap();
            }
            else
            {
                var extendedBitmap = new ExtendedBitmap(Size)
                {
                    Cache = Cache
                };
                extendedBitmap._surface.DrawImageUnscaled(_bits, new Point(0, 0));
                extendedBitmap.Clip = Clip;
                extendedBitmap._isInitialised = _isInitialised;
                extendedBitmap._isNew = _isNew;
                obj = extendedBitmap;
            }

            return obj;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing || _isDisposed)
                return;
            _isNew = false;
            _isInitialised = false;
            _surface?.Dispose();
            _bits?.Dispose();
            Cache.Clip?.Dispose();
            _isDisposed = true;
        }

        private bool Initialise()

        {
            bool flag;
            if (!CanInitialise)
            {
                flag = false;
            }
            else
            {
                _surface?.Dispose();
                _bits?.Dispose();
                _bits = new Bitmap(Cache.Size.Width, Cache.Size.Height, Cache.BitDepth);
                _surface = Graphics.FromImage(_bits);
                Cache.Update(ref _bits);
                _surface.Clip = new Region(Cache.Bounds);
                Cache.Update(ref _surface);
                _isNew = true;
                _isInitialised = true;
                flag = true;
            }

            return flag;
        }

        private void Initialise(string fileName)

        {
            _surface?.Dispose();
            _bits?.Dispose();
            if (!File.Exists(fileName))
            {
                Cache = new PropertyCache
                {
                    Size = new Size(32, 32)
                };
                Initialise();
            }
            else
            {
                using (var ms = new MemoryStream(File.ReadAllBytes(fileName)))
                {
                    _bits = new Bitmap(ms);
                    _surface = Graphics.FromImage(_bits);
                }

                Cache.Update(ref _bits);
                _surface.Clip = new Region(Cache.Bounds);
                Cache.Update(ref _surface);
                _isNew = true;
                _isInitialised = true;
            }
        }

        protected class PropertyCache
        {
            private Point _location;
            public PixelFormat BitDepth = PixelFormat.Format32bppArgb;

            public Rectangle Bounds;
            public Region Clip;
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
                    (double) iRect.X <= 2147483648.0
                        ? (double) iRect.X >= (double) int.MinValue ? Convert.ToInt32(iRect.X) : int.MinValue
                        : int.MaxValue,
                    (double) iRect.Y <= 2147483648.0
                        ? (double) iRect.Y >= (double) int.MinValue ? Convert.ToInt32(iRect.Y) : int.MinValue
                        : int.MaxValue,
                    (double) iRect.Width <= 2147483648.0
                        ? (double) iRect.Width >= (double) int.MinValue ? Convert.ToInt32(iRect.Width) : int.MinValue
                        : int.MaxValue,
                    (double) iRect.Height <= 2147483648.0
                        ? (double) iRect.Height >= (double) int.MinValue ? Convert.ToInt32(iRect.Height) : int.MinValue
                        : int.MaxValue);
            }
        }
    }
}