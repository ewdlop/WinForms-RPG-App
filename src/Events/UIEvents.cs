using System.Drawing;
using System.Collections.Generic;

namespace WinFormsApp1.Events
{
    /// <summary>
    /// Event for displaying text in the game UI
    /// </summary>
    public class DisplayTextEvent : GameEvent
    {
        public string Text { get; set; }
        public Color? Color { get; set; }
        public bool IsImportant { get; set; }
        public string Category { get; set; }

        public override int Priority => IsImportant ? 10 : 5;

        public DisplayTextEvent(string text, Color? color = null, bool isImportant = false, string category = "General")
        {
            Text = text;
            Color = color;
            IsImportant = isImportant;
            Category = category;
            Source = "UIManager";
        }
    }

    /// <summary>
    /// Event for displaying formatted messages with multiple lines
    /// </summary>
    public class DisplayFormattedMessageEvent : GameEvent
    {
        public List<FormattedTextLine> Lines { get; set; }
        public string Title { get; set; }
        public bool ClearPrevious { get; set; }

        public override int Priority => 8;

        public DisplayFormattedMessageEvent(string title, List<FormattedTextLine> lines, bool clearPrevious = false)
        {
            Title = title;
            Lines = lines ?? new List<FormattedTextLine>();
            ClearPrevious = clearPrevious;
            Source = "UIManager";
        }
    }

    /// <summary>
    /// Represents a line of formatted text
    /// </summary>
    public class FormattedTextLine
    {
        public string Text { get; set; }
        public Color? Color { get; set; }
        public bool IsBold { get; set; }
        public bool IsItalic { get; set; }

        public FormattedTextLine(string text, Color? color = null, bool isBold = false, bool isItalic = false)
        {
            Text = text;
            Color = color;
            IsBold = isBold;
            IsItalic = isItalic;
        }
    }

    /// <summary>
    /// Event for updating UI controls state
    /// </summary>
    public class UIControlStateChangedEvent : GameEvent
    {
        public string ControlName { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }
        public string NewText { get; set; }
        public object NewValue { get; set; }

        public UIControlStateChangedEvent(string controlName, bool? isEnabled = null, bool? isVisible = null, string newText = null, object newValue = null)
        {
            ControlName = controlName;
            IsEnabled = isEnabled ?? true;
            IsVisible = isVisible ?? true;
            NewText = newText;
            NewValue = newValue;
            Source = "UIManager";
        }
    }

    /// <summary>
    /// Event for showing notifications or alerts
    /// </summary>
    public class NotificationEvent : GameEvent
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public NotificationType Type { get; set; }
        public int Duration { get; set; } // Duration in milliseconds, 0 = permanent
        public bool RequiresUserAction { get; set; }

        public override int Priority => Type == NotificationType.Critical ? 15 : 7;

        public NotificationEvent(string title, string message, NotificationType type = NotificationType.Info, int duration = 3000, bool requiresUserAction = false)
        {
            Title = title;
            Message = message;
            Type = type;
            Duration = duration;
            RequiresUserAction = requiresUserAction;
            Source = "UIManager";
        }
    }

    /// <summary>
    /// Types of notifications
    /// </summary>
    public enum NotificationType
    {
        Info,
        Success,
        Warning,
        Error,
        Critical
    }

    /// <summary>
    /// Event for updating progress bars
    /// </summary>
    public class ProgressUpdateEvent : GameEvent
    {
        public string ProgressBarName { get; set; }
        public int CurrentValue { get; set; }
        public int MaxValue { get; set; }
        public string Label { get; set; }
        public Color? BarColor { get; set; }

        public ProgressUpdateEvent(string progressBarName, int currentValue, int maxValue, string label = "", Color? barColor = null)
        {
            ProgressBarName = progressBarName;
            CurrentValue = currentValue;
            MaxValue = maxValue;
            Label = label;
            BarColor = barColor;
            Source = "UIManager";
        }
    }

    /// <summary>
    /// Event for requesting user input
    /// </summary>
    public class UserInputRequestEvent : GameEvent
    {
        public string Prompt { get; set; }
        public string DefaultValue { get; set; }
        public bool IsPassword { get; set; }
        public List<string> ValidOptions { get; set; }
        public string RequestId { get; set; }

        public override bool CanBeCancelled => true;

        public UserInputRequestEvent(string prompt, string defaultValue = "", bool isPassword = false, List<string> validOptions = null, string requestId = null)
        {
            Prompt = prompt;
            DefaultValue = defaultValue;
            IsPassword = isPassword;
            ValidOptions = validOptions;
            RequestId = requestId ?? Guid.NewGuid().ToString();
            Source = "UIManager";
        }
    }

    /// <summary>
    /// Event for user input response
    /// </summary>
    public class UserInputResponseEvent : GameEvent
    {
        public string RequestId { get; set; }
        public string Response { get; set; }
        public bool WasCancelled { get; set; }

        public UserInputResponseEvent(string requestId, string response, bool wasCancelled = false)
        {
            RequestId = requestId;
            Response = response;
            WasCancelled = wasCancelled;
            Source = "UIManager";
        }
    }

    /// <summary>
    /// Event for updating menu states
    /// </summary>
    public class MenuStateChangedEvent : GameEvent
    {
        public string MenuName { get; set; }
        public Dictionary<string, bool> MenuItemStates { get; set; }
        public bool IsMenuEnabled { get; set; }

        public MenuStateChangedEvent(string menuName, Dictionary<string, bool> menuItemStates, bool isMenuEnabled = true)
        {
            MenuName = menuName;
            MenuItemStates = menuItemStates ?? new Dictionary<string, bool>();
            IsMenuEnabled = isMenuEnabled;
            Source = "UIManager";
        }
    }

    /// <summary>
    /// Event for theme changes
    /// </summary>
    public class ThemeChangedEvent : GameEvent
    {
        public string ThemeName { get; set; }
        public Dictionary<string, Color> ColorScheme { get; set; }
        public Dictionary<string, object> ThemeSettings { get; set; }

        public ThemeChangedEvent(string themeName, Dictionary<string, Color> colorScheme, Dictionary<string, object> themeSettings = null)
        {
            ThemeName = themeName;
            ColorScheme = colorScheme ?? new Dictionary<string, Color>();
            ThemeSettings = themeSettings ?? new Dictionary<string, object>();
            Source = "UIManager";
        }
    }
} 