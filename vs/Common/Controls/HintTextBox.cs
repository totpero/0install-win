﻿/*
 * Copyright 2006-2010 Bastian Eicher
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Common.Properties;

namespace Common.Controls
{
    /// <summary>
    /// A special <see cref="TextBox"/> that displays a <see cref="HintText"/> when <see cref="TextBox.Text"/> is empty and a clear button when it is not.
    /// </summary>
    [Description("A special TextBox that displays a hint text when Text is empty.")]
    public class HintTextBox : TextBox
    {
        #region Events
        protected override void OnEnter(EventArgs e)
        {
            // Remove the hint when entering the TextBox
            HideHintText();

            base.OnGotFocus(e);
        }

        protected override void OnLeave(EventArgs e)
        {
            // Restore hint when leaving the TextBox with an empty Text
            if (string.IsNullOrEmpty(base.Text)) ShowHintText();

            base.OnLeave(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            // Show clear button only if it is enabled and there is text that can be cleared
            _buttonClear.Visible = _showClearButton && !string.IsNullOrEmpty(base.Text) && !IsHintTextVisible;

            // Prevent displaying of hint text from raising events
            if (_suppressTextChangedEvent) return;

            base.OnTextChanged(e);
        }

        protected override void OnBackColorChanged(EventArgs e)
        {
            // Make sure the clear button has the same background as the TextBox
            _buttonClear.BackColor = BackColor;

            base.OnBackColorChanged(e);
        }
        #endregion

        #region Variables
        /// <summary>Prevents <see cref="OnTextChanged"/> from raising any events.</summary>
        private bool _suppressTextChangedEvent;

        private readonly PictureBox _buttonClear = new PictureBox
        {
            Visible = false, Cursor = Cursors.Default,
            Location = new Point(79, -1), Size = new Size(18, 18), Anchor = AnchorStyles.Right,
            BackColor = SystemColors.Window, BackgroundImageLayout = ImageLayout.Center, BackgroundImage = Resources.ClearButton
        };
        #endregion

        #region Properties
        private Color _foreColor = SystemColors.ControlText;
        /// <inheritdoc/>
        public new Color ForeColor
        {
            get { return _foreColor; }
            set
            {
                _foreColor = value;
                if (!IsHintTextVisible) base.ForeColor = value;
            }
        }

        /// <inheritdoc/>
        public override string Text
        {
            get { return IsHintTextVisible ? "" : base.Text; }
            set
            {
                // Update the visibility of the hint text when the text is changed externally
                if (string.IsNullOrEmpty(value) && !Focused) ShowHintText();
                else HideHintText();

                // Inform the underlying TextBox
                base.Text = value;
            }
        }

        private string _hintText;
        /// <summary>
        /// A text to be displayed in <see cref="SystemColors.GrayText"/> when <see cref="TextBox.Text"/> is empty.
        /// </summary>
        [Category("Appearance"), Description("A text to be displayed in gray when Text is empty."), Localizable(true)]
        public string HintText
        {
            get { return _hintText; }
            set
            {
                _hintText = value;

                // Update the hint text on-screen if it is already visible
                if (IsHintTextVisible) ShowHintText();
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="HintText"/> is currently visible.
        /// </summary>
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsHintTextVisible { get; private set; }

        private bool _showClearButton;
        /// <summary>
        /// Controls whether the clear button is shown. Remains invisible when the HintText is visible.
        /// </summary>
        [DefaultValue(false), Category("Appearance"), Description("Controls whether the clear button is shown. Remains invisible when the HintText is visible.")]
        public bool ShowClearButton
        {
            get { return _showClearButton; }
            set
            {
                _showClearButton = value;

                // Show clear button only if it is enabled and there is text that can be cleared
                _buttonClear.Visible = value && !string.IsNullOrEmpty(base.Text) && !IsHintTextVisible;
            }
        }
        #endregion

        #region Constructor
        public HintTextBox()
        {
            // Since TextBoxes start off empty, hint texts are visible by default
            IsHintTextVisible = true;

            _buttonClear.Click += delegate
            {
                Focus();

                // Only clear the text if focus change was possible (might be prevented by validation)
                if (Focused) Clear();
            };
            Controls.Add(_buttonClear);
        }
        #endregion

        private void ShowHintText()
        {
            IsHintTextVisible = true;

            // Show the hint text without raising events
            _suppressTextChangedEvent = true;
            base.Text = _hintText;
            _suppressTextChangedEvent = false;
            base.ForeColor = SystemColors.GrayText;
        }

        private void HideHintText()
        {
            // Don't try to clear the TextBox if isn't displaying the hint text - it might contain real data
            if (!IsHintTextVisible) return;

            IsHintTextVisible = false;

            // Remove the hint text without raising events
            _suppressTextChangedEvent = true;
            base.Text = "";
            _suppressTextChangedEvent = false;
            base.ForeColor = _foreColor;
        }
    }
}