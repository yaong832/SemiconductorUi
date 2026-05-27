using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace SemiconductorUi.EventHandlers
{
    /// <summary>
    /// Paint 이벤트 핸들러
    /// </summary>
    public class PaintEventHandlers : Form1EventHandlersBase
    {
        /// <summary>
        /// PaintEventHandlers 생성자
        /// </summary>
        /// <param name="form">Form1 인스턴스</param>
        public PaintEventHandlers(Form1 form) : base(form) { }

        /// <summary>
        /// 웨이퍼 패널 Paint 이벤트 핸들러
        /// 웨이퍼를 원형으로 그립니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">Paint 이벤트 인자</param>
        public void WaferPanel_Paint(object sender, PaintEventArgs e)
        {
            if (sender is Panel panel && panel.Visible)
            {
                var color = panel.Tag is Color c ? c : Color.FromArgb(200, 220, 255);
                var rect = panel.ClientRectangle;
                rect.Inflate(-3, -3);
                using (var brush = new SolidBrush(color))
                using (var pen = new Pen(Color.FromArgb(180, 80, 80, 80), 1.2f))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillEllipse(brush, rect);
                    e.Graphics.DrawEllipse(pen, rect);
                }
            }
        }

        /// <summary>
        /// GroupBox Paint 이벤트 핸들러
        /// GroupBox의 테두리를 사용자 정의 스타일로 그립니다.
        /// </summary>
        /// <param name="sender">이벤트 발생 객체</param>
        /// <param name="e">Paint 이벤트 인자</param>
        public void GroupBox_Paint(object sender, PaintEventArgs e)
        {
            var groupBox = sender as GroupBox;
            if (groupBox == null) return;

            // GroupBox의 텍스트 영역 계산
            var textSize = e.Graphics.MeasureString(groupBox.Text, groupBox.Font);
            var textX = groupBox.Padding.Left;
            var textWidth = textSize.Width;
            var textHeight = textSize.Height;

            // 테두리 그리기
            using (var pen = new Pen(Color.Black, 1))
            {
                // 상단 선 (텍스트 왼쪽)
                e.Graphics.DrawLine(pen, 0, textHeight / 2, textX, textHeight / 2);
                
                // 상단 선 (텍스트 오른쪽)
                e.Graphics.DrawLine(pen, textX + textWidth, textHeight / 2, groupBox.Width, textHeight / 2);
                
                // 좌측 선
                e.Graphics.DrawLine(pen, 0, textHeight / 2, 0, groupBox.Height);
                
                // 우측 선
                e.Graphics.DrawLine(pen, groupBox.Width - 1, textHeight / 2, groupBox.Width - 1, groupBox.Height);
                
                // 하단 선
                e.Graphics.DrawLine(pen, 0, groupBox.Height - 1, groupBox.Width, groupBox.Height - 1);
            }
        }
    }
}

