using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SemiconductorUi.Controls;
using SemiconductorUi.Forms;
using SemiconductorUi.Helpers;

namespace SemiconductorUi
{
    /// <summary>
    /// 장비 그림에서 챔버나 FOUP를 누르면 상세 정보를 별도 창(UnitDetailForm) 하나에 띄웁니다.
    /// 레시피 창과 달리 메인 화면을 잠그지 않으므로(모달 아님), 떠 있는 동안에도
    /// 긴급 정지, 일시 정지 같은 버튼을 그대로 누를 수 있습니다. 값은 화면 갱신 때마다 다시 채웁니다.
    /// 창은 Windows 제목 줄로 옮기며, 오른쪽 제어 패널(긴급 정지 포함) 위에는 놓이지 않게 합니다.
    /// 화면 표시만 하며 장비에는 아무 명령도 보내지 않습니다.
    /// </summary>
    public partial class Form1
    {
        private UnitDetailForm unitDetailForm;

        // 장비 그림의 챔버와 FOUP에 클릭을 연결합니다 (Tag는 다른 용도로 쓰므로 건드리지 않음).
        private void RegisterEquipmentDetailHandlers()
        {
            AttachUnitDetailClick(panelChamberA, panelChamberA, "PMA");
            AttachUnitDetailClick(panelChamberB, panelChamberB, "PMB");
            AttachUnitDetailClick(panelChamberC, panelChamberC, "PMC");
            AttachUnitDetailClick(panelFoupA, panelFoupA, "FOUP A");
            AttachUnitDetailClick(panelFoupB, panelFoupB, "FOUP B");
        }

        private void AttachUnitDetailClick(Control control, Control anchor, string unitKey)
        {
            if (control == null || anchor == null)
            {
                return;
            }

            control.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    HandleEquipmentUnitClick(unitKey, anchor);
                }
            };

            foreach (Control child in control.Controls)
            {
                AttachUnitDetailClick(child, anchor, unitKey);
            }

