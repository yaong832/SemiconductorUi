using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SemiconductorUi.Controls;
using SemiconductorUi.Helpers;

namespace SemiconductorUi
{
    /// <summary>
    /// 장비 그림에서 챔버나 FOUP를 누르면 그 옆에 작은 상세 창을 띄웁니다.
    /// 창은 메인 화면 위에 놓인 일반 컨트롤이라 화면을 잠그지 않으므로, 떠 있는 동안에도
    /// 긴급 정지, 일시 정지 같은 버튼을 그대로 누를 수 있습니다. 값은 화면 갱신 때마다 다시 채웁니다.
    /// 화면 표시만 하며 장비에는 아무 명령도 보내지 않습니다.
    /// </summary>
    public partial class Form1
    {
        private UnitDetailPopup unitDetailPopup;
        private Control unitDetailAnchor;

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

            if (unitDetailPopup == null || unitDetailPopup.IsDisposed)
            {
                unitDetailPopup = new UnitDetailPopup { Name = "unitDetailPopup", Visible = false };
                unitDetailPopup.CloseRequested += (s, e) => CloseUnitDetail();
                Controls.Add(unitDetailPopup);
                // 공정이 멈춰 화면 갱신이 없을 때도 창 크기가 바뀌면 자리를 다시 잡습니다.
                Resize += (s, e) => PlaceUnitDetail();
            }

            unitDetailPopup.UnitKey = unitKey;
            unitDetailAnchor = anchor;
            RefreshUnitDetail();
            unitDetailPopup.Visible = true;
            unitDetailPopup.BringToFront();
        }

        internal void CloseUnitDetail()
        {
            if (unitDetailPopup != null)
            {
                unitDetailPopup.Visible = false;
                unitDetailPopup.UnitKey = null;
            }
            unitDetailAnchor = null;
        }

        /// <summary>떠 있는 상세 창의 값과 위치를 지금 상태로 다시 맞춥니다 (화면 갱신마다 호출).</summary>
        internal void RefreshUnitDetail()
        {
            if (unitDetailPopup == null || unitDetailPopup.IsDisposed || string.IsNullOrEmpty(unitDetailPopup.UnitKey))
            {
                return;
            }

            string key = unitDetailPopup.UnitKey;
            unitDetailPopup.DetailScale = EquipmentCanvasStyler.DrawScale;
            unitDetailPopup.SetContent(key.StartsWith("FOUP") ? BuildFoupDetail(key) : BuildChamberDetail(key));
            PlaceUnitDetail();
        }

        // 창은 위에서 본 장비 그림(챔버, TM, FOUP) 오른쪽, TM 정면·옆면 보기 자리에 둡니다.
        // 그래야 공정 중 TM 움직임과 세 챔버가 가려지지 않습니다. 높이는 누른 곳에 맞추고,
        // 오른쪽 제어 패널(긴급 정지 포함)과 장비 그림 칸 밖으로는 나가지 않습니다.
        private void PlaceUnitDetail()
        {
            var popup = unitDetailPopup;
            var anchor = unitDetailAnchor;
            if (popup == null || anchor == null || !anchor.IsHandleCreated || !IsHandleCreated)
            {
                return;
            }

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
                var canvas = ToClient(panelEquipmentCanvas);
                minTop = canvas.Top + margin;
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
                if (x + popup.Width > maxRight)
                {
                    x = a.Left - gap - popup.Width;
                }
            }
            x = Math.Max(margin, Math.Min(x, maxRight - popup.Width));

            int y = a.Top + a.Height / 2 - popup.Height / 2;
            y = Math.Max(minTop, Math.Min(y, maxBottom - popup.Height));

            var location = new Point(x, y);
            if (popup.Location != location)
            {
                popup.Location = location;
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
