using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace IotFrosting.Pimoroni
{
    public class AlphaDisplay : HT16K33, ITick
    {
        public static async Task<AlphaDisplay> Open(int i2c_address = DEFAULT_ADDRESS)
        {
            var device = await GetDevice(i2c_address);
            var result = new AlphaDisplay(device);

            return result;
        }

        protected AlphaDisplay(I2cDevice device) : base(device)
        {
        }

        /// <summary>
        /// Set only a single character
        /// </summary>
        /// <param name="c">What character to set</param>
        /// <param name="position">Which display position, 0-3</param>
        public void SetCharacter(char c, int position, bool show = true)
        {
            base.SetWordAt(position * 2, Digits.Values[c]);
            if (show)
                base.Show();
        }
        
        /// <summary>
        /// Message string to show on the display. Will scroll if length > 4
        /// </summary>
        public string Message
        {
            get
            {
                return _Message;
            }
            set
            {
                _Message = value;

                IsScrolling = (_Message?.Length > 4);
                ScrollIndex = 0;
                if (IsScrolling)
                    NextScrollAt = DateTimeOffset.Now + ScrollDelay;

                ShowMessage();
            }
        }
        private string _Message;

        /// <summary>
        /// Delay between scrolling movements, when the display is scrolling
        /// </summary>
        public TimeSpan ScrollDelay { get; set; } = TimeSpan.FromMilliseconds(500);

        /// <summary>
        /// Exact moment when we will next update the display
        /// </summary>
        private DateTimeOffset NextScrollAt { get; set; }

        /// <summary>
        /// Update the scroll display
        /// </summary>
        /// <remarks>
        /// Call at least as frequently as the ScrollDelay
        /// </remarks>
        public void Tick()
        {
            if (IsScrolling && DateTime.Now > NextScrollAt)
            {
                ++ScrollIndex;
                if (ScrollIndex >= Message.Length)
                    ScrollIndex = 0;
                ShowMessage();

                NextScrollAt += ScrollDelay;
            }
        }

        private bool IsScrolling = false;
        private int ScrollIndex = 0;

        /// <summary>
        /// Extract the next 4 chars of message, and send them to the display
        /// </summary>
        /// <remarks>
        /// Padded by spaces at the end
        /// </remarks>
        private void ShowMessage()
        {
            int messageindex = ScrollIndex;
            int characterindex = 0;
            int loopindex = 4;
            while (loopindex-- > 0)
            {
                char output = ' ';
                if (Message?.Length < messageindex)
                    output = Message[messageindex++];

                SetCharacter(output, characterindex++,false);
            }

            base.Show();
        }
    }
}