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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Declarations;
using LibVlcWrapper;
using Declarations.Players;
using Declarations.Enums;
using System.Runtime.InteropServices;

namespace Implementation.Players
{
    internal class DiskPlayer : VideoPlayer, IDiskPlayer
    {
        public DiskPlayer(IntPtr hMediaLib)
            : base(hMediaLib)
        {

        }

        public int AudioTrack
        {
            get
            {
                return NativeMethods.libvlc_audio_get_track(m_hMediaPlayer);
            }
            set
            {
                NativeMethods.libvlc_audio_set_track(m_hMediaPlayer, value);
            }
        }

        public int AudioTrackCount
        {
            get
            {
                return NativeMethods.libvlc_audio_get_track_count(m_hMediaPlayer);
            }
        }

        public IEnumerable<TrackDescription> AudioTracksInfo
        {
            get
            {
                IntPtr trackInfo = NativeMethods.libvlc_audio_get_track_description(m_hMediaPlayer);
                return GetDescription(trackInfo);
            }
        }

        public IEnumerable<TrackDescription> VideoTracksInfo
        {
            get
            {
                IntPtr trackInfo = NativeMethods.libvlc_video_get_track_description(m_hMediaPlayer);
                return GetDescription(trackInfo);
            }
        }

        public IEnumerable<TrackDescription> SubtitleTracksInfo
        {
            get
            {
                IntPtr trackInfo = NativeMethods.libvlc_video_get_spu_description(m_hMediaPlayer);
                return GetDescription(trackInfo);
            }
        }

        public IEnumerable<TrackDescription> TitleInfo
        {
            get
            {
                IntPtr trackInfo = NativeMethods.libvlc_video_get_title_description(m_hMediaPlayer);
                return GetDescription(trackInfo);
            }
        }

        public IEnumerable<TrackDescription> GetChapterDescription(int title)
        {
            IntPtr trackInfo = NativeMethods.libvlc_video_get_chapter_description(m_hMediaPlayer, title);
            return GetDescription(trackInfo);
        }

        private IEnumerable<TrackDescription> GetDescription(IntPtr trackInfo)
        {
            if (trackInfo == IntPtr.Zero)
            {
                yield break;
            }
                
            libvlc_track_description_t trackDesc = (libvlc_track_description_t)Marshal.PtrToStructure(trackInfo, typeof(libvlc_track_description_t));
            do
            {
                yield return new TrackDescription()
                {
                    Id = trackDesc.i_id,
                    Name = Marshal.PtrToStringAnsi(trackDesc.psz_name)
                };

                if (trackDesc.p_next != IntPtr.Zero)
                {
                    trackDesc = (libvlc_track_description_t)Marshal.PtrToStructure(trackDesc.p_next, typeof(libvlc_track_description_t));
                }
                else
                {
                    break;
                }
            }
            while (true);
            NativeMethods.libvlc_track_description_release(trackInfo);
        }
        
        public int SubTitle
        {
            get
            {
                return NativeMethods.libvlc_video_get_spu(m_hMediaPlayer);
            }
            set
            {
                NativeMethods.libvlc_video_set_spu(m_hMediaPlayer, value);
            }
        }

        public int SubTitleCount
        {
            get
            {
                return NativeMethods.libvlc_video_get_spu_count(m_hMediaPlayer);
            }
        }

        public void NextChapter()
        {
            NativeMethods.libvlc_media_player_next_chapter(m_hMediaPlayer);
        }

        public void PreviousChapter()
        {
            NativeMethods.libvlc_media_player_previous_chapter(m_hMediaPlayer);
        }

        public int Title
        {
            get
            {
                return NativeMethods.libvlc_media_player_get_title(m_hMediaPlayer);
            }
            set
            {
                NativeMethods.libvlc_media_player_set_title(m_hMediaPlayer, value);
            }
        }

        public int TitleCount
        {
            get
            {
                return NativeMethods.libvlc_media_player_get_title_count(m_hMediaPlayer);
            }
        }

        public int GetChapterCountForTitle(int title)
        {
            return NativeMethods.libvlc_media_player_get_chapter_count_for_title(m_hMediaPlayer, Title);
        }

        public int ChapterCount
        {
            get
            {
                return NativeMethods.libvlc_media_player_get_chapter_count(m_hMediaPlayer);
            }
        }

        public int Chapter
        {
            get
            {
                return NativeMethods.libvlc_media_player_get_chapter(m_hMediaPlayer);
            }
            set
            {
                NativeMethods.libvlc_media_player_set_chapter(m_hMediaPlayer, value);
            }
        }

        public int VideoTrackCount
        {
            get
            {
                return NativeMethods.libvlc_video_get_track_count(m_hMediaPlayer);
            }
        }

        public int VideoTrack
        {
            get
            {
                return NativeMethods.libvlc_video_get_track(m_hMediaPlayer);
            }
            set
            {
                NativeMethods.libvlc_video_set_track(m_hMediaPlayer, value);
            }
        }

        public void Navigate(NavigationMode mode)
        {
            NativeMethods.libvlc_media_player_navigate(m_hMediaPlayer, (libvlc_navigate_mode_t)mode);
        }
    }
}
