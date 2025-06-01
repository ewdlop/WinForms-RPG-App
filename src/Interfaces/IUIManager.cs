using System.Drawing;
using System.Collections.Generic;
using WinFormsApp1.Events;

namespace WinFormsApp1.Interfaces
{
    /// <summary>
    /// Interface for managing UI operations through events
    /// </summary>
    public interface IUIManager : IBaseManager
    {
        /// <summary>
        /// Display text in the main game area
        /// </summary>
        /// <param name="text">Text to display</param>
        /// <param name="color">Color of the text</param>
        /// <param name="isImportant">Whether this is important text</param>
        /// <param name="category">Category of the message</param>
        void DisplayText(string text, Color? color = null, bool isImportant = false, string category = "General");

        /// <summary>
        /// Display a formatted message with multiple lines
        /// </summary>
        /// <param name="title">Title of the message</param>
        /// <param name="lines">Lines of formatted text</param>
        /// <param name="clearPrevious">Whether to clear previous text</param>
        void DisplayFormattedMessage(string title, List<FormattedTextLine> lines, bool clearPrevious = false);

        /// <summary>
        /// Show a notification to the user
        /// </summary>
        /// <param name="title">Notification title</param>
        /// <param name="message">Notification message</param>
        /// <param name="type">Type of notification</param>
        /// <param name="duration">Duration in milliseconds</param>
        /// <param name="requiresUserAction">Whether user action is required</param>
        void ShowNotification(string title, string message, NotificationType type = NotificationType.Info, int duration = 3000, bool requiresUserAction = false);

        /// <summary>
        /// Update a progress bar
        /// </summary>
        /// <param name="progressBarName">Name of the progress bar</param>
        /// <param name="currentValue">Current value</param>
        /// <param name="maxValue">Maximum value</param>
        /// <param name="label">Label text</param>
        /// <param name="barColor">Color of the progress bar</param>
        void UpdateProgress(string progressBarName, int currentValue, int maxValue, string label = "", Color? barColor = null);

        /// <summary>
        /// Update UI control state
        /// </summary>
        /// <param name="controlName">Name of the control</param>
        /// <param name="isEnabled">Whether the control is enabled</param>
        /// <param name="isVisible">Whether the control is visible</param>
        /// <param name="newText">New text for the control</param>
        /// <param name="newValue">New value for the control</param>
        void UpdateControlState(string controlName, bool? isEnabled = null, bool? isVisible = null, string newText = null, object newValue = null);

        /// <summary>
        /// Update menu state
        /// </summary>
        /// <param name="menuName">Name of the menu</param>
        /// <param name="menuItemStates">States of menu items</param>
        /// <param name="isMenuEnabled">Whether the menu is enabled</param>
        void UpdateMenuState(string menuName, Dictionary<string, bool> menuItemStates, bool isMenuEnabled = true);

        /// <summary>
        /// Request user input
        /// </summary>
        /// <param name="prompt">Input prompt</param>
        /// <param name="defaultValue">Default value</param>
        /// <param name="isPassword">Whether this is a password field</param>
        /// <param name="validOptions">Valid options for input</param>
        /// <returns>Request ID for tracking the response</returns>
        string RequestUserInput(string prompt, string defaultValue = "", bool isPassword = false, List<string> validOptions = null);

        /// <summary>
        /// Change the UI theme
        /// </summary>
        /// <param name="themeName">Name of the theme</param>
        /// <param name="colorScheme">Color scheme for the theme</param>
        /// <param name="themeSettings">Additional theme settings</param>
        void ChangeTheme(string themeName, Dictionary<string, Color> colorScheme, Dictionary<string, object> themeSettings = null);

        /// <summary>
        /// Enable or disable game controls
        /// </summary>
        /// <param name="enabled">Whether controls should be enabled</param>
        void SetGameControlsEnabled(bool enabled);

        /// <summary>
        /// Clear the display area
        /// </summary>
        void ClearDisplay();

        /// <summary>
        /// Display help information
        /// </summary>
        /// <param name="helpCategory">Category of help to display</param>
        void ShowHelp(string helpCategory = "general");

        /// <summary>
        /// Display character statistics
        /// </summary>
        void ShowCharacterStats();

        /// <summary>
        /// Display location information
        /// </summary>
        void ShowLocationInfo();
    }
} 