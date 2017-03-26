using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using SharpDX.DXGI;
using AlphaMode = SharpDX.Direct2D1.AlphaMode;
using Factory = SharpDX.Direct2D1.Factory;
using FontFactory = SharpDX.DirectWrite.Factory;
using FontStyle = SharpDX.DirectWrite.FontStyle;
using TextAntialiasMode = SharpDX.Direct2D1.TextAntialiasMode;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace Control.Overlay
{
    internal class Direct2DRenderer
    {
        //caching used brushes and fonts
        private List<SolidColorBrush> _brushContainer = new List<SolidColorBrush>(32);
        private List<TextFormat> _fontContainer = new List<TextFormat>(32);

        private List<LayoutBuffer> _layoutContainer = new List<LayoutBuffer>(32);

        //pre allocating memory depending on last buffer usage
        //better performance when allocating new font's and brushes within the render loop
        public int BufferBrushSize { get; private set; }
        public int BufferFontSize { get; private set; }
        public int BufferLayoutSize { get; private set; }

        //thread safe resizing
        private bool _doResize;
        private int _resizeX;
        private int _resizeY;

        //transparent background color
        private static readonly SharpDX.Color4 Transparent = new SharpDX.Color4(SharpDX.Color.Transparent.R, SharpDX.Color.Transparent.G, SharpDX.Color.Transparent.B, SharpDX.Color.Transparent.A);

        //direct x vars
        private readonly WindowRenderTarget _device;
        private readonly FontFactory _fontFactory;
        private readonly Factory _factory;

        public Direct2DRenderer(IntPtr hwnd, bool limitFps)
        {
            _factory = new Factory();

            _fontFactory = new FontFactory();

            RECT bounds;//immer 1920x1080. resizing muss durch die Overlay klasse geregelt sein
            NativeMethods.GetWindowRect(hwnd, out bounds);

            var targetProperties = new HwndRenderTargetProperties
            {
                Hwnd = hwnd,
                PixelSize = new Size2(bounds.Right - bounds.Left, bounds.Bottom - bounds.Top),
                PresentOptions = limitFps ? PresentOptions.None : PresentOptions.Immediately //Immediatly -> Zeichnet sofort ohne auf 60fps zu locken. None lockt auf 60fps
            };

            var prop = new RenderTargetProperties(RenderTargetType.Hardware, new PixelFormat(Format.B8G8R8A8_UNorm, AlphaMode.Premultiplied), 0, 0, RenderTargetUsage.None, FeatureLevel.Level_DEFAULT);

            _device = new WindowRenderTarget(_factory, prop, targetProperties)
            {
                TextAntialiasMode = TextAntialiasMode.Cleartype,
                AntialiasMode = AntialiasMode.PerPrimitive
            };
        }

        /// <summary>
        /// Do not call if you use OverlayWindow class
        /// </summary>
        public void Dispose()
        {
            DeleteBrushContainer();
            DeleteFontContainer();
            DeleteLayoutContainer();

            _brushContainer = null;
            _fontContainer = null;
            _layoutContainer = null;

            _fontFactory.Dispose();
            _factory.Dispose();
            _device.Dispose();
        }

        /// <summary>
        /// tells renderer to resize when possible
        /// </summary>
        /// <param name="x">Width</param>
        /// <param name="y">Height</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AutoResize(int x, int y)
        {
            _doResize = true;
            _resizeX = x;
            _resizeY = y;
        }

        #region Ressource Management
        /// <summary>
        /// Call this after EndScene if you created brushes within a loop
        /// </summary>
        public void DeleteBrushContainer()
        {
            BufferBrushSize = _brushContainer.Count;
            for (var i = 0; i < _brushContainer.Count; i++)
            {
                _brushContainer[i].Dispose();
            }
            _brushContainer = new List<SolidColorBrush>(BufferBrushSize);
        }
        /// <summary>
        /// Call this after EndScene if you created fonts within a loop
        /// </summary>
        public void DeleteFontContainer()
        {
            BufferFontSize = _fontContainer.Count;
            for (var i = 0; i < _fontContainer.Count; i++)
            {
                _fontContainer[i].Dispose();
            }
            _fontContainer = new List<TextFormat>(BufferFontSize);
        }
        /// <summary>
        /// Call this after EndScene if you changed your text's font or have problems with huge memory usage
        /// </summary>
        public void DeleteLayoutContainer()
        {
            BufferLayoutSize = _layoutContainer.Count;
            for (var i = 0; i < _layoutContainer.Count; i++)
            {
                _layoutContainer[i].Dispose();
            }
            _layoutContainer = new List<LayoutBuffer>(BufferLayoutSize);
        }

        /// <summary>
        /// Creates a new SolidColorBrush
        /// </summary>
        /// <param name="color">0x7FFFFFF Premultiplied alpha color</param>
        /// <returns>int Brush identifier</returns>
        public int CreateBrush(int color)
        {
            _brushContainer.Add(new SolidColorBrush(_device, new SharpDX.Color4(color >> 16 & 255L, color >> 8 & 255L, (byte)color & 255L, color >> 24 & 255L)));
            return _brushContainer.Count - 1;
        }
        /// <summary>
        /// Creates a new SolidColorBrush. Make sure you applied an alpha value
        /// </summary>
        /// <param name="color">System.Drawing.Color struct</param>
        /// <returns>int Brush identifier</returns>
        public int CreateBrush(SharpDX.Color color)
        {
            //if (color.A == 0)
            //    color = Color.FromArgb(255, color);

            _brushContainer.Add(new SolidColorBrush(_device, new SharpDX.Color4(color.R, color.G, color.B, color.A / 255.0f)));
            return _brushContainer.Count - 1;
        }

        /// <summary>
        /// Creates a new Font
        /// </summary>
        /// <param name="fontFamilyName">i.e. Arial</param>
        /// <param name="size">size in units</param>
        /// <param name="bold">print bold text</param>
        /// <param name="italic">print italic text</param>
        /// <returns></returns>
        public int CreateFont(string fontFamilyName, float size, bool bold = false, bool italic = false)
        {
            _fontContainer.Add(new TextFormat(_fontFactory, fontFamilyName, bold ? FontWeight.Bold : FontWeight.Normal, italic ? FontStyle.Italic : FontStyle.Normal, size));
            return _fontContainer.Count - 1;
        }
        #endregion

        #region Scene related
        /// <summary>
        /// Do your drawing after this
        /// </summary>
        public void BeginScene()
        {
            if (_doResize)
            {
                _device.Resize(new Size2(_resizeX, _resizeY));

                _doResize = false;
            }
            _device.BeginDraw();
        }
        /// <summary>
        /// Present frame. Do not draw after this.
        /// </summary>
        public void EndScene()
        {
            _device.EndDraw();
            if (!_doResize) return;

            _device.Resize(new Size2(_resizeX, _resizeY));
            _doResize = false;
        }
        /// <summary>
        /// Clears the frame
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearScene()
        {
            //var color = System.Drawing.Color.Transparent;
            _device.Clear(Transparent);//new RawColor4(color.R, color.G, color.B, color.A));
        }
        #endregion

        #region Drawing stuff
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawLine(float startX, float startY, float endX, float endY, float stroke, int brush)
        {
            _device.DrawLine(new SharpDX.Vector2(startX, startY), new SharpDX.Vector2(endX, endY), _brushContainer[brush], stroke);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawRectangle(float x, float y, float width, float height, float stroke, int brush)
        {
            _device.DrawRectangle(new SharpDX.RectangleF(x, y, x + width, y + height), _brushContainer[brush], stroke);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void DrawCircle(float x, float y, float radius, float stroke, int brush)
        {
            _device.DrawEllipse(new Ellipse(new SharpDX.Vector2(x, y), radius, radius), _brushContainer[brush], stroke);
        }

        public void DrawBox2D(float x, float y, float width, float height, float stroke, int brush, int interiorBrush)
        {
            _device.DrawRectangle(new SharpDX.RectangleF(x, y, x + width, y + height), _brushContainer[brush], stroke);
            _device.FillRectangle(new SharpDX.RectangleF(x + stroke, y + stroke, x + width - stroke, y + height - stroke), _brushContainer[interiorBrush]);
        }

        public void DrawBox3D(int x, int y, int width, int height, int length, float stroke, int brush, int interiorBrush)
        {
            var first = new SharpDX.RectangleF(x, y, x + width, y + height);
            var second = new SharpDX.RectangleF(x + length, y - length, first.Right + length, first.Bottom - length);

            var lineStart = new Vector2(x, y);
            var lineEnd = new Vector2(second.Left, second.Top);

            _device.DrawRectangle(first, _brushContainer[brush], stroke);
            _device.DrawRectangle(second, _brushContainer[brush], stroke);

            _device.FillRectangle(first, _brushContainer[interiorBrush]);
            _device.FillRectangle(second, _brushContainer[interiorBrush]);

            //up left line
            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            //up right
            lineStart.X += width;
            lineEnd.X = lineStart.X + length;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            //down rigth
            lineStart.Y += height;
            lineEnd.Y += height;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            //down left
            lineStart.X -= width;
            lineEnd.X -= width;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);
        }

        public void DrawRectangle3D(int x, int y, int width, int height, int length, float stroke, int brush)
        {
            var first = new SharpDX.RectangleF(x, y, x + width, y + height);
            var second = new SharpDX.RectangleF(x + length, y - length, first.Right + length, first.Bottom - length);

            var lineStart = new Vector2(x, y);
            var lineEnd = new Vector2(second.Left, second.Top);

            _device.DrawRectangle(first, _brushContainer[brush], stroke);
            _device.DrawRectangle(second, _brushContainer[brush], stroke);

            //up left line
            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            //up right
            lineStart.X += width;
            lineEnd.X = lineStart.X + length;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            //down rigth
            lineStart.Y += height;
            lineEnd.Y += height;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);

            //down left
            lineStart.X -= width;
            lineEnd.X -= width;

            _device.DrawLine(lineStart, lineEnd, _brushContainer[brush], stroke);
        }

        public void DrawPlus(float x, float y, int length, float stroke, int brush)
        {
            //left to right
            var first = new Vector2(x - length, y);
            var second = new Vector2(x + length, y);

            //top down
            var third = new Vector2(x, y - length);
            var fourth = new Vector2(x, y + length);

            _device.DrawLine(first, second, _brushContainer[brush], stroke);
            _device.DrawLine(third, fourth, _brushContainer[brush], stroke);
        }

        public void DrawEdge(float x, float y, float width, float height, int length, float stroke, int brush)//geht
        {
            //upper left edge
            var first = new Vector2(x, y);
            var second = new Vector2(x, y + length);
            var third = new Vector2(x + length, y);

            _device.DrawLine(first, second, _brushContainer[brush], stroke);
            _device.DrawLine(first, third, _brushContainer[brush], stroke);

            //down left edge
            first.Y += height;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X + length;

            _device.DrawLine(first, second, _brushContainer[brush], stroke);
            _device.DrawLine(first, third, _brushContainer[brush], stroke);

            //up right edge
            first.X = x + width;
            first.Y = y;
            second.X = first.X - length;
            second.Y = first.Y;
            third.X = first.X;
            third.Y = first.Y + length;

            _device.DrawLine(first, second, _brushContainer[brush], stroke);
            _device.DrawLine(first, third, _brushContainer[brush], stroke);

            //down right edge
            first.Y += height;
            second.X += length;
            second.Y = first.Y - length;
            third.Y = first.Y;
            third.X = first.X - length;

            _device.DrawLine(first, second, _brushContainer[brush], stroke);
            _device.DrawLine(first, third, _brushContainer[brush], stroke);
        }

        public void DrawBarVertical(int x, int y, int width, int height, float value, float stroke, int brush, int interiorBrush)
        {
            var first = new SharpDX.RectangleF(x, y, x + width, y + height);

            _device.DrawRectangle(first, _brushContainer[brush], stroke);

            if (value == 0)
                return;

            first.Top += height - (height / 100.0f * value);

            _device.FillRectangle(first, _brushContainer[interiorBrush]);
        }
        public void DrawBarHorizontal(int x, int y, int width, int height, float value, float stroke, int brush, int interiorBrush)
        {
            var first = new SharpDX.RectangleF(x, y, x + width, y + height);

            _device.DrawRectangle(first, _brushContainer[brush], stroke);

            if (value == 0)
                return;

            first.Right -= width - (width / 100.0f * value);

            _device.FillRectangle(first, _brushContainer[interiorBrush]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillRectangle(int x, int y, int width, int height, int brush)
        {
            _device.FillRectangle(new SharpDX.RectangleF(x, y, x + width, y + height), _brushContainer[brush]);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FillCircle(float x, float y, float radius, int brush)
        {
            _device.FillEllipse(new Ellipse(new Vector2(x, y), radius, radius), _brushContainer[brush]);
        }

        public void BorderedLine(int startX, int startY, int endX, int endY, float stroke, int brush, int borderBrush)
        {
            _device.DrawLine(new Vector2(startX, startY), new Vector2(endX, endY), _brushContainer[brush], stroke);

            _device.DrawLine(new Vector2(startX, startY - stroke), new Vector2(endX, endY - stroke), _brushContainer[borderBrush], stroke); //top
            _device.DrawLine(new Vector2(startX, startY + stroke), new Vector2(endX, endY + stroke), _brushContainer[borderBrush], stroke); //bot

            _device.DrawLine(new Vector2(startX - stroke / 2, startY - stroke * 1.5f), new Vector2(startX - stroke / 2, startY + stroke * 1.5f), _brushContainer[borderBrush], stroke);//left
            _device.DrawLine(new Vector2(endX - stroke / 2, endY - stroke * 1.5f), new Vector2(endX - stroke / 2, endY + stroke * 1.5f), _brushContainer[borderBrush], stroke);///rigth
        }

        public void BorderedRectangle(float x, float y, float width, float height, float stroke, float borderStroke, int brush, int borderBrush)
        {
            _device.DrawRectangle(new SharpDX.RectangleF(x - (stroke - borderStroke), y - (stroke - borderStroke), x + width + stroke - borderStroke, y + height + stroke - borderStroke), _brushContainer[borderBrush], borderStroke);

            _device.DrawRectangle(new SharpDX.RectangleF(x, y, x + width, y + height), _brushContainer[brush], stroke);

            _device.DrawRectangle(new SharpDX.RectangleF(x + (stroke - borderStroke), y + (stroke - borderStroke), x + width - stroke + borderStroke, y + height - stroke + borderStroke), _brushContainer[borderBrush], borderStroke);
        }

        public void BorderedCircle(int x, int y, int radius, float stroke, int brush, int borderBrush)
        {
            _device.DrawEllipse(new Ellipse(new Vector2(x, y), radius + stroke, radius + stroke), _brushContainer[borderBrush], stroke);

            _device.DrawEllipse(new Ellipse(new Vector2(x, y), radius, radius), _brushContainer[brush], stroke);

            _device.DrawEllipse(new Ellipse(new Vector2(x, y), radius - stroke, radius - stroke), _brushContainer[borderBrush], stroke);
        }

        public void DrawText(string text, int font, int brush, float x, float y, bool bufferText = true)
        {
            if (bufferText)
            {
                var bufferPos = -1;

                for (var i = 0; i < _layoutContainer.Count; i++)
                {
                    if (_layoutContainer[i].Text.Length != text.Length || _layoutContainer[i].Text != text) continue;
                    bufferPos = i;
                    break;
                }

                if (bufferPos == -1)
                {
                    _layoutContainer.Add(new LayoutBuffer(text, new TextLayout(_fontFactory, text, _fontContainer[font], float.MaxValue, float.MaxValue)));
                    bufferPos = _layoutContainer.Count - 1;
                }

                _device.DrawTextLayout(new Vector2(x, y), _layoutContainer[bufferPos].TextLayout, _brushContainer[brush], DrawTextOptions.NoSnap);
            }
            else
            {
                var layout = new TextLayout(_fontFactory, text, _fontContainer[font], float.MaxValue, float.MaxValue);
                _device.DrawTextLayout(new Vector2(x, y), layout, _brushContainer[brush]);
                layout.Dispose();
            }
        }
        #endregion
    }
}
