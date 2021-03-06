﻿// Copyright © 2017 Paddy Xu
// 
// This file is part of QuickLook program.
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Linq;
using System.Windows;
using ImageMagick;

namespace QuickLook.Plugin.ImageViewer
{
    public class Plugin : IViewer
    {
        private static readonly string[] Formats =
        {
            // camera raw
            ".3fr", ".ari", ".arw", ".bay", ".crw", ".cr2", ".cap", ".data", ".dcs", ".dcr", ".dng", ".drf", ".eip",
            ".erf", ".fff", ".gpr", ".iiq", ".k25", ".kdc", ".mdc", ".mef", ".mos", ".mrw", ".nef", ".nrw", ".obm",
            ".orf", ".pef", ".ptx", ".pxn", ".r3d", ".raf", ".raw", ".rwl", ".rw2", ".rwz", ".sr2", ".srf", ".srw",
            ".tif", ".x3f",
            // normal
            ".bmp", ".ico", ".icon", ".jpg", ".jpeg", ".psd", ".svg", ".wdp", ".tif", ".tiff", ".tga",
            ".webp", ".pbm", ".pgm", ".ppm", ".pnm",
            // animated
            ".png", ".apng", ".gif"
        };
        private Size _imageSize;
        private ImagePanel _ip;

        public int Priority => int.MaxValue;
        public bool AllowsTransparency => true;

        public void Init()
        {
            new MagickImage().Dispose();
        }

        public bool CanHandle(string path)
        {
            return !Directory.Exists(path) && Formats.Any(path.ToLower().EndsWith);
        }

        public void Prepare(string path, ContextObject context)
        {
            _imageSize = ImageFileHelper.GetImageSize(path) ?? Size.Empty;

            if (!_imageSize.IsEmpty)
                context.SetPreferredSizeFit(_imageSize, 0.8);
            else
                context.PreferredSize = new Size(1024, 768);

            Directory.SetCurrentDirectory(App.AppPath);

            context.TitlebarBlurVisibility = true;
            context.TitlebarOverlap = true;
            context.TitlebarAutoHide = true;
        }

        public void View(string path, ContextObject context)
        {
            _ip = new ImagePanel();

            context.ViewerContent = _ip;
            context.Title = _imageSize.IsEmpty
                ? $"{Path.GetFileName(path)}"
                : $"{Path.GetFileName(path)} ({_imageSize.Width}×{_imageSize.Height})";

            LoadImage(_ip, path);

            context.IsBusy = false;
        }

        public void Cleanup()
        {
            _ip = null;
        }

        private void LoadImage(ImagePanel ui, string path)
        {
            ui.ImageUriSource = new Uri(path);
        }
    }
}