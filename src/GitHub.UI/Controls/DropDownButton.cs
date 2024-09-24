﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace GitHub.UI
{
    public class DropDownButton : ContentControl
    {
        public static readonly DependencyProperty AutoCloseOnClickProperty =
            DependencyProperty.Register(
                "AutoCloseOnClick",
                typeof(bool),
                typeof(DropDownButton),
                new FrameworkPropertyMetadata(true));
        public static readonly DependencyProperty DropDownContentProperty =
            DependencyProperty.Register(nameof(DropDownContent), typeof(object), typeof(DropDownButton));
        public static readonly DependencyProperty IsOpenProperty =
            Popup.IsOpenProperty.AddOwner(typeof(DropDownButton));

        Button button;
        Popup popup;

        static DropDownButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(DropDownButton),
                new FrameworkPropertyMetadata(typeof(DropDownButton)));
        }

        public bool AutoCloseOnClick
        {
            get { return (bool)GetValue(AutoCloseOnClickProperty); }
            set { SetValue(AutoCloseOnClickProperty, value); }
        }

        public object DropDownContent
        {
            get { return GetValue(DropDownContentProperty); }
            set { SetValue(DropDownContentProperty, value); }
        }

        public bool IsOpen
        {
            get { return (bool)GetValue(IsOpenProperty); }
            set { SetValue(IsOpenProperty, value); }
        }

        public event EventHandler PopupOpened;
        public event EventHandler PopupClosed;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            button = (Button)Template.FindName("PART_Button", this);
            popup = (Popup)Template.FindName("PART_Popup", this);
            button.Click += ButtonClick;
            popup.Opened += OnPopupOpened;
            popup.Closed += OnPopupClosed;
            popup.AddHandler(MouseUpEvent, new RoutedEventHandler(PopupMouseUp), true);
        }

        void ButtonClick(object sender, RoutedEventArgs e)
        {
            IsOpen = true;
        }

        private void OnPopupOpened(object sender, EventArgs e)
        {
            IsHitTestVisible = false;
            PopupOpened?.Invoke(this, e);
        }

        private void OnPopupClosed(object sender, EventArgs e)
        {
            IsOpen = false;
            IsHitTestVisible = true;
            PopupClosed?.Invoke(this, e);
        }

        private void PopupMouseUp(object sender, RoutedEventArgs e)
        {
            if (AutoCloseOnClick)
            {
                IsOpen = false;
            }
        }
    }
}
