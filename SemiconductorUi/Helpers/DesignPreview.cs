using System.Windows.Forms;

namespace SemiconductorUi.Helpers
{
    /// <summary>
    /// Visual Studio 디자이너 안에서 그리는 중인지 알려 줍니다.
    /// 실행할 때만 코드가 그리는 버튼·판·칩(ShellNavButton, LampButton, PlateGroupBox, ShellChip 등)이
    /// 디자이너에서도 실행 화면과 같은 모양으로 보이게 할 때만 씁니다. 실행 중에는 항상 false입니다.
    /// </summary>
    internal static class DesignPreview
    {
        /// <summary>컨트롤 또는 그 상위 컨트롤이 디자이너에 올라가 있으면 true.</summary>
        public static bool IsDesigner(Control control)
        {
            for (var current = control; current != null; current = current.Parent)
            {
                var site = current.Site;
                if (site != null && site.DesignMode) return true;
            }
            return false;
        }

        /// <summary>디자이너에서는 미리보기 글자를, 실행 중에는 빈 글자를 돌려줍니다.</summary>
        public static string Text(Control control, string previewText)
        {
            return !string.IsNullOrEmpty(previewText) && IsDesigner(control) ? previewText : "";
        }
    }
}
