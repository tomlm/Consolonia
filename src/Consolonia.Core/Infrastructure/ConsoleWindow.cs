using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Input.Raw;
using Avalonia.Input.TextInput;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using Avalonia.Threading;
using Consolonia.Core.Drawing.PixelBufferImplementation;

namespace Consolonia.Core.Infrastructure
{
    internal class ConsoleWindow : IWindowImpl
    {
        private readonly IKeyboardDevice _myKeyboardDevice;
        [NotNull] internal readonly IConsole Console;
        private IInputRoot _inputRoot;

        public ConsoleWindow()
        {
            _myKeyboardDevice = AvaloniaLocator.Current.GetService<IKeyboardDevice>();
            MouseDevice = AvaloniaLocator.Current.GetService<IMouseDevice>();
            Console = AvaloniaLocator.Current.GetService<IConsole>() ?? throw new NotImplementedException();
            Console.Resized += OnConsoleOnResized;
            Console.KeyEvent += ConsoleOnKeyEvent;
            Console.MouseEvent += ConsoleOnMouseEvent;
            Console.FocusEvent += ConsoleOnFocusEvent;
            Handle = null!;
        }

        private IMouseDevice MouseDevice { get; }

        public void Dispose()
        {
            Closed?.Invoke();
            Console.Resized -= OnConsoleOnResized;
            Console.KeyEvent -= ConsoleOnKeyEvent;
            Console.MouseEvent -= ConsoleOnMouseEvent;
            Console.FocusEvent -= ConsoleOnFocusEvent;
            Console.Dispose();
        }

        public void SetInputRoot(IInputRoot inputRoot)
        {
            _inputRoot = inputRoot;
        }

        public Point PointToClient(PixelPoint point)
        {
            return point.ToPoint(1);
        }

        public PixelPoint PointToScreen(Point point)
        {
            // ReSharper disable once ArrangeObjectCreationWhenTypeNotEvident //todo: should we avoid suggesting type specification
            return new((int)point.X, (int)point.Y);
        }

        public void SetCursor(ICursorImpl cursor)
        {
            //todo: check whether we can work with cursors
        }

        public IPopupImpl CreatePopup()
        {
            return null; // when returning null top window overlay layer will be used
        }

        public void SetTransparencyLevelHint(IReadOnlyList<WindowTransparencyLevel> transparencyLevels)
        {
            throw new NotImplementedException("Consider this");
        }

        public void SetFrameThemeVariant(PlatformThemeVariant themeVariant)
        {
            //todo:
        }

        public Size ClientSize
        {
            get
            {
                PixelBufferSize pixelBufferSize = Console.Size;
                return new Size(pixelBufferSize.Width, pixelBufferSize.Height);
            }
        }

        public Size? FrameSize => ClientSize;

        public double RenderScaling => 1;
        public IEnumerable<object> Surfaces => new[] { this };

        public Action<RawInputEventArgs> Input { get; set; }

        public Action<Rect> Paint { get; set; }
        public Action<Size, WindowResizeReason> Resized { get; set; }


        public Action<double> ScalingChanged { get; set; }

        public Action<WindowTransparencyLevel> TransparencyLevelChanged { get; set; }

        public Compositor Compositor { get; } = new(null);
        public Action Closed { get; set; }
        public Action LostFocus { get; set; }

        public WindowTransparencyLevel TransparencyLevel => WindowTransparencyLevel.None;

        public AcrylicPlatformCompensationLevels AcrylicCompensationLevels => new(1, 1, 1);

        public void Show(bool activate, bool isDialog)
        {
            if (activate)
                Activated!();
        }

        public void Hide()
        {
            throw new NotImplementedException();
        }

        public void Activate()
        {
            throw new NotImplementedException();
        }

        public void SetTopmost(bool value)
        {
            throw new NotImplementedException();
        }