            // FOUP 그림처럼 나중에 들어오는 자식에도 연결합니다.
            control.ControlAdded += (s, e) => AttachUnitDetailClick(e.Control, anchor, unitKey);
        }

        private Control GetUnitDetailAnchor(string unitKey)
        {
            switch (unitKey)
            {
                case "PMA": return panelChamberA;
                case "PMB": return panelChamberB;
                case "PMC": return panelChamberC;
                case "FOUP A": return panelFoupA;
                case "FOUP B": return panelFoupB;
                default: return null;
            }
        }

        private void ShowUnitDetail(string unitKey, Control anchor)
        {
            anchor = anchor ?? GetUnitDetailAnchor(unitKey);
            if (anchor == null)
            {
                return;
            }

            if (unitDetailForm == null || unitDetailForm.IsDisposed)
            {
                unitDetailForm = new UnitDetailForm { Name = "unitDetailForm" };
                unitDetailForm.CloseRequested += (s, e) => CloseUnitDetail();
                // 사용자가 창을 옮기다 놓았을 때(ResizeEnd), 또는 마우스를 쓰지 않고 자리가 바뀌었을 때
                // 제어 패널 위에 걸쳐 있으면 왼쪽으로 비켜 놓습니다.
                unitDetailForm.ResizeEnd += (s, e) => KeepUnitDetailOffControlPanel();
                unitDetailForm.LocationChanged += (s, e) =>
                {
                    if (Control.MouseButtons == MouseButtons.None)
                    {
                        KeepUnitDetailOffControlPanel();
                    }
                };
            }

            bool wasVisible = unitDetailForm.Visible;
            unitDetailForm.UnitKey = unitKey;
            RefreshUnitDetail();

            if (!wasVisible)
            {
                // 처음 띄울 때만 기본 자리(TM 정면·옆면 보기 자리)에 둡니다.
                // 떠 있는 동안 다른 장비를 누르면 내용만 바뀌고 자리는 그대로입니다.
                unitDetailForm.Location = GetDefaultUnitDetailLocation(anchor, unitDetailForm.Size);
                unitDetailForm.Show(this);
            }
        }

        internal void CloseUnitDetail()
        {
            if (unitDetailForm != null && !unitDetailForm.IsDisposed)
            {
                unitDetailForm.Hide();
                unitDetailForm.UnitKey = null;
            }
        }

        /// <summary>떠 있는 상세 창의 값을 지금 상태로 다시 맞춥니다 (화면 갱신마다 호출).</summary>
        internal void RefreshUnitDetail()
        {
            var detailForm = unitDetailForm;
            if (detailForm == null || detailForm.IsDisposed || string.IsNullOrEmpty(detailForm.UnitKey))
            {
                return;
            }

            string key = detailForm.UnitKey;
            detailForm.DetailScale = EquipmentCanvasStyler.DrawScale;
            detailForm.SetContent(key.StartsWith("FOUP") ? BuildFoupDetail(key) : BuildChamberDetail(key));
        }

        // 기본 자리(화면 좌표): 위에서 본 장비 그림 오른쪽, TM 정면·옆면 보기 자리. 높이는 누른 곳에 맞춥니다.
        // 오른쪽 제어 패널(긴급 정지 포함)을 가리지 않고, 장비 그림 칸 위쪽과 아래 메뉴 줄 사이에 둡니다.
        private Point GetDefaultUnitDetailLocation(Control anchor, Size windowSize)
        {
            Rectangle ToClient(Control c) => RectangleToClient(c.RectangleToScreen(c.ClientRectangle));
            bool Usable(Control c) => c != null && c.Visible && c.IsHandleCreated;

            var a = ToClient(anchor);
            int margin = 8;
            int gap = Math.Max(8, (int)(12 * EquipmentCanvasStyler.DrawScale));

            int maxRight = ClientSize.Width - margin;
            if (Usable(panelControlPanel))
            {
                maxRight = Math.Min(maxRight, ToClient(panelControlPanel).Left - margin);
            }

            int minTop = margin;
            int maxBottom = ClientSize.Height - margin;
            if (Usable(panelEquipmentCanvas))
            {
                minTop = ToClient(panelEquipmentCanvas).Top + margin;
            }
            if (Usable(shellBottomBar))
            {
                maxBottom = ToClient(shellBottomBar).Top - margin;
            }

            int x;
            if (Usable(tmSideViewControl))
            {
                x = ToClient(tmSideViewControl).Left;
            }
            else
            {
                x = a.Right + gap;
                if (x + windowSize.Width > maxRight)
                {
                    x = a.Left - gap - windowSize.Width;
                }
            }

            int y = a.Top + a.Height / 2 - windowSize.Height / 2;
            x = Math.Max(margin, Math.Min(x, maxRight - windowSize.Width));
            y = Math.Max(minTop, Math.Min(y, maxBottom - windowSize.Height));
            return PointToScreen(new Point(x, y));
        }

        // 상세 창이 제어 패널(긴급 정지 포함) 위에 걸쳐 있으면 제어 패널 왼쪽으로 옮깁니다.
        // 창이 메인 화면 밖(다른 모니터 등)에 있거나 제어 패널과 겹치지 않으면 그대로 둡니다.
        private bool keepingUnitDetailOffPanel;
        private void KeepUnitDetailOffControlPanel()
        {
            var detailForm = unitDetailForm;
            if (keepingUnitDetailOffPanel || detailForm == null || detailForm.IsDisposed || !detailForm.Visible
                || panelControlPanel == null || !panelControlPanel.Visible || !panelControlPanel.IsHandleCreated
                || WindowState == FormWindowState.Minimized)
            {
                return;
            }

            var panel = panelControlPanel.RectangleToScreen(panelControlPanel.ClientRectangle);
            var bounds = detailForm.Bounds;
            if (!bounds.IntersectsWith(panel))
            {
                return;
            }

            keepingUnitDetailOffPanel = true;
            try
            {
                detailForm.Location = new Point(panel.Left - 8 - bounds.Width, bounds.Top);
            }
            finally
            {
                keepingUnitDetailOffPanel = false;
            }
        }

        private UnitDetailContent BuildChamberDetail(string unitKey)
        {
            var content = new UnitDetailContent { Title = EquipmentRegionHelper.FormatUnitLabel(unitKey) };
            var chamber = GetChamberStateForUnit(unitKey);
            PmDetails.TryGetValue(unitKey, out var detail);

            bool processing = chamber != null && IsChamberProcessing(chamber);
            bool completed = chamber != null && IsChamberCompleted(chamber);
            bool hasWafer = chamber?.CurrentWafer != null;
            bool alarm = ChamberAlarmStatus.ContainsKey(unitKey) && ChamberAlarmStatus[unitKey];

            content.Status = detail?.StatusText ?? chamber?.StatusText ?? ChamberStatusTexts.Idle;
            content.Tone = alarm ? LampTone.Red
                : processing ? LampTone.Green
                : completed ? LampTone.Blue
                : hasWafer ? LampTone.Amber
                : LampTone.Off;
            content.Live = processing && SimulationRunning;

            string Dash(string s) => string.IsNullOrWhiteSpace(s) ? "-" : s;
            content.Info.Add(new KeyValuePair<string, string>("레시피", Dash(detail?.RecipeName)));
            content.Info.Add(new KeyValuePair<string, string>("스텝", Dash(detail?.StepName)));
            content.Info.Add(new KeyValuePair<string, string>("웨이퍼", hasWafer ? Dash(detail?.ActiveWaferText) : "없음"));
            if (detail != null)
            {
                content.Info.Add(new KeyValuePair<string, string>("처리 순서", $"{detail.StepIndex} / {Math.Max(1, detail.StepCount)}"));
                content.Info.Add(new KeyValuePair<string, string>("이번 시간", detail.StepTimeTotal > 0 ? $"{detail.StepTimeCurrent} / {detail.StepTimeTotal} s" : "-"));
                content.Info.Add(new KeyValuePair<string, string>("전체 시간", detail.RecipeTimeTotal > 0 ? $"{detail.RecipeTimeCurrent} / {detail.RecipeTimeTotal} s" : "-"));
            }

            bool doorOpen = chamber != null && IsRegionDoorOpen(chamber.Region);
            content.Chips.Add(new KeyValuePair<string, bool>(doorOpen ? "도어 열림" : "도어 닫힘", !doorOpen));
            content.Chips.Add(new KeyValuePair<string, bool>(processing ? "램프 켜짐" : completed ? "램프 점멸" : "램프 꺼짐", processing || completed));
            if (alarm)
            {
                content.Chips.Add(new KeyValuePair<string, bool>("알람", false));
            }

            content.Env = GetPmStatusPanel(unitKey)?.GetEnvironmentRows();

            if (detail != null)
            {
                content.Progress = detail.StepProgress;
                string message = string.IsNullOrWhiteSpace(detail.StepMessage) ? content.Status : detail.StepMessage;
                content.ProgressText = $"{message} · {Math.Max(0, Math.Min(100, detail.StepProgress))}%";
            }

            return content;
        }

        private UnitDetailContent BuildFoupDetail(string unitKey)
        {
            bool isA = unitKey == "FOUP A";
            bool mounted = isA ? IsFoupAMounted : IsFoupBMounted;
            var content = new UnitDetailContent { Title = unitKey };

            var headline = isA ? labelFoupAStatusHeadline : labelFoupBStatusHeadline;
            content.Status = headline?.Text ?? "";

            int waferCount = 0;
            content.Slots = new List<KeyValuePair<string, bool>>();
            for (int layer = AppSettings.MaxFoupCapacity; layer >= 1; layer--)
            {
                bool hasWafer;
                if (isA)
                {
                    hasWafer = foupManager?.HasWafersInFoupA() == true && foupManager.HasWaferAtLayerFoupA(layer);
                }
                else
                {
                    hasWafer = FoupBCompleted != null && FoupBCompleted.Any(w => w.Id == layer);
                }

                string text;
                if (!mounted)
                {
                    text = $"슬롯 {layer} · 미장착";
                    hasWafer = false;
                }
                else if (hasWafer)
                {
                    waferCount++;
                    text = $"슬롯 {layer} · Wafer #{layer} · {(isA ? "대기" : "완료")}";
                }
                else
                {
                    text = $"슬롯 {layer} · 비어 있음";
                }
                content.Slots.Add(new KeyValuePair<string, bool>(text, hasWafer));
            }

            content.Tone = !mounted ? LampTone.Off : waferCount > 0 ? LampTone.Blue : LampTone.Amber;

            string Text(Label label) => string.IsNullOrWhiteSpace(label?.Text) ? "-" : label.Text;
            content.Info.Add(new KeyValuePair<string, string>("웨이퍼", mounted ? $"{waferCount}장" : "-"));
            content.Info.Add(new KeyValuePair<string, string>("Path", Text(isA ? labelFoupAPathValue : labelFoupBPathValue)));
            content.Info.Add(new KeyValuePair<string, string>("PPID", Text(isA ? labelFoupAPPIDValue : labelFoupBPPIDValue)));
            content.Info.Add(new KeyValuePair<string, string>("LOTID", Text(isA ? labelFoupALotIdValue : labelFoupBLotIdValue)));
            content.Info.Add(new KeyValuePair<string, string>("MID", Text(isA ? labelFoupAMidValue : labelFoupBMidValue)));
            content.Info.Add(new KeyValuePair<string, string>("Lock", Text(isA ? labelFoupALockValue : labelFoupBLockValue)));

            content.Chips.Add(new KeyValuePair<string, bool>(mounted ? "장착됨" : "미장착", mounted));
            return content;
        }
    }
}
