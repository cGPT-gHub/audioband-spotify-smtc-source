using Windows.Media.Control;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SpotifySMTCSource
{
    // This class is not a complete AudioBand IAudioSource implementation.
    // It demonstrates how to filter SMTC sessions to only Spotify.
    public class WindowsMediaProvider
    {
        private GlobalSystemMediaTransportControlsSessionManager _mgr;
        private GlobalSystemMediaTransportControlsSession _spotifySession;

        public async Task InitAsync()
        {
            _mgr = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
            // Hook session-changed events and refresh the Spotify session
            _mgr.SessionsChanged += (_, __) => RefreshSpotifySession();
            RefreshSpotifySession();
        }

        private void RefreshSpotifySession()
        {
            var sessions = _mgr.GetSessions();
            _spotifySession = sessions.FirstOrDefault(s =>
                (s.SourceAppUserModelId?.IndexOf("SpotifyAB.SpotifyMusic", StringComparison.OrdinalIgnoreCase) >= 0)
                || (s.SourceAppUserModelId?.IndexOf("Spotify.exe", StringComparison.OrdinalIgnoreCase) >= 0)
                || (s.SourceAppUserModelId?.IndexOf("Spotify", StringComparison.OrdinalIgnoreCase) >= 0));
        }

        public async Task<(string title, string artist, Uri art)> ReadNowPlayingAsync()
        {
            var s = _spotifySession;
            if (s == null)
            {
                return (null, null, null);
            }

            var mediaProps = await (await s.TryGetMediaPropertiesAsync());
            var display = await s.GetPlaybackInfo();
            // TODO: extract album art from mediaProps.Thumbnail if needed
            return (mediaProps.Title, mediaProps.Artist, null);
        }
    }
}
