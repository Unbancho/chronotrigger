using System;
using System.Runtime.InteropServices;
using SFML.Graphics;
using SFML.System;

namespace ChronoTrigger.Extensions
{
    public static class WindowExtensions
    {
        public static IntPtr CreateContextMenu(string[] options)
        {
            var menu = CreatePopupMenu();
            for (var i = 0u; i < options.Length; i++)
            {
                var s = options[i];
                AppendMenuA(menu, 0, i+1, s);
            }

            return menu;
        }
        
        
        public static int ShowContextMenu(this RenderWindow window, IntPtr menu, Vector2i position)
        {
            return TrackPopupMenu(menu, 0x100, position.X, position.Y, 0, 
                window.SystemHandle, IntPtr.Zero);
        }
        
        [DllImport("User32.dll")]
        private static extern int TrackPopupMenu(
            IntPtr hMenu,
            uint uFlags,
            int x,
            int y,
            int nReserved,
            IntPtr hWnd,
            IntPtr prcRect
        );
        
        [DllImport("User32.dll")]
        private static extern IntPtr CreatePopupMenu();
        
        [DllImport("User32.dll")]
        private static extern bool AppendMenuA(
            IntPtr hMenu,
            uint uFlags,
            uint uIDNewItem,
            string lpNewItem
        );
    }
}