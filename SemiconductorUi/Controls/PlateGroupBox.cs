using System.Windows.Forms;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Controls
{
    /// <summary>
    /// 오른쪽 제어 패널의 묶음 상자("제어 패널", "FOUP 장착 / 웨이퍼 로드 상태", "레시피 및 작업 설정").
    /// 실행 중 연회색 판 모양은 그대로 Form1.LampPanel.cs(ApplyLampPanelStyle)와 LampButtonPainter.AttachPlate가 그립니다(보통 GroupBox와 같음).
    /// 이 클래스는 Visual Studio 디자이너에서도 같은 판 모양으로 보이게 하는 것만 더합니다.
    /// </summary>
    public class PlateGroupBox : GroupBox
    {
        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignPreview.IsDesigner(this))
            {
                LampButtonPainter.PaintPlate(this, e.Graphics);
                return;
            }
            base.OnPaint(e);
        }
    }
}
