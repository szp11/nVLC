﻿//    nVLC
//    
//    Author:  Roman Ginzburg
//
//    nVLC is free software: you can redistribute it and/or modify
//    it under the terms of the GNU General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    nVLC is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU General Public License for more details.
//     
// ========================================================================

using System;
using Declarations;
using Declarations.Events;
using Declarations.Media;
using Declarations.Players;
using Implementation.Events;
using LibVlcWrapper;

namespace Implementation.Players
{
    internal class MediaListPlayer : DisposableBase, IMediaListPlayer, IEventProvider
    {
        private IntPtr m_hMediaListPlayer = IntPtr.Zero;
        private IDiskPlayer m_videoPlayer;
        private IMediaList m_mediaList;
        private PlaybackMode m_playbackMode = PlaybackMode.Default;
        IntPtr m_hEventManager = IntPtr.Zero;
        IMediaListPlayerEvents m_mediaListEvents = null;

        public MediaListPlayer(IntPtr hMediaLib, IMediaList mediaList)
        {
            m_mediaList = mediaList;
            m_hMediaListPlayer = NativeMethods.libvlc_media_list_player_new(hMediaLib);
            NativeMethods.libvlc_media_list_player_set_media_list(m_hMediaListPlayer, ((INativePointer)m_mediaList).Pointer);
            m_mediaList.Dispose();

            m_videoPlayer = new DiskPlayer(hMediaLib);
            NativeMethods.libvlc_media_list_player_set_media_player(m_hMediaListPlayer, ((INativePointer)m_videoPlayer).Pointer);
            m_videoPlayer.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (m_videoPlayer != null)
            {
                m_videoPlayer.Dispose();
                m_videoPlayer = null;
            }
            NativeMethods.libvlc_media_list_player_release(m_hMediaListPlayer);
        }

        #region IMediaListPlayer Members

        public void PlayNext()
        {
            NativeMethods.libvlc_media_list_player_next(m_hMediaListPlayer);
        }

        public void PlayPrevios()
        {
            NativeMethods.libvlc_media_list_player_previous(m_hMediaListPlayer);
        }

        public PlaybackMode PlaybackMode
        {
            get
            {
                return m_playbackMode;
            }
            set
            {
                NativeMethods.libvlc_media_list_player_set_playback_mode(m_hMediaListPlayer, (libvlc_playback_mode_t)value);
                m_playbackMode = value;
            }
        }

        public void PlayItemAt(int index)
        {
            NativeMethods.libvlc_media_list_player_play_item_at_index(m_hMediaListPlayer, index);
        }

        public MediaState PlayerState
        {
            get
            {
                return (MediaState)NativeMethods.libvlc_media_list_player_get_state(m_hMediaListPlayer);
            }
        }

        public IDiskPlayer InnerPlayer
        {
            get
            {
                return m_videoPlayer;
            }
        }

        #endregion

        #region INativePointer Members

        public IntPtr Pointer
        {
            get
            {
                return m_hMediaListPlayer;
            }
        }

        #endregion

        #region IPlayer Members

        public void Play()
        {
            NativeMethods.libvlc_media_list_player_play(m_hMediaListPlayer);
        }

        public void Pause()
        {
            NativeMethods.libvlc_media_list_player_pause(m_hMediaListPlayer);
        }

        public void Stop()
        {
            NativeMethods.libvlc_media_list_player_stop(m_hMediaListPlayer);
        }

        public void Open(IMedia media)
        {
            m_videoPlayer.Open(media);
        }

        public long Time
        {
            get
            {
                return m_videoPlayer.Time;
            }
            set
            {
                m_videoPlayer.Time = value;
            }
        }

        public float Position
        {
            get
            {
                return m_videoPlayer.Position;
            }
            set
            {
                m_videoPlayer.Position = value;
            }
        }

        public long Length
        {
            get
            {
                return m_videoPlayer.Length;
            }
        }

        public IEventBroker Events
        {
            get
            {
                return m_videoPlayer.Events;
            }
        }

        public bool IsPlaying
        {
            get
            {
                return m_videoPlayer.IsPlaying;
            }
        }

        public IMedia CurrentMedia
        {
            get
            {
                return m_videoPlayer.CurrentMedia;
            }
        }

        #endregion

        #region IEventProvider Members

        public IntPtr EventManagerHandle
        {
            get
            {
                if (m_hEventManager == IntPtr.Zero)
                {
                    m_hEventManager = NativeMethods.libvlc_media_list_player_event_manager(m_hMediaListPlayer);
                }

                return m_hEventManager;
            }
        }

        #endregion

        #region IMediaListPlayer Members

        public IMediaListPlayerEvents MediaListPlayerEvents
        {
            get
            {
                if (m_mediaListEvents == null)
                {
                    m_mediaListEvents = new MediaListPlayerEventManager(this);
                }
                return m_mediaListEvents;
            }
        }

        #endregion

        #region IEqualityComparer<IPlayer> Members

        public bool Equals(IPlayer x, IPlayer y)
        {
            INativePointer x1 = (INativePointer)x;
            INativePointer y1 = (INativePointer)y;

            return x1.Pointer == y1.Pointer;
        }

        public int GetHashCode(IPlayer obj)
        {
            return ((INativePointer)obj).Pointer.GetHashCode();
        }

        #endregion
    }
}
