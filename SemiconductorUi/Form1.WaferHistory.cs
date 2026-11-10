using System;
using System.Drawing;
using System.Windows.Forms;
using SemiconductorUi.Controllers;
using SemiconductorUi.Controls;
using SemiconductorUi.Forms;
using SemiconductorUi.Helpers;
using SemiconductorUi.Models;
using SemiconductorUi.Services;

namespace SemiconductorUi
{
    /// <summary>
    /// 웨이퍼별 공정 이력 (보고서 → 웨이퍼 이력). 공정 흐름의 몇 곳에서 시각만 알려 주고, 장비 동작은 바꾸지 않습니다.
    /// 기록 중 오류가 나도 공정에는 영향이 없도록 모든 호출을 감쌉니다.
    /// </summary>
    public partial class Form1
    {
        private WaferHistoryService waferHistory;
        private WaferHistoryForm waferHistoryForm;

        internal WaferHistoryService WaferHistory
        {
            get
            {
                if (waferHistory == null && !IsDesignEnvironment())
                {
                    waferHistory = new WaferHistoryService(AppSettings.WaferHistoryFolder, AddLogMessage);
                }
                return waferHistory;
            }
        }

        private string ChamberLetter(ChamberController.ChamberState chamber)
        {
            if (chamber == null) return null;
            if (chamber == ChamberAState) return "A";
            if (chamber == ChamberBState) return "B";
            if (chamber == ChamberCState) return "C";
            return null;
        }

        private static string RegionName(EquipmentRegion region)
        {
            switch (region)
            {
                case EquipmentRegion.FoupA: return "FOUP A";
                case EquipmentRegion.FoupB: return "FOUP B";
                case EquipmentRegion.ChamberA: return "A";
                case EquipmentRegion.ChamberB: return "B";
                case EquipmentRegion.ChamberC: return "C";
                default: return null;
            }
        }

        private void RecordWafer(Action<WaferHistoryService> action)
        {
            try
            {
                var history = WaferHistory;
                if (history != null)
                {
                    action(history);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"웨이퍼 이력 오류: {ex.Message}");
            }
        }

        internal void RecordWaferLotStarted(int waferCount)
        {
            RecordWafer(h => h.LotStarted(waferCount, LastSelectedRecipe, SecondExposureEnabled, IsTmHardwareModeAvailable(), IsLoggedIn ? CurrentUser : ""));
        }

        internal void RecordWaferLotFinished()
        {
            RecordWafer(h => h.LotFinished());
        }

        internal void RecordWaferLotAborted(string reason)
        {
            RecordWafer(h => h.LotAborted(reason));
        }

        private void RecordWaferPickup(TransferController.TransferTask task)
        {
            if (task?.Wafer == null) return;
            string from = task.SourceChamber != null ? ChamberLetter(task.SourceChamber) : (task.FromFoup ? "FOUP A" : RegionName(task.Pickup));
            if (from == null) return;
            RecordWafer(h => h.NotePickup(task.Wafer.Id, from));
        }

        private void RecordWaferDropoff(TransferController.TransferTask task)
        {
            if (task?.Wafer == null) return;
            string to = task.DestinationChamber != null ? ChamberLetter(task.DestinationChamber) : RegionName(task.Dropoff);
            if (to == null) return;
            RecordWafer(h => h.NoteDropoff(task.Wafer.Id, to));
        }

        private void RecordWaferProcessStart(ChamberController.ChamberState chamber, Wafer wafer)
        {
            string letter = ChamberLetter(chamber);
            if (letter == null || wafer == null || !ChamberStatusTexts.IsProcessing(chamber.StatusText)) return;
            RecordWafer(h => h.NoteProcessStart(wafer.Id, letter, chamber.TotalSeconds));
        }

        private void RecordWaferProcessEnd(ChamberController.ChamberState chamber)
        {
            string letter = ChamberLetter(chamber);
            var wafer = chamber?.CurrentWafer;
            if (letter == null || wafer == null) return;
            RecordWafer(h => h.NoteProcessEnd(wafer.Id, letter));
        }

        /// <summary>
        /// 웨이퍼 이력 창을 엽니다 (모달 아님). 이미 떠 있으면 앞으로 가져와 새로 읽습니다.
        /// </summary>
        internal void ShowWaferHistoryWindow()
        {
            try
            {
                var history = WaferHistory;
                if (history == null)
                {
                    return;
                }

                if (waferHistoryForm == null || waferHistoryForm.IsDisposed)
                {
                    waferHistoryForm = new WaferHistoryForm(history) { Owner = this };
                    var area = Screen.FromControl(this).WorkingArea;
                    int x = Left + (Width - waferHistoryForm.Width) / 2;
                    int y = Top + (Height - waferHistoryForm.Height) / 2;
                    x = Math.Max(area.Left, Math.Min(x, area.Right - waferHistoryForm.Width));
                    y = Math.Max(area.Top, Math.Min(y, area.Bottom - waferHistoryForm.Height));
                    waferHistoryForm.Location = new Point(x, y);
                }

                if (!waferHistoryForm.Visible)
                {
                    waferHistoryForm.Show(this);
                }
                else
                {
                    waferHistoryForm.RefreshRows();
                }
                waferHistoryForm.BringToFront();
                waferHistoryForm.Activate();
            }
            catch (Exception ex)
            {
                AddLogMessage($"웨이퍼 이력 창 오류: {ex.Message}", "WARN");
            }
        }
    }
}
