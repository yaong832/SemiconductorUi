using System.ComponentModel;
using System.Windows.Forms;
using SemiconductorUi.Helpers;

namespace SemiconductorUi.Controls
{
    /// <summary>메뉴 버튼 모양: 하단 메뉴 줄 버튼(그림 + 글자) 또는 본문 위 작은 탭.</summary>
    public enum ShellNavButtonKind
    {
        BarItem,
        SubTab
    }

    /// <summary>
    /// 하단 메뉴 줄(운전~보고서, 시스템)과 본문 위 작은 탭(모니터링/검증/이송) 버튼.
    /// 실행 중 모양·선택 표시·클릭은 그대로 Form1.Shell.cs와 ShellNavPainter가 정합니다(보통 Button과 같음).
    /// 이 클래스는 Visual Studio 디자이너에서도 실행 화면과 같은 모양으로 보이게 하는 것만 더합니다.
    /// 아래 "디자이너 미리보기" 값은 디자이너 그림에만 쓰이고 실행 화면에는 영향이 없습니다.
    /// </summary>
    public class ShellNavButton : Button
    {
        /// <summary>디자이너에서 그릴 모양(하단 메뉴 버튼 / 작은 탭).</summary>
        [Category("디자이너 미리보기")]
        [Description("디자이너에서 그릴 모양입니다. 실행 중 모양은 Form1.Shell.cs(InitShellBottomBar, InitShellSubTabs)가 정합니다.")]
        [DefaultValue(ShellNavButtonKind.BarItem)]
        public ShellNavButtonKind NavKind { get; set; } = ShellNavButtonKind.BarItem;

        /// <summary>디자이너에서 하단 메뉴 버튼 왼쪽에 그릴 그림.</summary>
        [Category("디자이너 미리보기")]
        [Description("디자이너에서 그릴 그림입니다. 실행 중 그림은 Form1.Shell.cs(InitShellBottomBar)의 StyleShellNavItem이 정합니다.")]
        [DefaultValue(ShellNavIcon.Operate)]
        public ShellNavIcon NavIcon { get; set; } = ShellNavIcon.Operate;

        /// <summary>디자이너에서 선택된 모양(파란 표시)으로 그릴지.</summary>
        [Category("디자이너 미리보기")]
        [Description("디자이너에서 선택된 모양으로 그립니다. 실행 중 선택 표시는 화면을 고를 때 코드가 정합니다.")]
        [DefaultValue(false)]
        public bool PreviewSelected { get; set; }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (DesignPreview.IsDesigner(this))
            {
                if (NavKind == ShellNavButtonKind.SubTab)
                {
                    ShellNavPainter.DrawSubTab(e.Graphics, this, PreviewSelected, false);
                }
                else
                {
                    ShellNavPainter.DrawNavItem(e.Graphics, this, NavIcon, true, PreviewSelected, false);
                }
                return;
            }
            base.OnPaint(e);
        }
    }
}
