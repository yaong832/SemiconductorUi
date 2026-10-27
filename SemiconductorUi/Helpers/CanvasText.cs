using System;
using System.Drawing;
using System.Windows.Forms;

namespace SemiconductorUi.Helpers
{
    /// <summary>
    /// 확대해서 그리는 장비 그림용 글자 그리기.
    ///
    /// TextRenderer는 Graphics의 확대(ScaleTransform)를 무시하므로, 확대가 걸려 있으면
    /// 글꼴과 영역을 그 배율만큼 키워서 확대 없이 그립니다. 확대가 없으면 TextRenderer와 같습니다.
    /// 회전이 걸린 상태에서는 쓰지 않습니다(장비 그림의 글자는 회전 밖에서 그립니다).
    /// </summary>
    public static class CanvasText
    {
        public static void Draw(Graphics g, string text, Font font, Rectangle bounds, Color color, TextFormatFlags flags)
        {
            float scale = GetScale(g, out float dx, out float dy);
            if (scale == 1f)
            {
                TextRenderer.DrawText(g, text, font, bounds, color, flags);
                return;
            }

            var target = Rectangle.Round(new RectangleF(bounds.X * scale + dx, bounds.Y * scale + dy, bounds.Width * scale, bounds.Height * scale));
            using (var scaled = ScaleFont(font, scale))
            {
                var state = g.Save();
                g.ResetTransform();
                TextRenderer.DrawText(g, text, scaled, target, color, flags);
                g.Restore(state);
            }
        }

        public static void Draw(Graphics g, string text, Font font, Point location, Color color, TextFormatFlags flags)
        {
            float scale = GetScale(g, out float dx, out float dy);
            if (scale == 1f)
            {
                TextRenderer.DrawText(g, text, font, location, color, flags);
                return;
            }

            var target = Point.Round(new PointF(location.X * scale + dx, location.Y * scale + dy));
            using (var scaled = ScaleFont(font, scale))
            {
                var state = g.Save();
                g.ResetTransform();
                TextRenderer.DrawText(g, text, scaled, target, color, flags);
                g.Restore(state);
            }
        }

        /// <summary>확대 전 좌표 기준의 글자 크기를 돌려줍니다.</summary>
        public static Size Measure(Graphics g, string text, Font font, Size proposedSize, TextFormatFlags flags)
        {
            float scale = GetScale(g, out _, out _);
            if (scale == 1f)
            {
                return TextRenderer.MeasureText(g, text, font, proposedSize, flags);
            }

            var proposed = proposedSize.IsEmpty
                ? Size.Empty
                : new Size((int)Math.Ceiling(proposedSize.Width * scale), (int)Math.Ceiling(proposedSize.Height * scale));
            using (var scaled = ScaleFont(font, scale))
            {
                var size = TextRenderer.MeasureText(g, text, scaled, proposed, flags);
                return new Size((int)Math.Ceiling(size.Width / scale), (int)Math.Ceiling(size.Height / scale));
            }
        }

        private static float GetScale(Graphics g, out float dx, out float dy)
        {
            using (var m = g.Transform)
            {
                var e = m.Elements;
                dx = e[4];
                dy = e[5];
                return e[0];
            }
        }

        private static Font ScaleFont(Font font, float scale)
        {
            return new Font(font.FontFamily, font.Size * scale, font.Style, font.Unit);
        }
    }
}
