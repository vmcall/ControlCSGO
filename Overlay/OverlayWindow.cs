using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Control.Overlay
{
    internal class OverlayWindow
    {
        public Direct2DRenderer Graphics;

        #region private fields
        public bool IsDisposing { get; private set; }
        public bool ParentWindowExists { get; private set; }
        #endregion

        #region public fields
        public bool IsTopMost { get; private set; }
        public bool IsVisible { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }

        public IntPtr Handle { get; private set; }

        public IntPtr ParentWindow { get; private set; }
        #endregion

        private Margin _margin;

        #region construct and destruct
        /// <summary>
        /// Makes a transparent Fullscreen window
        /// </summary>
        public OverlayWindow(bool limitFPS = true)
        {
            IsDisposing = false;
            IsVisible = true;
            IsTopMost = true;

            ParentWindowExists = false;

            X = 0;
            Y = 0;
            Width = NativeMethods.GetSystemMetrics(WindowConstants.SM_CX_SCREEN);
            Height = NativeMethods.GetSystemMetrics(WindowConstants.SM_CY_SCREEN);

            ParentWindow = IntPtr.Zero;

            if (!CreateWindow())
                throw new Exception("Could not create OverlayWindow");

            Graphics = new Direct2DRenderer(Handle, limitFPS);

            SetBounds(X, Y, Width, Height);
        }
        /// <summary>
        /// Makes a transparent window which adjust it's size and position to fit the parent window
        /// </summary>
        /// <param name="parent">HWND/Handle of a window</param>
        public OverlayWindow(IntPtr parent, bool limitFPS = true)
        {
            if (parent == IntPtr.Zero)
                throw new Exception("The handle of the parent window isn't valid");

            RECT bounds;
            NativeMethods.GetWindowRect(parent, out bounds);

            IsDisposing = false;
            IsVisible = true;
            IsTopMost = true;

            ParentWindowExists = true;

            X = bounds.Left;
            Y = bounds.Top;

            Width = bounds.Right - bounds.Left;
            Height = bounds.Bottom - bounds.Top;

            ParentWindow = parent;

            if (!CreateWindow())
                throw new Exception("Could not create OverlayWindow");

            Graphics = new Direct2DRenderer(Handle, limitFPS);

            SetBounds(X, Y, Width, Height);

            new Task(new Action(ParentServiceThread)).Start();
        }

        ~OverlayWindow()
        {
            Dispose();
        }

        /// <summary>
        /// Clean up used ressources and destroy window
        /// </summary>
        public void Dispose()
        {
            IsDisposing = true;
            Graphics.Dispose();
            NativeMethods.DestroyWindow(Handle);
        }
        #endregion

        /// <summary>
        /// Creates a window with the information's stored in this class
        /// </summary>
        /// <returns>true on success</returns>
        private bool CreateWindow()
        {
            Handle = NativeMethods.CreateWindowEx(
                WindowConstants.WINDOW_EX_STYLE_DX,
                WindowConstants.DESKTOP_CLASS,
                "",
                WindowConstants.WINDOW_STYLE_DX,
                X,
                Y,
                Width,
                Height,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero);

            if (Handle == IntPtr.Zero)
                return false;

            ExtendFrameIntoClient();

            return true;
        }

        /// <summary>
        /// resize and set new position if the parent window's bounds change
        /// </summary>
        private void ParentServiceThread()
        {
            while (!IsDisposing)
            {
                Thread.Sleep(10);

                RECT bounds;
                NativeMethods.GetWindowRect(ParentWindow, out bounds);

                if ((X != bounds.Left) || (Y != bounds.Top) || (Width != bounds.Right - bounds.Left) || (Height != bounds.Bottom - bounds.Top))
                    SetBounds(bounds.Left, bounds.Top, bounds.Right - bounds.Left, bounds.Bottom - bounds.Top);
            }
        }

        private void ExtendFrameIntoClient()
        {
            _margin = new Margin
            {
                cxLeftWidth = X,
                cxRightWidth = Width,
                cyBottomHeight = Height,
                cyTopHeight = Y
            };

            NativeMethods.DwmExtendFrameIntoClientArea(Handle, ref _margin);
        }

        #region window position and size
        public void SetPos(int x, int y)
        {
            X = x;
            Y = y;

            POINT pos;
            pos.X = x;
            pos.Y = y;

            POINT size;
            size.X = Width;
            size.Y = Height;

            NativeMethods.UpdateLayeredWindow(Handle, IntPtr.Zero, ref pos, ref size, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero, 0);

            ExtendFrameIntoClient();
        }
        public void SetSize(int width, int height)
        {
            Width = width;
            Height = height;

            POINT pos;
            pos.X = X;
            pos.Y = Y;

            POINT size;
            size.X = Width;
            size.Y = Height;

            NativeMethods.UpdateLayeredWindow(Handle, IntPtr.Zero, ref pos, ref size, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero, 0);


            Graphics.AutoResize(Width, Height);

            ExtendFrameIntoClient();
        }
        public void SetBounds(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;

            POINT pos;
            pos.X = x;
            pos.Y = y;

            POINT size;
            size.X = Width;
            size.Y = Height;

            NativeMethods.UpdateLayeredWindow(Handle, IntPtr.Zero, ref pos, ref size, IntPtr.Zero, IntPtr.Zero, 0, IntPtr.Zero, 0);

            Graphics?.AutoResize(Width, Height);

            ExtendFrameIntoClient();
        }
        #endregion

        #region window show and hide
        public void Show()
        {
            if (IsVisible)
                return;

            NativeMethods.ShowWindow(Handle, WindowConstants.SW_SHOW);
            IsVisible = true;

            ExtendFrameIntoClient();
        }
        public void Hide()
        {
            if (!IsVisible)
                return;

            NativeMethods.ShowWindow(Handle, WindowConstants.SW_HIDE);
            IsVisible = false;
        }
        #endregion
    }
}
