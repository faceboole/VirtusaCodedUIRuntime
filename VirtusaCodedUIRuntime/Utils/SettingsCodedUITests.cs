using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;

namespace CodedUI.Virtusa.Utils
{
    class SettingsCodedUITests
    {

        /// <summary> Test startup. </summary>
        public static void StartTest()
        {
            // Configure the playback engine
            int retry = Settings.Execution.Default.RETRY;
            int retryInterval = Settings.Execution.Default.RETRY_INTERVAL;
            Playback.PlaybackSettings.WaitForReadyLevel = WaitForReadyLevel.Disabled;
            Playback.PlaybackSettings.MaximumRetryCount = 10;
            Playback.PlaybackSettings.ShouldSearchFailFast = false;
            Playback.PlaybackSettings.DelayBetweenActions = 500;
            //Playback.PlaybackSettings.SearchTimeout = 1000;
            Playback.PlaybackSettings.SearchTimeout = (retry * retryInterval);

            // Add the error handler
            Playback.PlaybackError -= Playback_PlaybackError; // Remove the handler if it's already added
            Playback.PlaybackError += Playback_PlaybackError; // Ta dah...
        }

        /// <summary> PlaybackError event handler. </summary>
        private static void Playback_PlaybackError(object sender, PlaybackErrorEventArgs e)
        {
            // Wait a second
            System.Threading.Thread.Sleep(1000);

            // Retry the failed test operation
            e.Result = PlaybackErrorOptions.Retry;
            Keyboard.SendKeys("{Enter}");
        }
    }
}
