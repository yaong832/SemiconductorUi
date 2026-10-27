using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace SemiconductorUi
{
    /// <summary>
    /// 오른쪽 제어 패널(공정 시작 ~ 레시피 적용)을 창 크기에 맞춰 배치합니다.
    /// 버튼과 그룹 상자의 크기와 위치만 바꾸고, 버튼 객체와 클릭 처리는 그대로 둡니다.
    /// </summary>
    public partial class Form1
    {
        // 오른쪽 제어 패널 열 폭: 기본 320, 넓은 화면에서는 본문 폭의 16%까지(최대 400) 넓힙니다.
        private const int ControlColumnMinWidth = 320;
        private const int ControlColumnMaxWidth = 400;
        private const float ControlColumnRatio = 0.16f;

        // 제어 버튼 높이. 기존 버튼은 44(공정 시작 등)와 40(FOUP·웨이퍼)이었습니다.
        private const int ControlButtonDesignHeight = 44;
        private const int ControlButtonMinHeight = 38;
        private const int ControlButtonMaxHeight = 64;
        private const int SecondaryButtonHeightDiff = 4;
        private const float ControlButtonMaxFontScale = 1.2f;
        private const int ControlGroupMaxExtraGap = 24;

        private readonly Dictionary<Control, Font> controlPanelDesignFonts = new Dictionary<Control, Font>();
        private bool layingOutControlPanel;

        private void HookControlPanelLayout()
        {
            if (IsDesignEnvironment()) return;
            if (panelControlPanel == null || flowControlPanelStack == null || tableLayoutProcessArea == null) return;
            if (groupBoxControlButtons == null || groupBoxFoupReady == null || groupBoxRecipe == null) return;

            // 바깥 패널은 스크롤하지 않고, 창이 아주 작을 때만 안쪽 줄이 스크롤합니다.
            panelControlPanel.AutoScroll = false;
            foreach (var group in new[] { groupBoxControlButtons, groupBoxFoupReady, groupBoxRecipe })
            {
                group.AutoSize = false;
            }
            flowLayoutControlButtons.AutoSize = false;
            flowLayoutFoupReadyButtons.AutoSize = false;
            flowLayoutFoupReadyButtons.AutoScroll = false;

            foreach (var button in ControlPanelButtons(primary: true))
            {
                controlPanelDesignFonts[button] = button.Font;
            }
            foreach (var button in ControlPanelButtons(primary: false))
            {
                controlPanelDesignFonts[button] = button.Font;
            }

            tableLayoutProcessArea.Resize += (s, e) => UpdateControlColumnWidth();
            flowControlPanelStack.Resize += (s, e) => LayoutControlPanel();
            flowControlPanelStack.VisibleChanged += (s, e) => LayoutControlPanel();

            UpdateControlColumnWidth();
            LayoutControlPanel();
        }

        /// <summary>공정 시작~장비 제어(primary) 또는 FOUP·웨이퍼·레시피 적용 버튼(secondary).</summary>
        private IEnumerable<Button> ControlPanelButtons(bool primary)
        {
            var buttons = primary
                ? new[] { buttonStart, buttonPause, buttonStop, buttonResetAlarm, buttonResetProcess, buttonEquipmentControl }
                : new[] { buttonMountFoupA, buttonMountFoupB, buttonWaferLoading, buttonWaferUnloading, buttonApplyRecipe };
            foreach (var button in buttons)
            {
                if (button != null) yield return button;
            }
        }

        private void UpdateControlColumnWidth()
        {
            if (tableLayoutProcessArea.ColumnStyles.Count < 2) return;

            int width = (int)Math.Round(tableLayoutProcessArea.ClientSize.Width * ControlColumnRatio);
            width = Math.Max(ControlColumnMinWidth, Math.Min(ControlColumnMaxWidth, width));
            var style = tableLayoutProcessArea.ColumnStyles[1];
            if (style.SizeType == SizeType.Absolute && Math.Abs(style.Width - width) >= 1f)
            {
                style.Width = width;
            }
        }

        private void LayoutControlPanel()
        {
            if (layingOutControlPanel) return;
            layingOutControlPanel = true;
            try
            {
                var stack = flowControlPanelStack;
                int groupWidth = stack.ClientSize.Width - stack.Padding.Horizontal - SystemInformation.VerticalScrollBarWidth;
                if (groupWidth <= 0) return;

                var groups = new GroupBox[] { groupBoxControlButtons, groupBoxFoupReady, groupBoxRecipe };
                foreach (var group in groups)
                {
                    group.Width = groupWidth - group.Margin.Horizontal;
                }

                var primary = new List<Button>(ControlPanelButtons(primary: true));
                var secondary = new List<Button>(ControlPanelButtons(primary: false));
                secondary.Remove(buttonApplyRecipe);

                // 버튼 높이를 빼고 세로로 고정되어 쓰이는 길이를 모두 더합니다.
                int fixedHeight = stack.Padding.Vertical;
                if (labelControlTitle != null)
                {
                    fixedHeight += labelControlTitle.Height + labelControlTitle.Margin.Vertical;
                }
                foreach (var group in groups)
                {
                    fixedHeight += group.Margin.Vertical + GroupChromeHeight(group);
                }
                fixedHeight += flowLayoutControlButtons.Padding.Vertical + flowLayoutFoupReadyButtons.Padding.Vertical;
                foreach (var button in primary) fixedHeight += button.Margin.Vertical;
                foreach (var button in secondary) fixedHeight += button.Margin.Vertical;
                fixedHeight += RecipeGroupFixedHeight();

                // 남은 높이를 버튼에 나눕니다. 보조 버튼(FOUP, 웨이퍼, 레시피 적용)은 4px 낮습니다.
                int buttonCount = primary.Count + secondary.Count + 1;
                int secondaryCount = secondary.Count + 1;
                int available = stack.ClientSize.Height - fixedHeight;
                int height = (available + secondaryCount * SecondaryButtonHeightDiff) / buttonCount;
                height = Math.Max(ControlButtonMinHeight, Math.Min(ControlButtonMaxHeight, height));
                int secondaryHeight = height - SecondaryButtonHeightDiff;

                // 버튼이 최대 높이에 닿고도 남는 높이는 그룹 사이 간격으로 조금 나눠 줍니다.
                int used = height * primary.Count + secondaryHeight * secondaryCount;
                int extraGap = Math.Max(0, Math.Min(ControlGroupMaxExtraGap, (available - used) / 3));

                float fontScale = Math.Max(1f, Math.Min(ControlButtonMaxFontScale, height / (float)ControlButtonDesignHeight));

                SizeButtons(groupBoxControlButtons, flowLayoutControlButtons, primary, height, fontScale);
                SizeButtons(groupBoxFoupReady, flowLayoutFoupReadyButtons, secondary, secondaryHeight, fontScale);
                LayoutRecipeGroup(secondaryHeight, fontScale);

                groupBoxFoupReady.Margin = new Padding(0, 12 + extraGap, 0, 0);
                groupBoxRecipe.Margin = new Padding(0, 12 + extraGap, 0, 0);
            }
            finally
            {
                layingOutControlPanel = false;
            }
        }

        /// <summary>그룹 상자에서 제목 줄과 안쪽 여백이 차지하는 세로 길이.</summary>
        private static int GroupChromeHeight(GroupBox group)
        {
            return group.Height - group.DisplayRectangle.Height;
        }

        private void SizeButtons(GroupBox group, FlowLayoutPanel flow, List<Button> buttons, int height, float fontScale)
        {
            int innerWidth = group.DisplayRectangle.Width - flow.Padding.Horizontal;
            int contentHeight = flow.Padding.Vertical;
            foreach (var button in buttons)
            {
                button.Size = new Size(Math.Max(40, innerWidth - button.Margin.Horizontal), height);
                ApplyButtonFontScale(button, fontScale);
                contentHeight += height + button.Margin.Vertical;
            }
            group.Height = GroupChromeHeight(group) + contentHeight;
        }

        /// <summary>레시피 그룹에서 적용 버튼을 뺀 세로 길이(라벨, 콤보, 간격).</summary>
        private int RecipeGroupFixedHeight()
        {
            int labelHeight = labelRecipe?.Height ?? 0;
            int comboHeight = comboRecipeSelect?.Height ?? 0;
            return 4 + labelHeight + 8 + comboHeight + 10;
        }

        private void LayoutRecipeGroup(int buttonHeight, float fontScale)
        {
            var area = groupBoxRecipe.DisplayRectangle;
            int x = area.Left + 2;
            int width = Math.Max(40, area.Width - 4);
            int y = area.Top + 4;

            if (labelRecipe != null)
            {
                labelRecipe.Location = new Point(x, y);
                y = labelRecipe.Bottom + 8;
            }
            if (comboRecipeSelect != null)
            {
                comboRecipeSelect.Location = new Point(x, y);
                comboRecipeSelect.Width = width;
                y = comboRecipeSelect.Bottom + 10;
            }
            if (buttonApplyRecipe != null)
            {
                buttonApplyRecipe.Bounds = new Rectangle(x, y, width, buttonHeight);
                ApplyButtonFontScale(buttonApplyRecipe, fontScale);
                y = buttonApplyRecipe.Bottom;
            }
            groupBoxRecipe.Height = y + groupBoxRecipe.Padding.Bottom;
        }

        private void ApplyButtonFontScale(Button button, float scale)
        {
            if (!controlPanelDesignFonts.TryGetValue(button, out var designFont)) return;

            float size = (float)Math.Round(designFont.Size * scale * 2f) / 2f;
            if (Math.Abs(button.Font.Size - size) < 0.1f) return;

            var old = button.Font;
            button.Font = Math.Abs(size - designFont.Size) < 0.1f
                ? designFont
                : new Font(designFont.FontFamily, size, designFont.Style, designFont.Unit);
            if (old != designFont)
            {
                old.Dispose();
            }
        }
    }
}
