using System.ComponentModel;
using System.Windows.Forms;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Controls
{
    /// <summary>오른쪽 제어 패널 버튼 모양.</summary>
    public enum LampButtonLook
    {
        /// <summary>밝은 버튼 + 왼쪽 램프.</summary>
        Lamp,
        /// <summary>파란 바탕 + 왼쪽 램프 (공정 시작).</summary>
        Primary,
        /// <summary>빨간 긴급 정지.</summary>
        Emergency
    }

    /// <summary>
    /// 오른쪽 제어 패널의 램프 버튼(공정 시작, 일시 정지, 긴급 정지, FOUP 장착 등).
    /// 실행 중 모양·램프·상태 칩·클릭은 그대로 Form1.LampPanel.cs와 LampButtonPainter가 정합니다(보통 Button과 같음).
    /// 이 클래스는 Visual Studio 디자이너에서도 실행 화면과 같은 모양(램프 꺼짐)으로 보이게 하는 것만 더합니다.
    /// </summary>
    public class LampButton : Button
    {
        /// <summary>디자이너에서 그릴 모양.</summary>
        [Category("디자이너 미리보기")]
        [Description("디자이너에서 그릴 모양입니다. 실행 중 모양은 Form1.LampPanel.cs(ApplyLampPanelStyle)가 정합니다.")]
        [DefaultValue(LampButtonLook.Lamp)]
        public LampButtonLook LampLook { get; set; } = LampButtonLook.Lamp;

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignPreview.IsDesigner(this))
            {
                if (LampLook == LampButtonLook.Emergency)
                {
                    LampButtonPainter.PaintEmergency(this, e.Graphics);
                }
                else
                {
                    LampButtonPainter.PaintLampButton(this, e.Graphics, null, LampLook == LampButtonLook.Primary, true);
                }
                return;
            }
            base.OnPaint(e);
        }
    }
}