        public double DesktopScaling => 1d;

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: get from _console if supported
        public PixelPoint Position { get; }
        public Action<PixelPoint> PositionChanged { get; set; }
        public Action Deactivated { get; set; }
        public Action Activated { get; set; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: get from _console if supported
        public IPlatformHandle Handle { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public Size MaxAutoSizeHint { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public IScreenImpl Screen => null!;

        public void SetTitle(string title)
        {
            Console.SetTitle(title);
        }

        public void SetParent(IWindowImpl parent)
        {
            throw new NotImplementedException();
        }

        public void SetEnabled(bool enable)
        {
            throw new NotImplementedException();
        }

        public void SetSystemDecorations(SystemDecorations enabled)
        {
            throw new NotImplementedException();
        }

        public void SetIcon(IWindowIconImpl icon)
        {
        }

        public void ShowTaskbarIcon(bool value)
        {
        }

        public void CanResize(bool value)
        {
            throw new NotImplementedException();
        }

        public void BeginMoveDrag(PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void BeginResizeDrag(WindowEdge edge, PointerPressedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public void Resize(Size clientSize, WindowResizeReason reason = WindowResizeReason.Application)
        {
            //todo: can we deny resizing?
        }


        public void Move(PixelPoint point)
        {
            throw new NotImplementedException();
        }

        public void SetMinMaxSize(Size minSize, Size maxSize)
        {
            throw new NotImplementedException();
        }

        public void SetExtendClientAreaToDecorationsHint(bool extendIntoClientAreaHint)
        {
            throw new NotImplementedException();
        }

        public void SetExtendClientAreaChromeHints(ExtendClientAreaChromeHints hints)
        {
            throw new NotImplementedException();
        }

        public void SetExtendClientAreaTitleBarHeightHint(double titleBarHeight)
        {
            throw new NotImplementedException();
        }

        public WindowState WindowState { get; set; }
        public Action<WindowState> WindowStateChanged { get; set; }
        public Action GotInputWhenDisabled { get; set; }
        public Func<WindowCloseReason, bool> Closing { get; set; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public bool IsClientAreaExtendedToDecorations { get; }
        public Action<bool> ExtendClientAreaToDecorationsChanged { get; set; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public bool NeedsManagedDecorations { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public Thickness ExtendedMargins { get; }

        // ReSharper disable once UnassignedGetOnlyAutoProperty todo: what is this property
        public Thickness OffScreenMargin { get; }

        public object TryGetFeature(Type featureType)
        {
            if (featureType == typeof(ISystemNavigationManagerImpl))
                return null;
            if (featureType == typeof(ITextInputMethodImpl)) return null;
            throw new NotImplementedException("Consider this");
        }

        private void ConsoleOnMouseEvent(RawPointerEventType type, Point point, Vector? wheelDelta,
            RawInputModifiers modifiers)
        {
            ulong timestamp = (ulong)Stopwatch.GetTimestamp();
            Dispatcher.UIThread.Post(() =>
            {
                // ReSharper disable once SwitchStatementMissingSomeEnumCasesNoDefault
                switch (type)
                {
                    case RawPointerEventType.Move:
                    case RawPointerEventType.LeftButtonDown:
                    case RawPointerEventType.LeftButtonUp:
                    case RawPointerEventType.RightButtonUp:
                    case RawPointerEventType.RightButtonDown:
                    case RawPointerEventType.MiddleButtonDown:
                    case RawPointerEventType.XButton1Down:
                    case RawPointerEventType.XButton2Down:
                    case RawPointerEventType.NonClientLeftButtonDown:
                    case RawPointerEventType.MiddleButtonUp:
                    case RawPointerEventType.XButton1Up:
                    case RawPointerEventType.XButton2Up:
                        Input!(new RawPointerEventArgs(MouseDevice, timestamp, _inputRoot,
                            type, point,
                            modifiers));
                        break;
                    case RawPointerEventType.Wheel:
                        Input!(new RawMouseWheelEventArgs(MouseDevice, timestamp, _inputRoot, point,
                            (Vector)wheelDelta!, modifiers));
                        break;
                }
            });
        }

        private void ConsoleOnFocusEvent(bool focused)
        {
            Dispatcher.UIThread.Post(() =>
            {
                if (focused)
                    Activated?.Invoke();
                else Deactivated?.Invoke();
            });
        }

        private void OnConsoleOnResized()
        {
            Dispatcher.UIThread.Post(() =>
            {
                PixelBufferSize pixelBufferSize = Console.Size;
                var size = new Size(pixelBufferSize.Width, pixelBufferSize.Height);
                Resized!(size, WindowResizeReason.Unspecified);
            });
        }

        private async void ConsoleOnKeyEvent(Key key, char keyChar, RawInputModifiers rawInputModifiers, bool down,
            ulong timeStamp)
        {
            if (!down)
            {
                Dispatcher.UIThread.Post(() =>
                {
#pragma warning disable CS0618 // Type or member is obsolete // todo: change to correct constructor, CFA20A9A-3A24-4187-9CA3-9DF0081124EE 
                    var rawInputEventArgs = new RawKeyEventArgs(_myKeyboardDevice, timeStamp, _inputRoot,
                        RawKeyEventType.KeyUp, key,
                        rawInputModifiers);
#pragma warning restore CS0618 // Type or member is obsolete
                    Input!(rawInputEventArgs);
                }, DispatcherPriority.Input);
            }
            else
            {
                bool handled = false;
                await Dispatcher.UIThread.InvokeAsync(() =>
                {
#pragma warning disable CS0618 // Type or member is obsolete //todo: CFA20A9A-3A24-4187-9CA3-9DF0081124EE
                    var rawInputEventArgs = new RawKeyEventArgs(_myKeyboardDevice, timeStamp,
                        _inputRoot,
                        RawKeyEventType.KeyDown, key,
                        rawInputModifiers);
#pragma warning restore CS0618 // Type or member is obsolete
                    Input!(rawInputEventArgs);
                    handled = rawInputEventArgs.Handled;
                }, DispatcherPriority.Input).GetTask().ConfigureAwait(true);

                if (!handled && !char.IsControl(keyChar))
                    Dispatcher.UIThread.Post(() =>
                    {
                        Input!(new RawTextInputEventArgs(_myKeyboardDevice,
                            timeStamp,
                            _inputRoot,
                            keyChar.ToString()));
                    }, DispatcherPriority.Input);
            }
        }
    }
}